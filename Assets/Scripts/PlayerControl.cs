using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour 
{
	public GameManager Manager;
	[Space(10)]

	public Transform TempParent;
	public MeshRenderer Graphics;
	public Material DefaultMaterial;
	public Color CollideColor;
	public AnimationCurve ColorAnimation;
	public float AnimationSpeed = 1f;

	public ParticleSystem DeadFX;

	public enum TurnType
	{
		Left, LeftUp, UpLeft, DownLeft, LeftDown,
		Right, RightUp, UpRight, DownRight, RightDown
	}
	public LayerMask WalkableLayer;
	public float Speed = 2;


	public int XPos;

	public bool CanMove = true;
	public bool Hit = false;
	public bool moveLeft = false;
	public bool moveRight = false;

	public void SetDefaultMaterial(Material newMaterial)
	{
		DefaultMaterial = newMaterial;
	}

	public void MaterialResetToDefault()
	{
		Graphics.material = DefaultMaterial;
	}

	public void MaterialPlayHitAnimation()
	{
		StartCoroutine (CollideAnimation());
		//Graphics.material = CollideMaterial;
	}


	void Update ()
	{
		if (CanMove) 
		{
			if (Input.GetKey (KeyCode.A) || moveLeft) {
				Move (Vector3.left);
			}
			if (Input.GetKey (KeyCode.D) || moveRight) {
				Move (Vector3.right);
			}
		}
	}

	public void OnMoveLeft(bool value)
	{
		if (moveRight)
			return;
		moveLeft = value;
	}

	public void OnMoveRight(bool value)
	{
		if (moveLeft)
			return;
		moveRight = value;
	}

	public void Stop()
	{
		OnMoveLeft (false);
     	OnMoveRight (false);
	}

	void Move(Vector3 dir)
	{
		RaycastHit hit;

		if (Physics.Raycast (transform.position, dir, out hit, 0.6f)) 
		{
			if (hit.collider.gameObject.layer == 9)
			{
				RaycastHit subHit;
				if (Physics.Raycast (transform.position, Vector3.down, out subHit, 0.6f)) 
				{
					
				} 
				else 
				{
					if (dir == Vector3.right) {
						//Debug.Log ("RightDown");
						StartCoroutine (Move (TurnType.RightDown));
					} else {
						//Debug.Log ("LeftDown");
						StartCoroutine (Move (TurnType.LeftDown));
					}
				}
			} else {
				RaycastHit subHit;
				if (Physics.Raycast (hit.transform.position, Vector3.up, out subHit, 0.6f)) {
					if (subHit.collider.gameObject.layer != 10)
						return;
					if (dir == Vector3.right) {
						//Debug.Log ("UpRight");
						StartCoroutine (Move (TurnType.UpRight));
					} else {
						//Debug.Log ("UpLeft");
						StartCoroutine (Move (TurnType.UpLeft));
					}
				} else {
					if (hit.collider != null) {
						if (hit.collider.gameObject.layer != 10) {
							return;
						}
					}
					//Debug.Log ("Up");
					if (dir == Vector3.right) {
						//Debug.Log ("RightUp");
						StartCoroutine (Move (TurnType.RightUp));
					} else {
						//Debug.Log ("LeftUp");
						StartCoroutine (Move (TurnType.LeftUp));
					}
				}
			}
		} 
		else
		{
			
			RaycastHit subHit;
			if (Physics.Raycast (transform.position, Vector3.down, out subHit, 0.6f)) 
			{		
				if (subHit.collider.gameObject.layer == 10) {
					if (subHit.collider.tag == "Floor") {
						if (dir == Vector3.right) {

							StartCoroutine (Move (TurnType.Right));
						}
						if (dir == Vector3.left) {
							StartCoroutine (Move (TurnType.Left));
						}
					}
					else 
					{
						if (Physics.Raycast (subHit.transform.position, dir, out subHit, 0.6f)) 
						{
							if (dir == Vector3.right) {

								StartCoroutine (Move (TurnType.Right));
							}
							if (dir == Vector3.left) {
								StartCoroutine (Move (TurnType.Left));
							}
						}
						else 
						{
							if (dir == Vector3.right) {
								StartCoroutine (Move (TurnType.DownRight));
							}
							if (dir == Vector3.left) {
								StartCoroutine (Move (TurnType.DownLeft));
							}
						}

					}

				}
			} 
			else
			{
				if (dir == Vector3.right) {
					//Debug.Log ("RightDown");
					StartCoroutine (Move (TurnType.RightDown));
				} else {
					//Debug.Log ("LeftDown");
					StartCoroutine (Move (TurnType.LeftDown));
				}
			}

		}
	}

	IEnumerator Move(TurnType turn)
	{
		Debug.Log (turn);
		Manager.OnMove ();

		float time = 0;

		Keyframe[] keyframes = null;

		if (turn == TurnType.Left)
		{
			TempParent.position = transform.position + (Vector3.left / 2) + (Vector3.down / 2);// - Vector3.left * 0.0001f;
			keyframes = new Keyframe[2];
			keyframes [0].value = 0;
			keyframes [1].time = 1;
			keyframes [1].value = 90;
		}
		if (turn == TurnType.LeftUp)
		{
			TempParent.position = transform.position + (Vector3.left / 2) + (Vector3.up / 2);// - Vector3.left * 0.0001f;
			keyframes = new Keyframe[2];
			keyframes [0].value = 0;
			keyframes [1].time = 1;
			keyframes [1].value = 180;
		}
		if (turn == TurnType.UpLeft)
		{
			TempParent.position = transform.position + (Vector3.left / 2) + (Vector3.up / 2);// - Vector3.left * 0.0001f;// (UseError) ? 0.0001f : 0;
			keyframes = new Keyframe[2];
			keyframes [0].value = 0;
			keyframes [1].time = 1;
			keyframes [1].value = 90;
		}

		if (turn == TurnType.DownLeft)
		{
			TempParent.position = transform.position + (Vector3.left / 2) + (Vector3.down / 2);// - Vector3.left * 0.0001f;// (UseError) ? 0.0001f : 0;
			keyframes = new Keyframe[2];
			keyframes [0].value = 0;
			keyframes [1].time = 1;
			keyframes [1].value = 180;
		}

		if (turn == TurnType.LeftDown)
		{
			TempParent.position = transform.position + (Vector3.right / 2) + (Vector3.down / 2);// - Vector3.right * 0.0001f;// (UseError) ? 0.0001f : 0;
			keyframes = new Keyframe[2];
			keyframes [0].value = 0;
			keyframes [1].time = 1;
			keyframes [1].value = 90;
		}

		if (turn == TurnType.Right)
		{
			TempParent.position = transform.position + (Vector3.right / 2) + (Vector3.down / 2);// - Vector3.right * 0.0001f;
			keyframes = new Keyframe[2];
			keyframes [0].value = 0;
			keyframes [1].time = 1;
			keyframes [1].value = -90;
		}

		if (turn == TurnType.RightUp)
		{
			TempParent.position = transform.position + (Vector3.right / 2) + (Vector3.up / 2);// - Vector3.right * 0.0001f;// (UseError) ? 0.0001f : 0;
			keyframes = new Keyframe[2];
			keyframes [0].value = 0;
			keyframes [1].time = 1;
			keyframes [1].value = -180;
		}
		if (turn == TurnType.UpRight)
		{
			TempParent.position = transform.position + (Vector3.right / 2) + (Vector3.up / 2);// - Vector3.right * 0.0001f;// (UseError) ? 0.0001f : 0;
			keyframes = new Keyframe[2];
			keyframes [0].value = 0;
			keyframes [1].time = 1;
			keyframes [1].value = -90;
		}
		if (turn == TurnType.DownRight)
		{
			TempParent.position = transform.position + (Vector3.right / 2) + (Vector3.down / 2);// - Vector3.right * 0.0001f;// (UseError) ? 0.0001f : 0;
			keyframes = new Keyframe[2];
			keyframes [0].value = 0;
			keyframes [1].time = 1;
			keyframes [1].value = -180;
		}
		if (turn == TurnType.RightDown)
		{
			TempParent.position = transform.position + (Vector3.left / 2) + (Vector3.down / 2);// - Vector3.left * 0.0001f;// (UseError) ? 0.0001f : 0;
			keyframes = new Keyframe[2];
			keyframes [0].value = 0;
			keyframes [1].time = 1;
			keyframes [1].value = -90;
		}



		AnimationCurve movementCurve = new AnimationCurve (keyframes);

		transform.SetParent (TempParent);
		CanMove = false;
		while (true) 
		{
			if (Hit) {
				TempParent.eulerAngles = new Vector3 (0,0,movementCurve.Evaluate(1));
				CanMove = false;
				transform.SetParent (null);
				TempParent.rotation = Quaternion.identity;
				transform.rotation = Quaternion.identity;
				transform.position += Vector3.forward * 0.001f;
				//transform.position = 

				XPos = Mathf.FloorToInt(Mathf.Abs(transform.position.x));
				Hit = false;
				yield break;
			}
			time += Time.fixedDeltaTime * Speed;

			TempParent.eulerAngles = new Vector3 (0,0,movementCurve.Evaluate(time));


			if (time > 1) 
			{
				CanMove = true;
				transform.SetParent (null);
				TempParent.rotation = Quaternion.identity;
				transform.rotation = Quaternion.identity;
				//transform.position = 

				XPos = Mathf.FloorToInt(Mathf.Abs(transform.position.x));
	
//				Coin collectedCoint = Wall.Instance.SpawnedCoins.Find (x => x.XPos == XPos);
//				if (collectedCoint != null) {
//					if ((transform.position.y + 0.4f) > collectedCoint.transform.position.y && transform.position.y - 0.4f  < collectedCoint.transform.position.y) {	
//						collectedCoint.Collect ();
//					}
//
//				}

				yield break;
			}


			yield return new WaitForFixedUpdate ();
		}
	}

	IEnumerator CollideAnimation()
	{
		float time = 0;
		Color defaultColor = DefaultMaterial.color;
		while (true)
		{
			time += Time.fixedDeltaTime * AnimationSpeed;
			Graphics.material.color = Color.Lerp (defaultColor,CollideColor, ColorAnimation.Evaluate(time));
			if (time > 1) 
			{
				yield break;
			}
			yield return new WaitForFixedUpdate ();
		}
	}

	public void SetPos(Vector3 pos)
	{
		transform.position = pos;
		//XPos =  Mathf.FloorToInt(Mathf.Abs(transform.position.x));
	}

}
