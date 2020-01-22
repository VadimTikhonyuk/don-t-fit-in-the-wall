using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FigureManagerEditor : EditorWindow
{
	static FigureManager manager;
	List<Figure> figures = new List<Figure> ();

	Vector2 catalogScrollPos = new Vector2 ();

	private int id = -1;
	private bool selectPlayerPos = false;
	private bool selectPlayerTarget = false;
	private bool selectIconPos = false;

	private Texture2D PlayerPosIcon;
	private Texture2D PlayerTargetIcon;
	private Texture2D CoinIcon;

	[MenuItem("SquareBeam/FigureManager")]
	static void Init()
	{
		// Get existing open window or if none, make a new one:
		FigureManagerEditor window = (FigureManagerEditor)EditorWindow.GetWindow(typeof(FigureManagerEditor));
		window.Show();
	}

	void OnEnable()
	{
		PlayerPosIcon = (Texture2D)Resources.Load ("Gizmos/PlayerPos");
		PlayerTargetIcon = (Texture2D)Resources.Load ("Gizmos/PlayerTarget");
		CoinIcon = (Texture2D)Resources.Load ("Gizmos/Coin");

	}


	void OnGUI()
	{
		manager = (FigureManager)EditorGUILayout.ObjectField (manager, typeof(FigureManager), true);

		if (manager == null) {
//			if (FigureManager.Instance == null) 
//			{
//				manager = FigureManager.Instance;
//			}
//			else
//			{
//				manager = (FigureManager)EditorGUILayout.ObjectField (manager, typeof(FigureManager), true);//, GUILayout.MaxWidth(50), GUILayout.MinHeight(50));
//			}
		} 
		else 
		{
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Revert")) {
				figures = manager.AllFigures;

			}
			if (GUILayout.Button ("Apply")) {
				manager.AllFigures = figures;
			}
			GUILayout.EndHorizontal ();

			catalogScrollPos = EditorGUILayout.BeginScrollView(catalogScrollPos, false, true, GUILayout.Width(Screen.width),  GUILayout.Height(Screen.height - 90)); 

			for (int i = 0; i < figures.Count; i++) 
			{
				GUILayout.Label ("FIGURE : " + (i + 1).ToString (),EditorStyles.centeredGreyMiniLabel);
				if (id != i) {
					if (GUILayout.Button ("Remove")) {
						figures.RemoveAt (i);
					}
					if (GUILayout.Button ("SelectPlayerPos")) {
						selectPlayerPos = true;
						id = i;
					}
					if (GUILayout.Button ("SelectPlayerTarget")) {
						selectPlayerTarget = true;
						id = i;
					}
					if (GUILayout.Button ("SelectCoinPos")) {
						selectIconPos = true;
						id = i;
					}
				} 
				else if(selectPlayerPos || selectPlayerTarget || selectIconPos) 
				{
					if (GUILayout.Button ("Cancel")) 
					{
						CancelSelecting ();
					}
				}
				GUILayout.BeginHorizontal ();

				for (int x = 0; x < 6; x++)
				{
					GUILayout.BeginVertical ();

					for (int y = 3; y >= 0; y--) 
					{
						if (figures [i] == null)
							return;
					    if (figures [i]._figure == null)
							return;


						if (figures [i].PlayerPos == new Vector2 (x, y)) 
						{
							if (GUILayout.Button (PlayerPosIcon, GUILayout.MaxWidth (25), GUILayout.MaxHeight (25))) 
							{
								figures [i].PlayerPos = new Vector2 (-1, 0);
							}
						} 
						else if (figures [i].PlayerTarget == new Vector2 (x, y))
						{
							if (GUILayout.Button (PlayerTargetIcon, GUILayout.MaxWidth (25), GUILayout.MaxHeight (25)))
							{
								figures [i].PlayerTarget = new Vector2 (-1, 0);
							}
						}
						else if (figures [i].CoinPos.Contains(new Vector2 (x, y))) 
						{
							if (GUILayout.Button (CoinIcon, GUILayout.MaxWidth (25), GUILayout.MaxHeight (25))) 
							{
								figures [i].CoinPos.Remove(new Vector2 (x, y));
							}
						} 
						else 
						{

							if (!figures [i]._figure.Contains (new Vector2 (x, y))) //Select Obstacle
							{
								if (id != i) 
								{
									if (GUILayout.Button ("+", GUILayout.MaxWidth (25), GUILayout.MaxHeight (25))) 
									{
										figures [i]._figure.Add (new Vector2 (x, y));
									}
								} 
								else if (selectPlayerPos == true) //Select Player Pos
								{
									if (GUILayout.Button ("P", GUILayout.MaxWidth (25), GUILayout.MaxHeight (25))) 
									{
										figures [i].PlayerPos = new Vector2 (x, y);
										CancelSelecting ();
									}
								} 
								else if (selectPlayerTarget == true) //Select Player Target
								{
									if (GUILayout.Button ("T", GUILayout.MaxWidth (25), GUILayout.MaxHeight (25))) 
									{
										figures [i].PlayerTarget = new Vector2 (x, y);
										CancelSelecting ();
									}
								}
								else if (selectIconPos == true) //Select Coin Pos
								{
									if (GUILayout.Button ("C", GUILayout.MaxWidth (25), GUILayout.MaxHeight (25))) 
									{
										if (figures [i].CoinPos.Contains (new Vector2 (-1, 0))) 
										{
											figures [i].CoinPos [0] = new Vector2 (x,y);
										} 
										else 
										{
											figures [i].CoinPos.Add (new Vector2 (x, y));
										}
										CancelSelecting ();
									}
								}
							} 
							else 
							{
								if (GUILayout.Button ("X", GUILayout.MaxWidth (25), GUILayout.MaxHeight (25)))
								{
									figures [i]._figure.Remove (new Vector2 (x, y));
								}
							}
						}
					}
					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				GUILayout.Space (10);
			}
			if (GUILayout.Button ("AddNew")) 
			{
				Figure fig = new Figure ();
				fig._figure = new List<Vector2> ();
				figures.Add (fig);
			}

			EditorGUILayout.EndScrollView();
		}
	}

	private void CancelSelecting()
	{
		id = -1;
		selectPlayerPos = false;
		selectPlayerTarget = false;
		selectIconPos = false;
	}

}
