using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class SearchResultsScrollView : LoopVerticalScrollRect {
	public List<Item> m_Items;
	private bool m_EndSearch;
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

	// Use this for initialization
	void Start () {
		m_Items = new List<Item>();
		m_EndSearch = false;
		m_Controller = new SearchSceneController();
		m_DataSource = new LoopScrollListSource<Item>(m_Items);
		m_PrefabSource.InitPool();

//		onValueChanged.AddListener(position => {
//			if (!m_EndSearch) {
//				if (itemTypeEnd + 3 >= totalCount) {
//					SearchForPrograms();
//				};
//			};
//			print(position);
//		});
	}

	protected void SearchForPrograms () {
		m_Controller.SearchForPrograms()
			.ObserveOnMainThread() // Make sure that UI is updated on the main thread
			.Subscribe(UpdateView) // Update UI with new data
			.AddTo(this); // This line guarantees that the subscription is disposed when the game object is destroyed
	}

	public void UpdateView (List<Item> items) {
		m_Items.AddRange(items);
		totalCount = m_Items.Count;
		RefillCells();
	}
}
