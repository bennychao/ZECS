using System;
using ZECS;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using CommonEditorLib;

namespace ZECSEditor
{
	public class EntityPoolEditorBuildUtils
	{
		static string localPath = @"/ZECSData/";
		static string localEntityPath = localPath + @"Entitys/";
		static string localPoolPath = localPath + @"Pool/";
		static string configPath = localPath + @"Config/";

		static string assetpath = @"Assets";
		//static string entityPath = Application.dataPath +localEntityPath;
		static string ComponentPrefix = "&&&&&&&&";

		static string curPoolName = "";

		/// <summary>
		/// Initializes the <see cref="ZECSEditor.EntityPoolEditorBuildUtils"/> class.
		/// </summary>
		static EntityPoolEditorBuildUtils(){

			BuildLocalPathBase ();
		}

		/// <summary>
		/// Builds the local path base.
		/// </summary>
		static public void BuildLocalPathBase(){
			if (!System.IO.Directory.Exists(Application.dataPath +localPath))
			{
				System.IO.Directory.CreateDirectory (Application.dataPath + localPath);
			}

//			if (!System.IO.Directory.Exists(Application.dataPath +localEntityPath))
//			{
//				System.IO.Directory.CreateDirectory (Application.dataPath + localEntityPath);
//			}

			if (!System.IO.Directory.Exists(Application.dataPath +localPoolPath))
			{
				System.IO.Directory.CreateDirectory (Application.dataPath + localPoolPath);
			}

			if (!System.IO.Directory.Exists(Application.dataPath +configPath))
			{
				System.IO.Directory.CreateDirectory (Application.dataPath + configPath);
			}

			AssetDatabase.Refresh ();
		}

		/// <summary>
		/// Saves the entity.
		/// </summary>
		/// <param name="en">En.</param>
		static public void SaveEntity(ZEntity en)
		{
			//save entity data
			List<IZComponent> coms = en.GetComponents<IZComponent>();
			string strData = en.ID.ToString() + "\n" + en.EType.ToString() + "\n";
			foreach (var c in coms) {
				strData += SaveComponentData (c);
			}

			string strFileName = en.Name + ".json";

			string entityPath = Application.dataPath + GetCurPoolEntityPath ();

			System.IO.FileInfo file = new System.IO.FileInfo (entityPath + strFileName);
			StreamWriter sw = file.CreateText ();
			sw.WriteLine (strData);
			sw.Close ();
			sw.Dispose ();
		}
			

		/// <summary>
		/// Saves the component data.
		/// </summary>
		/// <returns>The component data.</returns>
		/// <param name="com">COM.</param>
		static private string SaveComponentData(IZComponent com)
		{
			string strData = EditorJsonUtility.ToJson (com, true);
			return ComponentPrefix + "\n" +  com.GetType().Name + "\n" + strData + "\n" + ComponentPrefix + "\n";
		}


		/// <summary>
		/// Loads the entity.
		/// </summary>
		/// <returns>The entity.</returns>
		/// <param name="name">Name.</param>
		static public ZEntity LoadEntity(string name, int ID = -1){

			string strFileName = name + ".json";

			string sl = "";

			//create the entity
			ZEntity newEntity = new ZEntity ();
			newEntity.Name = name;
			newEntity.ID = ID;

			string entityPath = Application.dataPath + GetCurPoolEntityPath ();
			if (!System.IO.File.Exists(entityPath + strFileName))
			{
				return newEntity;

			}

			try
			{
				StreamReader sr = new StreamReader(entityPath + strFileName, Encoding.Default);

				if (sr == null)
					return newEntity;
				
				//get entity ID
				sl = sr.ReadLine();
				//newEntity.ID = Convert.ToInt32(sl);
				//read type
				sl = sr.ReadLine();
				newEntity.EType = (EntityType)Enum.Parse(typeof(EntityType), sl);
				//Debug.Log("EType is " + newEntity.EType);

				while (sl != null)
				{
					sl = sr.ReadLine();
					if (sl != null && sl.CompareTo(ComponentPrefix) == 0)
					{
						var c = LoadAComponent(sr);
						if (c != null)
							newEntity.AddComponent(c);
					}
				}
				sr.Close();

			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
				throw new Exception ("Load Entity Error " + name + e.Message);
				//return null;
			}

			return newEntity;
		}

		/// <summary>
		/// Changes the name of the entity.
		/// </summary>
		/// <param name="en">En.</param>
		/// <param name="name">Name.</param>
		static public void ChangeEntityName(ZEntity en, string name){


			DelEntityFile (en);
			en.Name = name;

			SaveEntity (en);

		}

		/// <summary>
		/// Loads the entity file asset.
		/// </summary>
		/// <returns>The entity file asset.</returns>
		/// <param name="name">Name.</param>
		static public TextAsset LoadEntityFileAsset(string name){
			string strFileName = GetCurPoolEntityPath() + name + ".json";

			//Texture2D t = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/EntitasContextErrorHierarchyIcon.png", typeof(Texture2D));
			//poolConfig.testStr = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Libs/ZECSer/Entitys/entity9.json", typeof(TextAsset));
			//must be assset local path

			return (TextAsset)AssetDatabase.LoadAssetAtPath(assetpath + strFileName, typeof(TextAsset));
		}

