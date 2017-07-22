using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollList : MonoBehaviour {
 
    public List<Item> items;
    public GameObjectPool pool;
    public RectTransform content;
    public RectTransform viewport;
    public GameObject prefab;
    public ScrollRect scrollRect;

    private int maxVisibleItems;

    private void Initialize() 
    {
        //ForceUpdateCanvases to get the correct size of the view port
        Canvas.ForceUpdateCanvases();
        //Calculate maximum items that can be displayed
        LayoutElement layoutElement = prefab.GetComponent<LayoutElement>();
        float itemHeight = layoutElement.minHeight;
        float viewportHeight = viewport.rect.size.y;
        float max = viewportHeight / itemHeight;
        //Round up to get an integer value and improve performance later on
        maxVisibleItems = (int) Mathf.Ceil(max);

        scrollRect.onValueChanged.AddListener((Vector2 change) => {
            print(content.anchoredPosition.y);
        });
    }
   
	// Use this for initialization
	void Start () 
    {
        Initialize();
        RefreshDisplay();
        print(viewport.rect.size);
    }

    void Update()
    {

    }

    void OnDragEnd()
    {
        print("hellooooooooooooo");
    }

    private void RefreshDisplay() 
    {
        RemoveButtons();
        PopulateScrollList();
    }

    private void PopulateButtons() 
    {
        foreach (Item item in items) {
            GameObject gameObject = pool.GetGameObject();
            gameObject.transform.SetParent(content);

            SuggestButton suggestButton = gameObject.GetComponent<SuggestButton>();
            suggestButton.Bind(item);
        }
    }

    private void PopulateScrollList()
    {
        //Get number of items should be displayed.
        //If maxVisibleItems is less than total number of items, only display [maxVisibleItems] items
        //Else display all items
        int visibleItems = maxVisibleItems < items.Count ? maxVisibleItems : items.Count;
        //Binding data
        for (int index = 0; index < items.Count; index++)
        {
            GameObject gameObject = pool.GetGameObject();
            gameObject.transform.SetParent(content);

            SuggestButton button = gameObject.GetComponent<SuggestButton>();
            button.title.text = items[index].title;
        }
    }

    private void RemoveButtons() 
    {
        while (content.childCount > 0)
        {
            GameObject toRemove = transform.GetChild(0).gameObject;
            pool.PutToStack(toRemove);
        }
    }
}
