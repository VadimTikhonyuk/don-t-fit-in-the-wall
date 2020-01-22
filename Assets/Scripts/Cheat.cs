using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheat : MonoBehaviour {

	public bool addMoney = false;

	void Update()
	{
		if (Input.touchCount >= 5) {
			if (addMoney) {
				CoinManager.AddCoins (10000);
				GameManager.Instance.RecalculateCoins ();
				addMoney = false;
			}
		} else {
			addMoney = true;
		}
//		if (addMoney) {
//			CoinManager.AddCoins (1000);
//			GameManager.Instance.RecalculateCoins ();
//			addMoney = false;
//		}
	}
}
