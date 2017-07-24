using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class SearchSceneController : MonoBehaviour {
	[SerializeField]
	private InputField m_SearchField;
	public InputField searchField { get { return m_SearchField; } set { m_SearchField = value; } }
	[SerializeField]
	private SearchResultsScrollView m_ScrollView;
	public SearchResultsScrollView searchResultsScrollView { get { return m_ScrollView; } set { m_ScrollView = value; } }

	private YleAPI m_API;
	private BehaviorSubject<string> m_SearchEvent;
	private bool m_EndPreviousSearch;
	// Store current search query to determine whether the next search is the same or different. Work with off-set
	private string m_Query;
	// Store off-set of the current search so that it won't start from the beginning in the next search (if the next search query is the same)
	private int m_Offset;

	// Use this for initialization
	void Start() {
		// Initialize variables
		m_API = new YleAPI();
		m_SearchEvent = new BehaviorSubject<string>(""); // Subject require instantiation with a default value
		if (m_SearchField == null) {
			throw new Exception(this.GetType().Name + ": Input field for search not found");
		}
		if (m_ScrollView == null) {
			throw new Exception(this.GetType().Name + ": SearchResultsScrollView for search not found");
		}

		ResetSearchTrackingProperties();
		BindEvents();
	}

	private void BindEvents() {
		m_SearchField
				.OnValueChangedAsObservable()
				.Throttle(TimeSpan.FromSeconds(1f))
				.Subscribe(query => {
					if (query.Length == 0) {
						m_ScrollView.ClearCells();
						ResetSearchTrackingProperties();
					} else {
						m_SearchEvent.OnNext(query);
					}
				 })
				.AddTo(this);

		m_ScrollView.scrollChangedHandler = HandleOnScrollEvent;

		m_SearchEvent
			.Skip(1) // Skip the default value
			.SelectMany(query => {
				if (!m_Query.Equals(query)) {
					// New query => New search
					ResetSearchTrackingProperties();
					m_Query = query;
				}
				return Observable.FromCoroutine<YleResponse>((observer, cancellationToken) => m_API.GetSth(observer, cancellationToken, query, offset: m_Offset));
			})
			.Select(ToItems) // Map YleResponse to Item (`Select` operator is the same as `Map` operator in Reactive Programming)
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
					m_ScrollView.UpdateView(items);
				}
			})
			.AddTo(this);
	}

	private void ResetSearchTrackingProperties () {
		m_Query = "";
		m_Offset = 0;
		m_EndPreviousSearch = false;
	}

	public void HandleOnScrollEvent (int totalCount, int firstVisibleItemIndex, int lastVisibleItemIndex) {
		print("itemCount = " + totalCount + ", start = " + firstVisibleItemIndex + ", end = " + lastVisibleItemIndex);
		if (!m_EndPreviousSearch) {
			if (totalCount > 0 && lastVisibleItemIndex + 3 >= totalCount) {
				print("Moreeeeeeeeeee");
				m_SearchEvent.OnNext(m_SearchEvent.Value);
			};
		};	
	}
		
	// Use this for mapping YleResponse to Item
	public List<Item> ToItems (YleResponse response) {
		List<Item> items = new List<Item>();
		for (int index = 0; index < response.data.Length; index++) {
			Item item = new Item();
			string title = (response.data[index].title.fi != null) ? response.data[index].title.fi : "No finnish title found";

			item.title = title;
			items.Add(item);
		}
		return items;
	}
}
