using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ObjectDiff
{
    /// <summary>
    /// Набор атомов разностей объектов, приводящий состояние одного объекта к состоянию другого
    /// </summary>
    [Serializable]
    public class DiffChain : ICollection<DiffPart>
    {
        /// <summary>
        /// Цепочка изменений
        /// </summary>
        private readonly List<DiffPart> _chain = new List<DiffPart>();
        /// <summary>
        /// Пустая цепочка
        /// </summary>
        private readonly static DiffChain EmptyChain = new DiffChain();
        /// <summary>
        /// Делает цепочку ReadOnly коллекцией
        /// </summary>
        private bool _isLocked;

        public DiffChain()
        {
            _isLocked = false;
        }

        private DiffChain(bool locked)
        {
            _isLocked = locked;
        }

        public static DiffChain Empty()
        {
            return EmptyChain;
        }
        /// <summary>
        /// Перевести в ReadOnly режим
        /// </summary>
        public void Lock()
        {
            _isLocked = true;
        }

        #region ICollection implementation
        public void Add(DiffPart item)
        {
            if (_isLocked == false)
            {
                _chain.Add(item);
            }
            else
            {
                throw new InvalidOperationException("It is read only collection");
            }
        }

        public void Add(DiffChain items)
        {
            if (_isLocked == false)
            {
                foreach (var item in items)
                {
                    _chain.Add(item);
                }
            }
            else
            {
                throw new InvalidOperationException("It is read only collection");
            }
        }

        public void Clear()
        {
            if (_isLocked == false)
            {
                _chain.Clear();
            }
        }

        public bool Contains(DiffPart item)
        {
            if (_isLocked == false)
            {
                return Enumerable.Contains(_chain, item);
            }
            return false;
        }

        public void CopyTo(DiffPart[] array, int arrayIndex)
        {
            if (_isLocked == false)
            {
                if (arrayIndex < 0)
                {
                    throw new IndexOutOfRangeException();
                }
                int diff = _chain.Count - (array.Length - arrayIndex);
                if (diff > 0)
                {
                    Array.Resize(ref array, array.Length + diff);
                }
                for (int i = arrayIndex, j = 0; j < _chain.Count; i++, j++)
                {
                    array[i] = _chain[j];
                }
            }
            else
            {
                throw new InvalidOperationException("It is read only collection");
            }
        }

        public int Count
        {
            get { return _chain.Count; }
        }

        public bool IsReadOnly
        {
            get { return _isLocked == false; }
        }

        public bool Remove(DiffPart item)
        {
            if (_isLocked == false)
            {
                if (Contains(item))
                {
                    _chain.Remove(item);
                }
            }
            return false;
        }

        public IEnumerator<DiffPart> GetEnumerator()
        {
            return _chain.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _chain.GetEnumerator();
        }
        #endregion

        #region Serialization
        public static byte[] ToBinary(DiffChain chan)
        {
            return BinarySerializeService.ObjectToByteArray(chan);
        }

        public static DiffChain FromBinary(byte[] data)
        {
            return (DiffChain)BinarySerializeService.ByteArrayToObject(data);
        }
        #endregion
    }
}
