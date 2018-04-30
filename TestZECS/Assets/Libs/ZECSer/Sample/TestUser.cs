using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZECS;

public class TestUser : MonoBehaviour {

	private EntityPool pool;
	// Use this for initialization
	void Start () {
		pool = EntityPoolBuilder.FindByName ("TestPool");

		ZEntity en = pool.CreateEntity (1);

		Debug.Log (" en data is " + en.GetComponent<TestComponent4> ().data4.ToString ());

		//test singleton
		TestSingleton test = SingletonEntity<TestSingleton>.Instance;

		if (test != null)
			Debug.Log (" SingletonEntity data is " + test.data2.ToString ());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
