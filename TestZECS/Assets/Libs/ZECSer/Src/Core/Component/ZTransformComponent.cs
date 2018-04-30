using System;
namespace ZECS
{
	[System.Serializable]
	public class ZVector3{
		public float x;
		public float y;
		public float z;
	}

	[System.Serializable]
	public sealed class ZTransformComponent : IZComponent
	{
		//public int i;
		public ZVector3 position = new ZVector3();

		public ZTransformComponent ()
		{
			
		}
		void IZComponent.Start()
		{
		}
		void IZComponent.Destory()
		{
		}

	}
}

