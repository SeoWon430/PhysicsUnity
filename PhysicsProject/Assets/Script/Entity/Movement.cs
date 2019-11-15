using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public enum State
    {
        NONE,
        STOP,
        MOVE,
        DRIFT,
        JUMP
    }

    private RigidbodyCS rigid;
    public float acceleration = 10;
    public float rotSpeed=30;
    public bool isControll = true;
    public bool isOnRoad = false;
    public GameObject skidMark;
    private GameObject skid;
    public State state = State.NONE;

    float inputRotate;
    float inputMove;
    float inputDrift;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponentInChildren<RigidbodyCS>();
        if (!rigid)
            isControll = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isControll)
        {
            InputKey();

            if (isOnRoad && rigid.velocity.magnitude > 0.1f)
            {
                if (inputDrift > 0.1f)
                {
                    state = State.DRIFT;
                    SetSkidMark(true);
                }
                else
                {
                    state = State.MOVE;
                    SetSkidMark(false);
                }
            }
            else if (isOnRoad && rigid.velocity.magnitude <= 0.1f)
            {
                state = State.STOP;
            }
            else if (!isOnRoad && rigid.velocity.magnitude > 0.1f)
            {
                state = State.JUMP;
            }

            InputMove();
        }

    }

    void LateUpdate()
    {
        if(state != State.DRIFT && skid !=null)
        {
        }
    }

    public void InputKey()
    {
        inputRotate = Input.GetAxis("Horizontal");
        inputMove = Input.GetAxis("Vertical");
        inputDrift = Input.GetAxis("Drift");
    }



    void InputMove()
    {
        float stateAccel = 1;
        switch (state)
        {
            case State.STOP:
                stateAccel = 1;
                break;
            case State.MOVE:
                stateAccel = 1;
                break;
            case State.DRIFT:
                stateAccel = 0;
                break;
            default:
                stateAccel = 1;
                break;
        }

        rigid.AddForce(transform.forward * inputMove * acceleration * stateAccel);
        float turn = inputRotate * rotSpeed * Time.deltaTime;
        this.transform.rotation *= Quaternion.Euler(0, turn, 0);

        rigid.velocity = Vector3.Project(rigid.velocity, this.transform.forward);
    }


    void SetSkidMark(bool set)
    {
        if (skid == null && set)
        {
            skid = Instantiate(skidMark, this.transform.position, Quaternion.identity);
            skid.transform.parent = this.transform;
        }
        else  if (skid != null && !set)
        {
            skid.transform.parent = null;
            skid = null;
        }
    }

    void OnCollisionStayF(ColliderCS coll)
    {
        if (coll.CompareTag("Ground"))
        {
            isOnRoad = true;
        }
    }

    void OnCollisionExitF(ColliderCS coll)
    {
        if (coll.CompareTag("Ground"))
        {
            isOnRoad = false;
        }
    }

}
