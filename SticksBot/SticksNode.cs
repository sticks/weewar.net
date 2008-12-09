using System;
using System.Collections.Generic;
using System.Text;
using Eliza;

namespace SkippingRock.SticksBot
{
  class SticksNode : PuzzleState
  {
    private WeewarMap _map;
    private Unit _unit;
    private Coordinate _coord;

    public SticksNode(WeewarMap map, Unit unit) : this(map, unit.Coordinate, unit) { }
    public SticksNode(WeewarMap map, Coordinate coord, Unit unit)
    {
      _map = map;
      _coord = coord;
      _unit = unit;
    }
    public Coordinate Coordinate
    {
      get { return _coord; }
    }
    #region PuzzleState Members

    public float GoalDistanceEstimate(PuzzleState state)
    {
      SticksNode nodeGoal = state as SticksNode;
      float xd = (float)_coord.X - (float)nodeGoal._coord.X;
      float yd = (float)_coord.Y - (float)nodeGoal._coord.Y;

      return ((xd * xd) + (yd * yd));
    }

    public bool IsGoal(PuzzleState puzzleState)
    {
      SticksNode nodeGoal = puzzleState as SticksNode;
      if (nodeGoal == null) return false;
      return _coord == nodeGoal._coord;
    }

    public bool GetSuccessors(AStarSearch astarsearch, PuzzleState parentState)
    {
      SticksNode parent_node = parentState as SticksNode;

      Coordinate pcoord = parent_node == null ? new Coordinate(-1,-1) : parent_node._coord;
      Coordinate ncoord;

      // push each possible move except allowing the search to go backwards
      int[] xx = { 0, 1, 1, 0, -1, -1 };
      int[] yy = { 1, 0, -1, -1, 0, 1 };
      for (int i = 0; i < xx.Length; i++)
      {
        ncoord = _coord.Xform(xx[i], yy[i]);
        if (pcoord != ncoord && _map.ContainsKey(ncoord) && GetCost(ncoord) < 9)
        {
          astarsearch.AddSuccessor(new SticksNode(_map, ncoord, _unit));
        }
      }
      return true;
    }

    public int GetCost(Coordinate coord)
    {
      Terrain ter = _map.get(coord);
      return _unit.GetMoveCost(ter.Type);
    }

    public float GetCost(PuzzleState successor)
    {
      SticksNode node = successor as SticksNode;
      if (node == null) return 9;
      return GetCost(node._coord);
    }

    public bool Equals(PuzzleState rhs)
    {
      SticksNode n = rhs as SticksNode;
      if (n == null) return false;
      return n._coord == _coord;
    }

    #endregion
  }
}
