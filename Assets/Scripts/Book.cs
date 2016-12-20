using UnityEngine;
using System.Collections;

public class Book : TargetableElement {

    public int correctPosition;
    public int actualPosition;

    private Rigidbody _rigidbody;

    void Start() {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void drop() {
        _rigidbody.isKinematic = false;
        _rigidbody.AddForce(3f, 0f, 0f, ForceMode.Impulse);
    }

}
