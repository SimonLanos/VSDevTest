/* Introduction
 *
 * VoodooSauce is a Unity SDK that we implement into all of our games here at Voodoo.  This SDK is responsible for
 * providing Ads, Analytics, IAP, GDPR, etc. functionality in an easy to use package for internal and external studios
 * to integrate into their games. The SDK is used around the world by more than 200+ games, thus reliability and ease of use is
 * incredibly important for us. 
 *
 * For this exercise, we would like you to create a basic VoodooSauce that integrates the fake "TopAds" and "TopAnalytics"
 * SDKs.
 *
 * At the end we ask that you answer some quick questions at the bottom of this file. 
 * 
 */

/* Instructions 
 *
 * Please fill out the method implementations below 
 * Feel free to create additional classes to help with your implementation 
 * Please do not spend more than 2.5 hours on the code implementation portion of this exercise
 * Please do not modify the code in the 3rdParty folder
 * Make sure to read this entire file before starting to code.  We include important instructions on how to use the TopAds and TopAnalytics SDKs
 * 
 */

// Bonus Question : Show an android Toast when you launch the app.


using _3rdParty;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoodooSauce
{

	// Before calling methods in TopAds and TopAnalytics you must call their init methods 
	// TopAds requires the TopAds prefab to be created in the scene
	// You also need to collect user GDPR consent and pass that boolean value to TopAds and TopAnalytics 
	// You can collect this consent by displaying a popup to the user at the start of the game and then storing that value for future use 
	//DONE

	static bool adLoaded = false;
	static bool addFailed = false;
	
	static Coroutine loadingAdCoroutine = null;

	static int secondsBetweenAds = 0;
	static float lastAdShownTime = 0;
	static int lastAdShownGameIndex = 0;

	static int gamesBetweenAds = 0;
	static int currentGameIndex = 0;

	static List<string> adUnitIDList = new List<string>();

	static public void GrantConsent()
	{
		TopAds.GrantConsent();
		TopAnalytics.InitWithConsent(true);
		if (!PlayerPrefs.HasKey("consent") || PlayerPrefs.GetInt("consent") == 0)
		{
			PlayerPrefs.SetInt("consent", 1);
			PlayerPrefs.Save();
		}
	}

	static public void RevokeConsent()
	{
		TopAds.RevokeConsent();
		TopAnalytics.InitWithConsent(false);
		if (!PlayerPrefs.HasKey("consent") || PlayerPrefs.GetInt("consent") != 0)
		{
			PlayerPrefs.SetInt("consent", 0);
			PlayerPrefs.Save();
		}
	}

	public static void SetAdUnitIDs(AdUnitIDList _adUnitIDList)
	{
		adUnitIDList = _adUnitIDList.adUnitIDList;
	}
	public static void StartGame()
	{
		//DONE
		// Track in TopAnalytics that a game has started 
		TopAnalytics.TrackEvent("game_start");
		currentGameIndex++;
	}

	public static void EndGame()
	{
		//DONE
		// Track in TopAnalytics that a game has ended 
		TopAnalytics.TrackEvent("game_end");
	}

	public static void ShowAd()
	{
		//DONE
		// TopAds methods must be called with a unique "string" ad unit id 
		// For your test app that id is "f4280fh0318rf0h2" 
		// However, when releasing the SDK to other studios, their ad unit id will be different 
		// Please find a flexible way to allow studios to provide their ad unit id to your VoodooSauce SDK 

		//-----------------ANSWER---------------------
		/* 
		 * If I were doing this for production I'd argue for using a WebRequest to a list of all our clients' ad unit id.
		 * Like something on GoogleSheet.
		 * That way we'd be able to update the table without having to upgrade the game
		 * In the meantime I'm using a ScriptableObject for easy use for the rest of the team.
		 */
		//------------------------------------------

		// Before an ad is available to display, you must call TopAds.RequestAd 
		// You must call RequestAd each time before an ad is ready to display 

		// RequestAd will make a "fake" request for an ad that will take 0 to 10 seconds to complete
		// Afterwards, either the OnAdLoadedEvent or OnAdFailedEvent will be invoked 
		// Please implement an autorequest system that ensures an ad is always ready to be displayed
		// Keep in mind that RequestAd can fail multiple times in a row 

		//-----------------ANSWER---------------------
		/* 
		 * See LoadAd() and LoadAdCoroutine()
		 */
		//------------------------------------------

		// If an ad is loaded correctly, clicking on the "Show Ad" button within Unity-VoodooSauceTestApp 
		// should display a fake ad popup that you can close. 
		if (Time.unscaledTime < lastAdShownTime + secondsBetweenAds)
		{
			Debug.LogWarning((secondsBetweenAds - (Time.unscaledTime - lastAdShownTime)).ToString() + " seconds to wait remaining before ad can be displayed");
			return;
		}
		if (currentGameIndex < lastAdShownGameIndex + gamesBetweenAds)
		{
			Debug.LogWarning((gamesBetweenAds - (currentGameIndex - lastAdShownGameIndex)).ToString() + " games to play remaining before ad can be displayed");
			return;
		}
		if (adLoaded)
		{
			TopAds.ShowAd(adUnitIDList[0]);
		}


		// Track in TopAnalytics when an ad is displayed.  Hint: TopAds.OnAdShownEvent 

		//-----------------ANSWER---------------------
		/* 
		 * See OnAdShown()
		 */
		//------------------------------------------
	}

	public static void LoadAd()
	{
		if (loadingAdCoroutine != null)
		{
			return;
		}
		if (TopAdsBehaviour._instance == null)
		{
			Debug.LogError("TopAdsBehaviour._instance is null");
			return;
		}
		Debug.Log("try load new ad");
		//Using TopAdsBehaviour._instance monobehaviour to startCoroutine. Allowed because we need it to exist to display our ads
		loadingAdCoroutine = TopAdsBehaviour._instance.StartCoroutine(LoadAdCoroutine());
	}

	static IEnumerator LoadAdCoroutine()
	{
		adLoaded = false;
		addFailed = false;
		while (!adLoaded)
		{
			Debug.Log("request new ad");
			TopAds.RequestAd(adUnitIDList[0]);
			while (!adLoaded && !addFailed)
			{
				yield return null;
			}
			if (addFailed)
			{
				addFailed = false;
			}
		}
		loadingAdCoroutine = null;
	}

	public static void OnAdLoaded()
	{
		Debug.Log("ad load succes !");
		adLoaded = true;
	}
	public static void OnAdLoadingFail()
	{
		Debug.Log("ad load fail");
		addFailed = true;
	}
	public static void OnAdShown()
	{
		lastAdShownTime = Time.unscaledTime;
		lastAdShownGameIndex = currentGameIndex;
		adLoaded = false;
		TopAnalytics.TrackEvent("ad_shown");
		LoadAd();
	}

	public static void SetAdDisplayConditions(int _secondsBetweenAds, int _gamesBetweenAds)
	{
		// Sometimes studios call "ShowAd" too often and bombard players with ads 
		// Add a system that prevents the "ShowAd" method from showing an available ad 
		// Unless EITHER condition provided is true: 
		// 1) secondsBetweenAds: only show an ad if the previous ad was shown more than "secondBetweenAds" ago 
		// 2) gamesBetweenAds: only show an ad if "gamesBetweenAds" amount of games was played since the previous ad 

		secondsBetweenAds = _secondsBetweenAds;
		gamesBetweenAds = _gamesBetweenAds;
	}


	// === Please answer these quick questions within the code file ===  

	// In the VoodooSauce we integrate many 3rd party SDKs of varying reliability that display Ads, Analytics, etc.
	// What processes would you suggest to ensure that the VoodooSauce SDK is minimally affected by crashes 
	// in another SDK?

	//-----------------ANSWER---------------------
	/* 
	 * I'm unsure as to if there is a solution to avoid the crash bringing down the entire application.
	 * I don't think there is a reliable way to avoid a disastre once the crash happened.
	 * Though, we can act on two things : lowering the probability of a crash and knowing when a crash happened.
	 * The former can be done with diligent unit testing and careful updates of the SDKs the company is using.
	 * The later can be done through impmementing a crash report tool and sending logs to a database.
	 * We have to log somewhere wic ad the game is displaying, so we could log what it attends to load and the result of the loading.
	 * That way if we are using different ad providers we can see if one of them is causing a particular issue.
	 */
	//------------------------------------------



	// What are some pitfalls/shortcomings in your above implementation?
	//-----------------ANSWER---------------------
	/* 
	 * The main pitfall I see is that we need to go through the loading scene for the ads to work.
	 * If I had more time I'd try to make a fallback that initiates VoodooSauce the same way InitSceneController does, but during play time.
	 * It's very usual for developpers to play the game from any scene during development and it can be tedious and time-consumming to always come back to the loading scene to start the game.
	 * (Say, when you are testing a very small feature in the late game)
	 */
	//------------------------------------------

	// How would you improve your implementation if you had more than 2 hours? 
	//-----------------ANSWER---------------------
	/* 
	 * I's make the system mentionned above.
	 */
	//------------------------------------------


	// What do you enjoy the most about being a developer? 
	//-----------------ANSWER---------------------
	/* 
	 * Planning and making systems that work.
	 */
	//------------------------------------------

	// What do you enjoy the least about being a developer? 
	//-----------------ANSWER---------------------
	/* 
	 * Sitting at my desk all day and burning my eyes to a crisp looking at the monitor.
	 */
	//------------------------------------------

	// Why do you want to work on the VoodooSauce SDK vs. creating games in Unity?
	//-----------------ANSWER---------------------
	/* 
	 * Versus as in "instead of" ?
	 * It's not like you're not making games. You're still a part of the process.
	 * No one to work on the SDK -> no ads -> no income -> no company -> no game.
	 * Saying tht's not the case would be the same as saying the Concept Artist whose work isn't directly implemented isn't part of the process of creating a game.
	 */
	//------------------------------------------



}
