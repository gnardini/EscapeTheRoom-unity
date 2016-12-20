using UnityEngine;
using System.Collections;

public class Key : TargetableElement {

    private bool alreadyClicked;
void Start()
    {
        base.Start();
        alreadyClicked = false;
    }


    void Update()
    {
        if (alreadyClicked)
            return;
        transform.Rotate(new Vector3(0f, (90f * Time.deltaTime), 0f));
    }

    public void onClicked()
    {
        base.onClicked();
        alreadyClicked = true;
    }
}
