/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: June 1, 2019
* 
* The AssetBundleManager class is used to load assets from asset bundles stored in an Azure blob.
*
*/


using Azure.StorageServices;
using RESTClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AssetManagers {
	
	public class AssetBundleManager : MonoBehaviour {

		public static AssetBundleManager instance;	/// <value> Global instance </value>

		[Header("Azure Storage Service")]
		[SerializeField]
		private string storageAccount;	/// <value> Storage account </value>
		[SerializeField]		
		private string accessKey;		/// <value> Access key </value>
		[SerializeField]
		private string container;		/// <value> Container name in azure blob </value>

		private StorageServiceClient client;	/// <value> Client to set endpoint to </value>
		private BlobService blobService;		/// <value> Blob </value>

		private List<AssetBundle> assetBundles = new List<AssetBundle>();	/// <value> List of asset bundles loaded </value>
		private AssetBundle monsterBundle;		/// <value> Asset bundle fo monster sprites </value>
		private AssetBundle mAnimationBundle;	/// <value> Asset bundle fo monster animations </value>
		private AssetBundle pmAnimationBundle;	/// <value> Asset bundle for partyMember animations </value>		
		private string monsterBundleName = "monster";					/// <value> Name of monster sprite bundle </value>
		private string mAnimationBundleName = "monsteranimation";		/// <value> Name of monster animation bundle </value>
		private string pmAnimationBundleName = "partymemberanimation";	/// <value> Name of partyMember animation bundle </value>
		public bool isReady { get; private set; } = false;  			/// <value> Program waits for asset bundles to finish loading </value>

		/// <summary>
		/// Awake to instantiate singleton
		/// </summary> 
		void Awake() { 
			if (instance == null) {
				instance = this;
			}
			else if (instance != this) {
				DestroyImmediate (gameObject);
				instance = this;
			}
		}

		/// <summary>
		/// Load all asset bundles on start up
		/// </summary>
		IEnumerator Start() {
			if (string.IsNullOrEmpty(storageAccount) || string.IsNullOrEmpty(accessKey)) {
				Debug.Log("Storage account and access key are required");
			}

			client = StorageServiceClient.Create(storageAccount, accessKey);
			blobService = client.GetBlobService();
			
			// should only load one asset bundle at a time when needed, get its resources, then unload it
			yield return StartCoroutine(LoadAssetBundle(monsterBundleName));
			yield return StartCoroutine(LoadAssetBundle(mAnimationBundleName));
			yield return StartCoroutine(LoadAssetBundle(pmAnimationBundleName));

			assetBundles.Add(monsterBundle);
			assetBundles.Add(mAnimationBundle);
			assetBundles.Add(pmAnimationBundle);

			isReady = true;
		}

		/// <summary>
		/// Unload all asset bundles when program ends
		/// </summary>
		void OnDestroy() {
			UnloadAllAssetBundle();
		}

		/// <summary>
		/// Unloads each asset bundle
		/// </summary>
		private void UnloadAllAssetBundle() {
			for (int i = 0; i < assetBundles.Count;i++) {
				assetBundles[i].Unload(true);
			}
		}

		/// <summary>
		/// Loads a specific asset bundle
		/// </summary>
		/// <param name="assetBundleName"> Name of asset bundle to be unloaded </param>
		/// <returns></returns>
		public IEnumerator LoadAssetBundle(string assetBundleName) {
			string filename = assetBundleName;
			string url = Path.Combine (client.SecondaryEndpoint() + container, filename);
			//Debug.Log ("Attempting to load asset bundle: " + url);
			UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url);
			yield return www.SendWebRequest();
			if (www.isNetworkError) {
				Debug.LogError("Load Asset Bundle url error: " + www.error);
				yield break;
			} else {
				if (assetBundleName == monsterBundleName) {
					monsterBundle = ((DownloadHandlerAssetBundle)www.downloadHandler).assetBundle;
				}
				else if (assetBundleName == mAnimationBundleName) {
					mAnimationBundle = ((DownloadHandlerAssetBundle)www.downloadHandler).assetBundle;
				}
				else if (assetBundleName == pmAnimationBundleName) {
					pmAnimationBundle = ((DownloadHandlerAssetBundle)www.downloadHandler).assetBundle;
				}
				Debug.Log("Load Asset Bundle url success: " + url);
			}
		}

		/// <summary>
		/// Loads a sprite asset from a given bundle
		/// </summary>
		/// <param name="assetBundleName"> Name of bundle </param>
		/// <param name="assetName"> Name of asset in bundle </param>
		/// <returns> Returns the sprite asset of type Texture2D (need to verify this) </returns>
		public IEnumerator LoadSpriteAssetFromBundle(string assetBundleName, string assetName) {
			if (assetBundleName == monsterBundleName) {
				AssetBundleRequest loadAsset = monsterBundle.LoadAssetAsync(assetName, typeof(Sprite));
				yield return loadAsset.asset;
			}
		}

		/// <summary>
		/// Determines the type of platform at runtime for specifying the type of bundle to be loaded
		/// </summary>
		/// <returns> String containing platform type </returns>
		private string GetAssetBundlePlatformName() {
			switch (Application.platform) {
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.WindowsPlayer:
				return SystemInfo.operatingSystem.Contains ("64 bit") ? "x64" : "x86";
			case RuntimePlatform.WSAPlayerX86:
			case RuntimePlatform.WSAPlayerX64:
			case RuntimePlatform.WSAPlayerARM:
				return "WSA";
			case RuntimePlatform.Android:
				return "Android";
			case RuntimePlatform.IPhonePlayer:
				return "iOS";
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.OSXPlayer:
				return "OSX";
			default:
				throw new Exception ("Platform not listed");
			}
		}
	}
}