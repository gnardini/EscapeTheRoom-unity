using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Menu : MonoBehaviour {

	void Start () {
	
	}

    public void Play() {
        SceneManager.LoadSceneAsync ("Main");
    }

    public void Help() {
        
    }

}
