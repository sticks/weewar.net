using System;
using System.Collections.Generic;
using System.Text;

namespace Eliza
{
  public interface PuzzleState
  {
    /// <summary>
    /// Return the estimated cost to goal from this node
    /// </summary>
    /// <param name="?"></param>
    /// <returns></returns>
    float GoalDistanceEstimate(PuzzleState nodeGoal);

    /// <summary>
    /// Return true if this node is the goal
    /// </summary>
    /// <param name="?"></param>
    /// <returns></returns>
    bool IsGoal(PuzzleState nodeGoal);

    /// <summary>
    /// For each successor to this state call the AStarSearch's AddSuccessor call to 
    /// add each one to the current search
    /// </summary>
    /// <param name="astarsearch"></param>
    bool GetSuccessors(AStarSearch astarsearch, PuzzleState state);

    /// <summary>
    /// Return the cost moving from this state to the state of successor
    /// </summary>
    /// <param name="successor"></param>
    /// <returns></returns>
    float GetCost(PuzzleState successor);

    /// <summary>
    /// Return true if the provided state is the same as this state
    /// </summary>
    /// <param name="rhs"></param>
    /// <returns></returns>
    bool Equals(PuzzleState rhs);
  }

  public enum SearchState
  {
    NotInitialized,
    Searching,
    Succeeded,
    Failed,
    OutOfMemory,
    Invalid
  };

  public class Node : IComparable<Node>
  {
    private PuzzleState m_userstate;

    public Node parent; // used during the search to record the parent of successor nodes
    public Node child; // used after the search for the application to view the search in reverse

    public float g; // cost of this node + it's predecessors
    public float h; // heuristic estimate of distance to goal
    public float f; // sum of cumulative cost of predecessors and self and heuristic

    public Node()
    {
      parent = null;
      child = null;
      g = 0.0f;
      h = 0.0f;
      f = 0.0f;
    }

    public PuzzleState PuzzleState
    {
      get { return m_userstate; }
      set { m_userstate = value; }
    }

    #region IComparable<Node> Members

    public int CompareTo(Node other)
    {
      if (this.f == other.f) return 0;
      else if (this.f < other.f) return -1;
      else return 1;
    }

    #endregion
  };

  public class AStarSearch
  {

    private int m_maxNodes;

    // Heap (simple vector but used as a heap, cf. Steve Rabin's game gems article)
    Heap<Node> m_OpenList = new Heap<Node>();

    // Closed list is a vector.
    List<Node> m_ClosedList = new List<Node>();

    // Successors is a vector filled out by the user each type successors to a node
    // are generated
    List<Node> m_Successors = new List<Node>();

    // State
    SearchState m_State;

    // Counts steps
    int m_Steps;

    // Start and goal state pointers
    Node m_Start;
    Node m_Goal;

    Node m_CurrentSolutionNode;

    // Memory
    //FixedSizeAllocator<Node> m_FixedSizeAllocator;

    //Debug : need to keep these two iterators around
    // for the user Dbg functions
    //typename vector< Node * >::iterator iterDbgOpen;
    //typename vector< Node * >::iterator iterDbgClosed;

    // debugging : count memory allocation and free's
    //int m_AllocateNodeCount;
    int m_FreeNodeCount;

    bool m_CancelRequest;

    public AStarSearch()
      : this(1000)
    {
    }
    public AStarSearch(int maxNodes)
    {
      m_maxNodes = maxNodes;
    }
    // call at any time to cancel the search and free up all the memory
    public void CancelSearch()
    {
      m_CancelRequest = true;
    }

    // Set Start and goal states
    public void SetStartAndGoalStates(PuzzleState Start, PuzzleState Goal)
    {
      m_CancelRequest = false;

      m_Start = AllocateNode();
      m_Goal = AllocateNode();

      m_Start.PuzzleState = Start;
      m_Goal.PuzzleState = Goal;

      m_State = SearchState.Searching;

      // Initialise the AStar specific parts of the Start Node
      // The user only needs fill out the state information

      m_Start.g = 0;
      m_Start.h = m_Start.PuzzleState.GoalDistanceEstimate(m_Goal.PuzzleState);
      m_Start.f = m_Start.g + m_Start.h;
      m_Start.parent = null;

      // Push the start node on the Open list

      m_OpenList.Add(m_Start);

      // Initialise counter for search steps
      m_Steps = 0;
    }

