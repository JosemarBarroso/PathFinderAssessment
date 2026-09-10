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
        // a Stack is a Linked List accessed form the front end

        private readonly LinkedList<T> _list = new LinkedList<T>();
        
        public void Push(T data)
        {
            _list.PushFront(data);
        }

        public T Pop()
        {
            T item;
            _list.PopFront(item);
            return item;
        }

        // TODO: Add other methods as necessary
    }
}
