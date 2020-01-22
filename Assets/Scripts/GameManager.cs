using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using UnityEngine.Analytics;
public static class CoinManager
{
	public static int Coins
	{
		get {
			return SPlayerPrefs.GetInt("Coins");
		}
		set {
			SPlayerPrefs.SetInt ("Coins",value);
		}
	}

	public static void AddCoin()
	{
		Coins++;
	}

	public static void AddCoins(int count)
	{
		Coins += count;
	}
}

public class GameManager : MonoBehaviour 
{
	public static GameManager Instance;

	[SerializeField] private GameObject WelcomeTab;
    [SerializeField] private GameObject GameEndTab;
    [SerializeField] private GameObject MobileControl;

	private int score;
	private int highscore
	{
		get {
			return SPlayerPrefs.GetInt ("Highscore");
		}
		set {
			SPlayerPrefs.SetInt ("Highscore",value);
		}
	}

	[SerializeField] private Text Score;
	[SerializeField] private Text FinalScore;
	[SerializeField] private Text SharedScore;
	[SerializeField] private Text Highscore;
	[SerializeField] private Text Coins;
	[SerializeField] private Text CoinsInPatterView;
    [SerializeField] private Animator CoinAnimator;

    [SerializeField] private List<GameObject> Details;

    [SerializeField] private Image LoadingFade;
    [SerializeField] private Image LoadingFadeHelper;
    [SerializeField] private AnimationCurve FadeAnimationCurve;
    [SerializeField] private float FadeAnimationSpeed = 1;

	[Header("GamePlay")]

    [SerializeField] private Transform WallSpawnPos;
    [SerializeField] private Wall MyWall;
    [SerializeField] private PlayerControl Player;

	private bool lockSpawn = false;
    [SerializeField] private float WallSpeed = 4;

    [SerializeField] private Image SkipBlink;
    [SerializeField] private AnimationCurve SkipBlinkScaleCurve;
    [SerializeField] private AnimationCurve SkipBlinkAlphaCurve;
    [SerializeField] private float SkipBlinkAnimationSpeed;

    [SerializeField] private int CoinsPerCollect = 1;

	[Header("Tutorial")]
    [SerializeField] private CanvasGroup TutorialGroup;
    [SerializeField] private AnimationCurve TutorialAlphaCurve;

    [SerializeField] private float TutorialWallSpeed = 1.5f;
    [SerializeField] private float TutorialLenght = 3;
    [SerializeField] private bool tutorialSkip = false;

	[Header("Patterns UI")]
    [SerializeField] private CanvasGroup DefaultEndGameView;
    [SerializeField] private CanvasGroup PatternsView;

    [SerializeField] private AnimationCurve SwitchAnimationCurve;
    [SerializeField] private float SwitchAnimationSpeed = 1;


	[Header("AdsSettings")]
    [SerializeField] private int InterstitialRate = 10;
    [SerializeField] private int RewardAdsRate = 5;
    [SerializeField] private GameObject WatchAdsButton;
    [SerializeField] private int RewardForVideoAds = 20;
    [SerializeField] private Text RewardPerVideoAdsText;
    [SerializeField] private TappxManagerUnity CrossPromotionManager;
    [SerializeField] private int CrossPromotionRate = 7;

	[Header("Themes")]
    [SerializeField] private List<Color> AllColors;
    [SerializeField] private int changeColorPerScore = 5;
    [SerializeField] private Image Background;
    [SerializeField] private Image Foreground;
    [SerializeField] private float ChangeColorSpeed;
    [SerializeField] private AnimationCurve ChangeColorCurve;

	[Header("Audio")]
	[SerializeField] private Image AudioFXButton;
	[SerializeField] private Sprite AudioFXOnSprite;
	[SerializeField] private Sprite AudioFXOffSprite;

	[SerializeField] private AudioSource FX;
	[SerializeField] private AudioSource Music;

