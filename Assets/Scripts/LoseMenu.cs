using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoseMenu : MonoBehaviour {
    
    public void TryAgain() {
        SceneManager.LoadSceneAsync ("Main");
    }

    public void Quit() {
        SceneManager.LoadSceneAsync ("Menu");
    }

}
