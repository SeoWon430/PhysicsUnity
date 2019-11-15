using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buster : MonoBehaviour
{
    public enum Direction
    {
        NONE,
        RIGHT,
        LEFT,
        UP,
        DOWN,
        FORWARD,
        BACK
    }
    public Direction busterDirection = Direction.NONE;
    public float busterForce;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnterF(ColliderCS coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            RigidbodyCS rigid= coll.gameObject.GetComponent<RigidbodyCS>();
            if(rigid != null)
            {
                Vector3 dir = Vector3.zero;
                switch (busterDirection)
                {
                    case Direction.RIGHT:
                        dir = this.transform.right;
                        break;
                    case Direction.LEFT:
                        dir = -this.transform.right;
                        break;
                    case Direction.UP:
                        dir = this.transform.up;
                        break;
                    case Direction.DOWN:
                        dir = -this.transform.up;
                        break;
                    case Direction.FORWARD:
                        dir = this.transform.forward;
                        break;
                    case Direction.BACK:
                        dir = -this.transform.forward;
                        break;
                }
                //Debug.Log(dir);
                rigid.AddForce( busterForce * dir);
            }

        }

        //c =StartCoroutine(RotateForce(coll.transform.eulerAngles, coll.contactPoint));
        //Log("Enter : " + coll.gameObject.name + "/" + contactObjects.Count);
    }
}
