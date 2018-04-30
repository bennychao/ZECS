using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas.VisualDebugging.Unity.Editor;
using DesperateDevs.Utils;
using UnityEditor;
using UnityEditorInternal;
using ZECS;
using System;
using System.Linq;
using DesperateDevs.Unity.Editor;
using CommonEditorLib;

namespace ZECSEditor
{
	/// <summary>
	/// Entity pool inspector editor.
	/// </summary>
	[CustomEditor(typeof(EntityPoolConfig))]
	[CanEditMultipleObjects]
	public class EntityPoolInspectorEditor : Editor {

		static  public EntityPoolInspectorEditor curEditor = null;

		EntityPoolConfig poolConfig;
		ZEntity curEntity;

		GUIStyle entityStyle;
		GUIStyle componentStyle;

		SerializedObject unfoldObj;


		void OnEnable()
		{
			curEditor = this;
			poolConfig = (EntityPoolConfig)target;
			EntityPoolEditorBuildUtils.SwitchPool (target);
			LoadPoolFromConfig (target.name);


			if (!Application.isPlaying)
				AssetDatabase.Refresh ();
		}

		void OnDestroy() {
			//Debug.Log("Script was destroyed");
			//unfoldMap.SaveToJson ();

		}

		void OnDisable() {
			//Debug.Log("script was removed");
			//AssetDatabase.Refresh ();
			//unfoldMap.SaveToJson ();
		}

		/// <summary>
		/// Loads the pool from config.
		/// </summary>
		/// <param name="name">Name.</param>
		private void LoadPoolFromConfig(string name)
		{
			//load the saved entity
			if (poolConfig != null && !poolConfig.IsReady && !Application.isPlaying) {

				Reload ();
			}


			serializedObject.SetIsDifferentCacheDirty ();
		}

		/// <summary>
		/// Reload this instance.
		/// </summary>
		private void Reload(){
			EntityPoolEditorBuildUtils.BuildLocalPathBase ();
			EntityPoolEditorBuildUtils.LoadPool (target);
			Repaint ();
		}



		//在这里方法中就可以绘制面板。
		public override void OnInspectorGUI()
		{
			curEntity = poolConfig.CurEntity;

			serializedObject.Update(); 
			EditorGUILayout.BeginHorizontal ();

			if (GUILayout.Button ("Edit")) {
				ZEditorWindow.ShowEditor ();
			}

			var oldColor = GUI.color;
			GUI.color = Color.yellow;
			EditorGUILayout.LabelField ("Entity x " + poolConfig.CurPool.TemplateCount);

			GUILayout.FlexibleSpace(); 

			GUI.color = oldColor;


			if (GUILayout.Button ("Reload")) {
				Reload ();
			}

			EditorGUILayout.EndHorizontal ();

			if (curEntity != null) {
				DrawEntity ();
			} else {
				EditorGUILayout.LabelField ("please select a entity");
			}

			//serializedObject.Update();  
			serializedObject.SetIsDifferentCacheDirty ();
			EditorApplication.DirtyHierarchyWindowSorting ();
			serializedObject.ApplyModifiedProperties();

		}

