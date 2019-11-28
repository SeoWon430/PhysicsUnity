using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderControl : MonoBehaviour
{
    public RigidbodyCS rigid;
    private float speed;
    private Material mat;



    // Start is called before the first frame update
    void Start()
    {
        mat = this.GetComponent<Renderer>().material;
    }




    // Update is called once per frame
    void Update()
    {
        speed = rigid.velocity.magnitude/2;
        if(speed > 1)
        {

            if (Vector3.Dot(rigid.transform.forward, rigid.velocity) > 0)
                speed *= 1;
            else
                speed *= -1;
        }else
            speed = 0;

        mat.SetFloat("_ScrollXSpeed", speed);
    }

}
