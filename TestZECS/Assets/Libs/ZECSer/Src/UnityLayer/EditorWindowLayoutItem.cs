using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ZECS
{


	[System.Serializable]
	public class TestJson {

		public int data;
		public int data1;
	}

	/// <summary>
	/// Entity json runtime mgr.
	/// </summary>
	[System.Serializable]
	public class EntityJsonRuntimeMgr
	{
		public List<TextAsset> AssetList = new List<TextAsset>();

		public void AddAsset(TextAsset asset)
		{
			if (asset != null)
				AssetList.Add (asset);
		}

		public void DelAsset(TextAsset asset)
		{
			if (asset != null)
				AssetList.Remove (asset);
		}

		public void Clear(){
			AssetList.Clear ();
		}

		/// <summary>
		/// Finds the asset.
		/// </summary>
		/// <returns>The asset.</returns>
		/// <param name="name">Name.</param>
		public string FindAsset(string name)
		{
			var list = AssetList.Where ((a) => a.name.CompareTo (name) == 0).Select ((a) => a.text).ToList();
			if (list.Count == 0)
				return null;
			
			string str = list.Single();

			return str;
		}
	}

	/// <summary>
	/// Entity inspector unfold datas.
	/// </summary>
	[System.Serializable]
	public class EntityInspectorUnfoldDatas
	{

		public Dictionary<string, bool> unfoldmap = new Dictionary<string, bool>();

		public bool IsUnfold(string strType)
		{
			return unfoldmap.ContainsKey (strType) && unfoldmap [strType];
		}

		public void SetUnfold(string strType, bool bUnfold)
		{
			unfoldmap [strType] = bUnfold;
		}

//		public void SaveToJson()
//		{
//			string strData = EditorJsonUtility.ToJson (this, true);
//			Debug.Log("" + strData);
//		}

	}

}

