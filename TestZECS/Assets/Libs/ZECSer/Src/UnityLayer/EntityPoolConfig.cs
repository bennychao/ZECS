using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZECS
{
	public class EntityPoolConfig : MonoBehaviour {

		[SerializeField]
		public EntityInspectorUnfoldDatas unfoldMap = new EntityInspectorUnfoldDatas();

		[SerializeField]
		public ZEntityPoolTree PoolNodeRoot = null;//new ZEntityPoolData();

		[SerializeField]
		public EntityJsonRuntimeMgr EntityFileMgr = new EntityJsonRuntimeMgr();

		private EntityPool entityPool = null;//new EntityPool();


		//save current entity's info
		private ZEntity curConfigEntity = null;   
		private int curConfigEntityID = -1;


		public bool IsReady{
			get{
				return PoolNodeRoot != null && entityPool != null ;
			}
		}


		public ZEntity CurEntity 
		{
			get{
				if (curConfigEntityID >= 0) {
					
					var get = entityPool.FindEntityTemplateByID (curConfigEntityID);
					if (get != null)
						curConfigEntity = get;
				}

				return curConfigEntity;
			}
		}

		public EntityPool CurPool
		{
			get{
				return entityPool;
			}
		}

		void Awake()
		{
			RuntimeBuildPool ();

			entityPool.InitPool ();

		}
		// Use this for initialization only this runtime
		void Start () {

		}

		/// <summary>
		/// Runtimes the build pool.
		/// </summary>
		private void RuntimeBuildPool(){
			
			InitEntityPoolConfigData (this.PoolNodeRoot);

			foreach (var e in PoolNodeRoot.GetAllEntities()) {

				string strData = EntityFileMgr.FindAsset (e.Name);

				ZEntity en = EntityPoolRuntimeBuildUtils.LoadEntity (e.Name, e.ID, strData);
				entityPool.AddEntityTemplate (en);

			}
			//build the entity tree
			entityPool.PoolNodeRoot = PoolNodeRoot;
		}

		/// <summary>
		/// Inits the entity pool config data.
		/// </summary>
		/// <param name="poolData">Pool data.</param>
		public void InitEntityPoolConfigData(ZEntityPoolTree poolData)
		{
			entityPool = new EntityPool ();

			this.PoolNodeRoot = poolData;
			//this.startID = poolData.GetEntityStartID ();
			entityPool.Name = gameObject.name;
			PoolNodeRoot.Name = gameObject.name;
			entityPool.Name = poolData.Name;
		}

		/// <summary>
		/// Creates the system.
		/// </summary>
		public void CreateSystem()
		{
			if (Application.isPlaying || !Application.isEditor) {
				throw new System.Exception ("this function can't call in run time");
			}
			
			int id = PoolNodeRoot.GetEntityStartID ();
			string enName = "system" + id.ToString();

			CreateSystem(id, enName);

			PoolNodeRoot.AddEntityData(new ZEntityNode(id, enName), null);
		}

		/// <summary>
		/// Creates the entity at root
		/// </summary>
		public void CreateEntity(){
			if (Application.isPlaying || !Application.isEditor) {
				throw new System.Exception ("this function can't call in run time");
			}
			
			CreateEntityAt (null);
		}

		/// <summary>
		/// Creates the entity at node.
		/// </summary>
		/// <param name="node">Node.</param>
		public void CreateEntityAt(ZEntityNode node){
			if (Application.isPlaying || !Application.isEditor) {
				throw new System.Exception ("this function can't call in run time");
			}

			int id = PoolNodeRoot.GetEntityStartID ();
			string enName = "entity" + id.ToString();

			CreateEntity(id, enName);

			PoolNodeRoot.AddEntityData(new ZEntityNode(id, enName), node);
		}


		/// <summary>
		/// Adds the entity template.
		/// </summary>
		/// <param name="en">En.</param>
		public void AddEntityTemplate(ZEntity en){
			if (Application.isPlaying || !Application.isEditor) {
				throw new System.Exception ("this function can't call in run time");
			}

			CurPool.AddEntityTemplate (en);
		}

		/// <summary>
		/// Deletes the entity.
		/// </summary>
		/// <returns>The entity.</returns>
		/// <param name="en">En.</param>
		public ZEntity DeleteEntity(ZEntity en){
			if (Application.isPlaying || !Application.isEditor) {
				throw new System.Exception ("this function can't call in run time");
			}
			
			entityPool.DelEntityTemplate (en.ID);
			//PoolData.Entities.RemoveAll ((a) => a.ID == en.ID);
			PoolNodeRoot.DeleteNode(en.ID);
			return en;
		}

		/// <summary>
		/// Deletes the current entity.
		/// </summary>
		/// <returns>The current entity.</returns>
		public ZEntity DeleteCurEntity()
		{			
			if (Application.isPlaying || !Application.isEditor) {
				throw new System.Exception ("this function can't call in run time");
			}

			if (curConfigEntity != null) {
				return DeleteEntity (curConfigEntity);
			}
			return null;
		}

		/// <summary>
		/// Deletes all entity.
		/// </summary>
		public void DeleteAllEntity()
		{
			if (Application.isPlaying || !Application.isEditor) {
				throw new System.Exception ("this function can't call in run time");
			}

			entityPool.DelAllEntityTemplate ();
			PoolNodeRoot.Clear ();
		}

		/// <summary>
		/// Creates the entity.
		/// </summary>
		/// <param name="id">Identifier.</param>
		/// <param name="name">Name.</param>
		private void CreateEntity(int id, string name){
			if (Application.isPlaying || !Application.isEditor) {
				throw new System.Exception ("this function can't call in run time");
			}

			ZEntity en = new ZEntity();

			en.ID = id;
			en.Name = name;
			curConfigEntity = en;
			curConfigEntityID = id;
			entityPool.AddEntityTemplate (en);   
		}

		/// <summary>
		/// Creates the system.
		/// </summary>
		/// <param name="id">Identifier.</param>
		/// <param name="name">Name.</param>
		private void CreateSystem(int id, string name){
			if (Application.isPlaying || !Application.isEditor) {
				throw new System.Exception ("this function can't call in run time");
			}

			ZEntity en = new ZEntity();

			en.ID = id;
			en.Name = name;
			en.EType = EntityType.System;
			curConfigEntity = en;
			curConfigEntityID = id;

			entityPool.AddEntityTemplate (en);   
		}

		/// <summary>
		/// Sels A entity.
		/// </summary>
		/// <param name="id">Identifier.</param>
		public ZEntityNode SelAEntity(int id)   
		{
//			if (Application.isPlaying || !Application.isEditor) {
//				throw new System.Exception ("this function can't call in run time");
//			}

			curConfigEntity = entityPool.FindEntityTemplateByID (id);

			curConfigEntityID = id;

			//ret current node
			ZEntityNode node = PoolNodeRoot.FindNode(id);
			return node;
		}

		/// <summary>
		/// Reset this instance.
		/// </summary>
		public void Reset()
		{
			if (Application.isPlaying || !Application.isEditor) {
				throw new System.Exception ("this function can't call in run time");
			}

			PoolNodeRoot = null;
			if (entityPool != null) {
				entityPool.ClearTemplate ();
				entityPool = null;
			}

			EntityFileMgr.Clear ();
		}


	}
}
