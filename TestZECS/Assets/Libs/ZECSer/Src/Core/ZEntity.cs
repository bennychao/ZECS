using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace ZECS
{
	public enum EntityType 
	{
		Entity,
		System,
		Data
	}

	[System.Serializable]
	public sealed class ZEntity
	{
		public int ID;
		public string Name;

		public EntityType EType  = EntityType.Entity;

		//use for runtime
		[System.NonSerialized]
		internal EntityPool pool;	


		[System.NonSerialized]
		internal List<ZEntity> subEntities = new List<ZEntity> ();

		public List<ZEntity> SubEntities
		{
			get{
				return subEntities;
			}
		}
		//use for runtime end

		private List<IZComponent> components = new List<IZComponent> ();

		/// <summary>
		/// Initializes a new instance of the <see cref="ZECS.ZEntity"/> class.
		/// </summary>
		public ZEntity ()
		{
			if (subEntities == null)
				subEntities = new List<ZEntity> ();
		}

		/// <summary>
		/// Adds the component.
		/// </summary>
		/// <param name="com">COM.</param>
		public void AddComponent(IZComponent com)
		{
			components.Add (com);
		}

		/// <summary>
		/// Dels the component.
		/// </summary>
		/// <param name="com">COM.</param>
		public void DelComponent(IZComponent com)
		{
			components.Remove (com);
		}

		/// <summary>
		/// Gets the component.
		/// </summary>
		/// <returns>The component.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public T GetComponent<T>()
		{
			IZComponent c = components.Find ((a) => a.GetType () == typeof(T) || a.GetType ().IsSubclassOf (typeof(T)));
			if (c != null)
				return (T)c;

			return default(T);
		}

		/// <summary>
		/// Gets the components.
		/// </summary>
		/// <returns>The components.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public List<T> GetComponents<T>()
		{
			List<IZComponent> c = components.FindAll ((a) =>
				{
					var atype = a.GetType ();
						
					return	atype == typeof(T) 
						|| atype.IsSubclassOf (typeof(T)) 
						|| typeof(T).IsAssignableFrom(a.GetType());
				});
			if (c != null)
				return c.Select((a)=> (T)a).ToList();

			return null;
		}

		/// <summary>
		/// Sets the get component in sub entities.
		/// </summary>
		/// <value>The get component in sub entities.</value>
		public T GetComponentInSubEntities<T>() where T : IZComponent{
			IZComponent t = GetComponent<T> () as IZComponent;
			if (t == null && subEntities != null) {


				foreach (var sub in subEntities) {
					t = sub.GetComponent<T> () as IZComponent;
					if (t != null)
						return (T)t;
				}
			}
			return (T)t;
		}

		/// <summary>
		/// Gets the components in sub entities.
		/// </summary>
		/// <returns>The components in sub entities.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public List<T> GetComponentsInSubEntities<T>(){
			List<T> ret = new List<T> ();
			ret.AddRange (GetComponents<T> ());

			if (subEntities != null)
				foreach (var sub in subEntities) {
					ret.AddRange (sub.GetComponentsInSubEntities<T> ());
				}

			return ret;
		}

		/// <summary>
		/// Gets the component support attribute.
		/// </summary>
		/// <returns>The component support attribute.</returns>
		/// <param name="attrType">Attr type.</param>
		public AType GetSupportAttribute<AType>(){

			var coms = components
				.Where ((c) => {
				var attributes = c.GetType ().GetCustomAttributes (typeof(AType), false);
				return attributes != null && attributes.Count () > 0;
			}).ToArray ();

			if (coms != null &&  coms.Count() > 0){
				return coms.Select ((c) => {
						var attributes = c.GetType ().GetCustomAttributes (typeof(AType), false);

						return attributes.Select ((a) => (AType)a).Single ();
					}).Single ();
			}

			return default(AType);
				
		}

		/// <summary>
		/// Gets the component count.
		/// </summary>
		/// <returns>The component count.</returns>
		public int GetComponentCount()
		{
			return components.Count;
		}

		/// <summary>
		/// Clone this instance.
		/// </summary>
		public ZEntity Clone()
		{
			ZEntity newEntity = new ZEntity();
			newEntity.Name = Name;
			newEntity.ID = ID;

			foreach (var e in components) {
				var newCom = EntityPoolRuntimeBuildUtils.CloneObject (e);
				if (newCom != null)
					newEntity.AddComponent ((IZComponent)newCom);
			}

			return newEntity;
		}

		/// <summary>
		/// Start this instance.
		/// </summary>
		internal void Start(){
			//start components
			foreach (var e in components) {


				ZComponent ze = e as ZComponent;
				if (ze != null) {
					ze.entity = this;
				}

				ZSystem zs = e as ZSystem;
				if (zs != null) {
					zs.entity = this;
				}

				e.Start ();
			}
		}

		/// <summary>
		/// Destory this instance.
		/// </summary>
		internal void Destory(){
			foreach (var e in components) {
				e.Destory ();
			}

			if (subEntities != null)
				foreach (var sub in subEntities) {
					sub.Destory ();
				}
		}

		//for pool friend
		//protected 
	}
}

