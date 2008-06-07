using System;
using System.Collections.Generic;

using System.Text;

namespace Eliza
{
    public class Faction
    {
        String state;
        String playerName;
        int credits;
        List<Unit> units = new List<Unit>();
        List<Terrain> terrains = new List<Terrain>();

        public string State
        {
            get { return state; }
            set { state = value; }
        }
        public List<Unit> Units
        {
            get { return units; }
            set { units = value; }
        }

        public string PlayerName
        {
            get { return playerName; }
            set { playerName = value; }
        }

        public List<Terrain> Terrains
        {
            get { return terrains; }
            set { terrains = value; }
        }

        public int Credits
        {
            get { return credits; }
            set { credits = value; }
        }
    }
}
