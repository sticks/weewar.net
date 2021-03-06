﻿using System;
using System.Collections.Generic;

using System.Text;
using System.Threading;
using Eliza;

namespace SkippingRock.SticksBot
{

  class SticksBot : Bot
  {
    protected override void AcceptInvite(Game game)
    {
      Console.WriteLine("  .. accepting invitation");
      base.AcceptInvite(game);
    }
    protected override bool IdleGame(Game game)
    {
      Console.Write(".");
      return base.IdleGame(game);
    }

    public SticksBot(ElizaApi eliza)
      : base(eliza)
    {
    }

    protected override void ProcessGame(Game detailed)
    {
      Console.WriteLine();
      Console.WriteLine("Game: " + detailed.Name + " (" + detailed.Id + ")");
      Console.WriteLine("  Getting Map info: " + detailed.MapId);
      WeewarMap wmap = eliza.GetMap(detailed.MapId);
      //Console.WriteLine(  "  Getting Map info: "+wmap.Terrains);
      Faction f = detailed.GetFactionByPlayerName(eliza.User);
      Console.WriteLine("  .. moving my dudes. ");
      foreach (Unit unit in f.Units)
      {
        Console.WriteLine("-----------------------------------------------------------------------------");
        Console.WriteLine("     " + unit.Type + "(" + unit.Quantity + ") on " + unit.Coordinate);

        // repair if quantity below 5
        if (!unit.Finished && unit.Quantity < 5)
        {
          String m = eliza.Repair(detailed.Id, unit.Coordinate);
          Console.WriteLine("     " + ".. repairing => " + m);
          unit.Finished = true;
          continue;
        }

        //
        // request movement coordinates
        List<Coordinate> possibleMovementCoordinates = eliza.GetMovementCoords(detailed.Id, unit.Coordinate, unit.Type);
        Util.Shuffle(possibleMovementCoordinates);
        possibleMovementCoordinates.Insert(0, unit.Coordinate);

        // check if there is a capturable base in range
        if (!unit.Finished && unit.CanCapture())
        {
          Coordinate c = MatchFreeBase(possibleMovementCoordinates, wmap, detailed, f);
          if (c != null)
          {
            String m = eliza.MoveAttackCapture(detailed.Id, unit.Coordinate, c, null, true);
            unit.Coordinate = c;
            Console.WriteLine("     " + ".. moving to " + c + " and capturing =>" + m);
            unit.Finished = true;
          }
        }
        List<Coordinate> targets = getTargets(detailed, wmap, f);
        int minDistance = MinDistance(unit.Coordinate, targets);

        int n = 5;

        if (minDistance <= 5 && !unit.Finished)
        {

          // Different moving spots that will be evaluated
          // check for possible attack targets from one of the targets
          for (int i = 0; i < n && i < possibleMovementCoordinates.Count; i++)
          {
            Coordinate c = possibleMovementCoordinates[i];
            Console.WriteLine("     " + ".. checking movement Option :" + c + " ");
            Coordinate a = getAttackCoordinate(detailed, unit, c);
            Console.WriteLine("     " + "..  attack coord :" + a + " ");
            if (a != null && detailed.getUnit(c) == null)
            {
              String m = eliza.MoveAttackCapture(detailed.Id, unit.Coordinate, c, a, false);
              Console.WriteLine("     " + ".. moving to " + c + " attacking " + a + " =>" + m);
              if (c != null)
                unit.Coordinate = c;
              unit.Finished = true;
              break;
            }
          }
        }

        if (!unit.Finished && possibleMovementCoordinates.Count > 1)
        {
          List<Coordinate> cities = getEnemyCities(detailed, wmap, f);
          AStarSearch s = new AStarSearch();
          s.SetStartAndGoalStates(new SticksNode(wmap, unit), new SticksNode(wmap, cities[0], unit));
          SearchState searchState;
          do
          {
            searchState = s.SearchStep();
          }
          while (searchState == SearchState.Searching);
          if (searchState == SearchState.Succeeded)
          {
          }

          Util.Shuffle(targets);
          Util.Shuffle(cities);
          possibleMovementCoordinates.RemoveAt(0);

          while (possibleMovementCoordinates.Count > 5) possibleMovementCoordinates.RemoveAt(5);
          while (cities.Count > 3) cities.RemoveAt(3);
          while (targets.Count > 3) targets.RemoveAt(3);

          bool cnt = true;
          while (cnt)
          {
            Console.WriteLine("     " + ".. possible movement options: " + Coordinate.ToString(possibleMovementCoordinates));
            Console.WriteLine("     " + ".. possible Targets: " + Coordinate.ToString(targets));
            Coordinate c;

            if (unit.Type == UnitType.Trooper)
              c = getClosest(possibleMovementCoordinates, cities);
            else
            {
              c = getClosest(possibleMovementCoordinates, targets);
              if (c.Equals(unit.Coordinate) && targets.Count == 0 && possibleMovementCoordinates.Count > 1)
                c = possibleMovementCoordinates[1];
            }

            if (!c.Equals(unit.Coordinate) && detailed.getUnit(c) == null)
            {
              String m = eliza.MoveAttackCapture(detailed.Id, unit.Coordinate, c, null, false);
              Console.WriteLine("     " + ".. moving to " + c + " =>" + m);
              unit.Coordinate = c;
              cnt = false;
            }
            else
              possibleMovementCoordinates.Remove(c);
            cnt = cnt && possibleMovementCoordinates.Count > 0;
          }
        }

        //Thread.sleep( 300 );
      }

      if (f.Units.Count * 3 < (detailed.GetUnitCount() * 2) || f.Units.Count < 15)
      {
        Console.WriteLine("     Terrains :" + Terrain.ToString(f.Terrains));
        if (f.Credits > 75)
        {
          foreach (Terrain terrain in f.Terrains)
          {
            Console.WriteLine("     " + terrain.Type + " on " + terrain.Coordinate + " finished:" + terrain.Finished + " unit: " + (detailed.getUnit(terrain.Coordinate) != null));
            if (!terrain.Finished && detailed.getUnit(terrain.Coordinate) == null)
            {
              Console.WriteLine("     " + terrain.Type + " on " + terrain.Coordinate);
              List<UnitType> options = buildOptions[terrain.Type];
              UnitType buildType;
              do
              {
                int nd = dice(options.Count);
                buildType = options[nd - 1];
              } while (f.Credits < Unit.GetCost(buildType));
              String x = eliza.Build(detailed.Id, terrain.Coordinate, buildType);
              Console.WriteLine("     .... building " + buildType + " " + x);
              f.Credits = (f.Credits - Unit.GetCost(buildType));
            }
          }
        }
      }
      if (eliza.EndTurn(detailed.Id))
        Console.WriteLine(" .. finished turn.");
      else
        Console.WriteLine(" .. failed to finish turn [" + eliza.GetLastResult() + "]");
    }

