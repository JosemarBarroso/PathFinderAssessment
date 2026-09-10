// COM 5113 Sample Code - Nick Mitchell 2025

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinderAssessment
{
    internal class Stack<T>
    {
        // A Stack is a Linked List accessed from the front end.
        // Stack behaviour is Last-In, First-Out (LIFO).

        private readonly LinkedList<T> _list = new LinkedList<T>();

        // Adds an item to the top of the stack.
        public void Push(T data)
        {
            _list.PushFront(data);
        }

        // Removes and returns the item at the top of the stack.
        public T Pop()
        {
            return _list.PopFront();
        }

        // Returns true if the stack contains no items.
        public bool IsEmpty()
        {
            return _list.IsEmpty();
        }

        // Returns true if the supplied item exists in the stack.
        public bool Contains(T data)
        {
            return _list.Contains(data);
        }
    }
}