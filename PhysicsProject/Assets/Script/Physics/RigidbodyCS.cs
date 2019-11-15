using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//리지드 바디
//물체의 외력 적용, 이동을 구현
//충돌의 주체가 되는 물체
public class RigidbodyCS : MonoBehaviour
{
    
    //현재 물체의 질량
	public float mass=1;

    //공기저항
    //대략적으로 구현
	public float drag=0;

    //회전저항
    //적용 방식이 확실하지 않아 구현x
	public float angularDrag = 0.05f;

    //중력 적용 여부
	public bool useGravity=true;

    //물리 충돌 적용 여부(Addforce 적용 여부)
    //false면 적용 ( true면 계산x)
	public bool isKinematic=false;

    //현재 물체가 가질 수 있는 최대 속력
    //충돌 구현이 완전하지 않아 일단 제한함
    public float maxSpeed = 100;


    public enum Detection
    {
        Default,    //기본 한 프레임 체크
        Continuous  //다음 프레임까지 체크
    };
    //충돌 체크 방식
    public Detection collisionDetection = Detection.Default;



    //현재 물체가 가지는 총 속도
	public Vector3 velocity { get; set; }

    //외력에 의한 외력(물리엔진)
    //ex 수직항력
    public Vector3 forceVelocity { get; private set; }

    //사용자 입력에 의한 외력
    public Vector3 inputVelocity { get; private set; }

    //현재 물체가 가지는 콜라이더
    //콜라이더는 박스콜라이더로 제한하여 구현
	public BoxColliderCS colliderCS { get; private set; }

    //현재 물체와 충돌한 콜라이더 리스트
    public List<ColliderCS> contactObjects;

    //현재 물체와 충돌한 트리거 리스트
    public List<ColliderCS> triggerObjects;



    
    //초기화
	private void Awake()
	{
		velocity = Vector3.zero;
        inputVelocity = Vector3.zero;
        forceVelocity = Vector3.zero;

        if (mass <= 0)
			mass = 1;

        if (maxSpeed < 1)
            maxSpeed = 1;

        colliderCS = GetComponent<BoxColliderCS>();
		if (!colliderCS)
			colliderCS = this.gameObject.AddComponent<BoxColliderCS>();

		contactObjects = new List<ColliderCS>();
        triggerObjects = new List<ColliderCS>();

    }


	void Start()
	{

	}

	void FixedUpdate()
	{
        //물리에 의한 프레임 단위 실행은 FixedUpdate()에서 실행
        //FixedUpdate는 프레임 단위가 고정 되어 실행됨
        //순서 문제로 인해 PhysicsManager.cs 에서 관리 하기로 함
    }

    private void LateUpdate()
	{

    }

    

