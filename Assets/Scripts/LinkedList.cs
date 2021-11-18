using System;
using System.Collections.Generic;
using Cinemachine;

namespace ADT
{
    public class LinkedList<T>

    {
        private ListNode head;
        private ListNode tail;

        public LinkedList()
        {
            head = default;
            tail = default;
        }

        private class ListNode
        {
            public T nodeItem;
            public ListNode nextNode;

            public ListNode(T item, ListNode node)
            {
                nodeItem = item;
                nextNode = node;
            }
        }

        public Iterator GetIterator()
        {
            return new Iterator(this);
        }
        
        public class Iterator
        {
            private ListNode currentNode;
            private ListNode previousNode;

            // this iterator starts at the node after the head because that's all we ever need in the snake class
            internal Iterator(LinkedList<T> linkedList)
            {
                previousNode = linkedList.head;
                currentNode = linkedList.head.nextNode;
            }
            public (T,T) GetNext()
            {
                if (currentNode == null)
                {
                    return (default, default);
                }
                (T, T) previousAndCurrent = (previousNode.nodeItem, currentNode.nodeItem);
                previousNode = currentNode;
                currentNode = currentNode.nextNode;
                return previousAndCurrent;
            }
        }
        
        public void Add(T item)
        {
            ListNode newNode = new ListNode(item, null);

            if (head == default)
            {
                head = newNode;
                tail = newNode;
            }
            else
            {
                tail.nextNode = newNode;
                tail = newNode;
            }
        }

        public T GetFirst()
        {
            return head.nodeItem;
        }

        public T GetLast()
        {
            return tail.nodeItem;
        }

        // array access thing
        public T this[int index]
        {
            get
            {
                if (index < 0)
                {
                    throw new IndexOutOfRangeException();
                }

                ListNode currentNode = head;

                int counter = 0;
                while (currentNode != null)
                {
                    if (counter == index)
                    {
                        return currentNode.nodeItem;
                    }

                    currentNode = currentNode.nextNode;
                    counter++;
                }

                throw new IndexOutOfRangeException();
            }
            set
            {
                if (index < 0)
                {
                    throw new IndexOutOfRangeException();
                }

                ListNode currentNode = head;

                int counter = 0;
                while (currentNode != null)
                {
                    if (counter == index)
                    {
                        currentNode.nodeItem = value;
                        return;
                    }

                    currentNode = currentNode.nextNode;
                    counter++;
                }

                throw new IndexOutOfRangeException();
            }
        }
    }
}