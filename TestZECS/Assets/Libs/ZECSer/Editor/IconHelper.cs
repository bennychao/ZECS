using System;
using UnityEngine;
using DesperateDevs.Unity.Editor;
using UnityEditor;

namespace ZECS
{
	[InitializeOnLoad]
	public class IconHelper
	{
		protected static Texture2D _systemsErrorHierarchyIcon;

		public static Texture2D SystemsHierarchyIcon {
			get {
				if (_systemsErrorHierarchyIcon == null) {
					_systemsErrorHierarchyIcon = EditorLayout.LoadTexture("l:EntitasSystemsErrorHierarchyIcon");
				}
				return _systemsErrorHierarchyIcon;
			}
		}

		public static Texture2D entityHierarchyIcon {
			get {
				if (_entityHierarchyIcon == null) {
					_entityHierarchyIcon = EditorLayout.LoadTexture("l:EntitasEntityHierarchyIcon");
				}
				return _entityHierarchyIcon;
			}
		}

		static Texture2D _entityErrorHierarchyIcon;
		public static Texture2D entityErrorHierarchyIcon {
			get {
				if (_entityErrorHierarchyIcon == null) {
					_entityErrorHierarchyIcon = EditorLayout.LoadTexture("l:EntitasEntityErrorHierarchyIcon");
				}
				return _entityErrorHierarchyIcon;
			}
		}


		protected static Texture2D _entityHierarchyIcon;

		public static Texture2D EntityHierarchyIcon {
			get {
				if (_entityHierarchyIcon == null) {
					_entityHierarchyIcon = EditorLayout.LoadTexture("l:EntitasContextErrorHierarchyIcon");
				}
				return _entityHierarchyIcon;
			}
		}
		protected static Texture2D _connectedIcon;
		protected static Texture2D _unconnectedIcon;
		public static Texture2D connectedIcon {
			get {
				if (_connectedIcon == null) {
					_connectedIcon = EditorLayout.LoadTexture("l:ConnectedIcon");
				}
				return _connectedIcon;
			}
		}

		public static Texture2D unconnectedIcon {
			get {
				if (_unconnectedIcon == null) {
					_unconnectedIcon = EditorLayout.LoadTexture("l:NoConnectedIcon");
				}
				return _unconnectedIcon;
			}
		}

	}
}

