using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Entitas.VisualDebugging.Unity.Editor;
using DesperateDevs.Utils;
using System;
using ZECS;
using UnityEditor;

namespace ZECSEditor
{


	[System.Serializable]
	public class EntityInspectorData
	{
		public int ID;
		public bool bUpfold;
	}

	public class EntityIDCreator{
		static private int curID = 10;
		public static int GetID()
		{
			return curID++;
		}
	}

}
//[System.Serializable]
//public class TestComponent1 : IZComponent{
//	public int data1;
//}
//
//[System.Serializable]
//public class TestComponent2 : IZComponent{
//	public int data2;
//}
//
//[System.Serializable]
//public class TestComponent3 : TestComponent2{
//	public int data3;
//}


//public class TestB
//{
//	public int data3;
//}
//public class TestA
//{
//	public int data1;
//	public string data2;
//	public TestB data4 = new TestB();
//	public GameObject obj;
//}
	