	[SerializeField] private AudioClip OnGameBegin;
	[SerializeField] private AudioClip OnMoveClip;
	[SerializeField] private AudioClip OnCollect;
	[SerializeField] private AudioClip OnSkip;
	[SerializeField] private AudioClip OnHit;
	[SerializeField] private AudioClip OnUnlockPatternClip;
    [SerializeField] private List<AudioClip> WallWhooshes;

	[Header("ShareScore")]
	[SerializeField] private Camera photoCamera;
	[SerializeField] private GameObject canvas;
	[SerializeField] private Image ImageInGameEnd;
	[SerializeField] private Image ImageInRender;
	[SerializeField] private List<Sprite> ShareIcons;
    [SerializeField] private Text sharedScore;
	private Texture2D renderedTexture;

	//Analytics
	private static int gamePlayed;
	[Header("IAP")]
    [SerializeField] private Purchaser purchaser;
    [SerializeField] private GameObject purchaseNoAdsButton;
    [SerializeField] private GameObject noAdsPurchased;

	void Awake()
	{
		Instance = this;
		GPLogin ();
	}


	// Use this for initialization
	void Start () 
	{
		WelcomeTab.SetActive (true);
		GameEndTab.SetActive (false);
		MobileControl.SetActive (false);

		MyWall.gameObject.SetActive (false);
		Player.gameObject.SetActive (false);

		UpdateCoins ();
		ReadAudioSetting ();
		purchaser.InitIAP (NoAdsPurchased);
		CheckIAP ();
		RemoteSettings.Updated += new RemoteSettings.UpdatedEventHandler(HandleRemoteUpdate);
	}
	private void HandleRemoteUpdate(){
		InterstitialRate = RemoteSettings.GetInt ("InterstitialRate", 10);
		RewardAdsRate = RemoteSettings.GetInt ("VideoRewardAdsRate", 5);
		RewardForVideoAds = RemoteSettings.GetInt ("RewardForVideoAds", 20);
		CoinsPerCollect = RemoteSettings.GetInt ("CoinsPerCollect", 1);
		CrossPromotionRate = RemoteSettings.GetInt ("CrossPromotionRate", 7);
	}
	private void CheckIAP()
	{
		if (SPlayerPrefs.GetInt ("NoAds") == 1) {
			purchaseNoAdsButton.SetActive (false);
			noAdsPurchased.SetActive (true);
		} else {
			purchaseNoAdsButton.SetActive (true);
			noAdsPurchased.SetActive (false);
			AdsManager.Instance.ShowBanner ();
		}
	}

	private bool lockButton = false;
	public void Play()
	{
		if (!lockButton) {
			FX.PlayOneShot (OnGameBegin);
			StartCoroutine (PlayFadeAnimation ());
			Analytics.CustomEvent ("User Click On Play Button");
			lockButton = true;
		}
	}

	IEnumerator PlayFadeAnimation()
	{
		float time = 0;
		bool evaluate = false;
		while (true) 
		{
			time += Time.fixedDeltaTime * FadeAnimationSpeed;

			LoadingFade.color = Color.Lerp (new Color (1, 1, 1, 0), new Color (1, 1, 1, 1), FadeAnimationCurve.Evaluate (time));

			if (time > 0.5f)
			{
				if (!evaluate) 
				{
					GameBegin ();
					evaluate = true;
				}
			}

			if (time > 1) {
				LoadingFade.color = new Color (1, 1, 1, 0);
				lockButton = false;
				yield break;
			}
			yield return new WaitForFixedUpdate ();
		}
	}

	private void GameBegin()
	{
		WelcomeTab.SetActive (false);
		GameEndTab.SetActive (false);
		MobileControl.SetActive (true);

		MyWall.gameObject.SetActive (true);
		Player.gameObject.SetActive (true);

		CreateInitWall ();

		Details.ForEach (x => x.SetActive(true));

		TutorialGroup.alpha = 1;
	}

