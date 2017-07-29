using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;

public class YleAPI {
	public static readonly string BASE_URL = "https://external.api.yle.fi";
	public static readonly string BASE_IMAGE_URL = "http://images.cdn.yle.fi/image/upload";
	public static readonly string VERSION = "v1";
	private UrlBuilder m_Builder = new UrlBuilder(BASE_URL);
	private string m_AppId = "d14e023d";
	private string m_AppKey = "96da296c91609b7835e8bb861a4ca6fc";

	//GET Request
	public IEnumerator GetPrograms(IObserver<YleResponse> observer, CancellationToken cancellationToken, string query, int offset = 0, int limit = 30) {
		using (
			UnityWebRequest webRequest = UnityWebRequest.Get(
				UrlBuilder.Create(BASE_URL)
				.Path(VERSION).Path("programs").Path("items.json")
				.Query("availability", "ondemand").Query("limit", limit.ToString()).Query("offset", offset.ToString()).Query("q", query)
				.Query("app_id", m_AppId).Query("app_key", m_AppKey)
				.Build()
			)
		) {
			yield return webRequest.Send();

			if (webRequest.isHttpError || webRequest.isNetworkError) {
				observer.OnError(new System.Exception(webRequest.error));
			} else {
				YleResponse response = JsonUtility.FromJson<YleResponse>(webRequest.downloadHandler.text);
				if (response != null) {
					observer.OnNext(response);
				}
				observer.OnCompleted();
			}
		}
	}

	public IEnumerator GetCoverImage(IObserver<Texture> observer, CancellationToken cancellationToken,  string id, string transformation = "",string format = "png") {
		using (
			UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(
				UrlBuilder.Create(BASE_IMAGE_URL)
				.Path(transformation).Path(id + "." + format)
				.Build()
			)
		) {
			yield return webRequest.Send();

			if (webRequest.isHttpError || webRequest.isNetworkError) {
				observer.OnError(new System.Exception(webRequest.error));
			} else {
				Texture texture = ((DownloadHandlerTexture) webRequest.downloadHandler).texture;
				if (texture != null) {
					observer.OnNext(texture);
				}
				observer.OnCompleted();
			}
		}
	}

	public class UrlBuilder {
		private StringBuilder m_Builder = new StringBuilder();
		private bool m_BeginQuery = false;

		public UrlBuilder (string url) {
			if (!url.EndsWith("/")) {
				url.Remove(url.Length - 1);
			}
			m_Builder.Append(url);
		}

		public static UrlBuilder Create(string url) {
			return new UrlBuilder(url);
		}

		public UrlBuilder Path (string path) {
			if (path.Length != 0) {
				m_Builder.Append("/").Append(path);
			} else {
				Debug.LogWarning(this.GetType().Name + ": Invalid path");
			}
			return this;
		}

		public UrlBuilder Query (string key, string value) {
			if (key.Length !=0 && value.Length !=0) {
				if (!m_BeginQuery) {
					m_Builder.Append("?").Append(key).Append("=").Append(value);
					m_BeginQuery = true;
				} else {
					m_Builder.Append("&").Append(key).Append("=").Append(value);
				}
			} else {
				Debug.LogWarning(this.GetType().Name + ": Invalid query");
			}
			return this;
		}

		public string Build() {
			return m_Builder.ToString();
		}
	}
}

