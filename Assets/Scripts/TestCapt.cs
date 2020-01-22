using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class TestCapt : MonoBehaviour 
{


	public Camera photoCamera;
	public GameObject canvas;
	public Text sharedScore;
	private Texture2D renderedTexture;

	// Use this for initialization
	void Start () {
		//RenderAndShare ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private IEnumerator ShareScore(string text, int score, string url, string subject)
	{
		
		canvas.SetActive (true);
		sharedScore.text = score.ToString ();
		string screenShotPath = Application.persistentDataPath + "/" + "screenshot.png";
		if(File.Exists(screenShotPath)) File.Delete(screenShotPath);

		File.WriteAllBytes (screenShotPath, RenderImage ().EncodeToPNG ());
		yield return new WaitForSeconds(.05f);
		NativeShare.Share(text, screenShotPath, "", "", "image/png", true, "");
		canvas.SetActive (false);
	}

	Texture2D RenderImage()
	{
		
		photoCamera.gameObject.SetActive (true);
		int sqr = 512;

		RenderTexture tempRT = new RenderTexture(sqr,sqr, 0 );

		photoCamera.targetTexture = tempRT;
		photoCamera.Render();

		RenderTexture.active = tempRT;
		//Texture2D virtualPhoto = new Texture2D(sqr-128,sqr-128, TextureFormat.ARGB32, false);
		Texture2D virtualPhoto = new Texture2D(sqr,sqr, TextureFormat.ARGB32, false);
		// false, meaning no need for mipmaps
		//virtualPhoto.ReadPixels( new Rect(64, 64, sqr-64,sqr-64), 0, 0);
		virtualPhoto.ReadPixels( new Rect(0, 0, sqr,sqr), 0, 0);
		renderedTexture = virtualPhoto;
		renderedTexture.Apply ();
		RenderTexture.active = null; //can help avoid errors 
		photoCamera.targetTexture = null;
		photoCamera.gameObject.SetActive (false);
		return renderedTexture;
	}

}
