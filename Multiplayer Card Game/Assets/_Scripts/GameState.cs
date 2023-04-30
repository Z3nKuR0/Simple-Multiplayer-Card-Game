using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    //Singelton pattern used for GameState so not more than one exists at a time for a player
    public static GameState Instance;

    private GridXZ<GridCell> _grid;
    [SerializeField] private int _gWidth;
    [SerializeField] private int _gHeight;
    [SerializeField] private Transform _tGrid;
    [SerializeField] private PlayerScript _player;

    private Networking _net;
    private string _lastAction;
    private float _cellsize = 10f;
    private string[] _arr;

    public Transform card;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _grid = new GridXZ<GridCell>(_gWidth, _gHeight, _cellsize, _tGrid, (GridXZ<GridCell> g, int x, int z) => new GridCell(g, x, z)); //Creates a grid and for each grid space instansiates a GridCell
        _net = GameObject.Find("Network Manager").GetComponent<Networking>(); //Finds the Network Manager which the game instance will use to commuicate with centralised server.
        _lastAction = ""; // sets client side last action to "" to match the server side variable so all clients and the server have the same start point

        StartCoroutine(RepeatNetQuery()); // A coroutine is used to repeatedly query the server for the server-side last action
    }

    private IEnumerator RepeatNetQuery()
    {
        while (true)
        {
            NetQuery();
            yield return new WaitForSeconds(0.2f); // Wait for 0.2 seconds
        }
    }

    private void NetQuery() //Queires for server sides last action
    {
        Transform builtTransform;

        _net.RequestLastAction();

        if (_net.GetLastActionData().Equals(""))
            return;
        else
            _arr = _net.GetLastActionData().Split(",");

        if (!_lastAction.Equals(_net.GetLastActionData())) //if the server side has a differenet last action the the client, client side is updated to match server-side
        {

            _lastAction = _net.GetLastActionData(); //The last action is given as a string of values '1,3,4'
            _arr = _lastAction.Split(","); // 1 = id, 3 = x, 4 = y
            int x = Convert.ToInt32(_arr[1]);
            int z = Convert.ToInt32(_arr[2]);

            GridCell currentGridObject = _grid.GetGridObject(x, z);
            builtTransform = Instantiate(card, _grid.GetWorldPositon(x, z) + new Vector2(5, 5), Quaternion.identity); // instansiats card object at position of grid space + offset so it is in the centre
            builtTransform.SetParent(_tGrid);
            builtTransform.gameObject.GetComponent<SpriteRenderer>().color = GetColour(Convert.ToInt32(_arr[0])); // set the colour of the card to the players colour based on their id
            currentGridObject.SetPlaced(true);
            //Debug.Log("Upnack string get coords instansiate");

        }
    }


    void Update()
    {

        if (Input.GetMouseButtonUp(0)) //Check for client-side input
        {
            Vector3 vec = Mouse3D.GetMouseWorldPosition();

            if (vec == Vector3.zero) //if the mouseposition didnt hit a grid layer should not be counted
                return;

            _grid.GetXZ(vec, out int x, out int z); //returns the x and y of a grid
            GridCell currentGridObject = _grid.GetGridObject(x, z);

            if (!currentGridObject.IsPlaced()) //checks if the gridcell of the x and y has been used to prevent players placing over each other
            {
                _player.SetColour(GetColour(_net._id));
                _net.SendMessage(_net._id, x, z); //sends a message containing id,x,y to the server (however may not be counted due to server-side values)
            }


        }

    }

    public Color GetColour(int x) //Returns a colour based on the id given (deterministic way for having each player have their own colour)
    {
        float r = ((x * 5 * 33) % 256) / 255.0f;
        float g = ((x * 2 * 47) % 256) / 255.0f;
        float b = ((x * 9 * 67) % 256) / 255.0f;

        return new Color(r, g, b);
    }

}

