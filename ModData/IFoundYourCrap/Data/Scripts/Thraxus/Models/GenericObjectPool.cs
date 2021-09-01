using System;
using System.Collections.Concurrent;

namespace AwwScrap_IFoundYourCrap.Thraxus.Models
{
	public class GenericObjectPool<T>
	{
		private readonly ConcurrentBag<T> _objects;
		private readonly Func<T> _objectGenerator;

		public GenericObjectPool(Func<T> objectGenerator)
		{
			_objectGenerator = objectGenerator;
			_objects = new ConcurrentBag<T>();
		}

		public T Get()
		{
			T item;
			return _objects.TryTake(out item) ? item : _objectGenerator();
		}

		public void Return(T item) => _objects.Add(item);
	}
}