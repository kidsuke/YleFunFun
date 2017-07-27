using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class SearchResultsCell : MonoBehaviour {
	
	[SerializeField]
	private Text m_Text;
	public Text text { get { return m_Text; } set { m_Text = value; } }
	[SerializeField]
	private Button m_Button; 
	public Button button { get { return m_Button; } set { m_Button = value; } }

	private Item m_Item;
	private LevelManager m_LevelManager;

	void Awake () {
		m_LevelManager = GameObject.FindObjectOfType<LevelManager>();
	}

	void Start () {
		if (m_Text == null) {
			throw new Exception(this.GetType().Name + ": UI Text not found");
		}
		if (m_Button == null) {
			throw new Exception(this.GetType().Name + ": UI Button not found");
		}
			
		m_Button.onClick.AddListener(HandleOnItemClickedEvent);
	}

	void ScrollCellContent (Item item) 
	{
		m_Item = item;
		m_Text.text = item.title;
	}

	public void HandleOnItemClickedEvent () {
		SceneTransitionData.currentItem = m_Item;
		m_LevelManager.LoadLevel("Detail");
    }
}
