using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class FigureManager : MonoBehaviour 
{
    public FigureContainer Container;
	public static FigureManager Instance;

	void Awake()
	{
		Instance = this;
    }

	public Figure RandomFigure()
	{
        return Container.AllFigures[Random.Range(0, Container.AllFigures.Count - 1)];
    }
}
