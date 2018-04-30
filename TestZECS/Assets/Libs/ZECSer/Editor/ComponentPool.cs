using System;
using CommonEditorLib;
using UnityEditor;
using System.Linq;
using UnityEngine;
using DesperateDevs.Unity.Editor;
using ZECS;
using DesperateDevs.Utils;

namespace ZECSEditor
{
	/// <summary>
	/// Components drawer.
	/// </summary>   
	public class ComponentsPool<T>{
		static T[] componentInfos = null;

		/// <summary>
		/// Initializes the <see cref="ZECSEditor.ComponentsPool`1"/> class.
		/// </summary>
		static ComponentsPool()
		{
			if (componentInfos == null)
				componentInfos = InspectorDrawer.GetTypes<T> ()
					.Where ((info) => {
						var comAttribute = info.GetType ().GetCustomAttributes (typeof(DontShowInComponentsAttribute), false);
						return comAttribute == null || comAttribute.Count () <= 0;
					}).ToArray ();
		}

		/// <summary>
		/// Draws the add component menu.
		/// </summary>
		/// <returns>The add component menu.</returns>
		static public T DrawAddComponentMenu() {
			var componentInfoss = componentInfos.Where (
				(info) => {
					return 
						!typeof(IZSystem).IsAssignableFrom(info.GetType());
				}
			).ToArray();

			var componentNames = componentInfoss
				.Select(info => info.GetType().Name )
				.ToArray();
			var index = EditorGUILayout.Popup( -1, componentNames, GUILayout.MaxWidth(40));

			if (index >= 0) {
				return componentInfoss[index];
			}

			return default(T);
		}

		/// <summary>
		/// Draws the add system menu.
		/// </summary>
		/// <returns>The add system menu.</returns>
		static public T DrawAddSystemMenu(){
			var componentInfoss = componentInfos.Where (
				 (info) => {
					return typeof(IZSystem).IsAssignableFrom(info.GetType());
				}
			).ToArray();

			var componentNames = componentInfoss
				.Select(info => info.GetType().Name )
				.ToArray();
			
			var index = EditorGUILayout.Popup( -1, componentNames, GUILayout.MaxWidth(40));

			if (index >= 0) {
				return componentInfoss[index];
			}

			return default(T);
		}

		/// <summary>
		/// Creates the component.
		/// </summary>
		/// <returns>The component.</returns>
		/// <typeparam name="SubT">The 1st type parameter.</typeparam>
		static public SubT CreateComponent<SubT>()
		{
			T newType = componentInfos.ToList ().Find ((t) => t.GetType () == typeof(SubT));
			return (SubT)Activator.CreateInstance (newType.GetType ());
		}

		/// <summary>
		/// Creates the component.
		/// </summary>
		/// <returns>The component.</returns>
		/// <param name="type1">Type1.</param>
		static public T CreateComponent(Type type1)
		{
			T newType = componentInfos.ToList ().Find ((t) => t.GetType () == type1);
			return (T)Activator.CreateInstance (newType.GetType ());
		}

		/// <summary>
		/// Gets the type.
		/// </summary>
		/// <returns>The type.</returns>
		/// <param name="strType">String type.</param>
		static public Type GetType(string strType)
		{
			T newType = componentInfos.ToList ().Find ((t) => t.GetType ().Name == strType);
			return newType.GetType ();
		}


		/// <summary>
		/// Determines if is system the specified type1.
		/// </summary>
		/// <returns><c>true</c> if is system the specified type1; otherwise, <c>false</c>.</returns>
		/// <param name="type1">Type1.</param>
//		static public bool IsSystem<SubT>(){
//			//return typeof(IZSystem).IsAssignableFrom(type1.GetType());
//			var componentInfoss = componentInfos.Where (
//				(info) => {
//					return typeof(IZSystem).IsAssignableFrom(info.GetType());
//				}
//			).ToList();
//			//Debug.Log ("Issystem " + type1.Name);
//			return componentInfoss.Find ((a) => a.GetType () == typeof(SubT)) != null;
//		}


		static public bool IsSystem(object o)
		{
			return typeof(IZSystem).IsAssignableFrom(o.GetType());
//			var componentInfoss = componentInfos.Where (
//				(info) => {
//					return typeof(IZSystem).IsAssignableFrom(info.GetType());
//				}
//			).ToList();
//
//			return componentInfoss.Find ((a) => a.GetType() == o.GetType()) != null;
		}
	}

}

