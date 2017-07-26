using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
	
	public void LoadLevel(string name){
		Debug.Log ("New Level load: " + name);		
        SceneManager.LoadScene(name);
	}

	public void LoadNextLevel () {		
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

	public void LoadLastLevel () {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex -1);
    }

}
