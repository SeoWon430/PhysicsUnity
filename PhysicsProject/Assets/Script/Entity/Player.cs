using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private RigidbodyCS rigid;
    public float moveSpeed=10;


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
            rigid.AddForce(transform.right * moveX * moveSpeed);
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
