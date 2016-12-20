using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : MonoBehaviour {

    private CameraView _cameraView;

	void Start () {
        _cameraView = GameObject.Find("Main Camera").GetComponent<CameraView>();
	}

    public void Resume() {
        _cameraView.SetPauseActive(false);
    }

    public void Restart() {
        SceneManager.LoadSceneAsync ("Main");
    }

    public void Quit() {
        SceneManager.LoadSceneAsync ("Menu");
    }

}
