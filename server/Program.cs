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
            var server = new WebSocketServer("ws://159.65.83.56:8181");
            var sockets = new Dictionary<IWebSocketConnection, string>();
            var connectingSockets = new Dictionary<IWebSocketConnection, string>();
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
                    string id = sockets[socket];
                    if (id != null)
                    {
                        if (entities.ContainsKey(id))
                        {
                            entities.Remove(id);
                        }
                        if (sockets.ContainsKey(socket))
                        {
                            sockets.Remove(socket);
                        }
                    }
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
                var update = new UpdatePacket(entities.Values.ToList());
                foreach (KeyValuePair<IWebSocketConnection, string> socket in sockets)
                {
                    socket.Key.Send(JsonConvert.SerializeObject(update));
                }
                
                foreach (KeyValuePair<IWebSocketConnection, string> socket in connectingSockets)
                {
                    sockets.Add(socket.Key, socket.Value);
                }
                connectingSockets.Clear();

                System.Threading.Thread.Sleep(16);
            }        
        }
    }
}
