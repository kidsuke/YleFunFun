using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;

public class YleAPI {
	public static readonly string BASE_URL = "https://external.api.yle.fi";
	public static readonly string VERSION = "v1";
	private string mAppId = "";
	private string mAppKey = "";

	//GET Request
	public IEnumerator GetSth(IObserver<YleResponse> observer, CancellationToken cancellationToken, string query, int limit = 30, int offset = 0) {
		using (
			UnityWebRequest webRequest = UnityWebRequest.Get(
				BASE_URL + "/" + VERSION + "/programs/items.json?" + 
				GetAppId() + "&" + GetAppKey() + "&" +
				"availability=ondemand" + "&" + "limit=" + limit + "&" + "offset=" + offset
			)
		) {
			yield return webRequest.Send();

			if (webRequest.isHttpError || webRequest.isNetworkError) {
				observer.OnError(new System.Exception(webRequest.error));
			} else {
				YleResponse response = JsonUtility.FromJson<YleResponse>(webRequest.downloadHandler.text);
				observer.OnNext(response);
				observer.OnCompleted();
			}
		}
	}

	public string GetAppId()  {
		return "app_id=" + mAppId;
	}

	public string GetAppKey()  {
		return "app_key=" + mAppKey;
	}
		
}