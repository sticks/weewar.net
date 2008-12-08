using System;
using System.Collections.Generic;
using System.Text;
using Eliza;

namespace SkippingRock.SticksBot
{
  class FindPath
  {

    // Global data

    // The world map

    const int MAP_WIDTH = 20;
    const int MAP_HEIGHT = 20;

    static int[] map =  
  {

// 0001020304050607080910111213141516171819
  	1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,   // 00
  	1,9,9,1,9,9,1,1,1,9,1,9,9,9,9,9,1,1,1,1,   // 01
  	1,1,9,1,1,9,9,9,1,9,1,9,1,9,1,9,9,9,1,1,   // 02
  	1,9,9,1,1,9,9,9,1,9,1,9,1,9,1,9,9,9,1,1,   // 03
  	1,1,1,1,1,1,9,9,1,9,1,9,1,1,1,1,9,9,1,1,   // 04
  	1,1,1,1,9,1,1,1,1,9,1,1,1,1,9,1,1,1,1,1,   // 05
  	1,9,9,9,9,1,1,1,1,1,1,9,9,9,9,1,1,1,1,1,   // 06
  	1,1,9,9,9,9,9,9,9,1,1,1,9,9,9,9,9,9,9,1,   // 07
  	1,9,1,1,1,1,1,1,1,1,1,9,1,1,1,1,1,1,1,1,   // 08
  	1,1,1,9,9,9,9,9,9,9,1,1,9,9,9,9,9,9,9,1,   // 09
  	1,1,1,1,1,1,9,1,1,9,1,1,1,1,1,1,1,1,1,1,   // 10
  	1,9,9,9,9,9,1,9,1,9,1,9,9,9,9,9,1,1,1,1,   // 11
  	1,9,1,9,1,9,9,9,1,9,1,9,1,9,1,9,9,9,1,1,   // 12
  	1,9,1,9,1,9,9,9,1,9,1,9,1,9,1,9,9,9,1,1,   // 13
  	1,9,1,1,1,1,9,9,1,9,1,9,1,1,1,1,9,9,1,1,   // 14
  	1,1,1,1,9,1,1,1,1,9,1,1,1,1,9,1,1,1,1,1,   // 15
  	1,9,9,9,9,1,1,1,1,1,1,9,9,9,9,1,1,1,1,1,   // 16
  	1,1,9,9,9,9,9,9,9,1,1,1,9,9,9,9,9,9,9,1,   // 17
  	1,9,1,1,1,1,1,1,1,1,1,9,1,1,1,1,1,1,1,1,   // 18
  	1,1,9,9,9,9,9,9,9,9,1,1,9,9,9,9,9,9,1,1,   // 19

  };

    // map helper functions

    static int GetMap(uint x, uint y)
    {

      if (x < 0 ||
          x >= MAP_WIDTH ||
         y < 0 ||
         y >= MAP_HEIGHT
        )
      {
        return 9;
      }

      return map[(y * MAP_WIDTH) + x];
    }



    // Definitions

    class MapSearchNode : PuzzleState
    {
      public uint x;	 // the (x,y) positions of the node
      public uint y;

      public MapSearchNode() { x = y = 0; }
      public MapSearchNode(uint px, uint py) { x = px; y = py; }

