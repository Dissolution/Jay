using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Jay.Collections.Uber
{
    [DebuggerDisplay("Count = {Count}")]
    public class LinkedCollection<T> : ICollection<T>, IReadOnlyCollection<T>, IEnumerable<T>
    {
        public sealed class Node
        {
            private LinkedCollection<T> _collection;
            
            internal Node? _next;
            internal Node? _previous;
            internal T _value;
            
            public LinkedCollection<T> Collection => _collection;

            public Node? Next
            {
                get
                {
                    // Prevent wraparound
                    if (_next != null && _next != _collection._head)
                        return _next;
                    else
                        return null;
                }
                internal set => _next = value;
            }

            public Node? Previous
            {
                get
                {
                    // Prevent wraparound
                    if (_previous != null && this != _collection._head)
                        return _previous;
                    else
                        return null;
                }
                internal set => _previous = value;
            }

            public ref T Value => ref _value;
            
            internal Node(LinkedCollection<T> collection, T value)
            {
                _collection = collection;
                _value = value;
            }

            public void Dispose()
            {
                _collection = null;
                _previous = null;
                _next = null;
                _value = default;
            }
        }

        
        internal Node? _head;
        internal int _count;
        internal int _version;
        
        public Node? First => _head;
        public Node? Last => _head?._previous;

        public int Count => _count;
        
        bool ICollection<T>.IsReadOnly => false;
        
        public LinkedCollection()
        {
        }

        private void InternalInsertNodeBefore(Node node, Node newNode)
        {
            newNode._next = node;
            newNode._previous = node._previous;
            node._previous._next = newNode;
            node._previous = newNode;
            ++_version;
            ++_count;
        }

        private void InternalInsertNodeToEmptyList(Node newNode)
        {
            newNode._next = newNode;
            newNode._previous = newNode;
            _head = newNode;
            ++_version;
            ++_count;
        }

        internal void InternalRemoveNode(Node node)
        {
            if (node._next == node)
            {
                _head = null;
            }
            else
            {
                node._next._previous = node._previous;
                node._previous._next = node._next;
                if (_head == node)
                    _head = node._next;
            }
            node.Dispose();
            --_count;
            ++_version;
        }

        internal void ValidateNewNode(Node node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof (node));
            if (node.Collection != null)
                throw new InvalidOperationException("SR.LinkedListNodeIsAttached");
        }

        internal void ValidateNode(Node? node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof (node));
            if (node.Collection != this)
                throw new InvalidOperationException("SR.ExternalLinkedListNode");
        }
        
        void ICollection<T>.Add(T value) => this.AddLast(value);

        public Node AddAfter(Node node, T value)
        {
            this.ValidateNode(node);
            Node newNode = new Node(this, value);
            this.InternalInsertNodeBefore(node._next, newNode);
            return newNode;
        }

        // public void AddAfter(Node node, Node newNode)
        // {
        //     this.ValidateNode(node);
        //     this.ValidateNewNode(newNode);
        //     this.InternalInsertNodeBefore(node._next, newNode);
        //     newNode._collection = this;
        // }

        public Node AddBefore(Node node, T value)
        {
            this.ValidateNode(node);
            Node newNode = new Node(this, value);
            this.InternalInsertNodeBefore(node, newNode);
            if (node == _head)
                _head = newNode;
            return newNode;
        }

        // public void AddBefore(Node node, Node newNode)
        // {
        //     this.ValidateNode(node);
        //     this.ValidateNewNode(newNode);
        //     this.InternalInsertNodeBefore(node, newNode);
        //     newNode._collection = this;
        //     if (node != _head)
        //         return;
        //     _head = newNode;
        // }

        public Node AddFirst(T value)
        {
            Node newNode = new Node(this, value);
            if (_head == null)
            {
                this.InternalInsertNodeToEmptyList(newNode);
            }
            else
            {
                this.InternalInsertNodeBefore(_head, newNode);
                _head = newNode;
            }
            return newNode;
        }

        // public void AddFirst(Node node)
        // {
        //     this.ValidateNewNode(node);
        //     if (_head == null)
        //     {
        //         this.InternalInsertNodeToEmptyList(node);
        //     }
        //     else
        //     {
        //         this.InternalInsertNodeBefore(_head, node);
        //         _head = node;
        //     }
        //     node._collection = this;
        // }

        public Node AddLast(T value)
        {
            Node newNode = new Node(this, value);
            if (_head == null)
                this.InternalInsertNodeToEmptyList(newNode);
            else
                this.InternalInsertNodeBefore(_head, newNode);
            return newNode;
        }

        // public void AddLast(Node node)
        // {
        //     this.ValidateNewNode(node);
        //     if (_head == null)
        //         this.InternalInsertNodeToEmptyList(node);
        //     else
        //         this.InternalInsertNodeBefore(_head, node);
        //     node._collection = this;
        // }

        public void TrimAfter(Node node)
        {
            this.ValidateNode(node);
            node = node.Next;
            while (node != null)
            {
                var next = node.Next;
                this.InternalRemoveNode(node);
                node = next;
            }
        }

        public void Clear()
        {
            Node? node = _head;
            while (node != null)
            {
                var next = node.Next;
                node.Dispose();
                node = next;
            }
            _head = null;
            _count = 0;
            ++_version;
        }

        public bool Contains(T value) => this.Find(value) != null;

        void ICollection<T>.CopyTo(T[] array, int index)
        {
            if (array == null)
                throw new ArgumentNullException(nameof (array));
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof (index), (object) index, "SR.ArgumentOutOfRange_NeedNonNegNum");
            if (index > array.Length)
                throw new ArgumentOutOfRangeException(nameof (index), (object) index, "SR.ArgumentOutOfRange_BiggerThanCollection");
            if (array.Length - index < this.Count)
                throw new ArgumentException("SR.Arg_InsufficientSpace");
            Node? node = _head;
            if (node == null)
                return;
            do
            {
                array[index++] = node!._value;
                node = node._next;
            }
            while (node != _head);
        }

        public Node? Find(T value)
        {
            Node? node = _head;
            if (node != null)
            {
                if (value != null)
                {
                    while (!EqualityComparer<T>.Default.Equals(node!._value, value))
                    {
                        node = node._next;
                        if (node == _head)
                            goto notFound;
                    }
                    return node;
                }
                while (node!._value != null)
                {
                    node = node._next;
                    if (node == _head)
                        goto notFound;
                }
                return node;
            }
            notFound:
            return null;
        }

        public Node? FindLast(T value)
        {
            if (_head == null)
                return null;
            Node? prev = _head._previous;
            Node? node = prev;
            if (node != null)
            {
                if (value != null)
                {
                    while (!EqualityComparer<T>.Default.Equals(node!._value, value))
                    {
                        node = node._previous;
                        if (node == prev)
                            goto notFound;
                    }
                    return node;
                }
                while (node!._value != null)
                {
                    node = node._previous;
                    if (node == prev)
                        goto notFound;
                }
                return node;
            }
            notFound:
            return null;
        }

        public bool Remove(T value)
        {
            Node? node = this.Find(value);
            if (node == null)
                return false;
            this.InternalRemoveNode(node);
            return true;
        }

        public void Remove(Node node)
        {
            this.ValidateNode(node);
            this.InternalRemoveNode(node);
        }

        public void RemoveFirst()
        {
            if (_head == null)
                throw new InvalidOperationException("SR.LinkedListEmpty");
            this.InternalRemoveNode(_head);
        }

        public void RemoveLast()
        {
            if (_head == null)
                throw new InvalidOperationException("SR.LinkedListEmpty");
            this.InternalRemoveNode(_head._previous!);
        }
        
        public LinkedCollectionEnumerator GetEnumerator() => new LinkedCollectionEnumerator(this);
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => new LinkedCollectionEnumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => new LinkedCollectionEnumerator(this);
        
        public struct LinkedCollectionEnumerator : IEnumerator<T>, IEnumerator, IDisposable
        {
            private readonly LinkedCollection<T> _collection;
            private readonly int _version;
            
            private Node? _node;
            private T? _current;
            private int _index;

            [MaybeNull]
            public T Current => _current;

            object? IEnumerator.Current
            {
                get
                {
                    if (_index == 0 || _index == _collection.Count + 1)
                        throw new InvalidOperationException("SR.InvalidOperation_EnumOpCantHappen");
                    return (object?) _current;
                }
            }

            public int Index => _index;

            public Node? Node => _node;
            
            internal LinkedCollectionEnumerator(LinkedCollection<T> collection)
            {
                _collection = collection;
                _version = collection._version;
                _node = collection._head;
                _current = default (T);
                _index = 0;
            }

            public bool MoveNext()
            {
                if (_version != _collection._version)
                    throw new InvalidOperationException("SR.InvalidOperation_EnumFailedVersion");
                if (_node == null)
                {
                    _index = _collection.Count + 1;
                    return false;
                }
                ++_index;
                _current = _node._value;
                _node = _node._next;
                if (_node == _collection._head)
                    _node = null;
                return true;
            }

            void IEnumerator.Reset()
            {
                if (_version != _collection._version)
                    throw new InvalidOperationException("SR.InvalidOperation_EnumFailedVersion");
                _current = default (T);
                _node = _collection._head;
                _index = 0;
            }

            void IDisposable.Dispose()
            {
            }
        }
    }
}