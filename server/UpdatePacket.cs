using System;
using System.Collections.Generic;
using System.Text;

namespace simpleGame
{
    class UpdatePacket
    {
        //setup
        public Dictionary<String, String> setup { get; set; }
        
        //entities
        public List<Entity> entities { get; set; }

        public UpdatePacket()
        {
            setup = new Dictionary<string, string>();
            entities = new List<Entity>();
        }
        
        public UpdatePacket(List<Entity> entities)
        {
            setup = new Dictionary<string, string>();
            this.entities = entities;
        }
    }
}
