const express = require("express"); // Express, is a web application framework for Node.js,
const http = require("http");
const socketIo = require("socket.io");

const port = process.env.PORT || 4001;
const index = require("./routes/index"); // This is to respond back for http request
const app = express();
app.use(index);
const server = http.createServer(app);
const io = socketIo(server);
var zmq = require('zmq');


// We can build cache on the middleware server, to have data cached for each topic.. let's evaluate all the options later
var cachedData = new Map();


var requester = zmq.socket('req');
var subscriber = zmq.socket('sub')

requester.connect("tcp://127.0.0.1:5002");
subscriber.connect("tcp://127.0.0.1:5001")


var topics = ["Topic_A", "Topic_B", "Topic_C"];

// Buffer class is useful to send bytes, we will use this later when we send complex objects
requester.on("message", function (reply, requestIdBuffer, payloadBuffer) {
    cachedData.set(reply.toString(), payloadBuffer.readInt32LE(0)); // override the previous value in cache
});


num = 6;
topics.map(function (topic) {
    // let's send a request to the middleware and get all messages caches
    var sendBuffer = new Buffer(4);// TODO, see if we can reuse the send buffer memory
    sendBuffer.writeInt32LE(num, 0);
    requester.send(topic, zmq.ZMQ_SNDMORE)
    requester.send(sendBuffer) // we are sending random request id
    num++;
});


topics.map(function (topic) {
    subscriber.subscribe(topic)
});

subscriber.on('message', function (subcriptionTopic, payloadBuffer) {
    cachedData.set(subcriptionTopic.toString(), payloadBuffer.readInt32LE(0)); // override the previous value for demo
    if (subscriptionSockets.get(subcriptionTopic.toString()) != undefined) {
        socketList = subscriptionSockets.get(subcriptionTopic.toString())
        socketList.forEach((socket) => {
            var obj = JSON.stringify({
                topic: subcriptionTopic.toString(),
                value: cachedData.get(subcriptionTopic.toString()) // Lots of improvement can be made here
            });
            socket.emit("message", obj);
        });
    }

})

var subscriptionSockets = new Map();

clientSocketList = [];


function addToClientList(socket) {
    clientSocketList.push(socket)
}

io.on("connect", socket => {

    console.log("New client connected"), addToClientList(socket);

    socket.on("disconnect",
        () => {
            console.log("Client disconnected")
            var index = clientSocketList.indexOf(socket);
            if (index > -1) {
                clientSocketList.splice(index, 1);
            }
        });

    socket.on("subscribe",
        data => {
            console.log(`Subscription received on topic ${data.toString()}`);
            if (cachedData.get(data.toString()) != undefined) {
                var obj = JSON.stringify({
                    topic: data.toString(),
                    value: cachedData.get(data.toString())
                });

                // socket-io already have path based subscription list, see if we can use that
                if (subscriptionSockets.get(data.toString()) != undefined) {
                    subscriptionSockets.get(data.toString()).push(socket);
                }
                else {
                    var list = [];
                    list.push(socket);
                    subscriptionSockets.set(data.toString(), list);
                }
                console.log(`Going to send data to client of ${obj}`)
                // we should test this compression
                socket.compress(true).emit("message", obj);
            }
        }
    );
});

server.listen(port, () => console.log(`Listening on port ${port}`));
