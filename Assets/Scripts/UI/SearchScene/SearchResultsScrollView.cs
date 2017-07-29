using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class SearchResultsScrollView : LoopVerticalScrollRect {
	
	[SerializeField]
	private InputField m_SearchField;
	public InputField searchField { get { return m_SearchField; } set { m_SearchField = value; } }

	private List<Program> m_Programs;
	private SearchSceneController m_Controller;
	private bool m_EndPreviousSearch; // Determine whether the previous search should be ended
	private bool m_NeedRefill; // Determine whether the scroll view need to be refilled
	private string m_Query; // Store current search query to determine whether the next search is the same or different. Work with off-set
	private int m_Offset; // Store off-set of the current search so that it won't start from the beginning in the next search (if the next search query is the same)

	// The SearchResultsScrollView is based on an open source and in my opinion it may be hard to undestand at first...
	// Therefore, I decide to rename the variables to increase readability and add some comment
	private LoopScrollDataSource m_DataSource { get { return dataSource; } set { dataSource = value; } } // A data source to provide data from the data list for each cell
	private LoopScrollPrefabSource m_PrefabSource { get { return prefabSource; } } // A prefab source to provide a pool of objects based on a given prefab
	private int m_TotalCount { get { return totalCount; } set { totalCount = value; } } // The total number of items of the data list (not the current displated cells)
	private int m_FirstVisibleItemIndex { get { return itemTypeStart; } } // The index of the first visible cell
	private int m_LastVisibleItemIndex { get { return itemTypeEnd; } } // The index of the last visible cell

	void Awake () {
		m_Programs = new List<Program>();
		m_NeedRefill = true;
		m_DataSource = new LoopScrollListSource<Program>(m_Programs);
		m_Controller = GameObject.FindObjectOfType<SearchSceneController>();
	}

	// Use this for initialization
	void Start () {
		m_PrefabSource.InitPool();
		RedisplaySearchResultsIfExist();
		ResetSearchTrackingProperties();
		BindEvents();
	}

	private void BindEvents() {
		m_SearchField // Listen to text changed event
			.OnValueChangedAsObservable()
			.Skip(1) // Skip the default value
			.Throttle(TimeSpan.FromSeconds(1f)) // Since the user may input too fast, this function will take an emitted value from the stream every 1 second. 
												// Values which are emitted within that 1 second are ignored
			.Subscribe(query => {
				// New query has been entered...
				// Clear all the results and reset tracking properties
				ClearCells();
				ResetSearchTrackingProperties();
				if (query.Length == 0) {
					// If user has cleared the text
					m_Controller.SetState(SearchSceneController.SearchSceneState.STATE_EMPTY);
				} else {
					m_Controller.SetState(SearchSceneController.SearchSceneState.STATE_LOADING);
					m_Query = query;
					SearchForPrograms();
				}
			})
			.AddTo(this);

		onValueChanged.AddListener(HandleOnScrollEvent); // Listen to scrolling event
	}

	public void HandleOnScrollEvent (Vector2 position) {
		if (!m_EndPreviousSearch) {
			if (m_TotalCount > 0 && m_LastVisibleItemIndex + 3 >= m_TotalCount) {
				SearchForPrograms();
			};
		};	
	}

	public void SearchForPrograms () {
		m_Controller.GetPrograms(m_Query, m_Offset)
					.Do(programs => m_Offset += (programs.Count - 1)) // Cache the off-set
					.DelayFrameSubscription(2) // Delay some frames to improve performance
					.Subscribe(programs => {
						if (programs.Count == 0) {
							// If no results found
							if (!m_EndPreviousSearch) {
								// Auto search means continuing to provide more results of the previous search if possible when scrolling (because we just provide 10 results at a time)
								// If the amount of items received is 0, it means no (or no more) result found.
								// Mark m_EndPreviousSearch as true to prevent auto search when scrolling
								m_EndPreviousSearch = true;
							}
							if (m_Programs.Count <= 0) {
								m_Controller.SetState(SearchSceneController.SearchSceneState.STATE_NOT_FOUND);
							}
						} else {
							m_Controller.SetState(SearchSceneController.SearchSceneState.STATE_LOADED);
							UpdateView(programs);
							
							// Cache data in case user go back from detail scence
							SceneTransitionData.query = m_Query;
							SceneTransitionData.currentSearchResults = m_Programs;
						}
					})
					;
	}

	public override void ClearCells() {
		//Remove all the cells from the scroll view
		m_Programs.Clear();
		m_TotalCount = 0;
		m_NeedRefill = true;
		base.ClearCells();
	}

	public void ResetSearchTrackingProperties () {
		m_Query = "";
		m_Offset = 0;
		m_EndPreviousSearch = false;
	}

	public void RedisplaySearchResultsIfExist() {
		if (SceneTransitionData.query != null && SceneTransitionData.query.Length != 0) {
			m_SearchField.text = SceneTransitionData.query;
		}
		if (SceneTransitionData.currentSearchResults != null && SceneTransitionData.currentSearchResults.Count > 0) {
			UpdateView(SceneTransitionData.currentSearchResults);
		}
	}

	public void UpdateView (List<Program> programs) {
		m_Programs.AddRange(programs);
		m_TotalCount = m_Programs.Count;

		if (m_NeedRefill) {
			// Fill the game scene with cells of items. Because the scroll view will reuse the cells, this should only be done when ClearCells is called
			RefillCells();
			m_NeedRefill = false;
		}
	}
}
