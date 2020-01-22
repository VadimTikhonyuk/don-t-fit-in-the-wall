using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPattern : MonoBehaviour 
{
	private PatternManager m_manager;
	private int m_id;
	private bool m_locked = false;
	private int m_price = 99999;
	private Sprite Icon;

	public Text PriceLabel;
	public Image IconImage;
	public Image LockImage;
	public GameObject LockedView;
	public GameObject UnclockedView;


	public void Init(PatternManager manager, int id, bool locked, int price, Color lockColor, Sprite icon)
	{
		m_manager = manager;
		m_id = id;
		m_locked = locked;
		m_price = price;

		PriceLabel.text = price.ToString ();
		LockImage.color = lockColor;

		Icon = icon;

		if(!locked)
			IconImage.sprite = Icon;

		LockedView.SetActive (locked);
		UnclockedView.SetActive (!locked);
	}

	public void Select()
	{
		if (m_locked) {
			if (CoinManager.Coins >= m_price) 
			{
				CoinManager.Coins -= m_price;
				GameManager.Instance.RecalculateCoins ();
				Unlock ();
			}
		} else {
			m_manager.SelectPattern (m_id,false);
		}
	}

	private void Unlock()
	{
		Debug.Log ("Unlock");
		m_locked = false;

		IconImage.sprite = Icon;
		LockedView.SetActive (m_locked);
		UnclockedView.SetActive (!m_locked);
		SPlayerPrefs.SetInt ("PatternUnlocked", 1);
		m_manager.SelectPattern (m_id,true);
	}



}
