using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

//広告枠IDの設定
[System.Serializable]
public class AdfurikunMovieNativeAdViewAdConfig
{
	public string iPhoneAppID;
	//iOS
	public string androidAppID;
	//android
}

public class AdfurikunMovieNativeAdViewUtility : MonoBehaviour
{

	public Action<String> onLoadFinish = null;
	public Action<String, String> onLoadError = null;
	public Action<String> onPlayStart = null;
	public Action<String, bool> onPlayFinish = null;
	public Action<String, String> onPlayError = null;

	public enum ADF_MovieStatus
	{
		/// <summary>
		/// ネイティブ広告の読み込み成功
		/// </summary>
		LoadFinish,
		/// <summary>
		/// ネイティブ広告の読み込み失敗
		/// </summary>
		LoadError,
		/// <summary>
		/// ネイティブ広告の再生開始
		/// </summary>
		PlayStart,
		/// <summary>
		/// ネイティブ広告の再生完了
		/// </summary>
		PlayFinish,
		/// <summary>
		/// ネイティブ広告の再生失敗
		/// </summary>
		PlayError,
	}

	//広告枠IDの設定
	public AdfurikunMovieNativeAdViewAdConfig config;

	private static AdfurikunMovieNativeAdViewUtility mInstance = null;
	private GameObject mMovieNativeAdViewSrcObject = null;

	private String unityPluginVersion = "2.20.0";

	//TODO:

	#if UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern void initializeMovieNativeAdViewIOS_(string appID, string pluginVersion, string unityVersion);

	[DllImport("__Internal")]
	private static extern void loadMovieNativeAdViewIOS_(string appID);

	[DllImport("__Internal")]
	private static extern void showMovieNativeAdViewIOS_(string appID, float x, float y, float width, float height, float screenW);

	[DllImport("__Internal")]
	private static extern void setMovieNativeAdViewFrameIOS_(string appID, float x, float y, float width, float height, float screenW);

	[DllImport("__Internal")]
	private static extern void playMovieNativeAdViewIOS_(string appID);

	[DllImport("__Internal")]
	private static extern void hideMovieNativeAdViewIOS_(string appID);

	#endif

	public void Awake ()
	{
		if (mInstance == null) {
			mInstance = this;
			DontDestroyOnLoad (gameObject);
		} else {
			Destroy (gameObject);
		}
	}


	// Use this for initialization
	void Start ()
	{
		if (Application.isEditor)
			return;

		#if UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			AndroidJavaClass player = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			AndroidJavaObject activity = player.GetStatic<AndroidJavaObject> ("currentActivity");

			AndroidJavaObject wm = activity.Call<AndroidJavaObject> ("getWindowManager");
			if (wm != null) {
				AndroidJavaObject display = wm.Call<AndroidJavaObject> ("getDefaultDisplay");
				if (display != null) {
					AndroidJavaObject displayMetrics = new AndroidJavaObject ("android.util.DisplayMetrics");
					display.Call ("getMetrics", displayMetrics);
				}
			}
		}
		#endif

