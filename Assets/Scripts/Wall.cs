using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class Wall : MonoBehaviour 
{
	public static Wall Instance;
	public GameManager Manager;

	[System.Serializable]
	public class WallPoint
	{
		public GameObject Cube;
		public Vector2 Pos;
	}
		
	public RectTransform Line;

	public GameObject WallCellPrefab;
	public Transform MyWallParent;
	public List<WallPoint> MyWall;
	public List<WallPoint> PlayerPart;

	public PlayerControl Player;
	public Figure CurrentFigure;

	public Vector2 PlayerPos;
	[Space(10)]
	public Coin CoinPrefab;
	public List<Coin> SpawnedCoins;

	public Transform CointParent;
	public Vector3 CointPosOffset;


	public bool CanMove = true;
	private Vector3 _startPos;
	private Vector3 _playerPartPos;
	public float Speed;

	public float ScaleSpeed;
	public AnimationCurve ScaleCurve;

	public AnimationCurve HitAnimationCurve;
	public float HitAnimationSpeed = 1;
	public float HitAnimationMultiplier = 1;

	public Image HitBlink;
	public AnimationCurve HitBlinkAnimationCurve;
	public float HitBlinkAnimationSpeed = 1;


	void Awake()
	{
		Instance = this;
		for (int x = -1; x < 7; x++) {
			for (int y = 0; y < 5; y++) {
				WallPoint point = new WallPoint ();
				point.Pos = new Vector2 (x,y);
				point.Cube = Instantiate (WallCellPrefab,transform);
				point.Cube.transform.position = new Vector3 (x+1,y, transform.position.z);
				MyWall.Add (point);

			}
		}
	}
		
	public void CreateFigure(Vector3 startPos, Vector3 playerPartPos, float speed)
	{
		
		foreach (var i in MyWall) {
			i.Cube.transform.SetParent (transform);
			i.Cube.transform.localPosition = new Vector3 (i.Cube.transform.localPosition.x, i.Cube.transform.localPosition.y, 0);
			i.Cube.SetActive (true);
			i.Cube.gameObject.layer = 10;
		}

		SpawnedCoins.ForEach (x => Destroy(x.gameObject));
		SpawnedCoins.Clear ();

		Player.transform.SetParent (null);
		Player.DeadFX.transform.SetParent (Player.transform);
		Player.DeadFX.transform.localPosition = Vector3.zero;
		_startPos = startPos;
		_playerPartPos = playerPartPos;
		Speed = speed;
		MyWallParent.position = startPos;
		transform.position = startPos;

		CurrentFigure = FigureManager.Instance.RandomFigure ();

		PlayerPos = CurrentFigure.PlayerPos;
		Player.XPos = (int)PlayerPos.x + 1;
		Player.SetPos(new Vector3(CurrentFigure.PlayerPos.x + 1,CurrentFigure.PlayerPos.y,playerPartPos.z));


		for (int i = 0; i < MyWall.Count; i++) 
		{
			if (CurrentFigure._figure.Contains (MyWall [i].Pos)) //Player Pos
			{
				PlayerPart.Add (MyWall [i]);
				Vector3 pos = MyWall [i].Cube.transform.position;
				MyWall [i].Cube.transform.position = new Vector3 (pos.x, pos.y, playerPartPos.z);
			} 
			else if (CurrentFigure.PlayerTarget == MyWall [i].Pos) //Player Target
			{
				MyWall [i].Cube.gameObject.SetActive (false);

			} 
			else if (CurrentFigure.CoinPos.Contains (MyWall [i].Pos)) //Coin
			{
				
					MyWall [i].Cube.gameObject.SetActive (true);
					MyWall [i].Cube.transform.SetParent (MyWallParent);
				if (MyWall [i].Pos != new Vector2 (-1, 0)) {
					Vector3 pos = MyWall [i].Cube.transform.position;
					Coin coin = Instantiate (CoinPrefab, new Vector3 (pos.x + CointPosOffset.x, pos.y + CointPosOffset.y, playerPartPos.z), Quaternion.identity) as Coin;
					coin.XPos = Mathf.FloorToInt (pos.x);
					SpawnedCoins.Add (coin);
				}
			}
			else //Obstacle
			{
				MyWall [i].Cube.transform.SetParent (MyWallParent);
				MyWall [i].Cube.gameObject.layer = 9;
			}
		}


		StartCoroutine (ScaleAnimation());

	}

	void FixedUpdate()
	{
		if (CanMove) {
			MyWallParent.Translate (Vector3.back * Time.fixedDeltaTime * Speed);
			Line.position = Vector3.right * 3 + Vector3.forward * MyWallParent.position.z + Vector3.down * 0.5f;
			if (MyWallParent.position.z < _playerPartPos.z) {
				//RaycastHit hit;

				var hits = Physics.RaycastAll (Player.transform.position - Vector3.forward * 2, Vector3.forward,4f);

				foreach (var a in hits) {
					Debug.Log(a.collider.name);
				}

				if (hits.Length > 1) {
					MyWallParent.position = new Vector3 (transform.position.x, transform.position.y, 0);
					Debug.Log (hits.Length);
					//hits.ToList ().Find (x => x.collider.gameObject != Player.Graphics.gameObject).collider.gameObject.SetActive (false);
					Player.DeadFX.transform.SetParent (null);
					Player.Stop ();
					Player.CanMove = false;


					Manager.OnHitWall ();
				} else {
					Manager.OnDoneWall ();
				}

			}
		}
	}

	public IEnumerator HitAnimation(System.Action callback)
	{
		CanMove = false;
		float time = 0;
		float normalTime = 0;
		bool emit = true;

		Player.Hit = true;
		yield return null;

		var hits = Physics.RaycastAll (Player.transform.position - Vector3.forward * 2, Vector3.forward,4f);
		if (hits.Length > 1) {hits.ToList ().Find (x => x.collider.gameObject != Player.Graphics.gameObject).collider.gameObject.SetActive (false);}
		Player.transform.SetParent (MyWallParent);
		while (true) 
		{
			time += Time.fixedDeltaTime * HitAnimationSpeed;
			normalTime += Time.fixedDeltaTime;

			MyWallParent.Translate (Vector3.back * Time.fixedDeltaTime * HitAnimationCurve.Evaluate(time) * HitAnimationMultiplier);
			Line.position = Vector3.right * 3 + Vector3.forward * MyWallParent.position.z + Vector3.down * 0.5f;

			HitBlink.color = Color.Lerp (new Color(1,1,1,0), new Color(1,1,1,1), HitBlinkAnimationCurve.Evaluate(time));

			if (emit && time > 0.55f) {
				Player.DeadFX.Emit (160);
				emit = false;
			}

			if (time > 1) {
				HitBlink.color = new Color (1, 1, 1, 0);
				callback.Invoke ();
				yield break;
			}
			yield return new WaitForFixedUpdate ();

		}
	}

	IEnumerator ScaleAnimation()
	{
		float time = 0;
		while (true) 
		{
			time += Time.fixedDeltaTime * ScaleSpeed;

			MyWallParent.localScale = Vector3.one * ScaleCurve.Evaluate (time);
			if (time > 1)
				yield break;
			yield return new WaitForFixedUpdate ();

		}
	}


}