    static Dictionary<TerrainType, List<UnitType>> buildOptions = new Dictionary<TerrainType, List<UnitType>>();

    private static Random random = new Random();
    /**
     * generate numbers from 1 to n including
     * @param n
     */
    public static int dice(int n)
    {
      return random.Next(n) + 1;
    }

    static SticksBot()
    {
      List<UnitType> bbase = new List<UnitType>();
      bbase.Add(UnitType.Trooper);
      bbase.Add(UnitType.Trooper);
      bbase.Add(UnitType.Trooper);
      bbase.Add(UnitType.HeavyTrooper);
      bbase.Add(UnitType.Raider);
      bbase.Add(UnitType.Raider);
      bbase.Add(UnitType.Tank);
      bbase.Add(UnitType.Tank);
      bbase.Add(UnitType.HeavyTank);
      bbase.Add(UnitType.LightArtillery);
      bbase.Add(UnitType.HeavyArtillery);
      buildOptions.Add(TerrainType.Base, bbase);
    }

    private Coordinate getClosest(List<Coordinate> movementOptions, List<Coordinate> targets)
    {
      Coordinate best = null;
      int n = Int32.MaxValue;
      foreach (Coordinate c in movementOptions)
      {
        if (best == null)
          best = c;
        foreach (Coordinate target in targets)
        {
          int d = c.getDistance(target);
          if (d < n)
          {
            best = c;
            n = d;
          }
        }
      }
      return best;
    }

    private int MinDistance(Coordinate from, List<Coordinate> targets)
    {
      int n = Int32.MaxValue;
      foreach (Coordinate c in targets)
      {
        int d = from.getDistance(c);
        if (d < n)
        {
          n = d;
        }

      }
      return n;
    }

    private List<Coordinate> getTargets(Game game, WeewarMap wmap, Faction myFaction)
    {
      List<Coordinate> targets = new List<Coordinate>();
      foreach (Faction faction in game.Factions)
      {
        if (faction == myFaction)
          continue;
        foreach (Unit otherUnit in faction.Units)
          targets.Add(otherUnit.Coordinate);
      }
      return targets;
    }

    private List<Coordinate> getEnemyCities(Game game, WeewarMap wmap, Faction myFaction)
    {
      List<Coordinate> targets = new List<Coordinate>();
      List<Terrain> bases = wmap.getTerrainsByType(TerrainType.Base);
      foreach (Terrain ter in bases)
        if (game.getTerrainOwner(ter.Coordinate) != myFaction)
          targets.Add(ter.Coordinate);
      return targets;
    }

    private Coordinate MatchFreeBase(List<Coordinate> coords, WeewarMap wmap, Game g, Faction myFaction)
    {

      //Console.WriteLine("Coords:"+coords );
      foreach (Coordinate c in coords)
      {
        Terrain t = wmap.get(c);
        //Console.WriteLine("type :"+t.Type );
        Faction owner = g.getTerrainOwner(c);
        if (t.Type == TerrainType.Base && (owner == null || owner != myFaction) && g.getUnit(c) == null)
          return c;
      }
      return null;
    }

    private Coordinate getAttackCoordinate(Game game, Unit unit, Coordinate from)
    {
      List<Coordinate> coords = eliza.GetAttackCoords(game.Id, from, unit.Type);
      if (coords.Count > 0)
      {
        int n = dice(coords.Count);
        return coords[n - 1];
      }
      return null;
    }

    private Coordinate getMovementCoordinate(Game game, Unit unit, List<Coordinate> possibleMovementCoordinates)
    {
      List<Coordinate> coords = possibleMovementCoordinates;

      while (coords.Count != 0)
      {
        int n = dice(coords.Count);


        Coordinate c = coords[n - 1];
        //Console.WriteLine( "     "+".. trying to move to "+c+" =>"+ game.getUnit( c ));
        if (game.getUnit(c) == null)
          return c;
        else
          coords.Remove(c);
      }
      return null;
    }

  }
}
