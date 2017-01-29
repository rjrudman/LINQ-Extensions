using System.Collections;
using System.Collections.Generic;

namespace LINQ_Extensions
{
    public static class CachedEnumerableExtensions
    {
        public static CachedEnumerable<T> ToCachedEnumerable<T>(this IEnumerable<T> src)
        {
            return new CachedEnumerable<T>(src);
        }
    }

    /// <summary>
    /// Enumerates and caches the results of an enumerable.
    /// The wrapped enumerable will be evaluated lazily
    /// The wrapped enumerable will not be iterated more than once.
    /// Elements are guaranteed to be in the same order over multiple iterations.
    /// </summary>
    public class CachedEnumerable<T> : IEnumerable<T>
    {
        private readonly IEnumerator<T> m_Enumerator;
        private readonly List<T> m_Buffer = new List<T>();
        public CachedEnumerable(IEnumerable<T> source)
        {
            m_Enumerator = source.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new LazyMaterializerEnumerator(m_Enumerator, m_Buffer);
        }

        private class LazyMaterializerEnumerator : IEnumerator<T>
        {
            private readonly IEnumerator<T> m_SourceEnumerator;
            private readonly List<T> m_Buffer;
            private int m_Index = -1;

            public LazyMaterializerEnumerator(IEnumerator<T> sourceEnumerator, List<T> buffer)
            {
                m_SourceEnumerator = sourceEnumerator;
                m_Buffer = buffer;
            }

            public bool MoveNext()
            {
                if (m_Buffer.Count > ++m_Index)
                    return true;

                var didMoveNext = m_SourceEnumerator.MoveNext();

                if (didMoveNext)
                    m_Buffer.Add(m_SourceEnumerator.Current);

                return didMoveNext;
            }

            public void Reset()
            {
                m_Index = -1;
            }

            public T Current
            {
                get { return m_Buffer.Count > m_Index ? m_Buffer[m_Index] : m_SourceEnumerator.Current; }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public void Dispose()
            {
                m_SourceEnumerator.Dispose();
            }
        }
    }
}
