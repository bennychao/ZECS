using System;

namespace CommonLib
{
	public class Cache<T>
	{
		protected static T _instance = default(T);
		public static T  Instance {
			get{
				if (_instance == null) {
					_instance = Pool<T>.CreateInstance ();
				}

				return _instance;
			}
//			set{
//				if (value != null)
//					_instance = value;
//			}

		}
	}

	public class Temp<T>{
		public T _instance = default(T);

		public T Instance{
			get {
				
				if (_instance == null) {
					_instance = Pool<T>.CreateInstance ();
				}

				return _instance;
			}
			set{
				if (value != null)
					_instance = value;
			}
		}
	}

}

