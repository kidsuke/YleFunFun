using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class SearchResultsCell : MonoBehaviour 
{
	public Text text;
	public Button button;

	private Item m_Item;
	private SearchSceneController m_Controller;    

	void Start () {
		if (text == null) {
			throw new Exception(this.GetType().Name + ": UI Text not found");
		}
		if (button == null) {
			throw new Exception(this.GetType().Name + ": UI Button not found");
		}

		m_Controller = GameObject.FindObjectOfType<SearchSceneController>();        

		if (m_Controller) {
            //button.onClick.AddListener(() => { print ("Hello"); controller.HandleOnItemClickedEvent(m_Item);});
            button.onClick.AddListener(() => HandleOnItemClickedEvent());
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

	public void HandleOnItemClickedEvent () {
		SceneTransitionData.currentItem = m_Item;
        m_Controller.levelManager.LoadLevel("Detail");
    }
}
