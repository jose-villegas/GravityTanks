using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletLife : MonoBehaviour {
    public LayerMask exceptions;

    void OnTriggerEnter(Collider other)
    {
        if ((1 << other.gameObject.layer & exceptions) > 0) return;

        Destroy(gameObject);
    }
}
