using System;
using System.Collections.Generic;

namespace ZECS
{
	
	/// <summary>
	/// Z system.
	/// </summary>
	[System.Serializable]
	[DontShowInComponentsAttribute]
	public class ZSystem : IZSystem
	{
		internal ZEntity entity;

		public ZEntity Entity{
			get{
				return entity;
			}
		}

		public ZSystem ()
		{
		}

		protected virtual void OnStart(){
		}
		protected virtual void OnDestory(){
		}

		protected virtual void OnExecute(){
			
		}

		void IZSystem.Execute(){
			OnExecute ();
		}
		void IZComponent.Start()
		{
			OnStart();
		}
		void IZComponent.Destory()
		{
			OnDestory();
		}

		/// <summary>
		/// Gets the component.
		/// </summary>
		/// <returns>The component.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public T GetComponent<T>() where T : IZSystem
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
		public T GetComponentInSubEntities<T>() where T : IZSystem{
			return entity.GetComponentInSubEntities<T> ();
		}

		/// <summary>
		/// Gets the components in sub entities.
		/// </summary>
		/// <returns>The components in sub entities.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public List<T> GetComponentsInSubEntities<T>()  where T : IZSystem{
			return entity.GetComponentsInSubEntities<T> ();
		}
	}
}

