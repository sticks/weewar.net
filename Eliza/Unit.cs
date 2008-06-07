using System;
using System.Collections.Generic;

using System.Text;

namespace Eliza
{
    public class Unit
    {
        String type;
        bool finished;
        Coordinate coordinate;
        int quantity;

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public bool Finished
        {
            get { return finished; }
            set { finished = value; }
        }

        public Coordinate Coordinate { get { return coordinate; } set { coordinate = value;} }
        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }
        public override string ToString()
        {
            return "{\"type\":\"" + Type + "\",\"finished\":" + Finished + ",\"quantity\":" + quantity + ",\"coordinate\":" + coordinate + "}";
        }
    }
}
