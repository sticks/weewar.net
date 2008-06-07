using System;
using System.Collections.Generic;

using System.Text;

namespace Eliza
{
    public enum UnitType
    {
        Trooper,
        HeavyTrooper,
        Raider,
        Tank,
        HeavyTank,
        LightArtillery,
        HeavyArtillery
    }

    public class Unit
    {
        UnitType type;
        bool finished;
        Coordinate coordinate;
        int quantity;

        static Dictionary<UnitType, int> buildCost = new Dictionary<UnitType, int>();

        static Unit()
        {
            buildCost.Add(UnitType.Trooper, 75);
            buildCost.Add(UnitType.HeavyTrooper, 150);
            buildCost.Add(UnitType.Raider, 200);
            buildCost.Add(UnitType.Tank, 300);
            buildCost.Add(UnitType.HeavyTank, 600);
            buildCost.Add(UnitType.LightArtillery, 200);
            buildCost.Add(UnitType.HeavyArtillery, 600);
        }

        public static UnitType ToType(string type)
        {
            switch(type)
            {
                case "Tank":
                    return UnitType.Tank;
                case "Raider":
                    return UnitType.Raider;
                case "Trooper":
                    return UnitType.Trooper;
                case "Heavy Artillery":
                    return UnitType.HeavyArtillery;
                case "Heavy Tank":
                    return UnitType.HeavyTank;
                case "Heavy Trooper":
                    return UnitType.HeavyTrooper;
                case "Light Artillery":
                    return UnitType.LightArtillery;
                default:
                    throw new ArgumentException();
            }
        }

        public static string ToString(UnitType type)
        {
            switch(type)
            {
                case UnitType.Tank:
                    return "Tank";
                case UnitType.Raider:
                    return "Raider";
                case UnitType.Trooper:
                    return "Trooper";
                case UnitType.HeavyArtillery:
                    return "Heavy Artillery";
                case UnitType.HeavyTank:
                    return "Heavy Tank";
                case UnitType.HeavyTrooper:
                    return "Heavy Trooper";
                case UnitType.LightArtillery:
                    return "Light Artillery";
                default:
                    return "Unknown";
            }
        }

        public static int GetCost(UnitType type)
        {
            if (buildCost.ContainsKey(type))
                return buildCost[type];
            else
                return Int32.MaxValue;
        }

        public UnitType Type
        {
            get { return type; }
            set { type = value; }
        }
        public string TypeString
        {
            get { return ToString(type); }
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
