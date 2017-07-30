using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

	[SerializeField]
	private float m_AutoLoadDuration;
	public float autoLoadDuration { get { return m_AutoLoadDuration; } set { m_AutoLoadDuration = value; }}

	void Start () {
		if (m_AutoLoadDuration != 0) {
			Invoke("LoadNextLevel", m_AutoLoadDuration);
		}
	}

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
