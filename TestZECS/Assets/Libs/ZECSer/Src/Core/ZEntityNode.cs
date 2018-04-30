using System;
using System.Collections.Generic;
using System.Linq;

namespace ZECS
{
	/// <summary>
	/// Z entity node.
	/// </summary>
	[System.Serializable]
	public class ZEntityNode
	{
		public int ID;
		public string Name;
		public List<ZEntityNode> subs = new List<ZEntityNode>();

		public ZEntityNode()
		{
		}

		public ZEntityNode(int ID, string name){
			this.ID = ID;
			this.Name = name;
		}

		public ZEntityNode FindNode(int ID){
			var node = subs.Find((a)=> a.ID == ID);

			if (node == null) {
				foreach (var n in subs) {
					node = n.FindNode (ID);
					if (node != null)
						break;
				}
			}

			return node;
		}

		public ZEntityNode FindNodeWithParent(int ID, out ZEntityNode parent){

			var node = subs.Find((a)=> a.ID == ID);
			parent = null;

			if (node == null) {
				foreach (var n in subs) {
					node = n.FindNodeWithParent (ID, out parent);
					if (node != null)
						break;
				}
			} else {
				parent = this;
			}

			return node;
		}

		public List<KeyValuePair<int , int> > GetRelation(){
			List<KeyValuePair<int , int> > ret = new List<KeyValuePair<int, int>> ();
			ret = subs.Select ((a) => {
				return new KeyValuePair<int, int>(ID, a.ID);
			}).ToList();

			foreach (var e in subs) {
				ret.AddRange(e.GetRelation ());
			}

			return ret;
		}


		public List<ZEntityNode> GetAllEntities(){
			List<ZEntityNode> ret = new List<ZEntityNode> ();

			ret.AddRange (subs);
			foreach (var n in subs) {
				ret.AddRange(n.GetAllEntities());
			}

			return ret;
		}
	}

	/// <summary>
	/// Z entity pool tree.
	/// </summary>
	[System.Serializable]
	public class ZEntityPoolTree
	{
		public string Name;
		public int StartID;
		public List<ZEntityNode> Entities = new List<ZEntityNode>();

		public int GetEntityStartID()
		{
			return StartID++;
		}

		/// <summary>
		/// Adds the entity data.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="parent">Parent.</param>
		public void AddEntityData(ZEntityNode data, ZEntityNode parent = null){
			if (parent != null) {
				parent.subs.Add (data);
			} else {
				Entities.Add (data);
			}
		}

		/// <summary>
		/// Finds the node.
		/// </summary>
		/// <returns>The node.</returns>
		/// <param name="id">Identifier.</param>
		public ZEntityNode FindNode(int id){

			ZEntityNode node = Entities.Find((a)=> a.ID == id);

			if (node == null) {
				foreach (var n in Entities) {
					node = n.FindNode (id);
					if (node != null)
						break;
				}
			}
			return node;
		}

		/// <summary>
		/// Finds the node with parent.
		/// </summary>
		/// <returns>The node with parent.</returns>
		/// <param name="id">Identifier.</param>
		/// <param name="parent">Parent.</param>
		public ZEntityNode FindNodeWithParent(int id, out ZEntityNode parent){

			parent = null;
			ZEntityNode node = Entities.Find((a)=> a.ID == id);

			if (node == null) {
				foreach (var n in Entities) {
					node = n.FindNodeWithParent (id, out parent);
					if (node != null)
						break;
				}
			}

			return node;
		}

		/// <summary>
		/// Gets all entities.
		/// </summary>
		/// <returns>The all entities.</returns>
		public List<ZEntityNode> GetAllEntities(){
			List<ZEntityNode> ret = new List<ZEntityNode> ();
			ret.AddRange (Entities);
			foreach (var n in Entities) {
				ret.AddRange(n.GetAllEntities());
			}

			return ret;
		}

		/// <summary>
		/// Sets the relation.
		/// </summary>
		/// <param name="parentID">Parent I.</param>
		/// <param name="subID">Sub I.</param>
		public void SetRelation(int parentID, int subID)
		{
			ZEntityNode parentParentNode = null;
			ZEntityNode parentNode = FindNodeWithParent (parentID, out parentParentNode);

			ZEntityNode oldParentNode = null;
			ZEntityNode subNode = FindNodeWithParent (subID, out oldParentNode);

			if (oldParentNode != null && oldParentNode.ID == parentID)
				return;

			if (parentParentNode != null && parentParentNode.ID == subID)
				return;

			if (parentNode == null)
				return;


			parentNode.subs.Add (subNode);

			if (oldParentNode != null) {
				oldParentNode.subs.Remove (subNode);
			} else {
				Entities.Remove (subNode);
			}
		}

		/// <summary>
		/// Determines whether this instance isolate relation the specified subID.
		/// </summary>
		/// <returns><c>true</c> if this instance isolate relation the specified subID; otherwise, <c>false</c>.</returns>
		/// <param name="subID">Sub I.</param>
		public void IsolateRelation(int subID){
			ZEntityNode oldParentNode = null;
			ZEntityNode subNode = FindNodeWithParent (subID, out oldParentNode);

			if (oldParentNode != null) {
				oldParentNode.subs.Remove (subNode);

				Entities.Add (subNode);
			}
		}

		/// <summary>
		/// Gets the relation.
		/// </summary>
		/// <returns>The relation.</returns>
		public List<KeyValuePair<int , int> > GetRelation(){
			List<KeyValuePair<int , int> > ret = new List<KeyValuePair<int, int>> ();
			foreach (var e in Entities) {
				ret.AddRange(e.GetRelation ());
			}

			return ret;
		}

		/// <summary>
		/// Deletes the node.
		/// </summary>
		/// <param name="id">Identifier.</param>
		public void DeleteNode(int id){
			ZEntityNode parent = null;
			var node = FindNodeWithParent (id, out parent);
			if (parent != null)
				parent.subs.Remove (node);
			else
				Entities.Remove (node);

		}

		/// <summary>
		/// Rename the specified id and name.
		/// </summary>
		/// <param name="id">Identifier.</param>
		/// <param name="name">Name.</param>
		public void Rename(int id, string name){
			var node = FindNode (id);
			if (node != null) {
				node.Name = name;
			}
		}


		public void Clear(){
			Entities.Clear ();
		}

	}
}

