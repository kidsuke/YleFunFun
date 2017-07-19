using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollList : MonoBehaviour {
    public List<Item> items;
    public GameObjectPool pool;
    public Transform content;
	// Use this for initialization
	void Start () {
        RefreshDisplay();
	}

    private void RefreshDisplay()
    {
        RemoveButtons();
        PopulateButtons();
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

    private void RemoveButtons()
    {
        while (content.childCount > 0)
        {
            GameObject toRemove = transform.GetChild(0).gameObject;
            pool.PutToStack(toRemove);
        }
    }
}
