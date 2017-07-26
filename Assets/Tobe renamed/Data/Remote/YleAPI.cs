using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;

public class YleAPI {
	public static readonly string BASE_URL = "https://external.api.yle.fi";
	public static readonly string BASE_IMAGE_URL = "http://images.cdn.yle.fi/image/upload";
	public static readonly string VERSION = "v1";
	private string mAppId = "d14e023d";
	private string mAppKey = "96da296c91609b7835e8bb861a4ca6fc";

	//GET Request
	public IEnumerator GetPrograms(IObserver<YleResponse> observer, CancellationToken cancellationToken, string query, int limit = 30, int offset = 0) {
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

	public IEnumerator GetCoverImage(IObserver<Texture> observer, CancellationToken cancellationToken,  string id, string transformation = "",string format = "png") {
		using (
			UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(
				BASE_IMAGE_URL + "/" + transformation + (transformation.Length == 0 ? "" : "/") + id + "." + format
			)
		) {
			Debug.Log(BASE_IMAGE_URL + "/" + transformation + (transformation.Length == 0 ? "" : "/") + id + "." + format);
			yield return webRequest.Send();

			if (webRequest.isHttpError || webRequest.isNetworkError) {
				observer.OnError(new System.Exception(webRequest.error));
			} else {
				Texture texture = ((DownloadHandlerTexture) webRequest.downloadHandler).texture;
				observer.OnNext(texture);
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