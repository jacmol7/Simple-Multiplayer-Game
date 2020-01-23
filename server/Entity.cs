using System;
using System.Collections.Generic;
using System.Text;

namespace simpleGame
{
    class Entity
    {
        //position
        public int[] position { get; set; }
        //colour
        public int[] colour { get; set; }

        public Entity()
        {
            position = new int[2];
            colour = new int[3];
        }

        public Entity(int[] position, int[] colour)
        {
            this.position = position;
            this.colour = colour;
        }
    }
}
