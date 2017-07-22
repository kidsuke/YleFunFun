using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuggestButton : MonoBehaviour {
	public Text title;

	// Use this for initialization
	void Start () {
    
	}

    public void Bind(Item item)
    {
        title.text = item.title;
    }
}
