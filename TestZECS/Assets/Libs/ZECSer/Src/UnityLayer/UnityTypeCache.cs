using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace CommonLib
{
	public static class VectorCache{
		public static Vector3 Cache(float x, float y, float z){
			Vector3 vec = Cache<Vector3>.Instance;
			vec.x = x;
			vec.y = y;
			vec.z = z;
			return vec;
		}
	}
}

