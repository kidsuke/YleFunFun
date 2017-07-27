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

	public IObservable<List<Program>> GetPrograms (string query = "", int offset = 0) {
		return Observable.FromCoroutine<YleResponse>((observer, cancellationToken) => m_API.GetPrograms(observer, cancellationToken, query, offset))
						 .Select(ToPrograms) ;// Map YleResponse to Item (`Select` operator is the same as `Map` operator in Reactive Programming)				 
	}
		
	// Use this for mapping YleResponse to Item
	public List<Program> ToPrograms (YleResponse response) {
		List<Program> programs = new List<Program>();
		for (int index = 0; index < response.data.Length; index++) {
			Program program = new Program();

			program.id = response.data[index].id;
			program.imageId = response.data[index].image.id;
			program.title = (response.data[index].title.fi != null) ? response.data[index].title.fi : "No finnish title found";
			program.description = (response.data[index].description.fi !=null) ? response.data[index].description.fi : "No description found";
			programs.Add(program);
		}
		return programs;
	}
}
