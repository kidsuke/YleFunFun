using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
	
	public void LoadLevel(string name){
		Debug.Log ("New Level load: " + name);
		Application.LoadLevel (name);
	}

	public void LoadNextLevel () {
		Application.LoadLevel (Application.loadedLevel + 1);
	}

	public void LoadLastLevel () {
		Application.LoadLevel(Application.loadedLevel - 1);
	}

}
