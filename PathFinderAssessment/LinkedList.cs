// COM 5113 Sample Code - Nick Mitchell 2025

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PathFinderAssessment
{
    // Generic linked list class
    public class LinkedList<T>
    {
        // Only LinkedLists should know about Elements,
        // so Element is kept as a private class.
        private class Element<U>
        {
            // Data stored in this element.
            public U Data { get; }

            // Reference to the next element in the linked list.
            public Element<U>? Next { get; set; }

            // Constructor for a new element.
            public Element(U data)
            {
                Data = data;
                Next = null;
            }
        }

        // The head element of the list.
        // It will be null when the linked list is empty.
        private Element<T>? _head;

        // Constructor
        public LinkedList()
        {
            _head = null;
        }

        // Returns true when the linked list contains no elements.
        public bool IsEmpty()
        {
            return _head == null;
        }

        // Adds a new item to the front of the linked list.
        public void PushFront(T data)
        {
            // Create a new element containing the supplied data.
            Element<T> newElement = new Element<T>(data);

            // The new element points to the current head.
            newElement.Next = _head;

            // The new element now becomes the head of the list.
            _head = newElement;
        }

        // Adds a new item to the end of the linked list.
        public void PushBack(T data)
        {
            // Create the new element.
            Element<T> newElement = new Element<T>(data);

            // If the list is empty, the new element becomes the head.
            if (_head == null)
            {
                _head = newElement;
                return;
            }

            // Otherwise, move through the list until the last element.
            Element<T> currentElement = _head;

            while (currentElement.Next != null)
            {
                currentElement = currentElement.Next;
            }

            // Link the final element to the new element.
            currentElement.Next = newElement;
        }

        // Removes and returns the first item in the linked list.
        public T PopFront()
        {
            // A value cannot be removed from an empty list.
            if (_head == null)
            {
                throw new InvalidOperationException("Cannot remove an item from an empty linked list.");
            }

            // Store the data from the current head.
            T data = _head.Data;

            // Move the head to the next element.
            _head = _head.Next;

            // Return the removed data.
            return data;
        }

        // Removes and returns the last item in the linked list.
        public T PopBack()
        {
            // A value cannot be removed from an empty list.
            if (_head == null)
            {
                throw new InvalidOperationException("Cannot remove an item from an empty linked list.");
            }

            // If there is only one element,
            // store its data and make the list empty.
            if (_head.Next == null)
            {
                T data = _head.Data;
                _head = null;
                return data;
            }

            // Move through the list until currentElement
            // is the element immediately before the final element.
            Element<T> currentElement = _head;

            while (currentElement.Next != null &&
                   currentElement.Next.Next != null)
            {
                currentElement = currentElement.Next;
            }

            // Store the data from the final element.
            T lastData = currentElement.Next!.Data;

            // Remove the final element.
            currentElement.Next = null;

            // Return the removed data.
            return lastData;
        }

        // Contains - returns true when data is contained in the list.
        //
        // NOTE:
        // Because this is a generic LinkedList which can store ANY type,
        // we should use EqualityComparer<T>.Default.Equals(...)
        // instead of assuming the == operator is available.
        public bool Contains(T data)
        {
            Element<T>? currentElement = _head;
            bool found = false;

            // Continue until either:
            // 1. the item is found, or
            // 2. the end of the list is reached.
            while (currentElement != null && !found)
            {
                if (EqualityComparer<T>.Default.Equals(currentElement.Data, data))
                {
                    found = true;
                }

                currentElement = currentElement.Next;
            }

            return found;
        }

        // Returns the number of elements currently stored in the list.
        public int Count()
        {
            int count = 0;
            Element<T>? currentElement = _head;

            // Traverse the entire list.
            while (currentElement != null)
            {
                count++;
                currentElement = currentElement.Next;
            }

            return count;
        }

        // Removes all elements from the linked list.
        public void Clear()
        {
            // Removing the reference to the head makes the whole list
            // unreachable, so the garbage collector can reclaim it.
            _head = null;
        }

        // Inserts an item into the linked list in sorted order.
        //
        // The Comparison<T> supplied by the caller decides
        // which item should appear before another item.
        public void InsertSorted(T data, Comparison<T> comparison)
        {
            // Create the new element.
            Element<T> newElement = new Element<T>(data);

            // If the list is empty, the new element becomes the head.
            if (_head == null)
            {
                _head = newElement;
                return;
            }

            // If the new item should appear before the current head,
            // insert it at the front.
            if (comparison(data, _head.Data) < 0)
            {
                newElement.Next = _head;
                _head = newElement;
                return;
            }

            // Otherwise move through the list until the correct
            // insertion position is found.
            Element<T> currentElement = _head;

            while (currentElement.Next != null &&
                   comparison(data, currentElement.Next.Data) >= 0)
            {
                currentElement = currentElement.Next;
            }

            // Insert the new element into its sorted position.
            newElement.Next = currentElement.Next;
            currentElement.Next = newElement;
        }

        // Finds and returns the first item that matches the supplied condition.
        // Returns default if no matching item exists.
        public T? Find(Predicate<T> condition)
        {
            Element<T>? currentElement = _head;

            while (currentElement != null)
            {
                if (condition(currentElement.Data))
                {
                    return currentElement.Data;
                }

                currentElement = currentElement.Next;
            }

            return default;
        }


        // Removes the first occurrence of the supplied item.
        // Returns true if the item was removed.
        public bool Remove(T data)
        {
            // Nothing can be removed from an empty list.
            if (_head == null)
            {
                return false;
            }

            // Check whether the head contains the item.
            if (EqualityComparer<T>.Default.Equals(_head.Data, data))
            {
                _head = _head.Next;
                return true;
            }

            Element<T> currentElement = _head;

            // Search for the element immediately before the item
            // that needs to be removed.
            while (currentElement.Next != null)
            {
                if (EqualityComparer<T>.Default.Equals(
                    currentElement.Next.Data,
                    data))
                {
                    currentElement.Next = currentElement.Next.Next;
                    return true;
                }

                currentElement = currentElement.Next;
            }

            return false;
        }

        // Performs an action for every item stored in the linked list.
        // This is useful for displaying or writing path coordinates.
        public void ForEach(Action<T> action)
        {
            Element<T>? currentElement = _head;

            while (currentElement != null)
            {
                action(currentElement.Data);

                currentElement = currentElement.Next;
            }
        }
    }
}