using System;
using System.Collections.Generic;

using System.Text;
using System.IO;

namespace Eliza
{
  public enum UnitGroupType
  {
    Soft,
    Hard,
    Boat,
    Sub,
    Amphib,
    Air,
    Speedboat
  }

  public enum UnitType
  {
    Trooper,
    HeavyTrooper,
    Raider,
    Tank,
    HeavyTank,
    LightArtillery,
    HeavyArtillery,
    DFA,
    AssaultArtillery,
    AAGuns,
    Berserker,
    Hovercraft,
    Helicopter,
    Jet,
    Bomber,
    Speedboat,
    Destroyer,
    Submarine,
    Battleship
  }

  public class Unit
  {
    UnitType type;
    bool finished;
    Coordinate coordinate;
    int quantity;
    UnitGroupType grouptype;

    static Dictionary<UnitType, int> buildCost = new Dictionary<UnitType, int>();
    static Dictionary<UnitGroupType, Dictionary<TerrainType,int>> terrainCost = new Dictionary<UnitGroupType,Dictionary<TerrainType,int>>();


    static Unit()
    {
      String[] lines = File.ReadAllLines("SticksBot.vars");
      int bclen = "buildCost.".Length;
      int tclen = "terrainCost.".Length;
      String[] nv;
      foreach (String line in lines)
      {
        if (line.StartsWith("buildCost."))
        {
          nv = line.Substring(bclen).Split('=');
          buildCost.Add(Unit.ToType(nv[0]), Int32.Parse(nv[1]));
        }
        else if (line.StartsWith("terrainCost."))
        {
          nv = line.Substring(tclen).Split('.','=');
          UnitGroupType ugt = ToGroupType(nv[0]);
          if (!terrainCost.ContainsKey(ugt))
          {
            terrainCost.Add(ugt, new Dictionary<TerrainType, int>());
          }
          Dictionary<TerrainType, int> tercost = terrainCost[ugt];
          tercost.Add(Terrain.ToType(nv[1]), Int32.Parse(nv[2]));
        }
      }
    }

    public static UnitGroupType ToGroupType(string type)
    {
      switch (type)
      {
        case "Soft":
          return UnitGroupType.Soft;
        case "Hard":
          return UnitGroupType.Hard;
        case "Air":
          return UnitGroupType.Air;
        case "Sub":
          return UnitGroupType.Sub;
        case "Boat":
          return UnitGroupType.Boat;
        case "Amphib":
          return UnitGroupType.Amphib;
        case "Speedboat":
          return UnitGroupType.Speedboat;
        default:
          throw new ArgumentException("unknown");
      }

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
        case "HeavyArtillery":
          return UnitType.HeavyArtillery;
        case "Heavy Tank":
        case "HeavyTank":
          return UnitType.HeavyTank;
        case "Heavy Trooper":
        case "HeavyTrooper":
          return UnitType.HeavyTrooper;
        case "Light Artillery":
        case "LightArtillery":
          return UnitType.LightArtillery;
        case "AssaultArtillery":
        case "Assault Artillery":
          return UnitType.AssaultArtillery;
        case "DFA":
        case "Death From Above":
          return UnitType.DFA;
        case "Berserker":
          return UnitType.Berserker;
        case "AAGuns":
        case "AA-Guns":
          return UnitType.AAGuns;
        case "Hovercraft":
          return UnitType.Hovercraft;
        case "Sub":
        case "Submarine":
          return UnitType.Submarine;
        case "Speedboat":
          return UnitType.Speedboat;
        case "Helicopter":
          return UnitType.Helicopter;
        case "Bomber":
          return UnitType.Bomber;
        case "Jet":
          return UnitType.Jet;
        case "Destroyer":
          return UnitType.Destroyer;
        case "Battleship":
          return UnitType.Battleship;

        default:
          throw new ArgumentException();
      }
    }

    public static string ToString(UnitType type)
    {
      switch (type)
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

    public static int GetMoveCost(UnitGroupType type, TerrainType terrain)
    {
      return terrainCost[type][terrain];
    }

    public static int GetCost(UnitType type)
    {
      if (buildCost.ContainsKey(type))
        return buildCost[type];
      else
        return Int32.MaxValue;
    }

    public UnitGroupType GroupType
    {
      get { return grouptype; }
    }
    public UnitType Type
    {
      get { return type; }
      set 
      { 
        type = value; 
        switch (type)
        {
          case UnitType.Trooper:
          case UnitType.HeavyTrooper:
            grouptype = UnitGroupType.Soft;
            break;
          case UnitType.Hovercraft:
            grouptype = UnitGroupType.Amphib;
            break;
          case UnitType.Speedboat:
            grouptype = UnitGroupType.Speedboat;
            break;
          case UnitType.Submarine:
            grouptype = UnitGroupType.Sub;
            break;
          case UnitType.Helicopter:
          case UnitType.Jet:
          case UnitType.Bomber:
            grouptype = UnitGroupType.Air;
            break;
          case UnitType.Destroyer:
          case UnitType.Battleship:
            grouptype = UnitGroupType.Boat;
            break;
          default:
            grouptype = UnitGroupType.Hard;
            break;
        }
      }
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

    public Coordinate Coordinate { get { return coordinate; } set { coordinate = value; } }
    public int Quantity
    {
      get { return quantity; }
      set { quantity = value; }
    }
    public override string ToString()
    {
      return "{\"type\":\"" + Type + "\",\"finished\":" + Finished + ",\"quantity\":" + quantity + ",\"coordinate\":" + coordinate + "}";
    }

    public bool CanCapture()
    {
      return type == UnitType.HeavyTrooper
          || type == UnitType.Trooper;
    }

    public int GetCost()
    {
      return buildCost[type];
    }

    public int GetMoveCost(TerrainType terrain)
    {
      return terrainCost[grouptype][terrain];
    }
  }
}
