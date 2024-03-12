namespace CoMDS2
{
	public class NumberSection<T>
	{
		public T left;

		public T right;

		public NumberSection(T l, T r)
		{
			left = l;
			right = r;
		}

		public NumberSection(NumberSection<T> copy)
		{
			left = copy.left;
			right = copy.right;
		}

		public NumberSection(T value)
		{
			left = value;
			right = value;
		}
	}
}
