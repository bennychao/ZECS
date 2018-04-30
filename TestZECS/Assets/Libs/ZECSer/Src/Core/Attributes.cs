using System;

namespace ZECS
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class DontShowInComponentsAttribute : Attribute {
	}

	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class SingletonComponentAttribute : Attribute {
	}

	[AttributeUsage(AttributeTargets.All, Inherited = true)]
	public class ColorAttribute : Attribute {
		
		public float r, g, b, a;

		public ColorAttribute(float r, float g, float b, float a){
			//nodeColor = color;
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}
	}
}

