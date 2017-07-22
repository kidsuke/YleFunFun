using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecycleList : MonoBehaviour {
	private enum ScrollOrientation
	{
		HORIZONTAL,
		VERTICAL
	}

	private enum ScrollDirection
	{
		NEXT,
		PREVIOUS
	}

	[SerializeField] private RectTransform content;
	[SerializeField] private RectTransform viewport;
	[SerializeField] private ScrollRect scrollRect;
	[SerializeField] private ScrollOrientation scrollOrientation;
	[SerializeField] private bool centerOnItem;
	[SerializeField] private GameObject prefab;
	[SerializeField] public List<Item> items;
	private int maxVisibleItems;
	private float itemSize;
	private List<GameObject> gameObjectPool;
	//Store inactive instances of the prefab into a collection
	private Stack<GameObject> inactiveInstances = new Stack<GameObject>();


	private float _itemSize;
	private float lastAnchoredPosition;
	private int firstVisibleItemIndex;
	private int lastVisibleItemIndex;

	private int _itemsTotal;
	private int _itemsVisible;

	private int _itemsToRecycleBefore;
	private int _itemsToRecycleAfter;

	private int _currentItemIndex;
	private int _lastItemIndex;


	private int mThreshhold = 10;
	private List<GameObject> mCacheList;
	private List<GameObject> mDisplayedList;
	private List<GameObject> mPreparedList;

	private void init() {
		for (int i = 0; i < mThreshhold; i++) {
			GameObject gameObject = Instantiate(prefab) as GameObject;
			gameObject.transform.SetParent(content);
			mDisplayedList.Add(gameObject);
		}
	}





	private int mVisibleItems;
	private int mItemsToInstantiate;

	public RecycleList(GameObject prefab) { }

	void Start() {
		Initialize();
	}
	void Update() {
		//OnScrollValueChanged(new Vector2(0, 0));
	}

	private void Initialize()
	{
		//ForceUpdateCanvases to get the correct size of the view port
		Canvas.ForceUpdateCanvases();

		ToggleScrollOrientation();

		//Calculate maximum items that can be displayed
		LayoutElement layoutElement = prefab.GetComponent<LayoutElement>();
		itemSize = layoutElement.minHeight;
		float viewportHeight = viewport.rect.size.y;
		float max = viewportHeight / itemSize;
		mVisibleItems = (int) Mathf.Ceil(max);
		//Instatntiate object pool
		mItemsToInstantiate = mVisibleItems + 2;

		gameObjectPool = new List<GameObject>();
		for (int index = 0; index < mItemsToInstantiate; index++)
		{
			GameObject gameObject = Instantiate(prefab) as GameObject;
			gameObject.transform.SetParent(content);
			gameObjectPool.Add(gameObject);
		}

		scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
		PopulateItems();
	}

	public void ToggleScrollOrientation()
	{
		switch (scrollOrientation)
		{
		case ScrollOrientation.HORIZONTAL:
			scrollRect.horizontal = true;
			scrollRect.vertical = false;
			break;
		case ScrollOrientation.VERTICAL:
			scrollRect.vertical = true;
			scrollRect.horizontal = false;
			break;
		default:
			break;
		}
	}

	public void OnScrollValueChanged(Vector2 position)
	{
		if (lastAnchoredPosition <= 0)
		{
			//If lastAnchoredPosition <= 0, scrolling has not begun yet
			lastAnchoredPosition = GetContentAnchoredPosition();
			return;
		}

		//Scrolling has begun
		float currentAnchoredPosition = GetContentAnchoredPosition();
		float scrollDelta = Mathf.Abs(currentAnchoredPosition - lastAnchoredPosition);
		int replacedRows = Mathf.FloorToInt(scrollDelta / itemSize);

		if (replacedRows == 0)
		{
			//Scrolling has not exceeded a row
			return;
		}

		//If scrolling has exceeded a row, populate that row with new data
		ScrollDirection scrollDirection = GetScrollDirection();
		for (int i = 0; i < replacedRows; i++)
		{
			RecycleItem(scrollDirection);
		}
		//Finally, update lastAnchoredPosition again
		lastAnchoredPosition = currentAnchoredPosition;

	}

	private void RecycleItem(ScrollDirection scrollDirection)
	{
		print("first: " + firstVisibleItemIndex + ", last: " + lastVisibleItemIndex) ;
		if (scrollDirection == ScrollDirection.NEXT)
		{
			if (lastVisibleItemIndex < items.Count - 1)
			{
				lastVisibleItemIndex++;
				firstVisibleItemIndex++;

			}
		}
		else
		{
			if (firstVisibleItemIndex > 0)
			{
				firstVisibleItemIndex--;
				lastVisibleItemIndex--;

			}
		}

	}

	private void PopulateItems()
	{
		//Get number of items should be displayed.
		//If maxVisibleItems is less than total number of items, only display [maxVisibleItems] items
		//Else display all items
		int visibleItems = maxVisibleItems < items.Count ? maxVisibleItems : items.Count;
		firstVisibleItemIndex = 0;
		lastVisibleItemIndex = visibleItems - 2;
		//Binding data
		for (int index = 0; index < gameObjectPool.Count; index++)
		{
			GameObject gameObject = gameObjectPool[index];


			SuggestButton button = gameObject.GetComponent<SuggestButton>();
			button.title.text = items[index].title;
		}
	}

	public float GetContentAnchoredPosition()
	{
		switch (scrollOrientation)
		{
		case ScrollOrientation.HORIZONTAL:
			return content.anchoredPosition.x;

		case ScrollOrientation.VERTICAL:
			return content.anchoredPosition.y;

		default:
			return 0;
		}
	}

	private ScrollDirection GetScrollDirection()
	{
		switch (scrollOrientation)
		{
		case ScrollOrientation.HORIZONTAL:
			return lastAnchoredPosition < GetContentAnchoredPosition() ? 
				ScrollDirection.PREVIOUS : ScrollDirection.NEXT;
		case ScrollOrientation.VERTICAL:
			return lastAnchoredPosition > GetContentAnchoredPosition() ? 
				ScrollDirection.PREVIOUS : ScrollDirection.NEXT;
		default:
			return ScrollDirection.NEXT;
		}
	}
}
