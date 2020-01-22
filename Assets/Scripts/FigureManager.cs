using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class FigureManager : MonoBehaviour 
{
	public List<Figure> AllFigures;


	public static FigureManager Instance;

	void Awake()
	{
		Instance = this;
	}

	public Figure RandomFigure()
	{
		//return AllFigures [AllFigures.Count-1];
		return AllFigures [Random.Range (0, AllFigures.Count-1)];
	}
}
