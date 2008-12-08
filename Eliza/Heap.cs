using System;
using System.Collections.Generic;
using System.Text;

namespace Eliza
{
  /// <summary>
  /// The Heap allows to maintain a list sorted as long as needed.
  /// If no IComparer interface has been provided at construction, then the list expects the Objects to implement IComparer.
  /// If the list is not sorted it behaves like an ordinary list.
  /// When sorted, the list's "Add" method will put new objects at the right place.
  /// As well the "Contains" and "IndexOf" methods will perform a binary search.
  public class Heap<T> : IList<T>
  {
    #region Private Members

    private List<T> _list;
    private IComparer<T> _compareFunc = null;
    private bool _useObjectComparison;

    #endregion

    #region Constructors

    /// <summary>
    /// Default constructor.
    /// Since no IComparer is provided here, added objects must implement the IComparer interface.
    /// </summary>
    public Heap()
    {
      InitProperties(null, 0);
    }

    /// <summary>
    /// Constructor.
    /// Since no IComparer is provided, added objects must implement the IComparer interface.
    /// </summary>
    /// <param name="Capacity">Capacity of the list (<see cref="ArrayList.Capacity">ArrayList.Capacity</see>)</param>
    public Heap(int Capacity)
    {
      InitProperties(null, Capacity);
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="Comparer">Will be used to compare added elements for sort and search operations.</param>
    public Heap(IComparer<T> Comparer)
    {
      InitProperties(Comparer, 0);
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="Comparer">Will be used to compare added elements for sort and search operations.</param>
    /// <param name="Capacity">Capacity of the list (<see cref="ArrayList.Capacity">ArrayList.Capacity</see>)</param>
    public Heap(IComparer<T> Comparer, int Capacity)
    {
      InitProperties(Comparer, Capacity);
    }

    #endregion

    #region Properties

    /// <summary>
    /// If set to true, it will not be possible to add an object to the list if its value is already in the list.
    /// </summary>
    public bool AddDuplicates
    {
      set
      {
        _addDups = value;
      }
      get
      {
        return _addDups;
      }
    }
    private bool _addDups;

    /// <summary>
    /// Idem <see cref="ArrayList">ArrayList</see>
    /// </summary>
    public int Capacity
    {
      get
      {
        return _list.Capacity;
      }
      set
      {
        _list.Capacity = value;
      }
    }

    #endregion

    #region IList Members

    /// <summary>
    /// IList implementation.
    /// Gets object's value at a specified index.
    /// The set operation is impossible on a Heap.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Index is less than zero or Index is greater than Count.</exception>
    /// <exception cref="InvalidOperationException">[] operator cannot be used to set a value on a Heap.</exception>
    public T this[int Index]
    {
      get
      {
        if (Index >= _list.Count || Index < 0)
          throw new ArgumentOutOfRangeException("Index is less than zero or Index is greater than Count.");
        return _list[Index];
      }
      set
      {
        throw new InvalidOperationException("[] operator cannot be used to set a value in a Heap.");
      }
    }

    /// <summary>
    /// IList implementation.
    /// Adds the object at the right place.
    /// </summary>
    /// <param name="O">The object to add.</param>
    /// <returns>The index where the object has been added.</returns>
    /// <exception cref="ArgumentException">The Heap is set to use object's IComparable interface, and the specifed object does not implement this interface.</exception>
    public void Add(T O)
    {
      if (ObjectIsCompliant(O))
      {
        int index = IndexOf(O);
        int newIndex = index >= 0 ? index : -index - 1;
        if (newIndex >= Count) _list.Add(O);
        else _list.Insert(newIndex, O);
      }
    }

    /// <summary>
    /// IList implementation.
    /// Search for a specified object in the list.
    /// If the list is sorted, a <see cref="ArrayList.BinarySearch">BinarySearch</see> is performed using IComparer interface.
    /// Else the <see cref="Equals">Object.Equals</see> implementation is used.
    /// </summary>
    /// <param name="O">The object to look for</param>
    /// <returns>true if the object is in the list, otherwise false.</returns>
    public bool Contains(T O)
    {
      return _list.BinarySearch(O, _compareFunc) >= 0;
    }

    /// <summary>
    /// IList implementation.
    /// Returns the index of the specified object in the list.
    /// If the list is sorted, a <see cref="ArrayList.BinarySearch">BinarySearch</see> is performed using IComparer interface.
    /// Else the <see cref="Equals">Object.Equals</see> implementation of objects is used.
    /// </summary>
    /// <param name="O">The object to locate.</param>
    /// <returns>
    /// If the object has been found, a positive integer corresponding to its position.
    /// If the objects has not been found, a negative integer which is the bitwise complement of the index of the next element.
    /// </returns>
    public int IndexOf(T O)
    {
      int Result = -1;
      Result = _list.BinarySearch(O, _compareFunc);
      while (Result > 0 && _list[Result - 1].Equals(O))
        Result--;
      return Result;
    }

    /// <summary>
    /// IList implementation.
    /// Idem <see cref="ArrayList">ArrayList</see>
    /// </summary>
    public bool IsReadOnly
    {
      get
      {
        return false; // return _list.IsReadOnly;
      }
    }

    /// <summary>
    /// IList implementation.
    /// Idem <see cref="ArrayList">ArrayList</see>
    /// </summary>
    public void Clear()
    {
      _list.Clear();
    }

    /// <summary>
    /// IList implementation.
    /// Cannot be used on a Heap.
    /// </summary>
    /// <param name="Index">The index before which the object must be added.</param>
    /// <param name="O">The object to add.</param>
    /// <exception cref="InvalidOperationException">Insert method cannot be called on a Heap.</exception>
    public void Insert(int Index, T O)
    {
      throw new InvalidOperationException("Insert method cannot be called on a Heap.");
    }

    /// <summary>
    /// IList implementation.
    /// Idem <see cref="ArrayList">ArrayList</see>
    /// </summary>
    /// <param name="Value">The object whose value must be removed if found in the list.</param>
    public bool Remove(T Value)
    {
      return _list.Remove(Value);
    }

    /// <summary>
    /// IList implementation.
    /// Idem <see cref="ArrayList">ArrayList</see>
    /// </summary>
    /// <param name="Index">Index of object to remove.</param>
    public void RemoveAt(int Index)
    {
      _list.RemoveAt(Index);
    }

    #endregion

    #region IList.ICollection Members

    /// <summary>
    /// IList.ICollection implementation.
    /// Idem <see cref="ArrayList">ArrayList</see>
    /// </summary>
    /// <param name="array"></param>
    /// <param name="arrayIndex"></param>
    public void CopyTo(T[] array, int arrayIndex) { _list.CopyTo(array, arrayIndex); }

    /// <summary>
    /// IList.ICollection implementation.
    /// Idem <see cref="ArrayList">ArrayList</see>
    /// </summary>
    public int Count { get { return _list.Count; } }

    /// <summary>
    /// IList.ICollection implementation.
    /// Idem <see cref="ArrayList">ArrayList</see>
    /// </summary>
    /// 
    public bool IsSynchronized { get { return false; /* _list.IsSynchronized;*/ } }

#endregion

    #region IList.IEnumerable Members

    /// <summary>
    /// IList.IEnumerable implementation.
    /// Idem <see cref="ArrayList">ArrayList</see>
    /// </summary>
    /// <returns>Enumerator on the list.</returns>
    public IEnumerator<T> GetEnumerator()
    {
      return _list.GetEnumerator();
    }

    #endregion

#region Delegate Members

    /// <summary>
    /// Defines an equality for two objects
    /// </summary>
    public delegate bool Equality(object Object1, object Object2);

    #endregion

    #region Overridden Members

    /// <summary>
    /// Object.ToString() override.
    /// Build a string to represent the list.
    /// </summary>
    /// <returns>The string refecting the list.</returns>
    public override string ToString()
    {
      string ret = "{";
      for (int i = 0; i < _list.Count; i++)
        ret += _list[i].ToString() + (i != _list.Count - 1 ? "; " : "}");
      return ret;
    }

    /// <summary>
    /// Object.Equals() override.
    /// </summary>
    /// <returns>true if object is equal to this, otherwise false.</returns>
    public override bool Equals(object obj)
    {
      Heap<T> rhs = (Heap<T>)obj;
      if (rhs.Count != Count)
        return false;
      for (int i = 0; i < Count; i++)
        if (!rhs[i].Equals(this[i]))
          return false;
      return true;
    }

    /// <summary>
    /// Object.GetHashCode() override.
    /// </summary>
    /// <returns>Hash code for this.</returns>
    public override int GetHashCode()
    {
      return _list.GetHashCode();
    }

    #endregion

    #region Public Members

    /// <summary>
    /// Idem IndexOf(object), but starting at a specified position in the list
    /// </summary>
    /// <param name="Object">The object to locate.</param>
    /// <param name="Start">The index for start position.</param>
    /// <returns></returns>
    public int IndexOf(T t, int start)
    {
      int Result = -1;
      Result = _list.BinarySearch(start, _list.Count - start, t, _compareFunc);
      while (Result > start && _list[Result - 1].Equals(t))
        Result--;
      return Result;
    }

    /// <summary>
    /// Idem IndexOf(object), but with a specified equality function
    /// </summary>
    /// <param name="Object">The object to locate.</param>
    /// <param name="AreEqual">Equality function to use for the search.</param>
    /// <returns></returns>
    public int IndexOf(T t, Equality areEqual)
    {
      for (int i = 0; i < _list.Count; i++)
        if (areEqual(_list[i], t)) return i;
      return -1;
    }

    /// <summary>
    /// Idem IndexOf(object), but with a start index and a specified equality function
    /// </summary>
    /// <param name="Object">The object to locate.</param>
    /// <param name="Start">The index for start position.</param>
    /// <param name="AreEqual">Equality function to use for the search.</param>
    /// <returns></returns>
    public int IndexOf(T t, int start, Equality areEqual)
    {
      if (start < 0 || start >= _list.Count) throw new ArgumentException("Start index must belong to [0; Count-1].");
      for (int i = start; i < _list.Count; i++)
        if (areEqual(_list[i], t)) return i;
      return -1;
    }

    /// <summary>
    /// The objects will be added at the right place.
    /// </summary>
    /// <param name="C">The object to add.</param>
    /// <returns>The index where the object has been added.</returns>
    /// <exception cref="ArgumentException">The Heap is set to use object's IComparable interface, and the specifed object does not implement this interface.</exception>
    public void AddRange(ICollection<T> collection)
    {
      foreach (T t in collection)
        Add(t);
    }

    /// <summary>
    /// Cannot be called on a Heap.
    /// </summary>
    /// <param name="Index">The index before which the objects must be added.</param>
    /// <param name="C">The object to add.</param>
    /// <exception cref="InvalidOperationException">Insert cannot be called on a Heap.</exception>
    public void InsertRange(int Index, ICollection<T> C)
    {
      throw new InvalidOperationException("Insert cannot be called on a Heap.");
    }

    /// <summary>
    /// Limits the number of occurrences of a specified value.
    /// Same values are equals according to the Equals() method of objects in the list.
    /// The first occurrences encountered are kept.
    /// </summary>
    /// <param name="Value">Value whose occurrences number must be limited.</param>
    /// <param name="NumberToKeep">Number of occurrences to keep</param>
    public void LimitOccurrences(T t, int numberToKeep)
    {
      if (t == null)
        throw new ArgumentNullException("Value");
      int Pos = 0;
      while ((Pos = IndexOf(t, Pos)) >= 0)
      {
        if (numberToKeep <= 0)
          _list.RemoveAt(Pos);
        else
        {
          Pos++;
          numberToKeep--;
        }
        if (_compareFunc.Compare(_list[Pos], t) > 0)
          break;
      }
    }

    /// <summary>
    /// Removes all duplicates in the list.
    /// Each value encountered will have only one representant.
    /// </summary>
    public void RemoveDuplicates()
    {
      int pos;
      pos = 0;
      while (pos < Count - 1)
      {
        if (_compareFunc.Compare(this[pos], this[pos + 1]) == 0)
          RemoveAt(pos);
        else
          pos++;
      }
    }

    /// <summary>
    /// Returns the object of the list whose value is minimum
    /// </summary>
    /// <returns>The minimum object in the list</returns>
    public int IndexOfMin()
    {
      int ret = -1;
      if (_list.Count > 0)
      {
        ret = 0;
      }
      return ret;
    }

    /// <summary>
    /// Returns the object of the list whose value is maximum
    /// </summary>
    /// <returns>The maximum object in the list</returns>
    public int IndexOfMax()
    {
      int ret = -1;
      if (_list.Count > 0)
      {
        ret = _list.Count - 1;
      }
      return ret;
    }

    /// <summary>
    /// Returns the topmost object on the list and removes it from the list
    /// </summary>
    /// <returns>Returns the topmost object on the list</returns>
    public T Pop()
    {
      if (_list.Count == 0)
        throw new InvalidOperationException("The heap is empty.");
      T t = _list[Count - 1];
      _list.RemoveAt(Count - 1);
      return (t);
    }

    /// <summary>
    /// Pushes an object on list. It will be inserted at the right spot.
    /// </summary>
    /// <param name="Object">Object to add to the list</param>
    /// <returns></returns>
    public void Push(T t)
    {
      Add(t);
    }

    #endregion

    #region Private Members

    private bool ObjectIsCompliant(T t)
    {
      if (_useObjectComparison && !(t is IComparable<T>))
        throw new ArgumentException("The Heap is set to use the IComparable interface of objects, and the object to add does not implement the IComparable interface.");
      if (!_addDups && Contains(t))
        return false;
      return true;
    }

    private class Comparison : IComparer<T>
    {
      public int Compare(T Object1, T Object2)
      {
        IComparable<T> C = Object1 as IComparable<T>;
        return C.CompareTo(Object2);
      }
    }

    private void InitProperties(IComparer<T> comparer, int capacity)
    {
      if (comparer != null)
      {
        _compareFunc = comparer;
        _useObjectComparison = false;
      }
      else
      {
        _compareFunc = new Comparison();
        _useObjectComparison = true;
      }
      _list = capacity > 0 ? new List<T>(capacity) : new List<T>();
      _addDups = true;
    }

    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    #endregion
  }
}
