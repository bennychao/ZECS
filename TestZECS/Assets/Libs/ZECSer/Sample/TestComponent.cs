using System;
using ZECS;
using UnityEngine;

namespace ZECS
{

	[System.Serializable]
	public class TestComponent1 : ZComponent{
		public int data1;

		protected override void OnStart()
		{
			//base.Start ();
			Debug.Log("TestComponent1 Start");
		}

		protected override void OnDestory()
		{
			Debug.Log("TestComponent1 Destory");
		}
	}

	[DontShowInComponents]
	[System.Serializable]
	public class TestComponent2 : ZComponent{
		public int data2;

		protected override void OnStart()
		{
			//base.Start ();
			Debug.Log("TestComponent2 Start");
		}

		protected override void OnDestory()
		{
			Debug.Log("TestComponent2 Destory");
		}
	}

	[System.Serializable]
	public class TestComponent3 : TestComponent2{
		public int data3;

		protected override void OnStart()
		{
			//base.Start ();
			Debug.Log("TestComponent3 Start");
		}

		protected override void OnDestory()
		{
			Debug.Log("TestComponent3 Destory");
		}
	}

	[System.Serializable]
	public class TestSystem: ZSystem{
		public string strname;
		protected override void OnStart()
		{
			//base.Start ();
			Debug.Log("TestSystem Start");
		}

		protected override void OnDestory()
		{
			Debug.Log("TestSystem Destory");
		}
	}

	[System.Serializable]
	public class MyStartSystem : StartEventSystem {
		protected override void OnDestory()
		{
			Debug.Log("TestSystem Destory");
		}

		protected override void OnExecute ()
		{
			base.OnExecute ();
			Debug.Log("MyStartSystem StartEventSystem");
		}
	}


	[ColorAttribute(0.9f, 0.02f, 0.01f, 0.95f)]
	[System.Serializable]
	public class TestComponent4 : TestComponent2{
		public int data4;

		protected override void OnStart()
		{
			//base.Start ();
			Debug.Log("TestComponent4 Start " + data4);
		}

	}


	[System.Serializable]
	[SingletonComponentAttribute]
	public class TestSingleton : ZComponent{
		public int data2;

		protected override void OnStart()
		{
			//base.Start ();
			Debug.Log("TestSingleton Start");
		}

		protected override void OnDestory()
		{
			Debug.Log("TestSingleton Destory");
		}
	}
}



