using System;
using System.Collections.Generic;
using System.Text;

namespace Eliza
{
    public class Terrain
    {
        String type;
        Coordinate coordinate;
        bool finished;

        public String Type
        {
            get { return type; }
            set { type = value; }
        }
        public Coordinate Coordinate
        {
            get { return coordinate; }
            set { coordinate = value; }
        }
        public bool Finished
        {
            get { return finished; }
            set { finished = value; }
        }

        public override string ToString()
        {
            return "{\"type\":\"" + Type + "\",\"finished\":" + Finished + ",\"coordinate\":" + coordinate + "}";
        }

        public static string ToString(IList<Terrain> terrains)
        {
            StringBuilder sb = new StringBuilder("[");
            for (int i = 0; i < terrains.Count; i++)
            {
                if (i > 0) sb.Append(",");
                sb.Append(terrains[i].ToString());
            }
            sb.Append("]");
            return sb.ToString();

        }

    }
}
