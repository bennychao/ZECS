using System;
using System.Collections.Generic;
using System.Linq;

namespace ZECS
{
	public static partial class EntityPoolBuilder
	{
//		static internal PoolAllStartEndEvent OnAllStartEnd;
//		static internal PoolDestoryPreEvent OnPreAllDestory;

		static public Dictionary <string, EntityPool> Pools = new Dictionary <string, EntityPool> ();

		static public void InitPool(this EntityPool pool){

			pool.Init ();
			Pools.Add(pool.Name, pool);
			//pool.OnStartEnd += CheckAllStarted;
		}

		static public EntityPool FindByName(string name){
			return Pools [name];
		}

		/// <summary>
		/// Finds the type of the entity ob.
		/// </summary>
		/// <returns>The entity ob type.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		static public ZEntity FindEntityTemplateByType<T>() where T : IZComponent
		{
			var coms = Pools.Values.Where ((a) => a.FindEntityTemplateByType<T> () != null).ToList ();
			if (coms == null || coms.Count == 0)
				return null;
			var com = 	coms.Select((a)=> a.FindEntityTemplateByType<T>()).Single();
			return com;
		}

		/// <summary>
		/// Finds the type of the entity by.
		/// </summary>
		/// <returns>The entity by type.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		static public ZEntity FindEntityByType<T>() where T : IZComponent
		{
			var coms = Pools.Values.Where ((a) => a.FindEntityByType<T> () != null).ToList();
			if (coms == null || coms.Count == 0)
				return null;
			
			var com = coms.Select((a)=> a.FindEntityByType<T>()).Single();
			return com;
		}

//		static private Dictionary<string, bool> startStatus = new Dictionary<string, bool>();
//
//		static private void CheckAllStarted(EntityPool pool){
//			if (!startStatus.ContainsKey (pool.Name)) {
//				startStatus.Add (pool.Name, true);
//			}
//
//			if (startStatus.Count == Pools.Count) {
//				if (OnAllStartEnd != null) {
//					OnAllStartEnd ();
//				}
//			}
//		}
	}
}

