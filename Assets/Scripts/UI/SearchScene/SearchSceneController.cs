using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class SearchSceneController : MonoBehaviour {
	
	public enum SearchSceneState {
		STATE_EMPTY, STATE_NOT_FOUND, STATE_LOADED
	}

	[SerializeField]
	private RectTransform m_EmptySearch;
	public RectTransform emptySearch { get { return m_EmptySearch; } set { m_EmptySearch = value; } }
	[SerializeField]
	private RectTransform m_NoResultFound;
	public RectTransform noResultFound { get { return m_NoResultFound; } set { m_NoResultFound = value; } }
	[SerializeField]
	private SearchResultsScrollView m_ScrollView;
	public SearchResultsScrollView scrollView { get { return m_ScrollView; } set { m_ScrollView = value; } }

    private YleAPI m_API;
	private SearchSceneState m_State;

	void Awake () {
		// Initialize variables
		m_API = new YleAPI();
	}

	void Start () {
		m_State = SearchSceneState.STATE_EMPTY;
		SetupView();
	}

	public IObservable<List<Program>> GetPrograms (string query = "", int offset = 0) {
		return Observable.FromCoroutine<YleResponse>((observer, cancellationToken) => m_API.GetPrograms(observer, cancellationToken, query, offset))
						 .Select(ToPrograms) ;// Map YleResponse to Item (`Select` operator is the same as `Map` operator in Reactive Programming)				 
	}
		
	// Use this for mapping YleResponse to Item
	public List<Program> ToPrograms (YleResponse response) {
		List<Program> programs = new List<Program>();
		for (int index = 0; index < response.data.Length; index++) {
			Program program = new Program();

			program.id = response.data[index].id;
			program.imageId = response.data[index].image.id;
			program.title = (response.data[index].title.fi != null) ? response.data[index].title.fi : "No finnish title found";
			program.description = (response.data[index].description.fi !=null) ? response.data[index].description.fi : "No description found";
			programs.Add(program);
		}
		return programs;
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
				break;
			case SearchSceneState.STATE_NOT_FOUND:
				m_NoResultFound.gameObject.SetActive(true);
				m_EmptySearch.gameObject.SetActive(false);
				break;
			case SearchSceneState.STATE_LOADED:
				m_EmptySearch.gameObject.SetActive(false);
				m_NoResultFound.gameObject.SetActive(false);
				break;
			default:
				break;
		}
	}
}
