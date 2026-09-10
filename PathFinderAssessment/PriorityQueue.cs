// COM 5113 Pathfinding Assessment

using System;

namespace PathFinderAssessment
{
    // Generic PriorityQueue implemented using the custom LinkedList.
    //
    // The item with the highest priority is kept at the front.
    // Priority order is decided by the Comparison<T> supplied
    // when the PriorityQueue is created.
    internal class PriorityQueue<T>
    {
        // Custom LinkedList used internally.
        private readonly LinkedList<T> _list;

        // Comparison method used to determine priority.
        private readonly Comparison<T> _comparison;


        // Constructor
        public PriorityQueue(Comparison<T> comparison)
        {
            _list = new LinkedList<T>();
            _comparison = comparison;
        }


        // Adds an item into the correct priority position.
        public void Enqueue(T data)
        {
            _list.InsertSorted(data, _comparison);
        }


        // Removes and returns the highest-priority item.
        //
        // Because InsertSorted keeps the best item at the front,
        // we remove from the front of the LinkedList.
        public T Dequeue()
        {
            return _list.PopFront();
        }


        // Returns true when the queue is empty.
        public bool IsEmpty()
        {
            return _list.IsEmpty();
        }


        // Checks whether the queue contains the supplied item.
        public bool Contains(T data)
        {
            return _list.Contains(data);
        }


        // Returns the number of items currently stored.
        public int Count()
        {
            return _list.Count();
        }
    }
}