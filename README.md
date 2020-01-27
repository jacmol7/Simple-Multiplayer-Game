# Simple-Multiplayer-Game
A simple web based multiplayer game where you can move around
The server is written in C# and uses websockets for communication, the server trusts the client which does nothing to prevent a client from cheating (teleporting or moving faster than allowed). The only thing that a client is prevented from doing is pretending to be another client because they must now the correct unique id.

The client is written in javascript and uses a canvas to draw the graphics.
