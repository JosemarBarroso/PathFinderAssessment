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


        // Counts how many times an item has been inserted
        // into the priority-ordered queue.
        //
        // This will later allow A* to report how many times
        // the Open List ordering operation was performed.
        public int SortCount { get; private set; }


        // Constructor
        public PriorityQueue(Comparison<T> comparison)
        {
            // Create the custom LinkedList used internally.
            _list = new LinkedList<T>();

            // Store the comparison method that determines
            // the priority of items.
            _comparison = comparison;

            // No ordering operations have taken place yet.
            SortCount = 0;
        }


        // Adds an item into the correct priority position.
        public void Enqueue(T data)
        {
            // Insert the item into its correct position
            // according to the supplied comparison.
            _list.InsertSorted(data, _comparison);

            // Record that the priority-ordered list
            // has been updated.
            SortCount++;
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


        // Finds the first item matching the supplied condition.
        //
        // This is needed by algorithms such as Dijkstra and A*
        // when checking whether a coordinate already exists
        // on the Open List.
        public T? Find(Predicate<T> condition)
        {
            return _list.Find(condition);
        }


        // Removes an item from the priority queue.
        //
        // This allows an existing node to be removed and
        // reinserted when a cheaper path to it is discovered.
        public bool Remove(T data)
        {
            return _list.Remove(data);
        }
    }
}