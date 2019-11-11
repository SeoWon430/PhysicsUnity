using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsManager : MonoBehaviour
{
    public static PhysicsManager instance;
    public List<BoxColliderCS> colliderList;
	public List<RigidbodyCS> rigidbodyList;
     
    public Vector3 gravity = new Vector3(0, -9.81f, 0);
    public Vector3 gravityDirection;

    float deltaTime = 0.0f;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }
	void Start()
	{
		RigidbodyCS[] rigids = FindObjectsOfType<RigidbodyCS>();
		foreach (var r in rigids)
		{
			rigidbodyList.Add(r);
		}

		BoxColliderCS[] colls = FindObjectsOfType<BoxColliderCS>();
		foreach(var c in colls)
		{
			bool check=true;

			foreach (var r in rigids)
			{
				if (c.gameObject.Equals(r.gameObject))
				{
					check = false;
					break;
				}
			}
			if(check)
				colliderList.Add(c);
		}

        gravityDirection = gravity.normalized;

    }
	
    void FixedUpdate()
    {
		//maxSizeLength;
		foreach( var rigidObject in rigidbodyList)
		{
			foreach(var collObject in colliderList)
			{
				Vector3 forceDirection = Vector3.zero;
                bool isContain = rigidObject.contactObjects.ContainsKey(collObject);
                bool isCollision = false;

                if (rigidObject.collisionDetection == RigidbodyCS.Detection.Default)
                    isCollision = collObject.CheckCollision(rigidObject, out forceDirection, isContain, false);
                else
                    isCollision = collObject.CheckCollision(rigidObject, out forceDirection, isContain, true);
                //Debug.Log(isContain + " / " + isCollision);


                if (isCollision && !isContain)
                {
                    //CollisionCS coll = new CollisionCS(collObject.gameObject, forceDirection);4
                    //forceDirection *= Mathf.Cos(collObject.transform.eulerAngles.z * Mathf.Deg2Rad);
                    //forceDirection *= Mathf.Cos(collObject.transform.eulerAngles.x * Mathf.Deg2Rad);
                    //Debug.Log(forceDirection);
                    /*
                    Vector3 normalForce = new Vector3(rigidObject.velocity.x * Mathf.Abs(forceDirection.x)
                                                , rigidObject.velocity.y * Mathf.Abs(forceDirection.y)
                                                , rigidObject.velocity.z * Mathf.Abs(forceDirection.z));
                    normalForce = Vector3.Project(normalForce, forceDirection);

                    Debug.Log(forceDirection.magnitude + "/"+ normalForce.magnitude + "/" + rigidObject.velocity.magnitude);

                    Debug.DrawRay(rigidObject.transform.position, forceDirection, Color.red, 30);
                    Debug.DrawRay(rigidObject.transform.position, normalForce, Color.green, 30);
                    */
                    rigidObject.contactObjects.Add(collObject, forceDirection);
                    //rigidObject.contactObjects.Add(collObject, normalForce);



                    //collisionEnter
                    rigidObject.SendMessage("OnCollisionEnterF", collObject, SendMessageOptions.DontRequireReceiver);
                    //Debug.Log(forceDirection);
                }
                else if (!isCollision && isContain)
                {
                    rigidObject.contactObjects.Remove(collObject);
                    //collisionExit
                    rigidObject.SendMessage("OnCollisionExitF", collObject, SendMessageOptions.DontRequireReceiver);


                }
                else if (isCollision && isContain)
                {
                    //collisionStay
                    rigidObject.SendMessage("OnCollisionStayF", collObject, SendMessageOptions.DontRequireReceiver);


                }


            }

            rigidObject.MovementForce();

            //deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            //Debug.Log(1.0f / deltaTime);
        }

	}

    bool RayCast(Vector3 start, Vector3 end, out BoxCollider collider)
    {
        bool result = false;
        BoxCollider resultCollider = new BoxCollider();




        collider = resultCollider;
        return result;
    }
}