		this.initializeMovieNativeAdView ();
	}

	public void OnApplicationPause (bool pause)
	{
		if (Application.isEditor)
			return;

		#if UNITY_IPHONE
		#elif UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			if (pause) {
				this.callAndroidMovieNativeAdViewMethod ("onPause");
			} else {
				this.callAndroidMovieNativeAdViewMethod ("onResume");
			}
		}
		#endif
	}

	public void initializeMovieNativeAdView ()
	{
		this.initializeMovieNativeAdView (this.getAppID ());
	}

	public void initializeMovieNativeAdView (string appId)
	{
		#if UNITY_IPHONE
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			initializeMovieNativeAdViewIOS_(appId, unityPluginVersion, Application.unityVersion);
		}
		#elif UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			AndroidJavaClass player = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			AndroidJavaObject activity = player.GetStatic<AndroidJavaObject> ("currentActivity");
			//Unity Plugin Version Setup
			AndroidJavaClass manager = new AndroidJavaClass("jp.tjkapp.adfurikunsdk.moviereward.unityplugin.AdfurikunUnityManager");
			manager.CallStatic("initialize", activity, unityPluginVersion);
			this.makeInstance_AdfurikunMovieNativeAdViewController ().CallStatic ("initialize", activity, appId);
		}
		#endif
	}

	public void loadMovieNativeAdView ()
	{
		this.loadMovieNativeAdView (this.getAppID ());
	}

	public void loadMovieNativeAdView (string appId)
	{
		#if UNITY_IPHONE
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			loadMovieNativeAdViewIOS_(appId);
		}
		#elif UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			this.makeInstance_AdfurikunMovieNativeAdViewController ().CallStatic ("load", appId);

		}
		#endif
	}

	public void setMovieNativeAdView (float x, float y, float width, float height)
	{
		this.setMovieNativeAdView (this.getAppID (), x, y, width, height);
	}

	public void setMovieNativeAdView (string appId, float x, float y, float width, float height)
	{
		#if UNITY_IPHONE
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			setMovieNativeAdViewFrameIOS_(appId, x, y, width, height, Screen.width);
		}
		#elif UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			AndroidJavaClass player = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			AndroidJavaObject activity = player.GetStatic<AndroidJavaObject> ("currentActivity");
			AndroidJavaObject window = activity.Call<AndroidJavaObject> ("getWindow");
			AndroidJavaObject decorView = window.Call<AndroidJavaObject> ("getDecorView");
			int decorViewW  = decorView.Call<int> ("getWidth");
			int decorViewH  = decorView.Call<int> ("getHeight");

			float densityW = decorViewW * (width/Screen.width);
			float densityH = decorViewH * (height/Screen.height);
			float pointX   = decorViewW * (x/Screen.width);
			float pointY   = decorViewH * (y/Screen.height);

			this.makeInstance_AdfurikunMovieNativeAdViewController ().CallStatic ("show", appId, pointX, pointY, densityW, densityH);
		}
		#endif
	}

	public void setMovieNativeAdViewFrame (float x, float y, float width, float height)
	{
		this.setMovieNativeAdViewFrame (this.getAppID (), x, y, width, height);
	}

	public void setMovieNativeAdViewFrame (string appId, float x, float y, float width, float height)
	{
		#if UNITY_IPHONE
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			setMovieNativeAdViewFrameIOS_(appId, x, y, width, height, Screen.width);
		}
		#elif UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			this.setMovieNativeAdView(appId, x, y, width, height);
		}
		#endif
	}

	public void setFrameGravity (float displaySizeW, float displaySizeH, float width, float height, int horizontalGravity, int verticalGravity)
	{
		this.setFrameGravity (this.getAppID (), displaySizeW, displaySizeH, width, height, horizontalGravity, verticalGravity);
	}

	public void setFrameGravity (string appId, float displaySizeW, float displaySizeH, float width, float height, int horizontalGravity, int verticalGravity)
	{
		float scale = Screen.width / displaySizeW;
		float realWidth = width * scale;
		float realHeight = height * scale;

		float realX = this.getScreenPositionByGravity (horizontalGravity, Screen.width, realWidth);
		float realY = this.getScreenPositionByGravity (verticalGravity, Screen.height, realHeight);

		this.setMovieNativeAdViewFrame (appId, realX, realY, realWidth, realHeight);
	}

	public void setFitWidthFrame (float displaySizeH, float height, int verticalGravity) {
		this.setFitWidthFrame (this.getAppID (), displaySizeH, height, verticalGravity);
	}

	public void setFitWidthFrame (string appId, float displaySizeH, float height, int verticalGravity) {
		float scale = Screen.height / displaySizeH;
		float realHeight = height * scale;
		float realWidth = Screen.width;

		float realY = this.getScreenPositionByGravity (verticalGravity, Screen.height, realHeight);

		this.setMovieNativeAdViewFrame (appId, 0, realY, realWidth, realHeight);
	}

	public void playMovieNativeAdView ()
	{
		this.playMovieNativeAdViewNative (this.getAppID ());
	}

	public void playMovieNativeAdViewNative (string appId)
	{
		#if UNITY_IPHONE
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			playMovieNativeAdViewIOS_(appId);
		}
		#elif UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			this.makeInstance_AdfurikunMovieNativeAdViewController ().CallStatic ("play", appId);
		}
		#endif
	}

	public void hideMovieNativeAdView ()
	{
		this.hideMovieNativeAdView (this.getAppID ());
	}

	public void hideMovieNativeAdView (string appId)
	{
		#if UNITY_IPHONE
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			hideMovieNativeAdViewIOS_(appId);
		}
		#elif UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			this.makeInstance_AdfurikunMovieNativeAdViewController ().CallStatic ("hide", appId);
		}
		#endif
	}

	public void dispose ()
	{
		this.disposeResource ();
	}

	public void disposeResource ()
	{
		#if UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			this.callAndroidMovieNativeAdViewMethod ("onDestroy");
		}
		#endif
	}

	[Obsolete("Use Action delegate instead.")]
	public void setMovieNativeAdViewSrcObject (GameObject movieNativeAdViewSrcObject)
	{
		this.setMovieNativeAdViewSrcObject (movieNativeAdViewSrcObject, this.getAppID ());
	}

	public void setMovieNativeAdViewSrcObject (GameObject movieNativeAdViewSrcObject, string appId)
	{
		this.mMovieNativeAdViewSrcObject = movieNativeAdViewSrcObject;
	}

	// Update is called once per frame
	void Update ()
	{
		
	}

	private string getAppID ()
	{
		string appId = "";
		#if UNITY_IPHONE
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
		    appId = config.iPhoneAppID;
		}
		#elif UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			appId = config.androidAppID;
		}
		#endif
		return appId;
	}

	private float getScreenPositionByGravity (int gravity, float screenSize, float contentSize)
	{
		float p = 0;
		switch (gravity) {
		case 0:
			p = 0;
			break;
		case 1:
			p = (screenSize - contentSize) / 2;
			break;
		case 2:
			p = screenSize - contentSize;
			break;
		}
		return p;
	}

	/**
	 * コールバック受け取りメソッド
	 * iOS/Android共通
	 */
	public void MovieNativeAdViewCallback (string param_str)
	{
		string[] splitParamRoot = param_str.Split (';');
		string stateName = splitParamRoot [0].Split (':') [1];
		switch (stateName) {
		case "LoadFinish":
			this.MovieNativeAdViewLoadFinish (param_str);
			break;
		case "LoadError":
			this.MovieNativeAdViewLoadError (param_str);
			break;
		case "PlayStart":
			this.MovieNativeAdViewPlayStart (param_str);
			break;
		case "PlayFinish":
			this.MovieNativeAdViewPlayFinish (param_str);
			break;
		case "PlayFail":
			this.MovieNativeAdViewPlayError (param_str);
			break;
		default:
			return;
		}
	}

	private void MovieNativeAdViewLoadFinish (string param_str)
	{
		string[] splitParamRoot = param_str.Split (';');
		string appID = splitParamRoot [1].Split (':') [1];
		string errcode = "";

		this.onLoadFinish.NullSafe (appID);

		//{{{ Obsolete @v2.15.0
		ADF_MovieStatus messageStateName;
		messageStateName = ADF_MovieStatus.LoadFinish;
		ArrayList arr = new ArrayList ();
		arr.Add ((int)messageStateName);
		arr.Add (appID);
		arr.Add (errcode);
		if (this.mMovieNativeAdViewSrcObject != null) {
			this.mMovieNativeAdViewSrcObject.SendMessage ("MovieNativeAdViewCallback", arr);
		}
		//}}}
	}

	private void MovieNativeAdViewLoadError (string param_str)
	{

		string[] splitParamRoot = param_str.Split (';');
		string appID = splitParamRoot [1].Split (':') [1];
		string errcode = "";
		if (splitParamRoot.Length > 2) {
			errcode = splitParamRoot [2].Split (':') [1];
		}

		this.onLoadError.NullSafe (appID, errcode);

		//{{{ Obsolete @v2.15.0
		ADF_MovieStatus messageStateName;
		messageStateName = ADF_MovieStatus.LoadError;
		ArrayList arr = new ArrayList ();
		arr.Add ((int)messageStateName);
		arr.Add (appID);
		arr.Add (errcode);
		if (this.mMovieNativeAdViewSrcObject != null) {
			this.mMovieNativeAdViewSrcObject.SendMessage ("MovieNativeAdViewCallback", arr);
		}
		//}}}
	}
		
	private void MovieNativeAdViewPlayStart (string param_str)
	{
		string[] splitParamRoot = param_str.Split (';');
		string appID = splitParamRoot [1].Split (':') [1];

		this.onPlayStart.NullSafe (appID);
	}

	private void MovieNativeAdViewPlayFinish (string param_str)
	{
		string[] splitParamRoot = param_str.Split (';');
		string appID = splitParamRoot [1].Split (':') [1];
		bool isVideoBool = true;
		if (splitParamRoot.Length > 2) {
			string isVideo = splitParamRoot [2].Split (':') [1];
			#if UNITY_IPHONE
			if (isVideo == "0") {
				isVideoBool = false;
			}
			#elif UNITY_ANDROID
			if (isVideo == "false") {
				isVideoBool = false;
			}
			#endif
		}
			
		this.onPlayFinish.NullSafe (appID, isVideoBool);
	}

	private void MovieNativeAdViewPlayError (string param_str)
	{
		string[] splitParamRoot = param_str.Split (';');
		string appID = splitParamRoot [1].Split (':') [1];
		string errcode = "";
		if (splitParamRoot.Length > 2) {
			errcode = splitParamRoot [2].Split (':') [1];
		}
			
		this.onPlayError.NullSafe (appID, errcode);
	}


	#if UNITY_ANDROID
	private AndroidJavaClass makeInstance_AdfurikunMovieNativeAdViewController ()
	{
		return new AndroidJavaClass ("jp.tjkapp.adfurikunsdk.moviereward.unityplugin.AdfurikunMovieNativeAdViewController");
	}

	private void callAndroidMovieNativeAdViewMethod (string methodName)
	{
		this.makeInstance_AdfurikunMovieNativeAdViewController ().CallStatic (methodName);
	}
	#endif
}
