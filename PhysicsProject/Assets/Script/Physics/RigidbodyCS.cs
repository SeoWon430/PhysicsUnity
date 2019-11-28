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


    //AddForce() 할때 힘을 적용 방식
    public enum ForceMode
    {
        Force,              //deltaTime 과 질량을 고려하여 힘 적용
        Impulse,            //질량을 고려하여 힘 적용
        Acceleration,       //deltaTime 을 고려하여 힘 적용
        VelocityChange      //바로 힘 적용
    }

    public enum Detection
    {
        Default,    //기본 한 프레임 체크
        Continuous  //다음 프레임까지 체크
    };
    //충돌 체크 방식
    public Detection collisionDetection = Detection.Default;


    //현재 물체가 가지는 속도
	public Vector3 velocity { get; set; }

    //현재 물체가 가지는 각속도
    public Vector3 angularVelocity { get; private set; }
    private Vector3 angularDegree;



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
        angularDegree = Vector3.zero;

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
                // F = 마찰계수 * 수직항력 (마찰계수는 두 물체 간 마찰계수의 합으로 설정)
                float frictionForce = (coll.fricion + colliderCS.fricion) * normalForce.magnitude;

                //충돌 물체와 현재 물체의 탄성력 계산
                // F = 탄성계수 * 수직항력 (탄성계수는 두 물체 간 탄성계수의 합으로 설정)
                normalForce *= (1 + this.colliderCS.boundness + coll.boundness);

                //외력의 방향, 크기 조절
                float project = Vector3.Project(dir, normalForce).magnitude;
                //float angle = Vector3.Angle(normalForce, dir);

                //normalForce = dir * normalForce.magnitude / Mathf.Cos(angle * Mathf.Deg2Rad);

                //외력의 방향, 크기 재조정
                normalForce = Vector3.Project(normalForce, dir)* project * project * project;

                //탄성력을 포함한 외력 계산
                //기본적으로 수직항력에 탄성력을 포함하여 적용
                AddForce(-normalForce, ForceMode.Impulse);

                //마찰력에 의한 외력 계산
                //움직이는 방향의 반대로 적용
                AddForce(-velocity.normalized * frictionForce, ForceMode.Impulse);

                //Debug.Log(dir.x + " / " + dir.y + " / " + dir.z);
                //Debug.DrawRay(this.transform.position, dir * 10, Color.red, 30);
                //Debug.DrawRay(this.transform.position, -normalForce * 10, Color.green, 30);
            }
    }



    //속도(velocity)에 의한 움직임
    //PhysicsManager.cs에서 일괄 호출
	public void VelocityMove()
    {


        angularVelocity *= (1 - angularDrag);
        //속도가 일정량 이상인 경우만 물체 이동
        if (angularVelocity.magnitude > 0.1f)
        {
            this.transform.Rotate(this.transform.eulerAngles + angularVelocity);

            if (angularVelocity.y != 0)
            {
                Vector3 speedRight = transform.right * Mathf.Tan(angularVelocity.y * Mathf.Deg2Rad) * velocity.magnitude / Time.deltaTime;
                AddForce(speedRight);
            }

            if (angularVelocity.x != 0)
            {
                Vector3 speedUp = transform.up * Mathf.Tan(angularVelocity.x * Mathf.Deg2Rad) * velocity.magnitude / Time.deltaTime;
                AddForce(speedUp);
            }

            if (angularVelocity.z != 0)
            {
                Vector3 speedForward = transform.forward * Mathf.Tan(angularVelocity.z * Mathf.Deg2Rad) * velocity.magnitude / Time.deltaTime;
                AddForce(speedForward);
            }
        }
        else
        {
            angularVelocity = Vector3.zero;
        }




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

    }


    //이동 후 충돌해 있는 모든 물체와 겹쳐 있는지 판단
    //겹쳐진 양(overlap) 만큼 되돌림
    public void Overlap()
    {

        foreach (ColliderCS coll in contactObjects)
        {
            Vector3 distance = coll.centerPosition - colliderCS.points[coll.contactPointNumber];
            Vector3 project = Vector3.Project(distance, coll.contactNormal);
            float overlap = project.magnitude - coll.contactLength;
            if (overlap < 0)
            {

                this.transform.position -= coll.contactNormal * overlap * 1f;
                //Debug.Log(overlap);

            }

        }
    }



    //외력적용 함수
    //  F = m*a (F는 force / m은 질량 / a는 가속도)
    //  v= a*t (t는 시간 = Time.deltaTime / v = velocity)
    //  최종적으로 velocity값을 변경하여 물체가 이동하도록 함
    //ForceMode로 질량과 시간 적용 여부 결정
    public void AddForce(Vector3 force, ForceMode mode = ForceMode.Force)
    {
        //이 물체가 물리적 계산을 안할 경우 외력을 주지 않도록 함
        if (isKinematic) return;

        switch (mode)
        {
            //일반적인 외력 적용(디폴트)
            //시간(Time.deltaTime)에 따라 힘이 적용
            //사용자 키 입력에 따른 외력(이동)에 사용
            case ForceMode.Force:
                velocity += force / mass * Time.deltaTime;
                break;

            //물리적인 외력이 한번에 적용하기 위해 사용
            //시간(Time.deltaTime) 고려하지 않아 한번에 힘 적용 가능
            //충돌에 의한 수직항력과 관련된 물리적 힘에 사용
            case ForceMode.Impulse:
                velocity += force / mass;
                break;

            //일단 구현만 함
            case ForceMode.Acceleration:
                velocity += force * Time.deltaTime;
                break;

            //일단 구현만 함
            case ForceMode.VelocityChange:
                velocity += force;
                break;
        }
    }




    public void AddTorque(Vector3 force, ForceMode mode = ForceMode.Force)
    {
        //이 물체가 물리적 계산을 안할 경우 외력을 주지 않도록 함
        if (isKinematic) return;


        switch (mode)
        {
            case ForceMode.Force:
                angularVelocity += force / mass * Time.deltaTime;
                break;

            case ForceMode.Impulse:
                angularVelocity += force / mass;
                break;

            case ForceMode.Acceleration:
                angularVelocity += force * Time.deltaTime;
                break;

            case ForceMode.VelocityChange:
                angularVelocity += force;
                break;
        }


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