    // Advances search one step 
    public SearchState SearchStep()
    {
      // Next I want it to be safe to do a searchstep once the search has succeeded...
      if ((m_State == SearchState.Succeeded) ||
        (m_State == SearchState.Failed)
        )
      {
        return m_State;
      }

      // Failure is defined as emptying the open list as there is nothing left to 
      // search...
      // New: Allow user abort
      if (m_OpenList.Count == 0 || m_CancelRequest)
      {
        FreeAllNodes();
        m_State = SearchState.Failed;
        return m_State;
      }

      // Incremement step count
      m_Steps++;

      // Pop the best node (the one with the lowest f) 
      Node n = m_OpenList[0];
      m_OpenList.RemoveAt(0);

      // Check for the goal, once we pop that we're done
      if (n.PuzzleState.IsGoal(m_Goal.PuzzleState))
      {
        // The user is going to use the Goal Node he passed in 
        // so copy the parent pointer of n 
        m_Goal.parent = n.parent;

        // A special case is that the goal was passed in as the start state
        // so handle that here
        if (n != m_Start)
        {
          //delete n;
          FreeNode(n);

          // set the child pointers in each node (except Goal which has no child)
          Node nodeChild = m_Goal;
          Node nodeParent = m_Goal.parent;

          do
          {
            nodeParent.child = nodeChild;

            nodeChild = nodeParent;
            nodeParent = nodeParent.parent;

          }
          while (nodeChild != m_Start); // Start is always the first node by definition

        }

        // delete nodes that aren't needed for the solution
        FreeUnusedNodes();

        m_State = SearchState.Succeeded;

        return m_State;
      }
      else // not goal
      {

        // We now need to generate the successors of this node
        // The user helps us to do this, and we keep the new nodes in
        // m_Successors ...

        m_Successors.Clear(); // empty vector of successor nodes to n

        // User provides this functions and uses AddSuccessor to add each successor of
        // node 'n' to m_Successors
        bool ret = n.PuzzleState.GetSuccessors(this, n.parent == null ? null : n.parent.PuzzleState);

        if (!ret)
        {

          // free the nodes that may previously have been added 
          m_Successors.Clear(); // empty vector of successor nodes to n

          // free up everything else we allocated
          FreeAllNodes();

          m_State = SearchState.OutOfMemory;
          return m_State;
        }

        // Now handle each successor to the current node ...
        foreach (Node successor in m_Successors)
        {

          // 	The g value for this successor ...
          float newg = n.g + n.PuzzleState.GetCost(successor.PuzzleState);

          // Now we need to find whether the node is on the open or closed lists
          // If it is but the node that is already on them is better (lower g)
          // then we can forget about this successor

          // First linear search of open list to find node

          Node openlist_result = null;
          foreach (Node open in m_OpenList)
          {
            if (open.PuzzleState.Equals(successor.PuzzleState))
            {
              openlist_result = open;
              break;
            }
          }

          if (openlist_result != null)
          {

            // we found this state on open

            if (openlist_result.g <= newg)
            {
              FreeNode(successor);

              // the one on Open is cheaper than this one
              continue;
            }
          }

          Node closedlist_result = null;
          foreach (Node closed in m_ClosedList)
          {
            if (closed.PuzzleState.Equals(successor.PuzzleState))
            {
              closedlist_result = closed;
              break;
            }
          }

          if (closedlist_result != null)
          {

            // we found this state on closed

            if (closedlist_result.g <= newg)
            {
              // the one on Closed is cheaper than this one
              FreeNode(successor);

              continue;
            }
          }

          // This node is the best node so far with this particular state
          // so lets keep it and set up its AStar specific data ...

          successor.parent = n;
          successor.g = newg;
          successor.h = successor.PuzzleState.GoalDistanceEstimate(m_Goal.PuzzleState);
          successor.f = successor.g + successor.h;

          // Remove successor from closed if it was on it

          if (closedlist_result != null)
          {
            // remove it from Closed
            FreeNode(closedlist_result);
            m_ClosedList.Remove(closedlist_result);
          }

          // Update old version of this node
          if (openlist_result != null)
          {

            FreeNode(openlist_result);
            m_OpenList.Remove(openlist_result);

          }

          // heap now unsorted
          m_OpenList.Add(successor);

        }

        // push n onto Closed, as we have expanded it now

        m_ClosedList.Add(n);

      } // end else (not goal so expand)

      return m_State; // Succeeded bool is false at this point. 

    }

    // User calls this to add a successor to a list of successors
    // when expanding the search frontier
    public bool AddSuccessor(PuzzleState State)
    {
      Node node = AllocateNode();

      if (node != null)
      {
        node.PuzzleState = State;

        m_Successors.Add(node);

        return true;
      }

      return false;
    }

    // Free the solution nodes
    // This is done to clean up all used Node memory when you are done with the
    // search
    public void FreeSolutionNodes()
    {
      Node n = m_Start;

      if (m_Start.child != null)
      {
        do
        {
          Node del = n;
          n = n.child;
          FreeNode(del);

          del = null;

        } while (n != m_Goal);

        FreeNode(n); // Delete the goal

      }
      else
      {
        // if the start node is the solution we need to just delete the start and goal
        // nodes
        FreeNode(m_Start);
        FreeNode(m_Goal);
      }

    }

    // Functions for traversing the solution

    // Get start node
    public PuzzleState GetSolutionStart()
    {
      m_CurrentSolutionNode = m_Start;
      if (m_Start != null)
      {
        return m_Start.PuzzleState;
      }
      else
      {
        return null;
      }
    }

