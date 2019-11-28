using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    Rigidbody r;
    // Start is called before the first frame update
    void Start()
    {
        //r = GetComponent<Rigidbody>();
        //r.AddForce(this.transform.up*10, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnCollisionEnterF(ColliderCS coll)
    {
        Debug.DrawRay(this.transform.position, this.GetComponent<BoxColliderCS>().contactNormal * 10, Color.red, 100);
        
    }
}
