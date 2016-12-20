using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Menu : MonoBehaviour {

    public GameObject helpMenu;
    public GameObject demo;
    public Image level1;
    public Image level2;

    private int _maxLevel;

	void Start () {
        _maxLevel = PlayerPrefs.GetInt("max_level", 0);
        if (_maxLevel >= 1) {
            level2.color = level1.color;
        }
	}

    public void Play() {
        SceneManager.LoadSceneAsync ("Main");
    }

    public void Level2() {
        if (_maxLevel >= 1) {
            demo.SetActive(true);
            StartCoroutine(HideText());
        }
    }

    public void Help() {
        helpMenu.SetActive(true);
    }

    public void HideHelp() {
        helpMenu.SetActive(false);
    }

    private IEnumerator HideText() {
        yield return new WaitForSeconds(3);
        demo.SetActive(false);
    }

}
