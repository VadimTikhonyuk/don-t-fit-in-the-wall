using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Figure
{
	public List<Vector2> _figure = new List<Vector2>();
	public Vector2 PlayerPos = new Vector2(-1,0);
	public Vector2 PlayerTarget = new Vector2(-1,0);
	public List<Vector2> CoinPos = new List<Vector2>(){new Vector2(-1,0)};
}
