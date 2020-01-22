using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FigureContainer", menuName = "Figure/Create Figure Container", order = 1)]
public class FigureContainer : ScriptableObject
{
    public List<Figure> AllFigures;
}
