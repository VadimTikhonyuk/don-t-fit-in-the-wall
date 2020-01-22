using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pattern
{
	public int Id;
	public bool isLocked = true;
	public int Price = 100;
	public Sprite Icon;
	public Color LockColor;
	[Space(10)]
	public Color PlayerColor = Color.white;
	public Texture2D WallTexture;
	public Vector2 WallTexturePadding = new Vector2(1,1);
}

public class PatternManager : MonoBehaviour 
{
	public List<Pattern> Patterns;
	public RectTransform Parent;
	public UIPattern Prefab;
	public Material PlayerMaterial;
	public Material WallMaterial;

	void Awake()
	{
		int lastSelectedPattern = SPlayerPrefs.GetInt ("SelectedPattern");

		Pattern pattern = Patterns.Find (x => x.Id == lastSelectedPattern);

		PlayerMaterial.color = pattern.PlayerColor;
		WallMaterial.mainTexture = pattern.WallTexture;
		WallMaterial.mainTextureScale = pattern.WallTexturePadding;
	}

	void Start()
	{


		for (int i = 0; i < Patterns.Count; i++) 
		{
			Pattern pattern = Patterns [i];
			UIPattern uiPattern = Instantiate (Prefab, Parent) as UIPattern;

			bool locked = false;
			if(i != 0) locked = (SPlayerPrefs.GetInt ("PatternUnlocked") == 1) ? false : true;
		
			uiPattern.Init (this,pattern.Id, locked,pattern.Price, pattern.LockColor, pattern.Icon);
		}

	}

	public void SelectPattern(int id, bool firstSelection)
	{
		if (firstSelection) {
			GameManager.Instance.OnUnlockPattern ();
		} else {
			GameManager.Instance.OnSelectPattern ();
		}

		Pattern pattern = Patterns.Find (x => x.Id == id);

		PlayerMaterial.color = pattern.PlayerColor;
		WallMaterial.mainTexture = pattern.WallTexture;
		WallMaterial.mainTextureScale = pattern.WallTexturePadding;

		SPlayerPrefs.SetInt ("SelectedPattern", id);
	}


}
