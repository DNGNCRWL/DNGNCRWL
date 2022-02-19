using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboarder : MonoBehaviour
{
    Transform camTransform;

    void Awake(){
        if(!camTransform)
            camTransform = Camera.main.transform;
    }

    void Update(){
        if(camTransform)
            transform.LookAt(new Vector3(camTransform.position.x, transform.position.y, camTransform.position.z));
    }
}
