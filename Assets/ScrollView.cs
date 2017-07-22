using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollView : MonoBehaviour {
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

	private int first = 0;
	private int last = 5;
	private bool redrawed = false;
	private Vector2 defaultPos;

    public ScrollView(GameObject prefab) { }

    void Start() {
        Initialize();
    }

	void Update() {
		//OnScrollValueChanged(new Vector2(0, 0));
		//print(position);
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

		if (replacedRows < 1)
		{
			//Scrolling has not exceeded a row
			return;
		}

		ScrollDirection scrollDirection = GetScrollDirection();
		if (scrollDirection == ScrollDirection.NEXT) {
			first ++;
			last ++;
		} else {
			first--;
			last--;
		}
		redraw();
		content.position = defaultPos;


		
		print(replacedRows);
		//GameObject gameObject = Instantiate(prefab) as GameObject;
		//gameObject.transform.SetParent(content);
		//gameObject.transform.SetAsFirstSibling();

		//If scrolling has exceeded a row, populate that row with new data
		//ScrollDirection scrollDirection = GetScrollDirection();
		//for (int i = 0; i < replacedRows; i++)
		//{
		//RecycleItem(scrollDirection);
		/*}*/
		//Finally, update lastAnchoredPosition again
		lastAnchoredPosition = currentAnchoredPosition;
	}

    private void Initialize()
    {
        //ForceUpdateCanvases to get the correct size of the view port
        Canvas.ForceUpdateCanvases();
	
        //Calculate maximum items that can be displayed
        LayoutElement layoutElement = prefab.GetComponent<LayoutElement>();
        itemSize = layoutElement.minHeight;
        float viewportHeight = viewport.rect.size.y;
        float max = viewportHeight / itemSize;
        //Round up to get an integer value and improve performance later on
        maxVisibleItems = (int) Mathf.Ceil(max);
		//maxVisibleItems += 3;

        gameObjectPool = new List<GameObject>();


        //scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
		first = 0;
		last = 5;
		redrawed = false;
		defaultPos = content.position;
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
		//print(position);
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
	
        if (replacedRows <= 0)
        {
            //Scrolling has not exceeded a row
            return;
        }
		ScrollDirection scrollDirection = GetScrollDirection();
		if (scrollDirection == ScrollDirection.NEXT) {
			first ++;
			last ++;
		} else {
			first--;
			last--;
		}
		redraw();
		print(replacedRows);
		//GameObject gameObject = Instantiate(prefab) as GameObject;
		//gameObject.transform.SetParent(content);
		//gameObject.transform.SetAsFirstSibling();

        //If scrolling has exceeded a row, populate that row with new data
        //ScrollDirection scrollDirection = GetScrollDirection();
        //for (int i = 0; i < replacedRows; i++)
        //{
            //RecycleItem(scrollDirection);
        /*}*/
        //Finally, update lastAnchoredPosition again
        lastAnchoredPosition = currentAnchoredPosition;
    }

	void redraw() {
		foreach(Transform child in content.transform) {
			Destroy(child.gameObject);
		}
		PopulateItems();
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

                GameObject gameObject = gameObjectPool[0];
				//gameObject.transform.SetAsLastSibling();
				//scrollRect.
				//gameObject.transform.localPosition = content.GetChild(content.childCount - 1).position;
                //SuggestButton button = gameObject.GetComponent<SuggestButton>();
				//button.title.text = items[lastVisibleItemIndex].title;;
            }
        }
        else
        {
            if (firstVisibleItemIndex > 0)
            {
                firstVisibleItemIndex--;
				lastVisibleItemIndex--;

                GameObject gameObject = gameObjectPool[gameObjectPool.Count - 1];
				gameObject.transform.SetAsFirstSibling();

                SuggestButton button = gameObject.GetComponent<SuggestButton>();
                button.title.text = items[firstVisibleItemIndex].title;
            }
        }
        
    }

    private void PopulateItems()
    {
		int s = first;
		int s2 = last;
		for (int index = first; index <= last + 1; index++)
		{
			GameObject gameObject = Instantiate(prefab) as GameObject;
			gameObject.transform.SetParent(content);

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
