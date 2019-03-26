import React, { Component } from 'react';
import socketIOClient from 'socket.io-client';

import './App.css';

class App extends Component {

    constructor() {
		super();
        this.state = {
            // Defaulting some value so we don't have to take care of undefined
	       data : { topic:"Waiting for topic Subscription",value:0}
	       };
	}

    sendSubscriptionOnConnection(io) {
        // for demo we are subscribing to hard coded topic
        io.emit("subscribe", "Topic_A");
    }

	componentDidMount() {
        var io = socketIOClient.connect('http://localhost:4001', { reconnect: true }); // Websocket server   
	    io.on('connect', socket => {
            console.log("connected to server");
            this.sendSubscriptionOnConnection(io);
        });

        // For demo we are using Jason
	    io.on("message", data => {
	        console.log("Recieved value:");
	        var obj = JSON.parse(data);
	        console.log(obj.topic);
            console.log(obj.value);
            this.setState({ data: obj}); // this pushes the event for gui to refresh
	    });
  }
	
   render() {
    return (
		<div style={{ textAlign: "center" }}>
            <p>
                Subscription received on
                    <br/>topic: "{this.state.data.topic}"
                    <br />value: {this.state.data.value}
            </p>             
            
      </div>
    );
  }
}

export default App;