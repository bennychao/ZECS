using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Entitas.VisualDebugging.Unity.Editor;
using DesperateDevs.Utils;
using System;
using UnityEditor;
using System.Linq;

namespace CommonEditorLib
{
	/// <summary>
	/// Dont draw component attribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class DontDrawComponentAttribute : Attribute {
	}

	/// <summary>
	/// Inspector drawer.
	/// </summary>
	public class InspectorDrawer
	{
		public static readonly ITypeDrawer[] _typeDrawers;

		static InspectorDrawer()
		{
			_typeDrawers = System.AppDomain.CurrentDomain.GetInstancesOf<ITypeDrawer>();
		}

		/// <summary>
		/// Gets the type drawer.
		/// </summary>
		/// <returns>The type drawer.</returns>
		/// <param name="type">Type.</param>
		static public ITypeDrawer getTypeDrawer(System.Type type) {
			foreach (var drawer in _typeDrawers) {
				if (drawer.HandlesType(type)) {
					return drawer;
				}
			}

			return null;
		}

		/// <summary>
		/// Gets the types.
		/// </summary>
		/// <returns>The types.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		static public T[] GetTypes<T>(){
			T[] rets = default(T[]);

			rets = System.AppDomain.CurrentDomain.GetInstancesOf<T>();
			return rets;
		}

		/// <summary>
		/// Creates the texture.
		/// </summary>
		/// <returns>The texture.</returns>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		/// <param name="color">Color.</param>
		static public Texture2D createTexture(int width, int height, Color color) {
			var pixels = new Color[width * height];
			for (int i = 0; i < pixels.Length; ++i) {
				pixels[i] = color;
			}
			var result = new Texture2D(width, height);
			result.SetPixels(pixels);
			result.Apply();
			return result;
		}

		/// <summary>
		/// Draws the object member.
		/// </summary>
		/// <param name="obj">Object.</param>
		static public void DrawObjectMember(object obj)
		{
			Type ty = obj.GetType();
			var types = ty.GetPublicMemberInfos ();

			foreach (var t in types) {
				//t.SetValue (a, 1);

				var memberValue = t.GetValue (obj);
				var memberType = memberValue == null ? t.type : memberValue.GetType ();

				ITypeDrawer typeDrawer = InspectorDrawer.getTypeDrawer(t.type);
				if (typeDrawer != null) {

					var newValue = typeDrawer.DrawAndGetNewValue (memberType, t.name, memberValue, t);

					t.SetValue (obj, newValue);

				} else {
					var shouldDraw = !Attribute.IsDefined(memberType, typeof(DontDrawComponentAttribute));
					if (shouldDraw) {
						EditorGUILayout.LabelField(t.name, memberValue.ToString());

						var indent = EditorGUI.indentLevel;
						EditorGUI.indentLevel += 1;

						EditorGUILayout.BeginVertical();
						{
							DrawObjectMember(memberValue);
							//						foreach (var info in memberType.GetPublicMemberInfos()) {
							//							var mValue = info.GetValue(memberValue);
							////							var mType = mValue == null ? info.type : mValue.GetType();
							////							DrawObjectMember(mType, info.name, mValue, memberValue, info.SetValue);
							////							if (memberType.IsValueType) {
							////								setValue(target, value);
							////							}
							//							DrawObjectMember(mValue);
							//						}
						}
						EditorGUILayout.EndVertical();

						EditorGUI.indentLevel = indent;
					}
				}
			}
		}
	}

}

