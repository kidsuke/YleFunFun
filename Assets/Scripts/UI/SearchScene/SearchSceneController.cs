using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class SearchSceneController : MonoBehaviour {
	
    private YleAPI m_API;

	void Awake () {
		// Initialize variables
		m_API = new YleAPI();
	}

	public IObservable<List<Item>> GetPrograms (string query = "", int offset = 0) {
		return Observable.FromCoroutine<YleResponse>((observer, cancellationToken) => m_API.GetPrograms(observer, cancellationToken, query, offset))
						 .Select(ToItems) ;// Map YleResponse to Item (`Select` operator is the same as `Map` operator in Reactive Programming)				 
	}
		
	// Use this for mapping YleResponse to Item
	public List<Item> ToItems (YleResponse response) {
		List<Item> items = new List<Item>();
		for (int index = 0; index < response.data.Length; index++) {
			Item item = new Item();

			item.id = response.data[index].id;
			item.imageId = response.data[index].image.id;
			item.title = (response.data[index].title.fi != null) ? response.data[index].title.fi : "No finnish title found";
			item.description = (response.data[index].description.fi !=null) ? response.data[index].description.fi : "No description found";
			items.Add(item);
		}
		return items;
	}
}
