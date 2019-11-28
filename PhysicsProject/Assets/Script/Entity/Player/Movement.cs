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
        COLLISION,
        JUMP
    }

    public RigidbodyCS rigid { get; private set; }
    public float acceleration = 30;
    public float rotSpeed=15;
    public bool isControll = true;
    public bool isOnRoad = false;
    public GameObject body;
    public GameObject skidMark;
    public Transform skidPosition;
    private GameObject skid;
    public ParticleSystem dustParticle;
    public ParticleSystem boostParticle;

    public State state = State.NONE;

    private float inputRotate;
    private float inputMove;
    private float inputDrift;
    private bool isBoost;

    private Quaternion initRot;
    private float turn =0;
    private float turnPrev = 0;

    private float rigidSpeed = 0;
    private float rigidSpeedPrev = 0;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponentInChildren<RigidbodyCS>();
        if (!rigid)
            isControll = false;

        if (dustParticle != null)
            dustParticle.gameObject.SetActive(false);

        initRot = body.transform.localRotation;
        Init();

    }

    void Init()
    {
        turn = 0;
        turnPrev = 0;

        rigidSpeed = 0;
        rigidSpeedPrev = 0;
        inputRotate = 0;
        inputMove = 0;
        inputDrift = 0;
        body.transform.localRotation = initRot;
        body.transform.localPosition = new Vector3(body.transform.localPosition.x, -0.35f, body.transform.localPosition.z);
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        rigidSpeed = rigid.velocity.magnitude;

        if (isControll)
        {
            InputKey();

            if (isOnRoad && rigidSpeed > 1f)
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
            else if (isOnRoad && rigidSpeed <= 0.1f)
            {
                state = State.STOP;
            }
            else if (!isOnRoad && rigidSpeed > 0.1f)
            {
                state = State.JUMP;
            }

            if (isOnRoad)
            {

                InputMove();
                if (inputRotate!= 0)
                    InputRotate();

            }
        }

        if (dustParticle != null)
        {

            if (rigidSpeed > 10f)
            {
                dustParticle.gameObject.SetActive(true);
                dustParticle.startSize = rigidSpeed / 20;
            }
            else
                dustParticle.gameObject.SetActive(false);
        }

        rigidSpeedPrev = rigidSpeed;

    }

    void LateUpdate()
    {
        if (rigidSpeed < 1f)
            body.transform.localRotation = new Quaternion(initRot.x, 0, 0, 1);
    }

    public void InputKey()
    {
        inputRotate = Input.GetAxis("Horizontal");
        if(inputRotate==0)
            turn = 0;

        inputMove = Input.GetAxis("Vertical");

        //Debug.Log(inputRotate + " / " +inputMove);

        if (rigidSpeed > 1f)
            inputDrift = Input.GetAxis("Drift");
        else
            inputDrift = 0;

        if (isBoost && Input.GetKeyDown(KeyCode.Space))
        {
            isBoost = false;
            StartCoroutine(Boost(rigid.velocity.normalized, acceleration*2 ));
            boostParticle.Play();
        }

    }



    void InputMove()
    {
        if (inputMove == 0) { 
            return;
        }

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
                if(Vector3.Dot( rigid.velocity.normalized, this.transform.forward) > 0.1f)
                    stateAccel = -0.25f;
                else
                    stateAccel = 0f;
                break;
            default:
                stateAccel = 1;
                break;
        }

        Vector3 speedForward = transform.forward * inputMove * acceleration * stateAccel ;
        rigid.AddForce(speedForward);


        //body.transform.RotateAround(this.transform.position, this.transform.right, );

    }


    void InputRotate()
    {
        if (inputRotate == 0)
        {
            turnPrev = 0;
            return;
        }

        //inputRotate -= inputDrift * 0.25f;
        turn = inputRotate * rotSpeed * Time.deltaTime;
        body.transform.RotateAround(body.transform.position, body.transform.forward, (turnPrev- turn)*10);

        this.transform.Rotate(new Vector3(0, turn, 0));
        float speed = rigidSpeed;
        Vector3 speedRight = transform.right * Mathf.Tan(turn * Mathf.Deg2Rad) * rigidSpeed / Time.deltaTime;
        rigid.AddForce(speedRight);
        //rigid.velocity = speed * rigid.velocity.normalized;

        //rigid.AddTorque(inputRotate * rotSpeed * rigid.transform.up);
        turnPrev = turn;
    }

    void SetSkidMark(bool set)
    {
        if (skid == null && set)
        {
            skid = Instantiate(skidMark);
            skid.transform.position = skidPosition.position;
            skid.transform.parent = this.transform;
        }
        else  if (skid != null && !set)
        {
            skid.transform.parent = null;
            skid = null;
            if(!isBoost)
                StartCoroutine(DriftBoost());
        }

    }


    IEnumerator DriftBoost()
    {
        isBoost = true;
        yield return new WaitForSeconds(0.5f);
        isBoost = false;
    }

    public IEnumerator Boost(Vector3 dir, float accel)
    {
        float max = rigid.maxSpeed;
        rigid.maxSpeed += accel ;
        rigid.AddForce(dir * accel, RigidbodyCS.ForceMode.Impulse);
        yield return new WaitForSeconds(1f);
        rigid.maxSpeed = max;
    }

    void OnCollisionEnterF(ColliderCS coll)
    {
        if (coll.CompareTag("Wall"))
        {
            Init();
            Debug.Log(initRot);
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
        else if (coll.CompareTag("Wall"))
        {
            Init();
            Debug.Log(initRot);
        }
    }

}