	public void Retry()
	{
		if (!lockButton) {
			FX.PlayOneShot (OnGameBegin);
			StartCoroutine (RetryFadeAnimation ());
			Analytics.CustomEvent ("User Click On Retry Button");
			lockButton = true;
		}
	}

	IEnumerator RetryFadeAnimation()
	{
		float time = 0;
		bool evaluate = false;
		Color a0 = Background.color; a0.a = 0;
		Color a1 = Background.color;
		while (true) 
		{
			time += Time.fixedDeltaTime * FadeAnimationSpeed;

			LoadingFade.color = Color.Lerp (a0, a1, FadeAnimationCurve.Evaluate (time));

			if (time > 0.5f)
			{
				if (!evaluate) 
				{
					BeginRetryGame ();
					evaluate = true;
				}
			}

			if (time > 1) {
				LoadingFade.color = new Color (1, 1, 1, 0);
				lockButton = false;
				yield break;
			}
			yield return new WaitForFixedUpdate ();
		}
	}

	private void BeginRetryGame()
	{
		CreateInitWall ();
		WelcomeTab.SetActive (false);
		GameEndTab.SetActive (false);
		MobileControl.SetActive (true);


		MyWall.CanMove = true;

		Player.Hit = false;
		Player.CanMove = true;
		Player.MaterialResetToDefault ();

		score = 0;
		Score.text = "";
		//AdsManager.Instance.RemoveBanner ();
	}

	private void CreateInitWall()
	{
		MyWall.CreateFigure (WallSpawnPos.position, Vector3.zero, WallSpeed);
		StartCoroutine (Tutorial());
	}

	private void CreateWall()
	{
		MyWall.CreateFigure (WallSpawnPos.position, Vector3.zero, WallSpeed);
	}

	public void OnDoneWall()
	{
		StartCoroutine (SkipBlinkAnimation());
		if (!lockSpawn) {
			Debug.Log("Skip");
			lockSpawn = true;
			StartCoroutine (CreateWallEnumerator ());

			OnScore ();
			FX.PlayOneShot (OnSkip,1);
			StartCoroutine(PlayFXDelayed(0.1f,WallWhooshes[Random.Range(0, WallWhooshes.Count - 1)], 0.6f));
		}
	}
		
	public void OnHitWall()
	{
		TutorialGroup.alpha = 0;

		StartCoroutine (MyWall.HitAnimation(OnHitAnimationEnd));


		FX.PlayOneShot (OnHit);

		Debug.Log("Hit");

		Player.CanMove = false;
		Player.Stop ();
		Player.MaterialPlayHitAnimation ();
	}

	private void OnHitAnimationEnd()
	{

		WelcomeTab.SetActive (false);
		GameEndTab.SetActive (true);
		MobileControl.SetActive (false);
		WatchAdsButton.SetActive (false);

		gamePlayed++;
		tutorialSkip = false;
		if ((gamePlayed % InterstitialRate) == 0) 
		{
			if(SPlayerPrefs.GetInt("NoAds") == 0)
			AdsManager.Instance.ShowInter ();
		}
		else  if((gamePlayed % RewardAdsRate) == 0) 
		{
			WatchAdsButton.SetActive (true);
			RewardPerVideoAdsText.text = "+" + RewardForVideoAds.ToString();
		}
		else if((gamePlayed % CrossPromotionRate) == 0)
		{
			
			if (SPlayerPrefs.GetInt ("NoAds") == 0) {
				if (CrossPromotionManager.isInterstitialReady())
					CrossPromotionManager.interstitialShow ();
				CrossPromotionManager.loadInterstitial ();
			}
				
		}
		int shareIconId = Random.Range (0, ShareIcons.Count - 1);
		ImageInGameEnd.sprite = ShareIcons [shareIconId];
		ImageInRender.sprite = ShareIcons [shareIconId];

		Score.text = "";
		FinalScore.text = score.ToString ();
		SharedScore.text = score.ToString ();
		Highscore.text = "BEST " + highscore.ToString ();
		if (score > highscore) {
			highscore = score;
			Highscore.text = "BEST " + score.ToString ();
		}
		SendScore (score);
	}

