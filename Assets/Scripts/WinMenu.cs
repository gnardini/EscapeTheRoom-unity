using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class WinMenu : MonoBehaviour {

    public GameObject _explanationText;

    public void NextLevel() {
        _explanationText.SetActive(true);
        StartCoroutine(HideText());
    }

    public void Quit() {
        SceneManager.LoadSceneAsync ("Menu");
    }

    private IEnumerator HideText() {
        yield return new WaitForSeconds(3);
        _explanationText.SetActive(false);
    }

}
