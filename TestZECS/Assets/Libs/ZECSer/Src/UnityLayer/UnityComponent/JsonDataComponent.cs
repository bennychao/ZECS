using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZECS
{
	[System.Serializable]
	public class JsonDataComponent : ZComponent {

		public TextAsset jsonData;

		public TData GetData<TData>(){
			return JsonUtility.FromJson<TData> (jsonData.text);
		}
	}
}

