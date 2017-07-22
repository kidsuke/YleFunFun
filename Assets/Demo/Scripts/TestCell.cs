using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TestCell : MonoBehaviour 
{
	
	public Text text;

	void ScrollCellContent (Item idx) 
	{
		string name = "Cell " + idx.title;
		if (text != null) 
		{
			text.text = name;
		}
		gameObject.name = name;
	}
}
