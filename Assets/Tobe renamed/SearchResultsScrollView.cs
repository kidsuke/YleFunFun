using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class SearchResultsScrollView : LoopVerticalScrollRect {
	public List<Item> m_Items;
	private bool m_EndSearch;
	private bool m_FirstInvocation;
	private SearchSceneController m_Controller;
	private LoopScrollDataSource m_DataSource {
		get {
			return dataSource;
		}
		set {
			dataSource = value;
		}
	}
	private LoopScrollPrefabSource m_PrefabSource {
		get {
			return prefabSource;
		}
	}
	private int m_TotalCount {
		get {
			return totalCount;
		}
		set {
			totalCount = value;
		}
	}

	// Use this for initialization
	void Start () {
		m_Items = new List<Item>();
		m_EndSearch = false;
		m_FirstInvocation = true;
		m_Controller = new SearchSceneController();
		m_DataSource = new LoopScrollListSource<Item>(m_Items);
		m_PrefabSource.InitPool();

		onValueChanged.AddListener(position => {
			print("itemCount = " + totalCount + ", start = " + itemTypeStart + ", end = " + itemTypeEnd);
			if (!m_EndSearch) {
				
				if (totalCount > 0 && itemTypeEnd + 3 >= totalCount) {
					print("Moreeeeeeeeeee");
					SearchForPrograms();
				};
			};

		});

		SearchForPrograms();
	}

	protected void SearchForPrograms () {
		m_Controller.SearchForPrograms()
					.ObserveOnMainThread() // Make sure that UI is updated on the main thread
					.DelayFrameSubscription(2) // Make few frames delay to increase performance
					.Subscribe(UpdateView) // Update UI with new data
					.AddTo(this); // This line guarantees that the subscription is disposed when the game object is destroyed
	}

	public void UpdateView (List<Item> items) {
		// NOTE: auto search means continuing to provide more results of the previous search if possible (because we just provide 10 results at a time)
		if (items.Count <= 0) {
			// If the amount of items received is 0, it means no (or no more) result found.
			// Mark m_EndSearch as true to prevent auto search when scrolling
			m_EndSearch = true;
		} else {
			// If the amount of items received is larger than 0, it means results found.
			// Mark m_EndSearch as false to provide auto search when scrolling
			m_EndSearch = false;
		}

		m_Items.AddRange(items);
		m_TotalCount = m_Items.Count;

		if (m_FirstInvocation) {
			// Fill the game scene with cells of items. Because the scroll list will reuse the cells, this should only be called once 
			RefillCells();
			m_FirstInvocation = false;
		}
	}
}
