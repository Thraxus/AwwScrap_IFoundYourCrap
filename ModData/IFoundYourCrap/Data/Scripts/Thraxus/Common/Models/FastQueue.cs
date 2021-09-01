
namespace AwwScrap_IFoundYourCrap.Thraxus.Common.Models
{
	public class FastQueue<T>
	{
		private readonly T[] _nodes;
		private int _current;
		private int _emptySpot;

		public FastQueue(int size)
		{
			_nodes = new T[size];
			_current = 0;
			_emptySpot = 0;
		}

		public void Enqueue(T value)
		{
			_nodes[_emptySpot] = value;
			_emptySpot++;
			if (_emptySpot >= _nodes.Length)
			{
				_emptySpot = 0;
			}
		}
		public T Dequeue()
		{
			int ret = _current;
			_current++;
			if (_current >= _nodes.Length)
			{
				_current = 0;
			}
			return _nodes[ret];
		}

		public T[] GetQueue()
		{
			return _nodes;
		}



		public int Count => _nodes.Length;

		public void Clear()
		{
			for (int index = _nodes.Length - 1; index >= 0; index--)
			{
				_nodes[index] = default(T);
			}
		}
	}
}
