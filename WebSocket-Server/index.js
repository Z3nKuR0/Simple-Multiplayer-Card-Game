const WebSocket = require('ws'); //using Websocket & WebsocketSharp (Unity)

const wss = new WebSocket.Server({ port: 9090 }, () => {
  console.log("Server Up");
});

//Array to store client connections
let clients = [];
// count variable stores the id that will be given to a new connection player
let count = 0;

// lastaction and turn are server side values that control which action was the last input for both players
// turn controls who's turn it is to place a card
let lastAction = null;
let turn = null;

wss.on('connection', (ws) => {
    clients.push(ws);
    count++;
    console.log("Player Joined!");
    ws.on('message', (data) => {
        const message = JSON.parse(data);
        //Switch case used to differensiate clearly between the different types of possible messages
        //that can be recieved from the players and the appropriate responses
        switch (message.type) {
            case 'request_last_action': //Called frequently by both players and send the server's last recorded action to update their game instances client side
                if (lastAction) {
                ws.send(JSON.stringify({ type: 'last_action', data: lastAction }));
                }
                break;
            case 'last_action': //only updates server side values if there are 2 players and if it is the messenger's turn
                if (clients.length > 1 && turn != message.id) {
                lastAction = message.data;
                turn = message.id;
                }
                break;
            case 'request_id': //assigned the connected player an id they can use to differensiate who is who
                console.log('Client: %o', count);
                ws.send(JSON.stringify({ type: 'client_id', data: count }));
                console.log('Sent ID!');
                break;
            default:
                console.log('data: %o', data);
                ws.send(data);
                break;
        }

    });

  ws.on('close', () => {
    clients = clients.filter(client => client !== ws); // Remove the disconnected client from the array
    console.log('Client disconnected. Total clients: %o' , clients.length);
    //On disconnect sets the last action to an empty string to essentisally clear the server for the next set of players
    lastAction = ""
  });

});

wss.on('listening', () => {
  console.log("Listening: 9090");
});