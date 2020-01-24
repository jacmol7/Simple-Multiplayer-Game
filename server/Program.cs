using System;
using System.Collections.Generic;
using System.Linq;
using Fleck;
using Newtonsoft.Json;

namespace simpleGame
{
    class Program
    {
        static void Main(string[] args)
        {
            //FleckLog.Level = LogLevel.Debug;
            //var server = new WebSocketServer("ws://159.65.83.56:8181");
            var server = new WebSocketServer("ws://0.0.0.0:8181");
            var sockets = new Dictionary<IWebSocketConnection, string>();
            var connectingSockets = new Dictionary<IWebSocketConnection, string>();
            var disconectingClients = new Dictionary<IWebSocketConnection, string>();
            var entities = new Dictionary<string, Entity>();

            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    string id = System.Guid.NewGuid().ToString();
                    connectingSockets.Add(socket, id);
                    UpdatePacket packet = new UpdatePacket();
                    packet.setup.Add("id", id);
                    socket.Send(JsonConvert.SerializeObject(packet));
                };
                socket.OnClose = () =>
                {
                    disconectingClients.Add(socket, sockets[socket]);
                };
                socket.OnMessage = message =>
                {
                    PlayerPacket packet = JsonConvert.DeserializeObject<PlayerPacket>(message);
                    if (entities.ContainsKey(packet.id))
                    {
                        entities[packet.id].position = packet.position;
                        entities[packet.id].colour = packet.colour;
                    }
                    else
                    {
                        Entity entity = new Entity(packet.position, packet.colour);
                        entities.Add(packet.id, entity);
                    }

                };
            });

            while (true)
            {
                // remove any entities and sockets associated with players who have disconected
                foreach (KeyValuePair<IWebSocketConnection, string> player in disconectingClients)
                {
                    entities.Remove(player.Value);
                    sockets.Remove(player.Key);
                }
                disconectingClients.Clear();

                // create update packet
                var update = new UpdatePacket(entities.Values.ToList());
                var updateJson = JsonConvert.SerializeObject(update);
                // send update to every client
                foreach (KeyValuePair<IWebSocketConnection, string> socket in sockets)
                {
                    socket.Key.Send(updateJson);
                }

                // merge newly connected clients so they can start recieving updates
                foreach (KeyValuePair<IWebSocketConnection, string> socket in connectingSockets)
                {
                    sockets.Add(socket.Key, socket.Value);
                }
                connectingSockets.Clear();

                // creates a rate of 60 updates per second
                System.Threading.Thread.Sleep(16);
            }        
        }
    }
}
