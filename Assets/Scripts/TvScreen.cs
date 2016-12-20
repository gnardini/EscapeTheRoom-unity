using UnityEngine;
using System.Collections;

public class TvScreen : TargetableElement {

    private float _shortMinFlickerSpeed = 0.1f;
    private float _shortMaxFlickerSpeed = 0.3f;

    private float _minFlickerSpeed = 1f;
    private float _maxFlickerSpeed = 1.5f;

    private Light light1;
    private Light light2;

    public GameObject child;

    void Start() {
        base.Start();
        child = transform.GetChild(0).gameObject;
    }

    private void startFlickering() {
        StartCoroutine(FilckerLights());
    }

    private IEnumerator FilckerLights() {
        child.SetActive(false);
        yield return new WaitForSeconds(getShortFlickerDelay());
        child.SetActive(true);
        yield return new WaitForSeconds(getShortFlickerDelay());
        child.SetActive(false);
        yield return new WaitForSeconds(getShortFlickerDelay());
        child.SetActive(true);
        yield return new WaitForSeconds(getShortFlickerDelay());
        child.SetActive(false);
        yield return new WaitForSeconds(getShortFlickerDelay());
        child.SetActive(true);
        yield return new WaitForSeconds(getFlickerDelay());
        StartCoroutine(FilckerLights());
    }

    private float getShortFlickerDelay() {
        return _shortMinFlickerSpeed + Random.value * (_shortMaxFlickerSpeed - _shortMinFlickerSpeed);
    }

    private float getFlickerDelay() {
        return _minFlickerSpeed + Random.value * (_maxFlickerSpeed - _minFlickerSpeed);
    }

    public void turnOnScreen() {
        child.gameObject.SetActive(true);
        startFlickering();
    }

    public override float clickRange() {
        return 7f;
    }

}