	public void ShowPatterns()
	{
		FX.PlayOneShot (OnGameBegin);
		StartCoroutine (SwitchPatternAnimation(true));
		Analytics.CustomEvent ("User Click On Themes");
	}
	public void ClosePatterns()
	{
		FX.PlayOneShot (OnGameBegin);
		StartCoroutine (SwitchPatternAnimation(false));
	}

	public void WatchAds()
	{
		AdsManager.Instance.ShowRewardedVideo ();
		Analytics.CustomEvent ("User Watching Video ADS");
	}

	public void OnWatchAdsError()
	{
		WatchAdsButton.SetActive (false);
		Analytics.CustomEvent ("User Exit or Get Error in Video Ads");
	}

	public void OnWatchedAds()
	{
		WatchAdsButton.SetActive (false);
		CoinManager.AddCoins (RewardForVideoAds);
		UpdateCoins ();
		Analytics.CustomEvent ("User Watched Ads");
	}

	public void OnMove()
	{
		if (!tutorialSkip)
			tutorialSkip = true;
		FX.PlayOneShot (OnMoveClip);
	}

	public void OnScore()
	{
		score++;
		Score.text = score.ToString ();
		if ((score % changeColorPerScore) == 0) {
			StartCoroutine (ChangeColor());
		}
	}

	public void OnCollectCoin()
	{
		FX.PlayOneShot (OnCollect, 0.7f);
		CoinManager.AddCoins (CoinsPerCollect);
		UpdateCoins ();
		Analytics.CustomEvent ("User Collected Coin");
	}

	void UpdateCoins()
	{
		Coins.text = CoinManager.Coins.ToString ();
		CoinsInPatterView.text = CoinManager.Coins.ToString ();
		CoinAnimator.Play ("Collect");
	}

	public void RecalculateCoins()
	{
		Coins.text = CoinManager.Coins.ToString ();
		CoinsInPatterView.text = CoinManager.Coins.ToString ();
	}

	private void ReadAudioSetting()
	{
		if (SPlayerPrefs.GetInt ("FirtsLaunch") == 0)
		{
			SPlayerPrefs.SetInt ("AudioFX", 1);
			AudioFXButton.sprite = AudioFXOnSprite;
			FX.volume = 1;
			Music.volume = 1;
			SPlayerPrefs.SetInt ("FirtsLaunch", 1);
		} else {
			if (SPlayerPrefs.GetInt ("AudioFX") == 0) {
				AudioFXButton.sprite = AudioFXOffSprite;
				FX.volume = 0;
				Music.volume = 0;
			} else {
				AudioFXButton.sprite = AudioFXOnSprite;
				FX.volume = 1;
				Music.volume = 1;
			}
		}
	}

	public void OnAudioButton()
	{
		if (SPlayerPrefs.GetInt ("AudioFX") == 0) {
			SPlayerPrefs.SetInt ("AudioFX", 1);
			AudioFXButton.sprite = AudioFXOnSprite;
			FX.volume = 1;
			Music.volume = 1;
		} else {
			SPlayerPrefs.SetInt ("AudioFX", 0);
			AudioFXButton.sprite = AudioFXOffSprite;
			FX.volume = 0;
			Music.volume = 0;
		}
		Analytics.CustomEvent ("User Click On Audio Button");
	}

	public void OnNoAds()
	{
		purchaser.RemoveAds ();
	}

	private void NoAdsPurchased()
	{
		SPlayerPrefs.SetInt ("NoAds", 1);
		purchaseNoAdsButton.SetActive (false);
		noAdsPurchased.SetActive (true);
		AdsManager.Instance.RemoveBanner ();
		Analytics.CustomEvent ("User Purchased NoAds");
	}

	public void OnUnlockPattern()
	{
		FX.PlayOneShot (OnUnlockPatternClip);
		Analytics.CustomEvent ("User Unlock Pattern");
	}
	public void OnSelectPattern()
	{
		FX.PlayOneShot (OnGameBegin);
		Analytics.CustomEvent ("User Change Pattern");
	}


