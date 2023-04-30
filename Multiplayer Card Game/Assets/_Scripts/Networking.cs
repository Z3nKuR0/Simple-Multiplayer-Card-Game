using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using Newtonsoft.Json;
using System;

public class Networking : MonoBehaviour
{
    WebSocket _ws;
    private string _lastActionData;
    public int _id;

    // Start is called before the first frame update
    void Start()
    {
        _lastActionData = "";
        _id = int.MaxValue;

        _ws = new WebSocket("ws://34.142.126.12:9090"); //Sets up the websocket connection to centralised server

        _ws.OnMessage += (sender, e) => //Sets up how messages recieved are to be interpreted
        {
            var message = JsonConvert.DeserializeObject<Dictionary<string, object>>(e.Data);

            if (message.ContainsKey("type") && message["type"].ToString().Equals("last_action")) //gets the server-side last action
            {
                // Store the received data in the variable
                _lastActionData = (string)message["data"];
                //Debug.Log("LastAction: " + lastActionData);
            }
            else if (message.ContainsKey("type") && message["type"].ToString().Equals("client_id")) // gets the id allocated by the server
            {
                Debug.Log("ID: " + message["data"]);
                _id = Convert.ToInt32(message["data"]);
            }
            else
            {
                Debug.Log("Message: " + ((WebSocket)sender).Url + ", Data: " + e.Data);
            }

        };

        _ws.ConnectAsync();

        _ws.OnOpen += (sender, e) => //connection being opened it requests the id given by the server
        {
            _ws.Send(JsonConvert.SerializeObject(new { type = "request_id" }));
        };

        StartCoroutine(RepeatRequest()); // Corountine used to repeacted called lastactionrequest to keep things up to date without the need of the player manually requesting for it
    }

    public void RequestLastAction() //method used to request the last action (server-side) ran from GameState
    {
        if (_ws == null)
        {
            return;
        }

        _ws.Send(JsonConvert.SerializeObject(new { type = "request_last_action" }));
    }

    public string GetLastActionData() //Returns the server-side last action
    {
        return _lastActionData;
    }

    public void SendMessage(int id, int x , int y) //Used to send client-side last action
    {
        var lastaction = new Dictionary<string, object>
        {
                { "type", "last_action" },
                { "id", id},
                { "data", id + "," + x + "," + y }
        };

        _ws.Send(JsonConvert.SerializeObject(lastaction));
    }

    void OnDestroy() //On closing of the instance, a final message is sent to clear the server-side last action to get it prepated for next players
    {
        if (_ws != null)
        {
            var clear = new Dictionary<string, object>
            {
                { "type", "last_action" },
                { "data", ""}
            };

            _ws.Send(JsonConvert.SerializeObject(clear));

            _ws.CloseAsync();
        }
    }

    private IEnumerator RepeatRequest() //used to repeatedly request the last action of the server to keep everything up to date
    {
        while (true)
        {
            RequestLastAction();
            yield return new WaitForSeconds(0.2f); // Wait for 0.2 seconds
        }
    }
}
