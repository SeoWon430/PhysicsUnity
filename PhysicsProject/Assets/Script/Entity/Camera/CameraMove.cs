using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public GameObject target;
    public Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        if(offset.magnitude == 0)
        offset =  this.transform.position - target.transform.position;

    }

    // Update is called once per frame
    void LateUpdate()
    {
        this.transform.position = offset + target.transform.position;
    }
}
