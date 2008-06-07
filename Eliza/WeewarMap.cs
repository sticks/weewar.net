using System;
using System.Collections.Generic;

using System.Text;

namespace Eliza
{
    public class WeewarMap
    {
        Dictionary<Coordinate, Terrain> terrains = new Dictionary<Coordinate, Terrain>();
        int width, height;

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public ICollection<Terrain> Terrains
        {
            get { return terrains.Values; }
        }


        public void add(Terrain t)
        {
            terrains.Add(t.Coordinate, t);
        }

        public Terrain get(Coordinate c)
        {
            return terrains[c];
        }


        public List<Terrain> getTerrainsByType(String type)
        {
            List<Terrain> terrains = new List<Terrain>();
            foreach (Terrain t in Terrains)
                if (t.Type==type)
                    terrains.Add(t);
            return terrains;
        }

    }
}
