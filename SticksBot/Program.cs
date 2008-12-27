using System;
using System.Collections.Generic;

using System.Text;
using System.Threading;
using Eliza;
using System.Configuration;
using System.Xml;

namespace SkippingRock.SticksBot
{
  class Program
  {
    static void astar(String[] args)
    {
      XmlDocument doc = new XmlDocument();
      doc.Load("c:/njones/src/SkippingRock/weewar.net/map.xml");
      WeewarMap map = new WeewarMap(doc.DocumentElement);
      AStarSearch astarsearch = new AStarSearch();
      bool[] path = new bool[map.Height * map.Width];
      Unit u = new Unit();
      u.Type = UnitType.Trooper;
      List<Terrain> bases = map.getTerrainsByType(TerrainType.Base);
      Coordinate cs = bases[0].Coordinate;
      Coordinate ce = bases[bases.Count-1].Coordinate;

      SticksNode start = new SticksNode(map, cs, u);
      SticksNode end = new SticksNode(map, ce, u);

      astarsearch.SetStartAndGoalStates(start, end);

      SearchState searchState;
      uint searchSteps = 0;

      do
      {
        searchState = astarsearch.SearchStep();
        searchSteps++;
      }
      while (searchState == SearchState.Searching);

      if (searchState == SearchState.Succeeded)
      {
        Console.WriteLine("Search found goal state");
        SticksNode node = astarsearch.GetSolutionStart() as SticksNode;

        Console.WriteLine( "Displaying solution");
        int steps = 0;
        for (; ; )
        {
          node = astarsearch.GetSolutionNext() as SticksNode;
          if (node == null)
          {
            break;
          }

          
          path[node.Coordinate.Y * map.Height + node.Coordinate.X] = true;
          //node.PrintNodeInfo();
          steps++;
        };
        for (int y = 0; y < map.Height; y++)
        {
          if (y % 2 == 1) Console.Write(" ");
          for (int x = 0; x < map.Width; x++)
          {
            if (x == start.Coordinate.X &&
              y == start.Coordinate.Y)
              Console.Write("S");
            else if (x == end.Coordinate.X &&
              y == end.Coordinate.Y)
              Console.Write("G");
            else
              Console.Write(path[y * map.Height + x] ? "x" : "o");
          }
          Console.WriteLine();
        }
        Console.WriteLine("Solution steps {0}", steps);
        // Once you're done with the solution you can free the nodes up
        astarsearch.FreeSolutionNodes();
      }
      else if (searchState == SearchState.Failed)
      {
        Console.WriteLine("Search terminated. Did not find goal state");

      }

      // Display the number of loops the search went through
      Console.WriteLine("searchSteps : " + searchSteps);

    }

    static void Main(String[] args)
    {
      astar(args);
      //_Main(args);
      //FindPath.main(args);
      Console.ReadLine();
    }
    static void _Main(string[] args)
    {
      string user = ConfigurationManager.AppSettings["user"];
      string token = ConfigurationManager.AppSettings["token"];
      string elizaUrl = ConfigurationManager.AppSettings["elizaUrl"];
      ElizaApi eliza = new ElizaApi(elizaUrl, user, token);
      SticksBot bot = new SticksBot(eliza);
      Thread worker = new Thread(bot.DoWork);
      worker.Start();
      while (!worker.IsAlive) ; // loop till he's not there
      Thread.CurrentThread.Join();
    }
  }
}