		/// <summary>
		/// Dels the entity file.
		/// </summary>
		/// <param name="en">En.</param>
		static public void DelEntityFile(ZEntity en){
			
			DelEntityFile (en.Name);
		}

		/// <summary>
		/// Dels the entity.
		/// </summary>
		/// <param name="name">Name.</param>
		static public void DelEntityFile(string name){
			string strFileName = name + ".json";

			string entityPath = Application.dataPath + GetCurPoolEntityPath();

			if (System.IO.File.Exists(entityPath + strFileName))
			{
				System.IO.File.Delete (entityPath + strFileName);
			}
		}

		/// <summary>
		/// Dels all entity.
		/// </summary>
		/// <param name="config">Config.</param>
		static public void DelAllEntity(ZEntityPoolTree config){
			foreach (var en in config.GetAllEntities()) {
				DelEntityFile (en.Name);
			}
		}
		    
		/// <summary>
		/// Loads A component.
		/// </summary>
		/// <returns>The A component.</returns>
		/// <param name="sr">Sr.</param>
		static IZComponent LoadAComponent(StreamReader sr)
		{
			string sl = "";
			string ret = "";
			Type comType = default(Type);

			//read name
			sl = sr.ReadLine();
			comType = ComponentsPool<IZComponent>.GetType(sl);

			while (sl != null)
			{
				sl = sr.ReadLine();
				if (sl.CompareTo (ComponentPrefix) == 0)
					break;
				
				if (sl != "")
					ret+= sl;
			}
			//sr.Close();


			//create component
			//IZComponent c = ComponentsPool<IZComponent>.DrawAddComponentMenu();
			IZComponent c = ComponentsPool<IZComponent>.CreateComponent (comType);

			object boxedStruct = c;

			EditorJsonUtility.FromJsonOverwrite(ret, boxedStruct);	//editor.curPool.Json.text

			c = (IZComponent)boxedStruct;

			return c;
		}


		static private string GetCurPoolEntityPath(){
			return localPoolPath + curPoolName + "/Entities/";
		}

		/// <summary>
		/// Saves the entity pool config data.
		/// </summary>
		/// <param name="config">Config.</param>
		static public void SaveEntityPoolConfigData(ZEntityPoolTree config)
		{
			string strFileName = config.Name + "_pool.json";
			string path = Application.dataPath + localPoolPath + strFileName;

			string strData = EditorJsonUtility.ToJson (config, true);

			System.IO.FileInfo file = new System.IO.FileInfo (path);
			StreamWriter sw = file.CreateText ();
			sw.WriteLine (strData);
			sw.Close ();
			sw.Dispose ();
		}

		/// <summary>
		/// Loads the entity pool config data.
		/// </summary>
		/// <returns>The entity pool config data.</returns>
		/// <param name="name">Name.</param>
		static public ZEntityPoolTree LoadEntityPoolConfigData(string name){
			string strFileName = name + "_pool.json";
			string path = Application.dataPath + localPoolPath + strFileName;

			//System.IO.FileInfo file = new System.IO.FileInfo (path);

			string sl = "";
			string ret = "";
			ZEntityPoolTree data = new ZEntityPoolTree ();
			string entityPath = Application.dataPath +localPoolPath + name + "/Entities/";

			if (!System.IO.Directory.Exists(entityPath))
			{
				System.IO.Directory.CreateDirectory (entityPath);
			}

			if (!System.IO.File.Exists(path))
			{
				return data;	
			}



			try
			{
				StreamReader sr = new StreamReader(path, Encoding.Default);

				while (sl != null)
				{
					sl = sr.ReadLine();
					if (sl != "")
						ret+= sl;
				}
				sr.Close();

			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
			}


			object boxedStruct = data;

			EditorJsonUtility.FromJsonOverwrite(ret, boxedStruct);	//editor.curPool.Json.text

			return (ZEntityPoolTree)boxedStruct;

		}

		/// <summary>
		/// Loads the pool.
		/// </summary>
		/// <returns>The pool.</returns>
		/// <param name="name">Name.</param>
		static public void LoadPool(UnityEngine.Object target)
		{
			ZEntityPoolTree config = EntityPoolEditorBuildUtils.LoadEntityPoolConfigData(target.name);

			var poolConfig = (EntityPoolConfig)target;

			poolConfig.Reset ();

			poolConfig.InitEntityPoolConfigData(config); 
			  

			foreach (var e in config.GetAllEntities()) {

				ZEntity en = EntityPoolEditorBuildUtils.LoadEntity (e.Name, e.ID);

				poolConfig.EntityFileMgr.AddAsset( LoadEntityFileAsset (e.Name));

				poolConfig.AddEntityTemplate (en);

			}
		}

		/// <summary>
		/// Switchs the pool.
		/// </summary>
		/// <param name="target">Target.</param>
		static public void SwitchPool(UnityEngine.Object target){
			var poolConfig = (EntityPoolConfig)target;
			if (poolConfig != null)
				curPoolName = poolConfig.name;
		}
	}
}

