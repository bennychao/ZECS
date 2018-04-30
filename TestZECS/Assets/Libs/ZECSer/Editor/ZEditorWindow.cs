using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ZECS;
using System.IO;
using System;
using System.Text;
using CommonEditorLib;
using DesperateDevs.Unity.Editor;

namespace ZECSEditor
{
	/// <summary>
	/// Z editor window.
	/// </summary>
	public class ZEditorWindow : BaseEditorWindow {

		EntityPoolConfig curPoolConfig;

		List<KeyValuePair<int, int> > relations;
		string strTempEditEntityName;
		/// <summary>
		/// Shows the editor.
		/// </summary>
		[MenuItem("ZECS/EntityWindow")]
		static public void ShowEditor()
		{

			if (Selection.activeGameObject != null) {

				var poolConfig = Selection.activeGameObject.GetComponent<EntityPoolConfig> ();

				ZEditorWindow editor = EditorWindow.GetWindow<ZEditorWindow>("Pool Config");
				editor.curPoolConfig = poolConfig;


				if (editor.curPoolConfig != null) {
					editor.WndName = editor.curPoolConfig.gameObject.name;
					editor.loadLayout ();
				}

				editor.OnSelWindow += (wndID) => {
					//Debug.Log("OnSelWindow" + wndID);

					if (wndID >= 0)
					{
						editor.curPoolConfig.SelAEntity(wndID);

						editor.strTempEditEntityName = editor.curPoolConfig.CurEntity.Name;

						EditorApplication.RepaintHierarchyWindow();

						if (EntityPoolInspectorEditor.curEditor != null)
							EntityPoolInspectorEditor.curEditor.Repaint();
					}else{
						editor.strTempEditEntityName = "No Selected";
					}

				};

				Selection.selectionChanged += () => editor.RefreshAndRepaint();

				editor.OnLinkNode += (parentID, subID) => {
					//Debug.Log("OnLinkNode " + parentID + " sub " + subID);
					editor.curPoolConfig.PoolNodeRoot.SetRelation(parentID, subID);
					EntityPoolEditorBuildUtils.SaveEntityPoolConfigData(editor.curPoolConfig.PoolNodeRoot);
					editor.Refresh();
				};
			}



		}

		/// <summary>
		/// Refresh this instance.
		/// </summary>
		public void Refresh()
		{
			if (curPoolConfig != null) {
				WndName = curPoolConfig.gameObject.name;
				loadLayout ();
			}

		}

		public void RefreshAndRepaint(){
			OnFocus ();
			Repaint ();
		}

		/// <summary>
		/// Loads the layout.
		/// </summary>
		void loadLayout(){
			object boxedStruct = root;
			string strData = ReadConfigFile (WndName);
			EditorJsonUtility.FromJsonOverwrite(strData, boxedStruct);	//editor.curPool.Json.text

			root = (EidtorWindowLayouts)boxedStruct;

			relations = curPoolConfig.PoolNodeRoot.GetRelation ();

			//editor.curPool.unfoldMap
			if (root.SyncLayout (curPoolConfig.PoolNodeRoot)) {
				SaveLayout ();
			}
		}

		/// <summary>
		/// Raises the focus event.
		/// </summary>
		void OnFocus(){

			if (Selection.activeGameObject != null) {
				var poolConfig = Selection.activeGameObject.GetComponent<EntityPoolConfig> ();
				if (poolConfig != null && poolConfig != curPoolConfig) {
					curPoolConfig = poolConfig;

				}
			}

			Refresh ();

		}

		/// <summary>
		/// Raises the GU event.
		/// </summary>
		void OnGUI()
		{
			if (curPoolConfig == null ) {

				GUI.Label (new Rect (100, 40, Screen.width, 30), "Pelease select a Pool");  
				return;
			}

			BeginWindows();

			DrawBackgroundGrid ();

			DrawControlBar ();

			var oldColor = GUI.color;
			GUI.color = Color.yellow;
			EditorGUILayout.LabelField (curPoolConfig.name);
			GUI.color = oldColor;

			DrawItemWindow ();

			DrawRelations ();

			EndWindows();
		}