	#region Services
	private void GPLogin()
	{
		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()

			.Build();

		PlayGamesPlatform.InitializeInstance(config);
		// recommended for debugging:
		PlayGamesPlatform.DebugLogEnabled = true;
		// Activate the Google Play Games platform
		PlayGamesPlatform.Activate();

		Social.localUser.Authenticate((bool success) =>
			{
				if (success)
				{
					Debug.Log("You've successfully logged in");
					GetHighScore();
				}
				else
				{
					Debug.Log("Login failed for some reason");
				}
			});
	}

	private void GetHighScore()
	{
		ILeaderboard lb = PlayGamesPlatform.Instance.CreateLeaderboard();
		lb.id = GPGSIds.leaderboard_global;
		lb.LoadScores(ok =>
			{
				if (ok) {
					highscore = (int)lb.localUserScore.value;
				}
				else {
					Debug.Log("Error retrieving leaderboardi");
				}
			});
	}

	public void SendScore(long score)
	{
		if (Social.localUser.authenticated) 
		{
			Social.ReportScore (score, GPGSIds.leaderboard_global, (bool success) => {
				
			});
		}

	}

	public void ShowLeaderboards()
	{
		PlayGamesPlatform.Instance.ShowLeaderboardUI (GPGSIds.leaderboard_global);
		Analytics.CustomEvent ("User Watching Leaderboards");
	}

	public void RateGame()
	{
		Application.OpenURL ("https://play.google.com/store/apps/details?id=com.squarebeamgames.fitinthewall");
		Analytics.CustomEvent ("User Rate Game");
	}

	public void ShareScore()
	{
		string subject = "OMG!";
		string text = "I scored " + score.ToString () + " points in #DontFitInTheWall! Can you beat my score?";
		if (Application.systemLanguage == SystemLanguage.Ukrainian) {
			subject = "БОЖЕ!";
			text = "Я набрав " + score.ToString () + " очків в #DontFitInTheWall! Зможеш побити мій результат?";
		}
		if (Application.systemLanguage == SystemLanguage.Russian) {
			subject = "ОМГ!";
			text = "Я набрал " + score.ToString () + " очков в #DontFitInTheWall! Сможешь побить мой результат?";
		}
		StartCoroutine (ShareScore(subject,text,score,"https://play.google.com/store/apps/details?id=com.squarebeamgames.fitinthewall"));
		Analytics.CustomEvent ("User Shared Score");
	}

	private IEnumerator ShareScore(string subject,string text, int score, string url)
	{

		canvas.SetActive (true);
		sharedScore.text = score.ToString ();
		string screenShotPath = Application.persistentDataPath + "/" + "screenshot.png";
		if(File.Exists(screenShotPath)) File.Delete(screenShotPath);

		File.WriteAllBytes (screenShotPath, RenderImage ().EncodeToPNG ());

		while(!File.Exists(screenShotPath)) {
			yield return new WaitForSeconds(.05f);
		}
		NativeShare.Share(text, screenShotPath, url, subject, "image/png", true, "");
		canvas.SetActive (false);
	}

	Texture2D RenderImage()
	{

		photoCamera.gameObject.SetActive (true);
		int x = 512;
		int y = 570;
		RenderTexture tempRT = new RenderTexture(x,y, 0 );

		photoCamera.targetTexture = tempRT;
		photoCamera.Render();

		RenderTexture.active = tempRT;
		//Texture2D virtualPhoto = new Texture2D(sqr-128,sqr-128, TextureFormat.ARGB32, false);
		Texture2D virtualPhoto = new Texture2D(x,y, TextureFormat.ARGB32, false);
		// false, meaning no need for mipmaps
		//virtualPhoto.ReadPixels( new Rect(64, 64, sqr-64,sqr-64), 0, 0);
		virtualPhoto.ReadPixels( new Rect(0, 0, x,y), 0, 0);
		renderedTexture = virtualPhoto;
		renderedTexture.Apply ();
		RenderTexture.active = null; //can help avoid errors 
		photoCamera.targetTexture = null;
		photoCamera.gameObject.SetActive (false);
		return renderedTexture;
	}