		/// <summary>
		/// Draws the entity.
		/// </summary>
		void DrawEntity()
		{
			EditorGUI.BeginChangeCheck ();

			var componentColor = Color.HSVToRGB(0.5f, 0.7f, 1f);
			componentColor.a = 0.15f;
			entityStyle = new GUIStyle(GUI.skin.box);

			entityStyle.normal.background = InspectorDrawer.createTexture(2, 2, componentColor);

			List<IZComponent> coms = curEntity.GetComponents<IZComponent> ();


			//draw current selected entity
			EditorGUILayout.BeginVertical (entityStyle);

			//show the entity name
			EditorGUILayout.BeginHorizontal ();

			EditorGUILayout.LabelField ("Entity Name: " + curEntity.Name);
			//curEntity.Name = EditorGUILayout.TextField (curEntity.Name);
			EditorGUILayout.EndHorizontal ();

			//show component info
			EditorGUILayout.BeginHorizontal ();

			EditorGUILayout.LabelField ("Component x " + coms.Count, GUILayout.MaxWidth(100));
			GUILayout.FlexibleSpace(); 

			//show the component menu
			IZComponent c = curEntity.EType == EntityType.Entity ? 
				ComponentsPool<IZComponent>.DrawAddComponentMenu() : 
				ComponentsPool<IZComponent>.DrawAddSystemMenu();
			
			if (c != null) {
				var com = ComponentsPool<IZComponent>.CreateComponent (c.GetType ());
				curEntity.AddComponent (com);
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.EndVertical ();


			//Draw Component
			componentColor = Color.HSVToRGB(0.9f, 0.1f, 0.9f);
			componentColor.a = 0.15f;
			entityStyle = new GUIStyle(GUI.skin.box);

			entityStyle.normal.background = InspectorDrawer.createTexture(2, 2, componentColor);

			///EditorGUILayout.Space();

			foreach (IZComponent com in coms)
			{
				DrawComponent(com);
			}


			if (EditorGUI.EndChangeCheck ()) {
				// Code to execute if GUI.changed
				// was set to true inside the block of code above.

				if (poolConfig != null && poolConfig.CurPool != null) {
					//Debug.Log("EndChangeCheck");
					EntityPoolEditorBuildUtils.SaveEntity (curEntity);

					serializedObject.SetIsDifferentCacheDirty ();
					EditorApplication.DirtyHierarchyWindowSorting ();
					EditorApplication.RepaintProjectWindow ();
					//AssetDatabase.Refresh ();
					//EditorApplication.
				}
			}
		}

		//bool bUnfolded = false;
		/// <summary>
		/// Draws the component.
		/// </summary>
		/// <param name="com">COM.</param>
		void DrawComponent(IZComponent com)
		{
			int iconSize = (int)EditorGUIUtility.singleLineHeight;
			//draw component member
			Rect rect = EditorGUILayout.BeginVertical (entityStyle);
			Type ty = com.GetType();
			var memberInfos = ty.GetPublicMemberInfos ();

			bool bUnfolded = poolConfig.unfoldMap.IsUnfold(curEntity.Name + ty.Name);
			//SerializedProperty unfoledObj = serializedObject.FindProperty ("unfoldMap");

			//draw component file info
			EditorGUILayout.BeginHorizontal();
			{
				if (memberInfos.Count == 0) {
					EditorGUILayout.LabelField("    " + ty.Name, EditorStyles.boldLabel);
				} else {
					bool bNewUnfold = EditorLayout.Foldout(bUnfolded, ty.Name, iconSize + 10);
					//EditorGUILayout.LabelField(ty.Name, EditorStyles.boldLabel);
					if (bNewUnfold != bUnfolded) {
						poolConfig.unfoldMap.SetUnfold (curEntity.Name + ty.Name, bNewUnfold);

//						unfoldObj.Update();
//						unfoldObj.ApplyModifiedProperties ();
					}
				}
				if (EditorLayout.MiniButton("-")) {
					//entity.RemoveComponent(index);
					curEntity.DelComponent(com);
				}
			}
			EditorGUILayout.EndHorizontal();

			//Draw the icon
			if (ComponentsPool<IZComponent>.IsSystem(com)) {
				GUI.DrawTexture (new Rect (rect.x + 1, rect.y + 1, 
					iconSize - 2, iconSize - 2), IconHelper.SystemsHierarchyIcon);
			} else {
				GUI.DrawTexture (new Rect (rect.x + 1, rect.y + 1, 
					iconSize - 2, iconSize - 2), IconHelper.EntityHierarchyIcon);
			}


			if (bUnfolded)
				InspectorDrawer.DrawObjectMember (com);

			EditorGUILayout.EndVertical ();


		}


	}

}
