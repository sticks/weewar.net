using System;
using System.Collections.Generic;

using System.Text;

namespace Eliza
{
    public class Game
    {
        String name;
        String link;
        bool requiringAnInviteAccept = false;
        int id;
        bool inNeedOfAttention;
        int mapId;

        List<Faction> factions = new List<Faction>();
        List<String> players = new List<String>();


        public bool IsInNeedOfAttention
        {
            get { return inNeedOfAttention; }
            set { inNeedOfAttention = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public List<String> Players
        {
            get { return players; }
            set { players = value; }
        }

        public string Link
        {
            get { return link; }
            set { link = value; }
        }

        public bool RequiresAnInviteAccept
        {
            get { return requiringAnInviteAccept; }
            set { requiringAnInviteAccept = value; }
        }

        public List<Faction> Factions
        {
            get { return factions; }
            set { factions = value; }
        }
        public Faction GetFactionByPlayerName(String name)
        {
            foreach (Faction faction in factions)
            {
                if (faction.PlayerName==name)
                    return faction;
            }
            return null;
        }
        public Object getUnit(Coordinate c)
        {
            foreach (Faction faction in Factions)
            {
                foreach (Unit unit in faction.Units)
                {
                    if (unit.Coordinate == c)
                        return unit;
                }
            }
            return null;
        }
        public int MapId
        {
            get
            {
                return mapId;
            }
            set
            {
                this.mapId = value;
            }
        }

        public Faction getTerrainOwner(Coordinate c)
        {
            foreach (Faction faction in Factions)
            {
                foreach (Terrain t in faction.Terrains)
                {
                    if (t.Coordinate == c)
                        return faction;
                }
            }
            return null;
        }
        public int GetUnitCount()
        {
            int s = 0;
            foreach (Faction faction in Factions)
                s += faction.Units.Count;
            return s;
        }

    }
}