		/// <summary>
		/// Draws the control bar.
		/// </summary>
		void DrawControlBar()
		{
			var componentColor = Color.HSVToRGB(0.5f, 0.7f, 1f);
			componentColor.a = 0.15f;
			var style = new GUIStyle(GUI.skin.label);

			style.normal.background = InspectorDrawer.createTexture(2, 2, componentColor);
//
			EditorGUILayout.BeginHorizontal (style);
				

			//EditorGUILayout.LabelField ("");

			if (GUILayout.Button ("Add Entity",  GUILayout.MaxWidth(100))) {
				var node = curPoolConfig.PoolNodeRoot.FindNode (CurSelWndID);
				
				//curPoolConfig.CreateEntity ();
				curPoolConfig.CreateEntityAt(node);
				EntityPoolEditorBuildUtils.SaveEntity (curPoolConfig.CurEntity);

				EntityPoolEditorBuildUtils.SaveEntityPoolConfigData(curPoolConfig.PoolNodeRoot);
				Refresh ();
			}

			if (GUILayout.Button ("Delete Entity",  GUILayout.MaxWidth(100))) {
				ZEntity en = curPoolConfig.DeleteCurEntity ();
				EntityPoolEditorBuildUtils.DelEntityFile (en);

				EntityPoolEditorBuildUtils.SaveEntityPoolConfigData(curPoolConfig.PoolNodeRoot);

				Refresh ();
			}

			if (GUILayout.Button ("Add System",  GUILayout.MaxWidth(100))) {

				curPoolConfig.CreateSystem ();
				EntityPoolEditorBuildUtils.SaveEntity (curPoolConfig.CurEntity);

				EntityPoolEditorBuildUtils.SaveEntityPoolConfigData(curPoolConfig.PoolNodeRoot);
				Refresh ();
			}

			if (GUILayout.Button ("Reset",  GUILayout.MaxWidth(100))) {

				EntityPoolEditorBuildUtils.DelAllEntity (curPoolConfig.PoolNodeRoot);

				curPoolConfig.DeleteAllEntity ();

				EntityPoolEditorBuildUtils.SaveEntityPoolConfigData(curPoolConfig.PoolNodeRoot);

				Refresh ();
			}

			if (GUILayout.Button ("Isolate",  GUILayout.MaxWidth(100))) {
				if (curPoolConfig.CurEntity != null) {
					curPoolConfig.PoolNodeRoot.IsolateRelation (curPoolConfig.CurEntity.ID);
					EntityPoolEditorBuildUtils.SaveEntityPoolConfigData(curPoolConfig.PoolNodeRoot);
					Refresh();
				}
			}

			strTempEditEntityName = GUILayout.TextArea (strTempEditEntityName, GUILayout.MaxWidth(200));


			if (GUILayout.Button ("Rename",  GUILayout.MaxWidth(100))) {
				if (curPoolConfig.CurEntity != null && CurSelWndID >= 0) {
					//curPoolConfig.CurEntity.Name = strTempEditEntityName;
					curPoolConfig.PoolNodeRoot.Rename(curPoolConfig.CurEntity.ID, strTempEditEntityName);
					root.RenameTitle (curPoolConfig.CurEntity.ID, strTempEditEntityName);
					SaveLayout ();
					EntityPoolEditorBuildUtils.ChangeEntityName(curPoolConfig.CurEntity, strTempEditEntityName);
					EntityPoolEditorBuildUtils.SaveEntityPoolConfigData(curPoolConfig.PoolNodeRoot);
					Refresh();
				}
			}

			//curEntity.Name = EditorGUILayout.TextField (curEntity.Name);
			EditorGUILayout.EndHorizontal ();
		}

		/// <summary>
		/// Draws the relations.
		/// </summary>
		void DrawRelations(){
			if (relations == null)
				return;
			foreach (var r in relations) {
				DrawCurve (r.Key, r.Value);
			}
		}

		/// <summary>
		/// Draws the curve.
		/// </summary>
		/// <param name="fromID">From I.</param>
		/// <param name="toID">To I.</param>
		void DrawCurve(int fromID, int toID){
			var itemFrom = root.FindItem (fromID);
			var itemTo = root.FindItem (toID);

			DrawConCurve (itemFrom.GetRightCenterPoint (), itemTo.GetLeftCenterPoint (), Color.gray);
		}

		/// <summary>
		/// Pres the draw node window.
		/// </summary>
		/// <param name="item">Item.</param>
		protected override void PreDrawNodeWindow(EditorWindowLayoutItem item)
		{
			ZEntity en = curPoolConfig.CurPool.FindEntityTemplateByID (item.wndID);
			GUI.color = Color.yellow;

			if (en != null) {
				ColorAttribute ca = en.GetSupportAttribute<ColorAttribute> ();
				if (ca != null) {
					//Debug.Log ("NodeColor is " + ca.NodeColor);
					GUI.color  = ca.ToUnityColor(); 
				}
			}

			if (en.EType == EntityType.System) {
				GUI.color = Color.gray;
			}

		}

		/// <summary>
		/// Draws the node window.
		/// </summary>
		/// <param name="id">Identifier.</param>
		protected override void DrawNodeWindow(int id)
		{
			int iconSize = (int)EditorGUIUtility.singleLineHeight;

			ZEntity en = curPoolConfig.CurPool.FindEntityTemplateByID (id);

			if (en.EType == EntityType.System) {
				GUI.DrawTexture(new Rect(5, 20, iconSize, iconSize), IconHelper.SystemsHierarchyIcon);
			} else {
				GUI.DrawTexture(new Rect(5, 20, iconSize, iconSize), IconHelper.EntityHierarchyIcon);
			}



			if (en != null) {
				
				int count = en.GetComponentCount ();
				GUILayout.Label ("      x " + count.ToString());  

			}

			//draw delete button
			
			base.DrawNodeWindow (id);
		}


	}
}

