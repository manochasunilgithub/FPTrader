
import zmq
import time
import random
import datetime

context = zmq.Context()
reqSocket  = context.socket(zmq.REQ)
reqSocket.connect("tcp://127.0.0.1:5002")

topic = "Topic_A"

subSocket  = context.socket(zmq.SUB)
subSocket.connect("tcp://127.0.0.1:5001") 

TIMEOUT = 10000

poller = zmq.Poller()
poller.register(reqSocket,zmq.POLLIN)
poller.register(subSocket,zmq.POLLIN)

subSocket.setsockopt_string(zmq.SUBSCRIBE,topic) # Subscribe for the request


reqSocket.send_string(topic,flags=zmq.SNDMORE)
reqSocket.send(bytes([3])) # we are sending random request id

while True:
    polledSockets = dict(poller.poll(TIMEOUT)) # we are doing polling on sockets, so if there is any data they will return
    # see if we have any reply back on our request
    if reqSocket in polledSockets and polledSockets.get(reqSocket) == zmq.POLLIN:
        topic = reqSocket.recv_string(zmq.NOBLOCK)
        idBytes = reqSocket.recv(zmq.NOBLOCK)
        reply = reqSocket.recv(zmq.NOBLOCK)
        print(f"Received reply on topic {topic}, request id :{int.from_bytes(idBytes,byteorder='little',signed=False)} with reply {int.from_bytes(reply,byteorder='little', signed=False)}")

    # see if we have received any new subscription message
    if subSocket in polledSockets and polledSockets.get(subSocket) == zmq.POLLIN:
        topic = subSocket.recv_string(zmq.NOBLOCK)
        subMessageBytes = subSocket.recv(zmq.NOBLOCK)
        print(f"Received subscription on topic {topic}, message :{int.from_bytes(subMessageBytes,byteorder='little', signed=False)} at {str(datetime.datetime.now())}")

time.sleep(0.5)
reqSocket.close()
subSocket.close()