	#endregion


	IEnumerator Tutorial()
	{
		Debug.Log ("Tutorial");
		float time = 0;
		MyWall.Speed = TutorialWallSpeed;
		while (true) 
		{
			time += Time.fixedDeltaTime;
			TutorialGroup.alpha = Mathf.Lerp (0, 1, TutorialAlphaCurve.Evaluate (time));

			if (time > TutorialLenght || tutorialSkip) 
			{
				MyWall.Speed = WallSpeed;
				TutorialGroup.alpha = 0;
				yield break;
			}
			yield return new WaitForFixedUpdate ();
		}
	}

	IEnumerator SwitchPatternAnimation(bool direction)
	{
		float time = 0;

		DefaultEndGameView.interactable = !direction;
		DefaultEndGameView.blocksRaycasts = !direction;

		PatternsView.interactable = direction;
		PatternsView.blocksRaycasts = direction;

		if(direction)
			PatternsView.gameObject.SetActive (true);

		while (true) 
		{
			time += Time.fixedDeltaTime * SwitchAnimationSpeed;
			if (direction) {
				DefaultEndGameView.alpha = Mathf.Lerp (1, 0, SwitchAnimationCurve.Evaluate (time));
				PatternsView.alpha = Mathf.Lerp (0, 1, SwitchAnimationCurve.Evaluate (time));
			} else {
				DefaultEndGameView.alpha = Mathf.Lerp (0, 1, SwitchAnimationCurve.Evaluate (time));
				PatternsView.alpha = Mathf.Lerp (1, 0, SwitchAnimationCurve.Evaluate (time));
			}

			if (time > 1) 
			{
				if (!direction)
					PatternsView.gameObject.SetActive (false);
				yield break;
			}
			yield return new WaitForFixedUpdate ();
		}
	}

	IEnumerator SkipBlinkAnimation()
	{
		float time = 0;
		while (true) 
		{
			time += Time.fixedDeltaTime * SkipBlinkAnimationSpeed;

			SkipBlink.color = Color.Lerp (new Color (1, 1, 1, 0), new Color (1, 1, 1, 1), SkipBlinkAlphaCurve.Evaluate (time));
			SkipBlink.transform.localScale = Vector3.Lerp (Vector3.one, Vector3.one * 1.3f, SkipBlinkScaleCurve.Evaluate(time));


			if (time > 1) {
				SkipBlink.color = new Color (1, 1, 1, 0);
				SkipBlink.transform.localScale = Vector3.one;
				yield break;
			}
			yield return new WaitForFixedUpdate ();
		}
	}

	IEnumerator CreateWallEnumerator()
	{
		yield return new WaitForSeconds (0.1f);
		CreateWall ();
		lockSpawn = false;
	}
	IEnumerator PlayFXDelayed(float delay, AudioClip clip, float volume)
	{
		yield return new WaitForSeconds (delay);
		FX.PlayOneShot (clip,volume);
	}
	IEnumerator ChangeColor()
	{
		float time = 0;
		Color currentColorA = Background.color;
		Color currentColorB = Foreground.color;

		Color targetColorA = AllColors [Random.Range (0, AllColors.Count - 1)];
		Color targetColorB = AllColors [Random.Range (0, AllColors.Count - 1)];

		while (true)
		{
			time += Time.fixedDeltaTime * ChangeColorSpeed;
			Background.color = Color.Lerp (currentColorA,targetColorA, ChangeColorCurve.Evaluate(time));
			Foreground.color = Color.Lerp (currentColorB,targetColorB, ChangeColorCurve.Evaluate(time));
			if (time > 1)
				yield break;
			yield return new WaitForFixedUpdate ();
		}
	}
}
