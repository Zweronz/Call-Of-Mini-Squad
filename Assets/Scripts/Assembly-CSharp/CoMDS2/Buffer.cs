using System.Collections.Generic;

namespace CoMDS2
{
	public class Buffer<T> where T : class
	{
		protected Queue<T> m_buff = new Queue<T>();

		public T Obtain()
		{
			if (m_buff.Count != 0)
			{
				return m_buff.Dequeue();
			}
			return (T)null;
		}

		public void Recycle(T t)
		{
			m_buff.Enqueue(t);
		}
	}
}
