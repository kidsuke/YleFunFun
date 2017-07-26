using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class SearchResultsScrollView : LoopVerticalScrollRect {
	
	private List<Item> m_Items;
	public List<Item> items { get { return m_Items; } }

	private bool m_NeedRefill;
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
		onValueChanged.AddListener(position => scrollChangedHandler(m_TotalCount, m_FirstVisibleItemIndex, m_LastVisibleItemIndex));
	}

	public override void ClearCells() {
		m_Items.Clear();
		m_TotalCount = 0;
		m_NeedRefill = true;
		base.ClearCells();
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