    // Get next node
    public PuzzleState GetSolutionNext()
    {
      if (m_CurrentSolutionNode != null)
      {
        if (m_CurrentSolutionNode.child != null)
        {

          Node child = m_CurrentSolutionNode.child;

          m_CurrentSolutionNode = m_CurrentSolutionNode.child;

          return child.PuzzleState;
        }
      }

      return null;
    }

    // Get end node
    PuzzleState GetSolutionEnd()
    {
      m_CurrentSolutionNode = m_Goal;
      if (m_Goal != null)
      {
        return m_Goal.PuzzleState;
      }
      else
      {
        return null;
      }
    }

    // Step solution iterator backwards
    PuzzleState GetSolutionPrev()
    {
      if (m_CurrentSolutionNode != null)
      {
        if (m_CurrentSolutionNode.parent != null)
        {

          Node parent = m_CurrentSolutionNode.parent;

          m_CurrentSolutionNode = m_CurrentSolutionNode.parent;

          return parent.PuzzleState;
        }
      }

      return null;
    }

    // For educational use and debugging it is useful to be able to view
    // the open and closed list at each step, here are two functions to allow that.

    /*
     * * PuzzleState GetOpenListStart()
	{
		float f,g,h;
		return GetOpenListStart( f,g,h );
	}

     * PuzzleState GetOpenListStart( ref float f, ref float g, ref float h )
	{
		iterDbgOpen = m_OpenList.begin();
		if( iterDbgOpen != m_OpenList.end() )
		{
			f = (*iterDbgOpen).f;
			g = (*iterDbgOpen).g;
			h = (*iterDbgOpen).h;
			return &(*iterDbgOpen).PuzzleState;
		}

		return null;
	}

	PuzzleState GetOpenListNext()
	{
		float f,g,h;
		return GetOpenListNext( f,g,h );
	}

	PuzzleState GetOpenListNext( ref float f, ref float g, ref float h )
	{
		iterDbgOpen++;
		if( iterDbgOpen != m_OpenList.end() )
		{
			f = (*iterDbgOpen).f;
			g = (*iterDbgOpen).g;
			h = (*iterDbgOpen).h;
			return &(*iterDbgOpen).PuzzleState;
		}

		return null;
	}

	PuzzleState GetClosedListStart()
	{
		float f,g,h;
		return GetClosedListStart( f,g,h );
	}

	PuzzleState GetClosedListStart( ref float f, ref float g, ref float h )
	{
		iterDbgClosed = m_ClosedList.begin();
		if( iterDbgClosed != m_ClosedList.end() )
		{
			f = (*iterDbgClosed).f;
			g = (*iterDbgClosed).g;
			h = (*iterDbgClosed).h;

			return &(*iterDbgClosed).PuzzleState;
		}

		return null;
	}

	PuzzleState GetClosedListNext()
	{
		float f,g,h;
		return GetClosedListNext( f,g,h );
	}

	PuzzleState GetClosedListNext( ref float f, ref float g, ref float h )
	{
		iterDbgClosed++;
		if( iterDbgClosed != m_ClosedList.end() )
		{
			f = (*iterDbgClosed).f;
			g = (*iterDbgClosed).g;
			h = (*iterDbgClosed).h;

			return &(*iterDbgClosed).PuzzleState;
		}

		return null;
	}
     * * * */

    // Get the number of steps

    int GetStepCount() { return m_Steps; }

    // This is called when a search fails or is cancelled to free all used
    // memory 
    private void FreeAllNodes()
    {
      // iterate open list and delete all nodes

      m_OpenList.Clear();

      // iterate closed list and delete unused nodes
      m_ClosedList.Clear();
    }


    // This call is made by the search class when the search ends. A lot of nodes may be
    // created that are still present when the search ends. They will be deleted by this 
    // routine once the search ends
    void FreeUnusedNodes()
    {
      // iterate open list and delete unused nodes
      for (int i = 0; i < m_OpenList.Count; i++)
      {
        Node n = m_OpenList[i];
        if (n.child == null)
        {
          FreeNode(n);
          m_OpenList.RemoveAt(i);
        }
      }

      m_OpenList.Clear();

      // iterate closed list and delete unused nodes
      for (int i=0;i<m_ClosedList.Count;i++)
      {
        Node n = m_ClosedList[i];
        if (n.child == null)
        {
          FreeNode(n);
          m_ClosedList.RemoveAt(i);
        }
      }

      m_ClosedList.Clear();

    }

    // Node memory management
    Node AllocateNode()
    {

#if !USE_FSA_MEMORY
      Node p = new Node();
      return p;
#else
		Node *address = m_FixedSizeAllocator.alloc();

		if( !address )
		{
			return null;
		}
    m_AllocateNodeCount ++;
		Node *p = new (address) Node;
		return p;
#endif
    }

    void FreeNode(Node node)
    {

      m_FreeNodeCount++;

#if !USE_FSA_MEMORY
      // delete node
#else
		m_FixedSizeAllocator.free( node );
#endif
    }

  }

}
