using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class SearchResultsCell : MonoBehaviour 
{
	public Text text;
	public Button button;

	private Item m_Item;
    private SearchSceneController controller;    

	void Start () {
		if (text == null) {
			throw new Exception(this.GetType().Name + ": UI Text not found");
		}
		if (button == null) {
			throw new Exception(this.GetType().Name + ": UI Button not found");
		}

		controller = GameObject.FindObjectOfType<SearchSceneController>();        

		if (controller) {
            //button.onClick.AddListener(() => { print ("Hello"); controller.HandleOnItemClickedEvent(m_Item);});
            button.onClick.AddListener(() => HandleOnItemClickedEvent(m_Item));
		}
	}

	void ScrollCellContent (Item item) 
	{
		m_Item = item;

		if (text != null) 
		{
			text.text = item.title;
		}
	}

    public void HandleOnItemClickedEvent(Item item) {
        SceneTransitionData.currentItem = item;
        SceneTransitionData.query = controller.Query;
        controller.levelManager.LoadLevel("Detail");
    }
}
