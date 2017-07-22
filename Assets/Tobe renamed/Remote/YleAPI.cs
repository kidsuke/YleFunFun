using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class YleAPI : MonoBehaviour {
	public static readonly string BASE_URL = "https://external.api.yle.fi";
	public static readonly string VERSION = "v1";
	private string mAppId = "";
	private string mAppKey = "";
	private int mLimit = 1;

	public YleAPI() {
		
	}

	void Start() {
		StartCoroutine(GetSth());
	}

	//GET Request
	IEnumerator GetSth() {
		using (
			UnityWebRequest webRequest = UnityWebRequest.Get(
				BASE_URL + "/" + VERSION + "/programs/items.json?" + 
				GetAppId() + "&" + GetAppKey() + "&" +
				"availability=ondemand" + "&" +
				GetLimit()
			)
		) {
			yield return webRequest.Send();

			if (webRequest.isHttpError || webRequest.isNetworkError) {
				Debug.LogError("Web request error");
			} else {
				print(webRequest.downloadHandler.text);
				YleResponse resp = JsonUtility.FromJson<YleResponse> (webRequest.downloadHandler.text);
				print(resp);
			}
				
		}
	}

	public string GetAppId()  {
		return "app_id=" + mAppId;
	}

	public string GetAppKey()  {
		return "app_key=" + mAppKey;
	}

	public string GetLimit() {
		return "limit=" + mLimit;
	}

}