      // Here's the heuristic function that estimates the distance from a Node
      // to the Goal. 
      public float GoalDistanceEstimate(PuzzleState state)
      {
        MapSearchNode nodeGoal = state as MapSearchNode;
        float xd = (float)x - (float)nodeGoal.x;
        float yd = (float)y - (float)nodeGoal.y;

        return ((xd * xd) + (yd * yd));
      }
      public bool IsGoal(PuzzleState puzzleState)
      {
        MapSearchNode nodeGoal = puzzleState as MapSearchNode;
        if (nodeGoal == null) return false;
        if ((x == nodeGoal.x) &&
          (y == nodeGoal.y))
        {
          return true;
        }

        return false;
      }
      // This generates the successors to the given Node. It uses a helper function called
      // AddSuccessor to give the successors to the AStar class. The A* specific initialisation
      // is done for each node internally, so here you just set the state information that
      // is specific to the application
      public bool GetSuccessors(AStarSearch astarsearch, PuzzleState parentState)
      {
        MapSearchNode parent_node = parentState as MapSearchNode;

        int parent_x = -1;
        int parent_y = -1;

        if (parent_node != null)
        {
          parent_x = (int)parent_node.x;
          parent_y = (int)parent_node.y;
        }


        MapSearchNode NewNode;

        // push each possible move except allowing the search to go backwards

        if ((GetMap(x - 1, y) < 9)
          && !((parent_x == x - 1) && (parent_y == y))
          )
        {
          NewNode = new MapSearchNode(x - 1, y);
          astarsearch.AddSuccessor(NewNode);
        }

        if ((GetMap(x, y - 1) < 9)
          && !((parent_x == x) && (parent_y == y - 1))
          )
        {
          NewNode = new MapSearchNode(x, y - 1);
          astarsearch.AddSuccessor(NewNode);
        }

        if ((GetMap(x + 1, y) < 9)
          && !((parent_x == x + 1) && (parent_y == y))
          )
        {
          NewNode = new MapSearchNode(x + 1, y);
          astarsearch.AddSuccessor(NewNode);
        }


        if ((GetMap(x, y + 1) < 9)
          && !((parent_x == x) && (parent_y == y + 1))
          )
        {
          NewNode = new MapSearchNode(x, y + 1);
          astarsearch.AddSuccessor(NewNode);
        }

        return true;
      }

      // given this node, what does it cost to move to successor. In the case
      // of our map the answer is the map terrain value at this node since that is 
      // conceptually where we're moving

      public float GetCost(PuzzleState successor)
      {
        return (float)GetMap(x, y);
      }

      public bool Equals(PuzzleState prhs)
      {
        MapSearchNode rhs = prhs as MapSearchNode;
        // same state in a maze search is simply when (x,y) are the same
        if ((x == rhs.x) &&
          (y == rhs.y))
        {
          return true;
        }
        else
        {
          return false;
        }

      }

      public void PrintNodeInfo()
      {
        System.Console.WriteLine("Node position : ({0},{1})", x, y);
      }
    }

    // Main


    public static int main(string[] args)
    {

      Console.WriteLine("STL A* Search implementation\n(C)2001 Justin Heyes-Jones");

      // Our sample problem defines the world as a 2d array representing a terrain
      // Each element contains an integer from 0 to 5 which indicates the cost 
      // of travel across the terrain. Zero means the least possible difficulty 
      // in travelling (think ice rink if you can skate) whilst 5 represents the 
      // most difficult. 9 indicates that we cannot pass.

      // Create an instance of the search class...

      AStarSearch astarsearch = new AStarSearch();
      bool[] path = new bool[MAP_HEIGHT * MAP_WIDTH];

      // Create a start state
      MapSearchNode nodeStart = new MapSearchNode();
      nodeStart.x = 0;
      nodeStart.y = 0;

      // Define the goal state
      MapSearchNode nodeEnd = new MapSearchNode();
      nodeEnd.x = 19;
      nodeEnd.y = 19;

      // Set Start and goal states

      astarsearch.SetStartAndGoalStates(nodeStart, nodeEnd);

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
        Console.Write("Search found goal state");
        MapSearchNode node = astarsearch.GetSolutionStart() as MapSearchNode;

        Console.WriteLine( "Displaying solution");
        int steps = 0;
        node.PrintNodeInfo();
        for (; ; )
        {
          node = astarsearch.GetSolutionNext() as MapSearchNode;
          if (node == null)
          {
            break;
          }

          path[node.y * MAP_HEIGHT + node.x] = true;
          //node.PrintNodeInfo();
          steps++;
        };
        for (int y = 0; y < MAP_HEIGHT; y++)
        {
          for (int x = 0; x < MAP_WIDTH; x++)
          {
            Console.Write(path[y * MAP_HEIGHT + x] ? "x" : "o");
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

      return 0;
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  }
}