    //현재 물체에 적용된 외력의 합력을 계산하여 속도로 변환 시킴
    public void MovementForce()
    {
        //충돌한 물체에 의한 외력을 각각 계산
        if (contactObjects.Count > 0)
            foreach (ColliderCS coll in contactObjects)
            {
                //외력의 방향
                Vector3 dir = coll.contactNormal;

                //외력 계산
                //기본적으로 이 힘은 수직항력
                Vector3 normalForce = new Vector3(velocity.x * Mathf.Abs(dir.x), velocity.y * Mathf.Abs(dir.y), velocity.z * Mathf.Abs(dir.z));

                //충돌 물체와 현재 물체의 마찰력 계산
                // F = 마찰계수 * 수직항력
                // 마찰계수는 두 물체 간 마찰계수의 합으로 설정
                float frictionForce = (coll.fricion + colliderCS.fricion) * normalForce.magnitude;

                //충돌 물체와 현재 물체의 탄성력 계산
                // F = 탄성계수 * 수직항력
                // 탄성계수는 두 물체 간 탄성계수의 합으로 설정
                normalForce *= (1 + this.colliderCS.boundness + coll.boundness);

                //외력의 방향, 크기 조절
                float project = Vector3.Project(dir, normalForce).magnitude;
                //float angle = Vector3.Angle(normalForce, dir);

                //normalForce = dir * normalForce.magnitude / Mathf.Cos(angle * Mathf.Deg2Rad);

                //외력의 방향, 크기 재조정
                normalForce = Vector3.Project(normalForce, dir)* project * project * project;


                //탄성력을 포함한 외력 계산
                //기본적으로 수직항력에 탄성력을 포함하여 적용
                AddForceNormal(-normalForce);

                //마찰력에 의한 외력 계산
                //움직이는 방향의 반대로 적용
                AddForceNormal(-velocity.normalized * frictionForce);


                //Debug.Log(dir.x + " / " + dir.y + " / " + dir.z);
                //Debug.DrawRay(this.transform.position, dir * 10, Color.red, 30);
                //Debug.DrawRay(this.transform.position, -normalForce * 10, Color.green, 30);
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



    //속도에 의한 움직임
    //PhysicsManager.cs에서 일괄 호출
	public void VelocityMove()
    {
        //속도는 공기저항에 따라 방해 받음
        //실제 유니티의 drag가 구체적으로 어떻게 작용하는지 몰라 임의로 계산함
        //(원래는 drag가 무한 일 경우에 물체는 움직이지 않는다고 함)
        velocity *= (1 - drag);

        //속도의 크기 제한
        if (velocity.magnitude > maxSpeed)
            velocity = velocity.normalized * maxSpeed;


        //속도가 일정량 이상인 경우만 물체 이동
        if (velocity.magnitude > 0.1f)
        {
            this.transform.position += velocity * Time.deltaTime;
        }
        else
        {
            velocity = Vector3.zero;
        }

        //이동 후 충돌해 있는 모든 물체와 겹쳐 있는지 판단
        //겹쳐진 양(overlap) 만큼 되돌림
        foreach(ColliderCS coll in contactObjects)
        {
            Vector3 distance = coll.centerPosition - colliderCS.points[coll.contactPointNumber] ;
            Vector3 project = Vector3.Project(distance, coll.contactNormal);
            float overlap = project.magnitude - coll.contactLength;
            if (overlap < 0)
            {
                this.transform.position -= coll.contactNormal * overlap*0.95f;
            }

        }
	}



    //입력에 의한 외력
    //힘 조절 편리를 위해 Time.deltaTime를 넣음
    // F = m*a (/F는 force / m은 질량, a는 가속도)
    // v= a*t (t는 시간 = Time.deltaTime)
    public void AddForce(Vector3 force)
    {
        if (isKinematic) return;

        inputVelocity += force / mass * Time.deltaTime;
        velocity = forceVelocity + inputVelocity;
    }

    //자체 구현물리에 의한 외력
    //수직항력, 마찰력
    //한번에 작용해야 되기 때문에 Time.deltaTime를 넣지 않음
    public void AddForceNormal(Vector3 force)
    {
        if (isKinematic) return;

        forceVelocity += force / mass;
        velocity = forceVelocity+ inputVelocity;
    }




    public void ForceRotation(Quaternion rot)
    {
        this.transform.rotation = rot;
    }
    
    public void ForceRotation(Vector3 point, Vector3 force)
    {

        Vector3 cross = Vector3.Cross(-velocity.normalized, force.normalized);
        float angleX = Vector3.Angle(this.transform.right, force.normalized);
        float angleY = Vector3.Angle(this.transform.up, force.normalized);
        float angleZ = Vector3.Angle(this.transform.forward, force.normalized);

        float angle = Mathf.Min(angleX, angleY, angleZ);

        /*
        this.transform.rotation = Quaternion.identity;
        Vector3 cross = Vector3.Cross(-velocity.normalized, force.normalized);
        float angleX = Vector3.Angle(this.transform.right, force.normalized);
        float angleY = Vector3.Angle(this.transform.up, force.normalized);
        float angleZ = Vector3.Angle(this.transform.forward, force.normalized);

        float angle = Mathf.Min(angleX, angleY, angleZ);
        Debug.Log(angle);
        if (cross.magnitude > 0.1f && angle < 89f)
        {
            this.transform.RotateAround(point, cross, angle);
            //StartCoroutine(Rotation(point, cross, angle));
        }
        */
    }
    


    IEnumerator Rotation(Vector3 rotation)
    {
        /*
        float rot= rotation - this.transform.eulerAngles;
        while (true)
        {
            rot += angle * Time.deltaTime;
            if (rot >= angle)
                break;

            yield return new WaitForSeconds(Time.deltaTime);
        }
        */
        yield return null;
    }


    //충돌에 의한 함수 호출
    //PhysicsManager.cs에서 충돌 체크 후 일괄 호출
    void OnCollisionEnterF(ColliderCS coll)
    {
        //c =StartCoroutine(RotateForce(coll.transform.eulerAngles, coll.contactPoint));
        //Debug.Log("Enter : " + coll.gameObject.name + "/"+contactObjects.Count);
    }
    void OnCollisionExitF(ColliderCS coll)
    {
        //Debug.Log("Exit : " + coll.gameObject.name + "/" + contactObjects.Count);
    }
    void OnCollisionStayF(ColliderCS coll)
    {
        //Debug.Log("Stay : " + coll.gameObject.name);
    }


    void OnTriggerEnterF(ColliderCS coll)
    {
        //c =StartCoroutine(RotateForce(coll.transform.eulerAngles, coll.contactPoint));
        //Log("Enter : " + coll.gameObject.name + "/" + contactObjects.Count);
    }
    void OnTriggerExitF(ColliderCS coll)
    {
        //Debug.Log("Exit : " + coll.gameObject.name + "/" + contactObjects.Count);
    }
    void OnTriggerStayF(ColliderCS coll)
    {
        //Debug.Log("Stay : " + coll.gameObject.name);
    }
}
