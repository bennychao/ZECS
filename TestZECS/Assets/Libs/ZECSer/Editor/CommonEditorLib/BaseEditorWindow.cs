using System;
using UnityEngine;

using UnityEditor;
using System.IO;
using System.Text;
using System.Collections.Generic;
using ZECS;

namespace CommonEditorLib
{
	public class BaseEditorWindow  : EditorWindow
	{
		protected EidtorWindowLayouts root = new EidtorWindowLayouts();

		public Action<int> OnSelWindow = null;
		public Action<int, int> OnLinkNode = null;

		private int curWndID = -1;
		public string Path;
		public string WndName;

		//for draw connection
		protected bool bRelationSupport = true;
		protected bool bTempLine = false;
		protected Vector2 TempStartPos = Vector2.zero;
		protected int iTempStartID = -1;

		public BaseEditorWindow ()
		{
			
		}
		public int CurSelWndID{
			get{
				return curWndID;
			}
		}
		public string ReadConfigFile(string fileName)
		{
			string path = Application.dataPath + @"/ZECSData/Config/" + fileName + "_config.json";
			//System.IO.FileInfo file = new System.IO.FileInfo (path);

			string sl = "";
			string ret = "";
			try
			{
				StreamReader sr = new StreamReader(path, Encoding.Default);

				while (sl != null)
				{
					sl = sr.ReadLine();
					if (sl != "")
						ret+= sl;
				}
				sr.Close();

			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
			}

			return ret;

		}

		public void SaveConfigFile(string fileName, string strData)
		{
			string path = Application.dataPath + @"/ZECSData/Config/" + fileName + "_config.json";

			System.IO.FileInfo file = new System.IO.FileInfo (path);
			StreamWriter sw = file.CreateText ();
			sw.WriteLine (strData);
			sw.Close ();
			sw.Dispose ();

		}

		protected void DrawBackgroundGrid()
		{
			//EditorGUI.DrawRect (new Rect(0, 0, position.width, position.height), Color.grey);

			//int iColumns = 10;
			float LineWidth = 0.5f;
			int GridSize = 10;

			int iColumnCount = (int)(position.width / GridSize);
			int iRowCount = (int)(position.height / GridSize);

			Color lineColor = Color.black;

			for (int x = 0; x < iRowCount + 1; x++) {
				lineColor.a = x % 10 == 0 ? 0.3f : 0.1f;
				EditorGUI.DrawRect (new Rect(0, x * GridSize, position.width, LineWidth), lineColor);
			}


			for (int y = 0; y < iColumnCount + 1; y++) {
				lineColor.a = y % 10 == 0 ? 0.3f : 0.1f;
				EditorGUI.DrawRect (new Rect(y * GridSize, 0 , LineWidth, position.height), lineColor);
			}


		}

		public void DrawItemWindow()
		{
			List<EditorWindowLayoutItem> ents = root.items;//curPool.GetEntities ();
			//int id = 0;
			foreach (EditorWindowLayoutItem d in ents) {
				//Rect r = new Rect (d.rect);
				//d.wndID = id++;
				//GUI.color = Color.yellow;
				PreDrawNodeWindow(d);
				d.rect = GUI.Window(d.wndID, d.rect, DrawNodeWindow, d.title);		//"Status " + d.wndID.ToString()
			
				if (bRelationSupport) {
					GUI.DrawTexture(d.GetRightCenterRect(10), IconHelper.entityErrorHierarchyIcon);
				}
			}

			DoMouseInMain ();
		}

		protected virtual void PreDrawNodeWindow(EditorWindowLayoutItem item)
		{
		}


		protected virtual void DrawNodeWindow(int id)
		{
			//EditorGUI.DrawRect(new Rect(0, 0, 100, 70), Color.green);
			DoMouse (id);
			GUI.DragWindow();
		}

		static public void DrawConCurve(Vector3 start, Vector3 end, Color color)
		{
			Vector3 startTan = start + Vector3.right * 20;
			Vector3 endTan = end + Vector3.left * 20;
			Handles.DrawBezier(start, end, startTan, endTan, color, null, 2);
		}

		void DoMouseInMain()
		{
			Event e = Event.current;

			if (e != null) {
				switch (e.type) { 
				case EventType.MouseDown:    

					TempStartPos = Event.current.mousePosition;
					iTempStartID = root.CheckInRightRect (TempStartPos, 10);

					if (iTempStartID >= 0) {
						bTempLine = true;
					}

					curWndID = -1;
					if (OnSelWindow != null)
						OnSelWindow (curWndID);
					break;

				case EventType.MouseUp:

					int iTempEndID = root.CheckInDragRect (Event.current.mousePosition);
					bTempLine = false;

					if (iTempEndID >= 0 && iTempStartID != iTempEndID) {
						if (OnLinkNode != null)
							OnLinkNode (iTempStartID, iTempEndID);
					}

					break;

				case EventType.MouseMove:
				case EventType.MouseDrag:
					e.Use ();
					break;
				}
			}

			if (bTempLine) {
				//e.Use ();
				//Debug.Log("TempStartPos " + TempStartPos.ToString() + " mouse " + Event.current.mousePosition);
				DrawConCurve (TempStartPos,  Event.current.mousePosition, Color.gray);
			}
		}

	

		void DoMouse(int id)
		{
			Event e = Event.current;

			if (e != null) {
				switch (e.type) { 
				case EventType.MouseDown:  


					break;

				case EventType.MouseUp:
					SaveLayout ();

					curWndID = id;

					if (OnSelWindow != null)
						OnSelWindow (curWndID);

//					int iTempEndID = root.CheckInDragRect (Event.current.mousePosition);
//					bTempLine = false;
//
//					if (iTempStartID >= 0 && iTempStartID != iTempEndID) {
//						if (OnLinkNode != null)
//							OnLinkNode (iTempStartID, iTempEndID);
//					}

					break;



				}
			}
		}



		public void SaveLayout()
		{
			string strData = EditorJsonUtility.ToJson (root, true);
			SaveConfigFile (WndName, strData);
		}

		//class end
	}
}

