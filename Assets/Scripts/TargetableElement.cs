using UnityEngine;
using System;

public abstract class TargetableElement : MonoBehaviour {

    private CameraView cameraView;

    public void Start() {
        cameraView = GameObject.Find("Main Camera").GetComponent<CameraView>();
    }

    public void onGazeStarted() {
        cameraView.onElementGaze(this);
    }

    public void onGazeStopped() {
        cameraView.onElementGazeFinished();
    }

    public void onClicked() {
        //cameraView.onElementClicked(this);
    }

    public virtual float clickRange() {
        return 3f;
    }

}
