using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class SearchSceneController : MonoBehaviour {
	
	public enum SearchSceneState {
		STATE_EMPTY, STATE_NOT_FOUND, STATE_LOADING, STATE_LOADED, STATE_NO_INTERNET
	}

	[SerializeField]
	private RectTransform m_EmptySearch; // The view to show when no search is conducted
	public RectTransform emptySearch { get { return m_EmptySearch; } set { m_EmptySearch = value; } }
	[SerializeField]
	private RectTransform m_NoResultFound; // The view to show when no result is found
	public RectTransform noResultFound { get { return m_NoResultFound; } set { m_NoResultFound = value; } }
	[SerializeField]
	private RectTransform m_NoInternet; // The view to show when no result is found
	public RectTransform noInternet { get { return m_NoInternet; } set { m_NoInternet = value; } }
	[SerializeField]
	private SearchResultsScrollView m_ScrollView; // The view to show the search results
	public SearchResultsScrollView scrollView { get { return m_ScrollView; } set { m_ScrollView = value; } }
	[SerializeField]
	private GameObject m_LoadingIndicator; // The view to show when waiting for the search results
	public GameObject loadingIndicator { get { return m_LoadingIndicator; } set { m_LoadingIndicator = value; }}

    private YleAPI m_API;
	private SearchSceneState m_State; // The current state of the search scene

	void Awake () {
		// Initialize variables
		m_API = new YleAPI();
	}

	void Start () {
		SetupView();
	}

	// Get programs from Yle
	public IObservable<List<Program>> GetPrograms (string query = "", int offset = 0) {
		return Observable.FromCoroutine<YleResponse>((observer, cancellationToken) => m_API.GetPrograms(observer, cancellationToken, query, offset))
						 .DoOnError(error => {
							 int responseCode = 0;
							 if (int.TryParse(error.Message, out responseCode)) {
								 if (responseCode == 0) {
									 // This message is the response code. If the length of it is 0 (no response code returned), it means no network connection
									 SetState(SearchSceneState.STATE_NO_INTERNET);
								 }
							 }
						 })
						 .Select(ToPrograms) ;// Map YleResponse to Item (`Select` operator is the same as `Map` operator in Reactive Programming)				 
	}
		
	// Use this for mapping YleResponse to Item
	public List<Program> ToPrograms (YleResponse response) {
		List<Program> programs = new List<Program>();
		for (int index = 0; index < response.data.Length; index++) {
			Program program = new Program();

			program.id = response.data[index].id;
			program.imageId = response.data[index].image.id;
			program.title = (response.data[index].title.fi != null) ? response.data[index].title.fi : response.data[index].title.sv;
			program.title = (program.title != null) ? program.title : "No title found";
			program.description = (response.data[index].description.fi !=null) ? response.data[index].description.fi : "No description found";
			program.type = (response.data[index].type !=null) ? response.data[index].type : "No type found";
			program.duration = ParseDurationFromResponse(response.data[index].duration);

			programs.Add(program);
		}
		return programs;
	}

	private string ParseDurationFromResponse (string duration) {
		if (duration == null) {
			return "No duration found";
		}

		try {
			// If duration exists, it will be in the format "PT*H*M*S", where * is the amount of minutes and seconds
			StringBuilder builder = new StringBuilder();
			string[] seperator = new string[] {"PT", "H", "M", "S"};
			string[] durationAsArray = duration.Split(seperator, StringSplitOptions.RemoveEmptyEntries);

			if (durationAsArray.Length == 3) {
				builder.Append(durationAsArray[0]).Append(" ").Append("H").Append(" ");
				builder.Append(durationAsArray[1]).Append(" ").Append("MIN").Append(" ");
				builder.Append(durationAsArray[2]).Append(" ").Append("SEC");
			} else if (durationAsArray.Length == 2) {
				builder.Append(durationAsArray[0]).Append(" ").Append("MIN").Append(" ");
				builder.Append(durationAsArray[1]).Append(" ").Append("SEC");
			} else if (durationAsArray.Length == 1) {
				builder.Append(durationAsArray[0]).Append(" ").Append("SEC");
			}

			return builder.ToString();
		} catch (IndexOutOfRangeException e) {
			Debug.LogError(this.GetType().Name + ": " + e.Message);
			return "No duration found";
		}
	}

	public void SetState (SearchSceneState state) {
		if (m_State != state) {
			m_State = state;
			SetupView();
		}
	}

	private void SetupView () {
		switch(m_State) {
			case SearchSceneState.STATE_EMPTY:
				m_EmptySearch.gameObject.SetActive(true);
				m_NoResultFound.gameObject.SetActive(false);
				m_LoadingIndicator.SetActive(false);
				m_NoInternet.gameObject.SetActive(false);
				break;
			case SearchSceneState.STATE_NOT_FOUND:
				m_NoResultFound.gameObject.SetActive(true);
				m_EmptySearch.gameObject.SetActive(false);
				m_LoadingIndicator.SetActive(false);
				m_NoInternet.gameObject.SetActive(false);
				break;
			case SearchSceneState.STATE_LOADING:
				m_LoadingIndicator.SetActive(true);
				m_EmptySearch.gameObject.SetActive(false);
				m_NoResultFound.gameObject.SetActive(false);
				m_NoInternet.gameObject.SetActive(false);
				break;
			case SearchSceneState.STATE_LOADED:
				m_LoadingIndicator.SetActive(false);
				m_EmptySearch.gameObject.SetActive(false);
				m_NoResultFound.gameObject.SetActive(false);
				m_NoInternet.gameObject.SetActive(false);
				break;
			case SearchSceneState.STATE_NO_INTERNET:
				m_NoInternet.gameObject.SetActive(true);
				m_LoadingIndicator.SetActive(false);
				m_EmptySearch.gameObject.SetActive(false);
				m_NoResultFound.gameObject.SetActive(false);
				break;
			default:
				break;
		}
	}
}
