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
	private bool m_EndPreviousSearch;
	private bool m_NeedRefill;
	// Store current search query to determine whether the next search is the same or different. Work with off-set
	private string m_Query;
	// Store off-set of the current search so that it won't start from the beginning in the next search (if the next search query is the same)
	private int m_Offset;

	private LoopScrollDataSource m_DataSource { get { return dataSource; } set { dataSource = value; } }
	private LoopScrollPrefabSource m_PrefabSource { get { return prefabSource; } }
	private int m_TotalCount { get { return totalCount; } set { totalCount = value; } }
	private int m_FirstVisibleItemIndex { get { return itemTypeStart; } }
	private int m_LastVisibleItemIndex { get { return itemTypeEnd; } }

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
		m_SearchField
			.OnValueChangedAsObservable()
			.Skip(1) // Skip the default value
			.Throttle(TimeSpan.FromSeconds(1f)) // Since the user may input too fast, this function will take an emitted value from the stream every 1 second. 
												// Values which are emitted within that 1 second are ignored
			.Subscribe(query => {
				if (query.Length == 0) {
					// User has cleared the text. Clear all the results and reset tracking properties
					m_Controller.SetState(SearchSceneController.SearchSceneState.STATE_EMPTY);
					ClearCells();
					ResetSearchTrackingProperties();
				} else {
					m_Controller.SetState(SearchSceneController.SearchSceneState.STATE_LOADING);
					if (!m_Query.Equals(query)) {
						ClearCells();
						ResetSearchTrackingProperties();
						m_Query = query;
					}
					SearchForPrograms();
				}
			})
			.AddTo(this);

		onValueChanged.AddListener(HandleOnScrollEvent);
	}

	public void HandleOnScrollEvent (Vector2 position) {
		print("itemCount = " + m_TotalCount + ", start = " + m_FirstVisibleItemIndex + ", end = " + m_LastVisibleItemIndex);
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
								// Auto search means continuing to provide more results of the previous search if possible (because we just provide 10 results at a time)
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
			// Fill the game scene with cells of items. Because the scroll list will reuse the cells, this should only be called once 
			RefillCells();
			m_NeedRefill = false;
		}
	}
}
