using System;
using System.Collections.Generic;

namespace ZECS
{

	[System.Serializable]
	[DontShowInComponentsAttribute]
	public class ZComponent : IZComponent
	{
		internal ZEntity entity;

		public ZEntity Entity{
			get{
				return entity;
			}
		}

		public ZComponent ()
		{
		}

		/// <summary>
		/// Raises the start event.
		/// </summary>
		protected virtual void OnStart(){
		}

		/// <summary>
		/// Raises the destory event.
		/// </summary>
		protected virtual void OnDestory(){
		}

		/// <summary>
		/// Start this instance.
		/// </summary>
		void IZComponent.Start()
		{
			OnStart();
		}
		/// <summary>
		/// Destory this instance.
		/// </summary>
		void IZComponent.Destory()
		{
			OnDestory();
		}

		/// <summary>
		/// Adds the component.
		/// </summary>
		/// <returns>The component.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public T AddComponent<T>() where T : IZComponent
		{
			T a = (T)Activator.CreateInstance (typeof(T));
			entity.AddComponent (a);
			return a;
		}

		/// <summary>
		/// Gets the component.
		/// </summary>
		/// <returns>The component.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public T GetComponent<T>() where T : IZComponent
		{
			return entity.GetComponent<T>();
		}

		/// <summary>
		/// Gets the components.
		/// </summary>
		/// <returns>The components.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public List<T> GetComponents<T>(){
			return entity.GetComponents<T>();
		}

		/// <summary>
		/// Gets the component in sub entities.
		/// </summary>
		/// <returns>The component in sub entities.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public T GetComponentInSubEntities<T>() where T : IZComponent{
			return entity.GetComponentInSubEntities<T> ();
		}

		/// <summary>
		/// Gets the components in sub entities.
		/// </summary>
		/// <returns>The components in sub entities.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public List<T> GetComponentsInSubEntities<T>()  where T : IZComponent{
			return entity.GetComponentsInSubEntities<T> ();
		}

		/// <summary>
		/// Finds the type of the entity ob.
		/// </summary>
		/// <returns>The entity ob type.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
//		static public T FindEntityObType<T>()  where T : IZComponent
//		{
//			return EntityPoolBuilder.FindEntityTemplateByType<T> ();
//		}
	}
}

