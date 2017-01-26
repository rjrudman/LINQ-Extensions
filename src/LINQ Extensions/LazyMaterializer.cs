using System.Collections;
using System.Collections.Generic;

namespace LINQ_Extensions
{
	/// <summary>
	/// Used to lazily materialize a collection. 
	/// Multiple enumerations of this will only fetch from the underlying source as needed.
	/// </summary>
	public class LazyMaterializer<T> : IEnumerable<T>
	{
		private readonly IEnumerator<T> _sourceEnumerator;
		private readonly List<T> _buffer = new List<T>();
		public LazyMaterializer(IEnumerable<T> source)
		{
			_sourceEnumerator = source.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new LazyMaterializerEnumerator(_sourceEnumerator, _buffer);
		}

		private class LazyMaterializerEnumerator : IEnumerator<T>
		{
			private readonly IEnumerator<T> _sourceEnumerator;
			private readonly List<T> _buffer;
			private int _index = -1;

			public LazyMaterializerEnumerator(IEnumerator<T> sourceEnumerator, List<T> buffer)
			{
				_sourceEnumerator = sourceEnumerator;
				_buffer = buffer;
			}

			public bool MoveNext()
			{
				if (_buffer.Count > ++_index)
					return true;

				var didMoveNext = _sourceEnumerator.MoveNext();
				if (didMoveNext)
					_buffer.Add(_sourceEnumerator.Current);

				return didMoveNext;
			}

			public void Reset()
			{
				_index = -1;
				_sourceEnumerator.Reset();
			}

			public T Current => _index < _buffer.Count ? _buffer[_index] : _sourceEnumerator.Current;

			object IEnumerator.Current => Current;

			public void Dispose()
			{
				_sourceEnumerator.Dispose();
			}
		}
	}
}
