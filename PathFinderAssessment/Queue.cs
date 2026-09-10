// COM 5113 Sample Code - Nick Mitchell 2025

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinderAssessment
{
    internal class Queue<T>
    {
        // A Queue is a Linked List accessed from both ends.
        // Queue behaviour is First-In, First-Out (FIFO).

        private readonly LinkedList<T> _list = new LinkedList<T>();

        // Adds an item to the rear of the queue.
        public void Enqueue(T data)
        {
            _list.PushBack(data);
        }

        // Removes and returns the item at the front of the queue.
        public T Dequeue()
        {
            return _list.PopFront();
        }

        // Returns true if the queue contains no items.
        public bool IsEmpty()
        {
            return _list.IsEmpty();
        }

        // Returns true if the supplied item exists in the queue.
        public bool Contains(T data)
        {
            return _list.Contains(data);
        }
    }
}