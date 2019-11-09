using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyCS : MonoBehaviour
{
	public float mass=1;
	public bool useGravity=true;
	public bool isKinematic=false;
    public float maxSpeed = 100;

    public bool isMovable=true; 

    public enum Detection
    {
        Default,
        Continuous
    };
    public Detection collisionDetection = Detection.Default;

    //[HideInInspector]
	public Vector3 velocity;
	[HideInInspector]
	public Vector3 Acceleration;
	[HideInInspector]
	public BoxColliderCS colliderCS;

	//public List<GameObject> contactObjects;
	//public Dictionary<GameObject, Vector3> contactObjects;
	public Dictionary<BoxColliderCS, Vector3> contactObjects;
    public Dictionary<string, Vector3> forceDic;

	private void Awake()
	{
		velocity = Vector3.zero;
		if (mass <= 0)
			mass = 1;

        if (maxSpeed < 1)
            maxSpeed = 1;

        colliderCS = GetComponent<BoxColliderCS>();
		if (!colliderCS)
			colliderCS = this.gameObject.AddComponent<BoxColliderCS>();

		//contactObjects = new Dictionary<GameObject, Vector3>();
		contactObjects = new Dictionary<BoxColliderCS, Vector3>();

        isMovable = true;

    }


	void Start()
	{

	}

	void FixedUpdate()
	{

        if (useGravity)
		{
			AddForce(PhysicsManager.instance.gravity);
        }


        //Debug.DrawRay(this.transform.position, velocity, Color.red, 30);

        //Debug.Log(contactObjects.Values);
        //Debug.Log("3" + transform.position.y);
        //Debug.Log(contactObjects.Count);

    }
	private void LateUpdate()
	{
        if (velocity.magnitude > maxSpeed)
            velocity = velocity.normalized * maxSpeed;

        if (velocity.magnitude > 0.01f && isMovable)
        {
            VelocityMove();
        }
    }


    public void ForceSum()
    {

        if (contactObjects.Count > 0)
            foreach (var coll in contactObjects)
            {
                Vector3 dir = coll.Value;
                //Debug.Log(force);
                Vector3 verticalVelocity = new Vector3(velocity.x * Mathf.Abs(dir.x),
                                                velocity.y * Mathf.Abs(dir.y),
                                                velocity.z * Mathf.Abs(dir.z));
                //Debug.Log("@" + coll.Key .gameObject.name + " : "+ dir + " / " + velocity+" / " + verticalVelocity);

                verticalVelocity *= (1 + this.colliderCS.boundness + coll.Key.boundness);
                AddForceNormal(-verticalVelocity);
                Debug.DrawRay(this.transform.position, verticalVelocity, Color.green, 30);
            }

        isMovable = true;
    }

	private void VelocityMove()
	{
		this.transform.position += velocity * Time.deltaTime;
	}

	public void AddForce(Vector3 force)
	{
       // Debug.Log(force + " / "+velocity);
        /*
		Acceleration += force / mass;
		velocity += Acceleration * Time.deltaTime;
        */

        velocity += force / mass * Time.deltaTime;


        //힘을 받을떄 회전량 추가
    }

    public void AddForceNormal(Vector3 force)
    {
        velocity += force / mass;

    }




    void OnCollisionEnterF(BoxColliderCS coll)
    {
        //Debug.Log("Enter : " + coll.gameObject.name + "/"+contactObjects.Count);
    }
    void OnCollisionExitF(BoxColliderCS coll)
    {
        //Debug.Log("Exit : " + coll.gameObject.name + "/" + contactObjects.Count);
    }
    void OnCollisionStayF(BoxColliderCS coll)
    {
        //Debug.Log("Stay : " + coll.gameObject.name);
    }

}
