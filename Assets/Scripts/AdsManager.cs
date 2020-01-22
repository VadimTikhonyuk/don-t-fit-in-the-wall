using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour
{
	public static AdsManager Instance;

	private const string MY_BANNERS_AD_UNIT_ID		 = "ca-app-pub-7154658780712117/1596134155";
	private const string MY_INTERSTISIALS_AD_UNIT_ID =  "ca-app-pub-7154658780712117/4928874507";

	public BannerView banner;
	public InterstitialAd interstitial;

	void Awake ()
	{
		Instance = this;
	}

	void Start ()
	{
		Advertisement.Initialize ("1543361");

	}
	public void ShowBanner()
	{
		StartCoroutine (LoadInterstetial());
		// Create a 320x50 banner at the top of the screen.
		banner = new BannerView(MY_BANNERS_AD_UNIT_ID, AdSize.Banner, AdPosition.Bottom);
		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().Build();
		// Load the banner with the request.
		banner.LoadAd(request);
		banner.Show();
	}
	public void RemoveBanner()
	{
		if(banner!=null)
			banner.Destroy();
	}
	public void ShowInter ()
	{
		interstitial.Show ();
	}
	public IEnumerator LoadInterstetial()
	{
		// Initialize an InterstitialAd.
		interstitial = new InterstitialAd(MY_INTERSTISIALS_AD_UNIT_ID);
		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().Build();
		// Load the interstitial with the request.
		interstitial.LoadAd(request);

		while (!interstitial.IsLoaded())
			yield return null;	
	}

	public void ShowRewardedVideo ()
	{
		ShowOptions options = new ShowOptions();
		options.resultCallback = HandleShowResult;

		Advertisement.Show("rewardedVideo", options);
	}

	void HandleShowResult (ShowResult result)
	{
		if(result == ShowResult.Finished) 
		{
			GameManager.Instance.OnWatchedAds ();

		}else if(result == ShowResult.Skipped) {
			GameManager.Instance.OnWatchAdsError ();

		}else if(result == ShowResult.Failed) {
			GameManager.Instance.OnWatchAdsError ();
		}
	}


}
