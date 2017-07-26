using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class TestCell : MonoBehaviour 
{
	public Text text;
	public Button button;

	private Item m_Item;

	void Start () {
		if (text == null) {
			throw new Exception(this.GetType().Name + ": UI Text not found");
		}
		if (button == null) {
			throw new Exception(this.GetType().Name + ": UI Button not found");
		}

		SearchSceneController controller = GameObject.FindObjectOfType<SearchSceneController>();
		if (controller) {
			button.onClick.AddListener(() => { print ("Hello"); controller.HandleOnItemClickedEvent(m_Item);});
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
}
