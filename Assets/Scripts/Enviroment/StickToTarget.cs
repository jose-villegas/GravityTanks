using UnityEngine;
using System.Collections;

public class StickToTarget : MonoBehaviour {

    public GameObject target;


    PlayerMovement pMovement;

    void Awake()
    {
        pMovement = target.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = target.transform.position;
        transform.up = transform.position - pMovement.gravityPuller.transform.position;
    }
}
