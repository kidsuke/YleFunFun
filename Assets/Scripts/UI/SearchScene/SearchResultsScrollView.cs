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

	private List<Item> m_Items;
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

	public delegate void OnScrollChangedHandler (int totalCount, int firstVisibleItems, int lastVisibleItems);
	public OnScrollChangedHandler scrollChangedHandler;

	void Awake () {
		m_Items = new List<Item>();
		m_NeedRefill = true;
		m_DataSource = new LoopScrollListSource<Item>(m_Items);

	}

	// Use this for initialization
	void Start () {
		m_PrefabSource.InitPool();
		RedisplaySearchResultsIfExist();
		ResetSearchTrackingProperties();
		BindEvents();
		m_Controller = GameObject.FindObjectOfType<SearchSceneController>();
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
					ClearCells();
					ResetSearchTrackingProperties();
				} else {
					if (!m_Query.Equals(query)) {
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
					.Do(items => m_Offset += (items.Count - 1)) // Cache the off-set
					.DelayFrameSubscription(2) // Delay some frames to improve performance
					.Subscribe(items => {
						if (items.Count == 0) {
							// If no results found
							if (!m_EndPreviousSearch) {
								// Auto search means continuing to provide more results of the previous search if possible (because we just provide 10 results at a time)
								// If the amount of items received is 0, it means no (or no more) result found.
								// Mark m_EndPreviousSearch as true to prevent auto search when scrolling
								m_EndPreviousSearch = true;
							}
						} else {
							UpdateView(items);
							// Cache data in case user go back from detail scence
							SceneTransitionData.query = m_Query;
							SceneTransitionData.currentSearchResults = m_Items;
						}
					})
					;
	}

	public override void ClearCells() {
		m_Items.Clear();
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

	public void UpdateView (List<Item> items) {
		m_Items.AddRange(items);
		m_TotalCount = m_Items.Count;

		if (m_NeedRefill) {
			// Fill the game scene with cells of items. Because the scroll list will reuse the cells, this should only be called once 
			RefillCells();
			m_NeedRefill = false;
		}
	}
}
