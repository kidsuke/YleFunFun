using System.Collections;
using System.Collections.Generic;
using UniRx;

public class SearchSceneController {
	private YleAPI m_API;
	// Store current search query to determine whether the next search is the same or different. Work with off-set
	private string m_Query;
	// Store off-set of the current search so that it won't start from the beginning in the next search (if the next search query is the same)
	private int m_Offset;

	public SearchSceneController () {
		m_API = new YleAPI();
		m_Query = "";
		m_Offset = 0;
	}
		
	public IObservable<List<Item>> SearchForPrograms (string query = "") {
		if (!m_Query.Equals(query)) {
			m_Query = query;
			m_Offset = 0;
		}

		return Observable.FromCoroutine<YleResponse>((observer, cancellationToken) => m_API.GetSth(observer, cancellationToken, query, offset: m_Offset))
						 .Select(ToItems) // Map YleResponse to Item
						 .Do(items => m_Offset += (items.Count - 1)); // Cache the off-set	  		 
	}

	// Use this for mapping YleResponse to Item
	public List<Item> ToItems (YleResponse response) {
		List<Item> items = new List<Item>();
		for (int index = 0; index < response.data.Length; index++) {
			Item item = new Item();
			string title = (response.data[index].title.fi != null) ? response.data[index].title.fi : "No finnish title found";

			item.title = title;
			items.Add(item);
		}
		return items;
	}
}
