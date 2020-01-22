using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour 
{
	public int XPos = -1;

	public float SpinSpeed =1;
	public float MoveSpeed =1;
	public float MoveAmplitude = 1;
	public AnimationCurve MoveCurve;

	public float CollectAmplitude = 1;
	public AnimationCurve CollectCurve;
	public AnimationCurve ScaleCurve;

	public Transform mMesh;
	// Use this for initialization
	void Start () 
	{
		StartCoroutine (Anim ());
	}

	private bool collected = false;
	public void OnTriggerEnter(Collider other)
	{
		if (!collected && other.GetComponent<Collider>().tag == "Player") {
			Collect ();
			collected = true;
		}
	}

	private IEnumerator Anim()
	{
		float time = 0;
		while (true) 
		{
			time += Time.fixedDeltaTime;
			mMesh.eulerAngles += Vector3.up * SpinSpeed;
			mMesh.localPosition = Vector3.up * MoveCurve.Evaluate (time * MoveSpeed) * MoveAmplitude;

			yield return new WaitForFixedUpdate ();
		}
	}
	private IEnumerator CollectAnim()
	{
		float time = 0;
		while (true) 
		{
			time += Time.fixedDeltaTime;
			mMesh.eulerAngles += Vector3.up * SpinSpeed;
			mMesh.localPosition = Vector3.up * CollectCurve.Evaluate (time * MoveSpeed) * CollectAmplitude;
			mMesh.localScale = Vector3.one * ScaleCurve.Evaluate (time);

			yield return new WaitForFixedUpdate ();
		}
	}

	public void Collect()
	{
		GameManager.Instance.OnCollectCoin ();
		Wall.Instance.SpawnedCoins.Remove (this);
		StopCoroutine (Anim());
		StartCoroutine (CollectAnim ());
		Destroy (gameObject, 2f);
	}

}
