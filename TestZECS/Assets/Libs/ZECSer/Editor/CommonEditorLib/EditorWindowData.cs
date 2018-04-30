using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZECS;
using System.Linq;
using ZECSEditor;

namespace CommonEditorLib
{

	[System.Serializable]
	public class EditorWindowLayoutItem {
		public Rect rect = new Rect(10, 10, 100, 50);	
		public int wndID;
		public string title;

		public Vector2 GetLeftCenterPoint(){
			return new Vector2 (rect.x - 1, rect.y + (rect.height / 2));
		}

		public Vector2 GetRightCenterPoint(){
			return new Vector2 (rect.x + rect.width + 10, rect.y + (rect.height / 2));
		}

		public Rect GetRightCenterRect(float rectSize){
			return new Rect (rect.x + rect.width + 1, rect.y + (rect.height / 2) - (rectSize / 2), rectSize, rectSize);
		}
	}

	/// <summary>
	/// Eidtor window layouts.
	/// </summary>
	[System.Serializable]
	public class EidtorWindowLayouts
	{
		public List<EditorWindowLayoutItem> items = new List<EditorWindowLayoutItem>();

		/// <summary>
		/// Finds the item.
		/// </summary>
		/// <returns>The item.</returns>
		/// <param name="ID">I.</param>
		public EditorWindowLayoutItem FindItem(int ID){
			return items.Find ((a) => a.wndID == ID);
		}

		/// <summary>
		/// Checks the in drag rect.
		/// </summary>
		/// <returns>The in drag rect.</returns>
		/// <param name="pos">Position.</param>
		public int CheckInDragRect(Vector2 pos)
		{

			var s =  items.Find ((a) => {

				return a.rect.Contains(pos);
			});

			if (s != null) {
				return s.wndID;
			}

			//return ret;
			return -1;
		}

		/// <summary>
		/// Checks the in right rect.
		/// </summary>
		/// <returns>The in right rect.</returns>
		/// <param name="pos">Position.</param>
		public int CheckInRightRect(Vector2 pos, float rectSize)
		{

			var s =  items.Find ((a) => {

				return a.GetRightCenterRect(rectSize).Contains(pos);
			});

			if (s != null) {
				return s.wndID;
			}

			//return ret;
			return -1;
		}

		/// <summary>
		/// Syncs the layout.
		/// </summary>
		/// <returns><c>true</c>, if layout was synced, <c>false</c> otherwise.</returns>
		/// <param name="pool">Pool.</param>
		public bool SyncLayout(ZEntityPoolTree pool)
		{
			List<ZEntityNode> newItems = pool.GetAllEntities();
			if (items == null || items.Count == 0) {
				//newItems = pool.GetAllEntities();
			} else {
				var delItems = items.Where ((a) => newItems.Find ((b) => b.ID == a.wndID) != null).ToList();

				items = delItems;
				newItems = newItems.Where ((a) => items.Find ((b) => b.wndID == a.ID) == null).ToList();
			}


			foreach (var n in newItems) {
				EditorWindowLayoutItem item = new EditorWindowLayoutItem ();
				item.wndID = n.ID;
				item.title = n.Name;
				item.rect = GetRandomRect ();
				items.Add (item);
			}

			return newItems.Count > 0;
			///pool.Entities.Join(items, (a)=> a
		}

		/// <summary>
		/// Renames the title.
		/// </summary>
		/// <param name="wnd">Window.</param>
		/// <param name="title">Title.</param>
		public void RenameTitle(int wnd, string title){
			var s =  items.Find ((a) => {

				return a.wndID == wnd;
			});

			if (s != null)
				s.title = title;
		}

		/// <summary>
		/// Gets the random rect.
		/// </summary>
		/// <returns>The random rect.</returns>
		protected Rect GetRandomRect(){
			return new Rect(100, 100, 100, 50);	
		}

	}
}

