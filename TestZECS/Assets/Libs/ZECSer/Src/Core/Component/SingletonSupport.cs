using System;
using System.Collections.Generic;

namespace ZECS
{
	[DontShowInComponents]
	[System.Serializable]
	public static class SingletonEntity<T> where T : IZComponent
	{  
		private static  Dictionary<Type, ZEntity> _instances = new Dictionary<Type, ZEntity>();
		public static T Instance{
			get {
				ZEntity retEn = null;
				_instances.TryGetValue (typeof(T), out retEn);
				if (retEn == null) {
					retEn =( EntityPoolBuilder.FindEntityByType<T> ());

					if (retEn == null) {
						//find template
						retEn =( EntityPoolBuilder.FindEntityTemplateByType<T> ());

						if (retEn != null) {
							//create the entity
							retEn = retEn.pool.CreateEntity(retEn.ID);
						} else {
							throw new System.Exception ("No this template entity");
						}
					}
				}

				if (retEn != null)
					return retEn.GetComponent<T> ();

				else
					return default(T);
			}
		}
	}
}

