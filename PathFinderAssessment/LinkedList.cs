// COM 5113 Sample Code - Nick Mitchell 2025

using System;
using System.Collections.Generic;

namespace PathFinderAssessment
{
    // Generic linked list class.
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


        // -------------------------------------------------------------
        // LINKED LIST STATE
        // -------------------------------------------------------------

        // First element in the list.
        private Element<T>? _head;

        // Last element in the list.
        //
        // Keeping a tail reference allows PushBack()
        // to run in constant time.
        private Element<T>? _tail;

        // Number of elements currently stored.
        //
        // Keeping a count avoids traversing the whole list
        // every time Count() is requested.
        private int _count;


        // -------------------------------------------------------------
        // CONSTRUCTOR
        // -------------------------------------------------------------
        public LinkedList()
        {
            _head = null;
            _tail = null;
            _count = 0;
        }


        // -------------------------------------------------------------
        // IS EMPTY
        // -------------------------------------------------------------
        // O(1)
        public bool IsEmpty()
        {
            return _head == null;
        }


        // -------------------------------------------------------------
        // PUSH FRONT
        // -------------------------------------------------------------
        // O(1)
        public void PushFront(T data)
        {
            Element<T> newElement =
                new Element<T>(data);

            newElement.Next =
                _head;

            _head =
                newElement;

            // If this is the first element,
            // it is both head and tail.
            if (_tail == null)
            {
                _tail =
                    newElement;
            }

            _count++;
        }


        // -------------------------------------------------------------
        // PUSH BACK
        // -------------------------------------------------------------
        // O(1)
        public void PushBack(T data)
        {
            Element<T> newElement =
                new Element<T>(data);

            // Empty list.
            if (_head == null)
            {
                _head =
                    newElement;

                _tail =
                    newElement;

                _count++;

                return;
            }

            // Add directly after current tail.
            _tail!.Next =
                newElement;

            _tail =
                newElement;

            _count++;
        }


        // -------------------------------------------------------------
        // POP FRONT
        // -------------------------------------------------------------
        // O(1)
        public T PopFront()
        {
            if (_head == null)
            {
                throw new InvalidOperationException(
                    "Cannot remove an item from an empty linked list.");
            }

            T data =
                _head.Data;

            _head =
                _head.Next;

            _count--;

            // If the list is now empty,
            // the tail must also be cleared.
            if (_head == null)
            {
                _tail =
                    null;
            }

            return data;
        }


        // -------------------------------------------------------------
        // POP BACK
        // -------------------------------------------------------------
        // O(n) for a singly linked list.
        public T PopBack()
        {
            if (_head == null)
            {
                throw new InvalidOperationException(
                    "Cannot remove an item from an empty linked list.");
            }

            // Only one element.
            if (_head.Next == null)
            {
                T data =
                    _head.Data;

                _head =
                    null;

                _tail =
                    null;

                _count =
                    0;

                return data;
            }

            // Find the element immediately before the tail.
            Element<T> currentElement =
                _head;

            while (currentElement.Next != null &&
                   currentElement.Next.Next != null)
            {
                currentElement =
                    currentElement.Next;
            }

            T lastData =
                currentElement.Next!.Data;

            currentElement.Next =
                null;

            _tail =
                currentElement;

            _count--;

            return lastData;
        }


        // -------------------------------------------------------------
        // CONTAINS
        // -------------------------------------------------------------
        // O(n)
        public bool Contains(T data)
        {
            Element<T>? currentElement =
                _head;

            while (currentElement != null)
            {
                if (EqualityComparer<T>.Default.Equals(
                    currentElement.Data,
                    data))
                {
                    return true;
                }

                currentElement =
                    currentElement.Next;
            }

            return false;
        }


        // -------------------------------------------------------------
        // COUNT
        // -------------------------------------------------------------
        // O(1)
        public int Count()
        {
            return _count;
        }


        // -------------------------------------------------------------
        // CLEAR
        // -------------------------------------------------------------
        // O(1)
        public void Clear()
        {
            _head =
                null;

            _tail =
                null;

            _count =
                0;
        }


        // -------------------------------------------------------------
        // INSERT SORTED
        // -------------------------------------------------------------
        // O(n)
        //
        // Inserts an item in the position decided by comparison.
        public void InsertSorted(
            T data,
            Comparison<T> comparison)
        {
            Element<T> newElement =
                new Element<T>(data);


            // ---------------------------------------------------------
            // EMPTY LIST
            // ---------------------------------------------------------
            if (_head == null)
            {
                _head =
                    newElement;

                _tail =
                    newElement;

                _count++;

                return;
            }


            // ---------------------------------------------------------
            // INSERT BEFORE HEAD
            // ---------------------------------------------------------
            if (comparison(
                data,
                _head.Data) < 0)
            {
                newElement.Next =
                    _head;

                _head =
                    newElement;

                _count++;

                return;
            }


            // ---------------------------------------------------------
            // FIND INSERTION POSITION
            // ---------------------------------------------------------
            Element<T> currentElement =
                _head;

            while (currentElement.Next != null &&
                   comparison(
                       data,
                       currentElement.Next.Data) >= 0)
            {
                currentElement =
                    currentElement.Next;
            }


            newElement.Next =
                currentElement.Next;

            currentElement.Next =
                newElement;


            // If inserted at the end,
            // update the tail reference.
            if (newElement.Next == null)
            {
                _tail =
                    newElement;
            }

            _count++;
        }


        // -------------------------------------------------------------
        // FIND
        // -------------------------------------------------------------
        // O(n)
        public T? Find(
            Predicate<T> condition)
        {
            Element<T>? currentElement =
                _head;

            while (currentElement != null)
            {
                if (condition(
                    currentElement.Data))
                {
                    return currentElement.Data;
                }

                currentElement =
                    currentElement.Next;
            }

            return default;
        }


        // -------------------------------------------------------------
        // REMOVE
        // -------------------------------------------------------------
        // O(n)
        //
        // Removes the first occurrence of the supplied item.
        public bool Remove(T data)
        {
            // Empty list.
            if (_head == null)
            {
                return false;
            }


            // ---------------------------------------------------------
            // REMOVE HEAD
            // ---------------------------------------------------------
            if (EqualityComparer<T>.Default.Equals(
                _head.Data,
                data))
            {
                _head =
                    _head.Next;

                _count--;

                if (_head == null)
                {
                    _tail =
                        null;
                }

                return true;
            }


            // ---------------------------------------------------------
            // SEARCH FOR ITEM
            // ---------------------------------------------------------
            Element<T> currentElement =
                _head;

            while (currentElement.Next != null)
            {
                if (EqualityComparer<T>.Default.Equals(
                    currentElement.Next.Data,
                    data))
                {
                    Element<T> elementToRemove =
                        currentElement.Next;

                    currentElement.Next =
                        elementToRemove.Next;


                    // If the removed element was the tail,
                    // the previous element becomes the new tail.
                    if (elementToRemove == _tail)
                    {
                        _tail =
                            currentElement;
                    }

                    _count--;

                    return true;
                }

                currentElement =
                    currentElement.Next;
            }

            return false;
        }


        // -------------------------------------------------------------
        // FOR EACH
        // -------------------------------------------------------------
        // O(n)
        public void ForEach(
            Action<T> action)
        {
            Element<T>? currentElement =
                _head;

            while (currentElement != null)
            {
                action(
                    currentElement.Data);

                currentElement =
                    currentElement.Next;
            }
        }
    }
}