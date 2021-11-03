using System;
namespace ADT
{
    public interface IDynamicList<T>
    {
        public int Count { get; }

        public int IndexOf(T item);

        public bool Contains(T item);

        public void Add(T item);

        public void Insert(int index, T item);

        public void RemoveAt(int index);

        public bool Remove(T item);

        public void Clear();

        public void CopyTo(T[] target, int index);

        public T this[int index]
        {
            get;
            set;
        }
    }
}
