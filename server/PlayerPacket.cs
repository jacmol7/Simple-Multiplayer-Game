using System;
using System.Collections.Generic;
using System.Text;

namespace simpleGame
{
    class PlayerPacket
    {
        public string id { get; set; }
        public int[] position { get; set; }
        public int[] colour { get; set; }
    }
}
