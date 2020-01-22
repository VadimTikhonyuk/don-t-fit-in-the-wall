using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[CustomEditor(typeof(Figure))]
//[CanEditMultipleObjects]
public class FigureEditor : Editor
{
//	List<Vector2> _wall = new List<Vector2> ();
//	//SerializedProperty figure;
//	List<Vector2> _customFigure = new List<Vector2>();
//
//	SerializedObject myTarget;
//	Figure figure;
//	void OnEnable()
//	{
//		myTarget = new SerializedObject(target);
//		figure = (Figure)target;
//
//		for (int x = 0; x < 6; x++) {
//			for (int y = 0; y < 4; y++) {
//				_wall.Add (new Vector2(x,y));
//
//			}
//		}
//		_customFigure = figure._figure;
//
//	}
//
//	public override void OnInspectorGUI()
//	{ 
//		if (_customFigure == null)
//			return;
//		GUILayout.BeginHorizontal ();
//		for (int x = 0; x < 6; x++) {
//			GUILayout.BeginVertical ();
//			for (int y = 3; y >= 0; y--) 
//			{
//				if(!_customFigure.Contains(new Vector2(x,y)))
//				{
//					if (GUILayout.Button ("+", GUILayout.MinWidth(50f),GUILayout.MinHeight(50f))) {
//						_customFigure.Add (new Vector2(x,y));
//					}
//				}
//				else
//				{
//					if (GUILayout.Button ("X", GUILayout.MinWidth(50f),GUILayout.MinHeight(50f))) {
//						_customFigure.Remove (new Vector2(x,y));
//					}
//				}
//			}
//			GUILayout.EndVertical ();
//		}
//		GUILayout.EndHorizontal ();
//
//		if (GUILayout.Button ("Apply")) 
//		{
//			figure._figure = _customFigure;
////			var serializedObject = new SerializedObject(target);
////			var property = serializedObject.FindProperty("_figure");
////			serializedObject.Update();
////			property.
////			EditorGUILayout.PropertyField(property, true);
////			serializedObject.ApplyModifiedProperties();
//		}
//	}


}
