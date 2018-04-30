using System;

namespace CommonLib
{
	public class Pool<T>{

		static public T CreateInstance()
		{
			return (T)Activator.CreateInstance (typeof(T).GetType());
		}
	}
}

