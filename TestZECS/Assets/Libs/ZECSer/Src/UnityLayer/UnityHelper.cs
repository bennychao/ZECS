using System;
using UnityEngine;


namespace ZECS
{
	public static class UnityHelper
	{
		static public Vector3 ToUnityVector(this ZVector3 vec){
			return new Vector3(vec.x, vec.y, vec.z);
		}

		static public Color ToUnityColor(this ColorAttribute at){
			return new Color(at.a, at.g, at.b, at.r);
		}
	}
}

