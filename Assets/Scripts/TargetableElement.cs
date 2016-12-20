using UnityEngine;
using System;

public abstract class TargetableElement : MonoBehaviour {

    public virtual void onClicked() {
    }

    public virtual float clickRange() {
        return 3f;
    }

}
