using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private RigidbodyCS rigid;
    public float moveSpeed=10;
    public float rotSpeed=30;


    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<RigidbodyCS>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        InputMove();


    }

    public void InputMove()
    {

        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        if (moveX != 0)
        {
            this.transform.RotateAround(this.transform.position, this.transform.up, rotSpeed *moveX  * Time.deltaTime);
        }
        if (moveY != 0)
        {
            rigid.AddForce(transform.forward * moveY * moveSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rigid.AddForce(transform.up * 300);
        }
    }
}
