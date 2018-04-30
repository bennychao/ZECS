using System;
using UnityEngine;
using System.IO;
using System.Text;
using DesperateDevs.Utils;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace ZECS
{
	public class EntityPoolRuntimeBuildUtils
	{
		static string ComponentPrefix = "&&&&&&&&";

		/// <summary>
		/// Loads the entity.
		/// </summary>
		/// <returns>The entity.</returns>
		/// <param name="name">Name.</param>
		static public ZEntity LoadEntity(string name, int ID, string strData){

			//create the entity
			ZEntity newEntity = new ZEntity ();
			newEntity.Name = name;
			newEntity.ID = ID;
			if (strData != null) {
				StringReader sr = new StringReader (strData);

				string sl = "";

				sl = sr.ReadLine();
				//newEntity.ID = Convert.ToInt32(sl);
				//read type
				sl = sr.ReadLine();
				newEntity.EType = (EntityType)Enum.Parse(typeof(EntityType), sl);

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
			return newEntity;
		}

		/// <summary>
		/// Loads A component.
		/// </summary>
		/// <returns>The A component.</returns>
		/// <param name="sr">Sr.</param>
		static IZComponent LoadAComponent(StringReader sr)
		{
			string sl = "";
			string ret = "";
			Type comType = default(Type);

			//read name
			sl = sr.ReadLine();
			comType = ComponentsRuntimePool<IZComponent>.GetType(sl);

			while (sl != null)
			{
				sl = sr.ReadLine();
				if (sl.CompareTo (ComponentPrefix) == 0)
					break;

				if (sl != "")
					ret+= sl;
			}
			//sr.Close();

			IZComponent c = ComponentsRuntimePool<IZComponent>.CreateComponent (comType);

			object boxedStruct = c;

			JsonUtility.FromJsonOverwrite(ret, boxedStruct);	//editor.curPool.Json.text

			c = (IZComponent)boxedStruct;

			return c;
		}

		static public object CloneObject(object o){
			BinaryFormatter inputFormatter = new BinaryFormatter();  
			MemoryStream inputStream;  
			using (inputStream = new MemoryStream())  
			{  
				inputFormatter.Serialize(inputStream, o);  
			}  

			object obj = null;
			//将二进制流反序列化为对象  
			using (MemoryStream outputStream = new MemoryStream(inputStream.ToArray()))  
			{  
				BinaryFormatter outputFormatter = new BinaryFormatter();  
				obj = outputFormatter.Deserialize(outputStream);  
			} 

			return obj;
		}

	}


	public class ComponentsRuntimePool<T>{
		static T[] componentInfos = null;
		static ComponentsRuntimePool()
		{
			if (componentInfos == null)
			{
				componentInfos = System.AppDomain.CurrentDomain.GetInstancesOf<T>();
			}
		}

		static public SubT CreateComponent<SubT>()
		{
			T newType = componentInfos.ToList ().Find ((t) => t.GetType () == typeof(SubT));
			return (SubT)Activator.CreateInstance (newType.GetType ());
		}
//
		static public T CreateComponent(Type type1)
		{
			T newType = componentInfos.ToList ().Find ((t) => t.GetType () == type1);
			return (T)Activator.CreateInstance (newType.GetType ());
		}

		static public Type GetType(string strType)
		{
			T newType = componentInfos.ToList ().Find ((t) => t.GetType ().Name == strType);
			return newType.GetType ();
		}
	}
}

