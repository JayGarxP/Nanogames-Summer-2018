using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject player;


    private Vector3 offset;


	// Use this for initialization
	void Start () {
        offset = transform.position - player.transform.position; 
            }

    // Update is called once per frame
    //void Update () {
    //       transform.position = player.transform.position + offset;

    //}

    //Late Update is better for 'Follow Cameras' procederal animation, polling last known states etc. because
    // it runs after Update()
    private void LateUpdate()
    {
       transform.position = player.transform.position + offset;
    }
}