using System;
using System.Collections.Generic;
using System.Linq;

namespace ZECS
{

	/// <summary>
	/// Entity pool.
	/// </summary>
	public class EntityPool : IDisposable
	{
		public string Name;
		internal PoolStartEndEvent OnStartEnd = null;
		internal PoolDestoryPreEvent OnPreDestory;

		private List<ZEntity> entities = new List<ZEntity>();
		private List<ZEntity> entitieTemplates = new List<ZEntity>();

		internal ZEntityPoolTree PoolNodeRoot {
			get;
			set;
		}

		/// <summary>
		/// Gets the entities.
		/// </summary>
		/// <value>The entities.</value>
		public List<ZEntity> Entities
		{
			get {
				return entities;
			}
		}

		/// <summary>
		/// Gets the template count.
		/// </summary>
		/// <value>The template count.</value>
		public int TemplateCount{
			get{
				return entitieTemplates.Count;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ZECS.EntityPool"/> class.
		/// </summary>
		public EntityPool ()
		{
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		public void Init(){
			CreateAutoEntities ();

			if (OnStartEnd != null)
				OnStartEnd (this);
		}

		/// <summary>
		/// Releases all resource used by the <see cref="ZECS.EntityPool"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="ZECS.EntityPool"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="ZECS.EntityPool"/> in an unusable state. After calling
		/// <see cref="Dispose"/>, you must release all references to the <see cref="ZECS.EntityPool"/> so the garbage
		/// collector can reclaim the memory that the <see cref="ZECS.EntityPool"/> was occupying.</remarks>
		public void Dispose()
		{
			foreach (var e in entities) {
				e.Destory ();
			}

			entities.Clear ();
		
			if (OnPreDestory != null)
				OnPreDestory (this);
		}

		/// <summary>
		/// Creates the auto entities.
		/// </summary>
		internal void CreateAutoEntities()
		{
			//create autocreate entities (with the autocreate support component)

			var autos = entitieTemplates.ToArray ().Where ((t) => {
				return 
					//(t.GetType() == typeof(ZSystemEntity)) 
					t.EType == EntityType.System
					//|| typeof(IZSystem).IsAssignableFrom( t.GetType())
					|| t.GetComponent<AutoCreateSupport>() != null;
			}).ToList();

			if (autos != null) {
				foreach (var a in autos) {
					CreateEntity (a.ID);
					//entities.Add (en);
				}
			}

			//start Entities
//			var autoEntities = entities.Where( (e)=> e.EType != EntityType.System).ToList();
//			if (autoEntities != null) {
//				foreach (var ae in autoEntities) {
//					StartEntity (ae);
//				}
//			}

			//start System
//			var systemEntities = entities.Where( (e)=> e.EType == EntityType.System).ToList();
//			if (systemEntities != null) {
//				foreach (var ae in systemEntities) {
//					ae.Start ();
//				}
//			}

		}

		/// <summary>
		/// Starts the entity.
		/// </summary>
		/// <param name="newEntity">New entity.</param>
		private void StartEntity(ZEntity newEntity){
			if (newEntity != null) {
				newEntity.Start ();
			}
		}

		/// <summary>
		/// Creates the entity. for runtime
		/// </summary>
		/// <returns>The entity.</returns>
		/// <param name="ID">I.</param>
		public ZEntity CreateEntity(int ID)
		{
			ZEntity template = FindEntityTemplateByID (ID);
			if (template == null) {
				return null;
			}

			ZEntity newEntity = Clone(template);

			if (newEntity != null) {
				//build the sub entity
				BuildSubTree(newEntity);

				entities.Add (newEntity);

				newEntity.pool = this;

				//if (newEntity.EType != EntityType.System)
				newEntity.Start ();


			}

			return newEntity;
		}

		/// <summary>
		/// Builds the sub tree.
		/// </summary>
		/// <param name="newEntity">New entity.</param>
		private void BuildSubTree(ZEntity newEntity, ZEntityNode curNode = null){
			if (newEntity == null)
				return;


			ZEntityNode node = curNode;
			if (node == null)
				node = PoolNodeRoot.FindNode (newEntity.ID);
			
			if (node == null)
				return;

			foreach (var subNode in node.subs) {
				var subEntity = CreateSubEntity (subNode.ID);
				if (newEntity.subEntities == null)
					newEntity.subEntities = new List<ZEntity> ();
				newEntity.subEntities.Add (subEntity);
				BuildSubTree (subEntity, subNode);
			}
				
		}

		/// <summary>
		/// Creates the sub entity.
		/// </summary>
		/// <returns>The sub entity.</returns>
		/// <param name="ID">I.</param>
		private ZEntity CreateSubEntity(int ID){
			ZEntity template = FindEntityTemplateByID (ID);
			if (template == null) {
				return null;
			}

			ZEntity newEntity = Clone(template);
			if (newEntity != null) {
				newEntity.pool = this;
				newEntity.Start ();

			}

			return newEntity;
		}

		/// <summary>
		/// Clone the specified en.
		/// </summary>
		/// <param name="en">En.</param>
		public ZEntity Clone(ZEntity en)
		{
			ZEntity newEntity = (ZEntity)EntityPoolRuntimeBuildUtils.CloneObject ((object)en);

			return newEntity;
		}

		/// <summary>
		/// Releases the entity.
		/// </summary>
		/// <param name="en">En.</param>
		public void ReleaseEntity(ZEntity en){
			if (en != null) {
				en.Destory ();

				entities.Remove (en);

				//TODO reuse the entity
			}
		}

		/// <summary>
		/// Adds the entity template.
		/// </summary>
		/// <param name="en">En.</param>
		internal void AddEntityTemplate(ZEntity en)
		{
			entitieTemplates.Add (en);

			en.pool = this;
		}

		/// <summary>
		/// Dels the entity template.
		/// </summary>
		/// <param name="id">Identifier.</param>
		internal void DelEntityTemplate(int id)
		{
			entitieTemplates.RemoveAll ((a) => a.ID == id);
		}

		/// <summary>
		/// Dels all entity template.
		/// </summary>
		internal void DelAllEntityTemplate()
		{
			entitieTemplates.Clear ();
		}

		/// <summary>
		/// Finds the entity template by I.
		/// </summary>
		/// <returns>The entity template by I.</returns>
		/// <param name="id">Identifier.</param>
		public ZEntity FindEntityTemplateByID(int id)
		{
			return entitieTemplates.Find ((a) => a.ID == id);
		}

		/// <summary>
		/// Finds the type of the entity template by.
		/// </summary>
		/// <returns>The entity template by type.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public ZEntity FindEntityTemplateByType<T>() where T : IZComponent
		{
			return entitieTemplates.Find ((a) => a.GetComponentInSubEntities<T>() != null);
		}

		/// <summary>
		/// Finds the type of the entity by.
		/// </summary>
		/// <returns>The entity by type.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public ZEntity FindEntityByType<T>() where T : IZComponent
		{
			return entities.Find ((a) => a.GetComponentInSubEntities<T>() != null);
		}

		/// <summary>
		/// Clears the template.
		/// </summary>
		internal void ClearTemplate()
		{
			entitieTemplates.Clear ();
		}

	}
}

