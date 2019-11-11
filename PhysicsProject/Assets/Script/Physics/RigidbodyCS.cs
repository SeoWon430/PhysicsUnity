using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyCS : MonoBehaviour
{
	public float mass=1;
	public bool useGravity=true;
	public bool isKinematic=false;
    public float maxSpeed = 100;
    Coroutine c;
    bool test = false;

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


    }


	void Start()
	{

	}

	void FixedUpdate()
	{


    }
	private void LateUpdate()
	{
        //Debug.Log(contactObjects.Count);
    }

    

    public void MovementForce()
    {
        if (contactObjects.Count > 0)
            //Debug.Log("ADSG");
        foreach (var coll in contactObjects)
        {
            Vector3 dir = coll.Value.normalized;
            Vector3 normalForce = new Vector3(velocity.x * Mathf.Abs(dir.x), velocity.y * Mathf.Abs(dir.y), velocity.z * Mathf.Abs(dir.z));
            //Vector3 normalForce = coll.Value;


            //normalForce = Vector3.Scale(velocity, dir);
            //normalForce = dir * velocity.magnitude;
            //Debug.Log("@" + coll.Key .gameObject.name + " : "+ dir + " / " + velocity+" / " + verticalVelocity);

            //normalForce = -dir * velocity.magnitude * Mathf.Cos(coll.Key.transform.eulerAngles.x * Mathf.Deg2Rad);
            //Debug.Log(-normalForce + " / " + dir + " / " + Mathf.Cos(coll.Key.transform.eulerAngles.x * Mathf.Deg2Rad));


            float frictionForce = (coll.Key.fricion + colliderCS.fricion) * normalForce.magnitude;

            normalForce *= (1 + this.colliderCS.boundness + coll.Key.boundness);

            //normalForce += new Vector3(Mathf.Tan(coll.Key.transform.eulerAngles.z * Mathf.Deg2Rad), 0, -Mathf.Tan(coll.Key.transform.eulerAngles.x * Mathf.Deg2Rad));
            //normalForce = -dir * normalForce.magnitude * Mathf.Cos(coll.Key.transform.eulerAngles.x * Mathf.Deg2Rad);
            //if(coll.Key.name == "Cube")

            /*
            Vector3 project = Vector3.Project(normalForce, dir).normalized;
            float angle = Vector3.Angle(normalForce, project);
            normalForce = project * normalForce.magnitude * Mathf.Cos(angle * Mathf.Deg2Rad);
            */
            //Debug.Log(1/Mathf.Cos(angle * Mathf.Deg2Rad));
            float project = Vector3.Project(dir, normalForce).magnitude;
            normalForce = Vector3.Project(normalForce, dir)* project;

            //Debug.Log(dir.x + " / " + dir.y + " / " + dir.z);
            //Debug.Log(normalForce.magnitude +" / " + velocity.y);
            //Debug.DrawRay(this.transform.position, dir * 10, Color.red, 30);
            //Debug.DrawRay(this.transform.position, -normalForce * 10, Color.green, 30);




            //float project = Vector3.Project(dir, normalForce).magnitude;
            //Debug.Log(project);
            //normalForce = -dir  * normalForce.magnitude * project;
            AddForceNormal(-normalForce);

            // a:b = c:d
            //b.c = a.d
            //project : normalForce.magnitude = 1 : x
            // normalForce.magnitude = x . project
            //AddForceNormal(-velocity.normalized * frictionForce);


        }

    }


    IEnumerator RotateForce(Vector3 rot, Vector3 point)
    {

        float rotX=0, rotY=0, rotZ=0;
        while (true)
        {
            rotX += rot.x * Time.deltaTime;
            rotY += rot.y * Time.deltaTime;
            rotZ += rot.z * Time.deltaTime;

            if (rotX > rot.x || rotY > rot.y || rotY > rot.z)
            {
                Debug.Log(rot.x + " / " + rotX);
                break;
            }

        }
        yield return null;


    }

    public void FrictionForce() {

        float totalFriction = colliderCS.fricion;

        foreach (var coll in contactObjects)
        {
            totalFriction += coll.Key.fricion;
        }
        velocity *= (1 - totalFriction);
        Debug.Log(totalFriction);

    }


	public void VelocityMove()
    {
        if (velocity.magnitude > maxSpeed)
            velocity = velocity.normalized * maxSpeed;

        if (velocity.magnitude > 0.1f)
        {
            this.transform.position += velocity * Time.deltaTime;
        }
        else
            velocity = Vector3.zero;
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
        //Debug.Log(velocity.magnitude + " / " + force.magnitude);
        velocity += force / mass;

        //Debug.DrawRay(this.transform.position, velocity * 10, Color.green, 30);
    }




    void OnCollisionEnterF(BoxColliderCS coll)
    {
        //c =StartCoroutine(RotateForce(coll.transform.eulerAngles, coll.contactPoint));
        Debug.Log("Enter : " + coll.gameObject.name + "/"+contactObjects.Count);
    }
    void OnCollisionExitF(BoxColliderCS coll)
    {
        Debug.Log("Exit : " + coll.gameObject.name + "/" + contactObjects.Count);
    }
    void OnCollisionStayF(BoxColliderCS coll)
    {
        Debug.Log("Stay : " + coll.gameObject.name);
    }

}
