using System;
using System.Collections.Generic;
using System.Text;

namespace Eliza
{
  public enum TerrainType
  {
    Plains, Woods, Mountains, Desert, Swamp, Water, Base, Airbase, Harbor, Repair
  }
  public class Terrain
  {
    TerrainType type;
    Coordinate coordinate;
    bool finished;

    public TerrainType Type
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

    internal static TerrainType ToType(string type)
    {
      switch (type)
      {
        case "Airbase":
          return TerrainType.Airbase;
        case "Base":
          return TerrainType.Base;
        case "Desert":
          return TerrainType.Desert;
        case "Harbor":
          return TerrainType.Harbor;
        case "Mountains":
          return TerrainType.Mountains;
        case "Plains":
          return TerrainType.Plains;
        case "Repair":
          return TerrainType.Repair;
        case "Swamp":
          return TerrainType.Swamp;
        case "Water":
          return TerrainType.Water;
        case "Woods":
          return TerrainType.Woods;

        default:
          throw new ArgumentException();
      }
    }

    public static string ToString(TerrainType type)
    {
      switch (type)
      {
        default:
          return "Unknown";
      }
    }
  }
}
