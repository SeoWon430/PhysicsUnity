using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public enum State
    {
        NONE,
        START,
        STOP,
        MOVE,
        DRIFT,
        COLLISION,
        JUMP
    }

    public RigidbodyCS rigid { get; private set; }
    public float acceleration = 30;
    public float rotSpeed=15;
    public bool isControll = false;
    public bool isOnRoad = false;
    public GameObject body;
    public GameObject skidMark;
    public Transform skidPosition;
    private GameObject skid;
    public ParticleSystem dustParticle;
    public ParticleSystem boostParticle;

    public State stateCurrent;

    private float inputRotate;
    private float inputMove;
    private float inputDrift;
    private bool isBoost;

    private Quaternion initRot;
    private float turn =0;
    private float turnPrev = 0;

    private float rigidSpeed = 0;


    public GameObject collParticle;
    private ParticleSystem[] particles;

    private AudioSource audioSource;
    public AudioClip clipRun;
    public AudioClip clipDrift;
    public AudioClip clipCollision;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponentInChildren<RigidbodyCS>();
        if(collParticle!= null)
            particles = collParticle.GetComponentsInChildren<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();

        if (!rigid)
            isControll = false;

        if (dustParticle != null)
            dustParticle.gameObject.SetActive(false);

        initRot = body.transform.localRotation;
        Init();

    }

    void Init()
    {
        stateCurrent = State.NONE;

        turn = 0;
        turnPrev = 0;

        rigidSpeed = 0;
        inputRotate = 0;
        inputMove = 0;
        inputDrift = 0;
        body.transform.localRotation = initRot;
        body.transform.localPosition = new Vector3(body.transform.localPosition.x, -0.35f, body.transform.localPosition.z);
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isControll)
            return;

        rigidSpeed = rigid.velocity.magnitude;

        InputKey();

        if (isOnRoad && rigidSpeed > 1f)
        {
            if (inputDrift > 0.1f)
            {
                ChangeState(State.DRIFT);
            }
            else
            {
                ChangeState(State.MOVE);
            }
        }
        else if (isOnRoad && rigidSpeed <= 0.1f)
        {
            ChangeState(State.STOP);
        }
        else if (!isOnRoad && rigidSpeed > 0.1f)
        {
            ChangeState(State.JUMP);
        }

        if (isOnRoad)
        {
            InputMove();
            if (inputRotate!= 0)
                InputRotate();
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


    }

    void LateUpdate()
    {
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

        if (Input.GetKeyDown(KeyCode.M))
        {
            rigid.AddForce(this.transform.up * 50, RigidbodyCS.ForceMode.Impulse);
        }

    }



    void InputMove()
    {
        if (inputMove == 0) { 
            return;
        }

        float stateAccel = 1;
        switch (stateCurrent)
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


        turn = inputRotate * rotSpeed * Time.deltaTime;
        /*
        //inputRotate -= inputDrift * 0.25f;
        body.transform.RotateAround(body.transform.position, body.transform.forward, (turnPrev- turn)*10);
        */

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
        Debug.Log(dir + " " + accel);
        float max = rigid.maxSpeed;
        rigid.maxSpeed += accel ;
        rigid.AddForce(dir * accel, RigidbodyCS.ForceMode.Impulse);
        yield return new WaitForSeconds(1f);
        rigid.maxSpeed = max;
    }



    void ChangeState(State state) 
    {
        switch (state)
        {
            case State.START:
                break;
            case State.STOP:
                break;
            case State.MOVE:
                SetSkidMark(false);
                SoundPlay(clipRun);
                break;
            case State.DRIFT:
                SetSkidMark(true);
                SoundPlay(clipDrift);
                break;
            case State.COLLISION:
                SoundPlay(clipCollision);
                break;
            case State.JUMP:
                break;
            default:
                break;

        }
        stateCurrent = state;
    }



    void SoundPlay(AudioClip stateSound)
    {
        if (audioSource.clip!=null && audioSource.clip.Equals(stateSound))
        {
            audioSource.loop = true;
        }
        else
        {
            audioSource.loop = false;
            audioSource.clip = stateSound;
            audioSource.Play();
        }


    }



    void OnCollisionEnterF(ColliderCS coll)
    {
        ChangeState(State.COLLISION);
        if (coll.CompareTag("Wall") || coll.CompareTag("Obstacle"))
        {
            Init();
            foreach (ParticleSystem particle in particles)
            {
                particle.transform.position = coll.contactPoint + new Vector3(0, 0.5f, 0);
                particle.Play();
            }
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
        }
    }

}
