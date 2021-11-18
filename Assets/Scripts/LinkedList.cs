using System;
using System.Collections.Generic;

namespace ADT
{
    public class LinkedList<T> : IDynamicList<T>
    {
        private ListNode head;
        private ListNode tail;
        private int count;

        public LinkedList()
        {
            head = default;
            tail = default;
            count = 0;
        }

        private class ListNode
        {
            public T nodeItem;
            public ListNode nextNode;
            public ListNode previousNode;
            public int index = -1;

            public ListNode(T item, ListNode node, ListNode prevNode, int idx)
            {
                nodeItem = item;
                nextNode = node;
                previousNode = prevNode;
                index = idx;
            }
        }

        public int Count => count;

        public void Add(T item)
        {
            ListNode newNode = new ListNode(item, null, tail, count);

            if (count == 0)
            {
                head = newNode;
                tail = newNode;
            }
            else
            {
                tail.nextNode = newNode;
                tail = newNode;
            }
            count++;
        }

        public void Clear()
        {
            count = 0;
            head = null;
            tail = null;
        }

        public bool Contains(T item)
        {
            ListNode currentNode = head;

            while (currentNode != null)
            {
                if (EqualityComparer<T>.Default.Equals(currentNode.nodeItem, item))
                {
                    return true;
                }
                currentNode = currentNode.nextNode;
            }
            return false;
        }

        public void CopyTo(T[] target, int index)
        {
            ListNode currentNode = head;

            while (currentNode != null && index < target.Length)
            {
                target[index] = currentNode.nodeItem;
                currentNode = currentNode.nextNode;
                index++;
            }
        }

        public void InsertInArray(ref T[] target, int index)
        {
            ListNode currentNode = head;
            T[] newArray = new T[target.Length + count];
            int incrementIndex = 0;

            for (int i = 0; i < target.Length; i++)
            {
                if (i == index)
                {
                    while (currentNode != null)
                    {
                        newArray[i + currentNode.index] = currentNode.nodeItem;
                        incrementIndex++;
                        currentNode = currentNode.nextNode;
                    }
                }
                newArray[i + incrementIndex] = target[i];
            }
            target = newArray;
        }

        public void AddRange(T[] target, int index)
        {
            for (int i = 0; i < target.Length; i++)
            {
                Insert(index + i, target[i]);
            }
        }

        public int IndexOf(T item)
        {
            ListNode currentNode = head;

            while (currentNode != null)
            {
                if (EqualityComparer<T>.Default.Equals(currentNode.nodeItem, item))
                {
                    return currentNode.index;
                }
                currentNode = currentNode.nextNode;
            }
            return -1;
        }

        public T GetFirst()
        {
            return head.nodeItem;
        }
        
        public T GetLast()
        {
            return tail.nodeItem;
        }

        public void Insert(int index, T item)
        {
            ListNode currentNode = head;
            ListNode previousNode = head;
            ListNode newNode;

            while (currentNode != null)
            {
                if (index == currentNode.index)
                {
                    newNode = new ListNode(item, currentNode, previousNode, index);
                    previousNode.nextNode = newNode;
                    currentNode.previousNode = newNode;
                    IncreaseIndex(currentNode);
                    count++;
                    return;
                }
                previousNode = currentNode;
                currentNode = currentNode.nextNode;
            }
        }

        public bool Remove(T item)
        {
            ListNode previousNode = head;
            ListNode currentNode = head;

            while (currentNode != null)
            {
                if (EqualityComparer<T>.Default.Equals(currentNode.nodeItem, item))
                {
                    previousNode.nextNode = currentNode.nextNode;
                    currentNode.nextNode.previousNode = previousNode;
                    DecreaseIndex(currentNode.nextNode);
                    count--;
                    return true;
                }
                previousNode = currentNode;
                currentNode = currentNode.nextNode;
            }
            return false;
        }

        private void DecreaseIndex(ListNode node)
        {
            while (node != null)
            {
                node.index--;
                node = node.nextNode;
            }
        }

        private void IncreaseIndex(ListNode node)
        {
            while (node != null)
            {
                node.index++;
                node = node.nextNode;
            }
        }

        // array access thing
        public T this[int index]
        {
            get
            {
                if (index >= count || index < 0)
                {
                    throw new IndexOutOfRangeException();
                }

                ListNode currentNode = head;
                while (currentNode != null)
                {
                    if (index == currentNode.index)
                    {
                        return currentNode.nodeItem;
                    }
                    currentNode = currentNode.nextNode;
                }
                throw new IndexOutOfRangeException();
            }
            set
            {
                if (index >= count || index < 0)
                {
                    throw new IndexOutOfRangeException();
                }

                ListNode currentNode = head;
                while (currentNode != null)
                {
                    if (index == currentNode.index)
                    {
                        currentNode.nodeItem = value;
                        return;
                    }
                    currentNode = currentNode.nextNode;
                }
                throw new IndexOutOfRangeException();
            }
        }

        public void RemoveAt(int index)
        {
            ListNode currentNode = head;
            ListNode previousNode = head;

            while (currentNode != null)
            {
                if (index == currentNode.index)
                {
                    previousNode.nextNode = currentNode.nextNode;
                    currentNode.nextNode.previousNode = previousNode;
                    DecreaseIndex(currentNode.nextNode);
                    count--;
                    return;
                }
                previousNode = currentNode;
                currentNode = currentNode.nextNode;
            }
        }

        private ListNode GetNode(int index)
        {
            ListNode currentNode = head;
            ListNode previousNode = head;

            while (currentNode != null)
            {
                if (index == currentNode.index)
                {
                    return currentNode;
                }
                previousNode = currentNode;
                currentNode = currentNode.nextNode;
            }
            throw new IndexOutOfRangeException();
        }
    }
}
