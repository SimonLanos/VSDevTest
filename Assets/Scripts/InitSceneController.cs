using _3rdParty;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitSceneController : MonoBehaviour
{
    // Start is called before the first frame update
    public static InitSceneController instance;
    public TopAdsBehaviour topAdsPrefab;
    public GDPRPopUp GDPRPopUpPrefab;
    public AdUnitIDList adUnitIDList;

    void Awake()
    {
        //singleton pattern
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        Init();
    }

    void Init()
    {
        //Instatiate TopAds Prefab
        TopAdsBehaviour topAds = Instantiate(topAdsPrefab);
        DontDestroyOnLoad(topAds.gameObject);

        //Initialize TopAds
        TopAds.InitializeSDK();

        //Set VoodooSauce tracking events
        TopAds.OnAdLoadedEvent += VoodooSauce.OnAdLoaded;
        TopAds.OnAdFailedEvent += VoodooSauce.OnAdLoadingFail;
        TopAds.OnAdShownEvent += VoodooSauce.OnAdShown;

        //Set ad id list
        VoodooSauce.SetAdUnitIDs(adUnitIDList);

        //Display toast
        _ShowAndroidToastMessage("Hello Voodoo!");

        //Line added to see RGPD PopUp in Editor at every launch
#if UNITY_EDITOR
        PlayerPrefs.DeleteAll();
#endif

        //We check in playerPref if consent is granted or revoked
        //If consent hasn't granted nor revoked we ask for it
        if (!PlayerPrefs.HasKey("consent"))
        {
            Instantiate(GDPRPopUpPrefab);
        }
        else
        {
            if (PlayerPrefs.GetInt("consent")!=0)
            {
                VoodooSauce.GrantConsent();
            }
            else
            {
                VoodooSauce.RevokeConsent();
            }
            GoToMainScene();
        }
    }

    public void GoToMainScene()
    {
        VoodooSauce.LoadAd();
        SceneManager.LoadScene(1);
    }

    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, message, 0);
                toastObject.Call("show");
            }));
        }
    }
}
