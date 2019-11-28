using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//박스 콜라이더
//충돌 구현은 상대적으로 간단한 박스 콜라이더로 제한하여 구현
//ColliderCS.cs를 상속 받음
//충돌하여 데이터를 넘길때는 다형성을 이용하여 ColliderCS.cs로 필요한 만큼만 넘김
public class BoxColliderCS : ColliderCS
{
    //충돌시 외력 적용 여부
	public bool isTrigger=false;

    //콜라이더 구성의 중심점
    //현재 물체에 대해 상대적으로 적용
	public Vector3 center = Vector3.zero;

    //콜라이더의 스케일
    //현재 물체의 스케일(transform.localScale)에 대해 *으로 적용
	public Vector3 size = Vector3.one;

    //현재 물체가 가지는 local상 x, y, z의 길이
	public Vector3 length { get; private set; } = Vector3.one;

    //현재 물체의 대각 모서리의 길이
	public float maxLength { get; private set; }  = 0;

    //현재 물체가 움직이는 여부 판단
    //움질일 경우 콜라이더 구성 점(8개)를 매 프레임 계산
	private bool isStatic;

    //콜라이더를 구성하는 점 8개
    //Rigidbody.cs의 points와 현재 물체의 length를 고려하겨 길이 판단으로 충돌 체크
    public Vector3[] points { get; private set; }

    //박스의 6면 각각에 대한 중심 점
    private Vector3[] centerPoints;



	private void Awake()
	{
        //RigidbodyCS.cs가 포함되어 있다면 움직이는 물체
        //없다면 정지 물체 (isStatic = true)
        if (GetComponent<RigidbodyCS>())
			isStatic = false;
		else
			isStatic = true;

        //구성점을 저장할 배열
		points = new Vector3[8];
        centerPoints = new Vector3[6];

        //현재 시점에서 콜라이더 구성점을 계산
        SetBoundaryPoint();

        //콜라이더 사이즈와 현재 물체의 스케일을 고려하여 x,y,z 상 충돌거리 계산
        length = new Vector3(size.x * this.transform.lossyScale.x / 2,
                                size.y * this.transform.lossyScale.y / 2,
                                size.z * this.transform.lossyScale.z / 2);

        //충돌 가능한 최대거리 계산
        //모서리 길이
        maxLength = length.magnitude;

        //탄성계수 조절
        if(boundness<0)
            boundness = 0;

        //마찰계수 조절
        if (fricion < 0)
            fricion = 0;
    }



	void Start()
	{

	}



	void FixedUpdate()
	{
        //움직이는 물체(Rigidbody.cs 포함)이면 매 프레임 콜라이더 구성 점 재계산
		if (!isStatic)
			SetBoundaryPoint();

        //그 외의 연산은 PhysicsManager.cs 에서 계산
    }


    //현재 상태에서 콜라이더 구성 점을 계산
	private void SetBoundaryPoint()
	{
        //points는 물체의 구성점 8개
        //centerPoints 물체의 각 구성면 6개의 중심점
        centerPosition = this.transform.position + center;

        for (int i = 0; i<points.Length; i++)
		{
			int a, b, c;
			a = i % 8 >= 4 ? 1 : -1;
			b = i % 4 >= 2 ? 1 : -1;
			c = i % 2 >= 1 ? 1 : -1;

            points[i] = centerPosition + 
                this.transform.right * length.x * a +
                this.transform.up * length.y * b +
                this.transform.forward * length.z * c;

        }

        centerPoints[0] = centerPosition + this.transform.right * length.x;
        centerPoints[1] = centerPosition - this.transform.right * length.x;
        centerPoints[2] = centerPosition + this.transform.up * length.y;
        centerPoints[3] = centerPosition - this.transform.up * length.y;
        centerPoints[4] = centerPosition + this.transform.forward * length.z;
        centerPoints[5] = centerPosition - this.transform.forward * length.z;
    }



    //충돌 체크
    //PhysicsManager.cs 에서 호출
    public ColliderCS CheckCollision(RigidbodyCS rigid, bool isCheck, bool isCheckNext, out bool isColl, bool isTrigger)
                                    //출돌 체크할 물체, 충돌 체크 여부, 다음 프레임 계산 여부, 충돌 판단 여부, 트리거 여부
	{
        //충돌 여부 (기본적으로 false)
		bool result = false;

        //충돌 했을 때 적용할 외력의 방향
        //방향 = 충돌한 면의 nomal
		Vector3 resultDir = Vector3.zero;

        //충돌 체크 할 물체(RigidbodyCS.cs)와 현재 물체의 거리
        Vector3 distance = centerPosition - rigid.colliderCS.centerPosition;

        //충돌 했을 때 겹쳐 지는 정도
		float Overlap = 0;

        //충돌 체크 할 물체(RigidbodyCS.cs)와 현재 물체의 거리가 멀면 현재 함수 생략
        if ( (maxLength + rigid.colliderCS.maxLength) * 1.5f > distance.magnitude)
        {

            //충돌 체크 할 물체(RigidbodyCS.cs)의 콜라이더 구성 점(points)의 인덱스
            int i = 0;


            //충돌 체크 할 물체(RigidbodyCS.cs)의 콜라이더 구성 점(points)을 하나씩 충돌 체크
            foreach ( Vector3 point in rigid.colliderCS.points)
			{
                //충돌 체크 할 물체(RigidbodyCS.cs)의 콜라이더 구성 점(points)중 하나
                Vector3 p = point;
                p = point + rigid.velocity * Time.deltaTime / 2;


                //현재 물체가 회전 되어 있음을 고려하여 p를 현재 local 상 x, y, z축으로 project계산
                //원래 모든 vector3의 x, y, z는 각각 world 상 x, y, z의 project임
                //project 결과는 실제 현재 물체를 중심으로 한 x, y, z축 상의 거리
                float projectX = (Vector3.Project(p - centerPosition, this.transform.right)).magnitude ;
                float projectY = (Vector3.Project(p - centerPosition, this.transform.up)).magnitude ;
                float projectZ = (Vector3.Project(p - centerPosition, this.transform.forward)).magnitude ;
				
				//위에서 계산한 거리에 현재 물체가 가지는 너비(length)를 뺌
                //뺀 값들이 모두 0이하일 경우에 충돌
                projectX -= length.x;
                projectY -= length.y;
                projectZ -= length.z;


                //Debug.Log(this.name + " : " + p + "/" + projectX + " / " + projectY + " / " + projectZ);


                //위에서 계산 한 값들이 모두 0이하일 경우에 충돌 
                // (오차 고려하여 임시적으로 0.05이하로 적용)
                if (projectX <= 0.05f && projectY <= 0.05f && projectZ <= 0.05f)
                {
                    
                    //이미 충돌 체크 한 경우나 현재 물체가 트리거 이면 아래 계산 생략
                    //아래는 충돌한 면과 외력의 방향을 계산 하여 결정
                    if (!isCheck && !isTrigger)
                    {
                        //속도가 빠른 경우를 고려하여 이전 프레임 기준으로 점 계산
                        Vector3 prevRigidPosition = rigid.transform.position - rigid.velocity * Time.deltaTime;

                        //위에서 구한 이전 프레임의 점으로 다시 project한 x,y,z거리 계산
                        float intervalX = (Vector3.Project(prevRigidPosition - centerPosition, this.transform.right)).magnitude;
                        float intervalY = (Vector3.Project(prevRigidPosition - centerPosition, this.transform.up)).magnitude;
                        float intervalZ = (Vector3.Project(prevRigidPosition - centerPosition, this.transform.forward)).magnitude;
                        //rigid.transform.position -= new Vector3(intervalX, intervalY, intervalZ);
                        //Debug.Log(distanceX + " / " + distanceY + " / " + distanceZ);


                        //속도가 빠른 경우 오차를 재계산
                        //기본적으로 projectX, projectY, projectZ 중 0에 가장 근접한 쪽에 충돌 했다 판단함
                        //projectX, Y, Z는 모두 음수 값이 나옴
                        // x, y, z는 각각 스케일(길이)가 다르므로 비율 고려를 위해 projectX, Y, Z 에 각각 나눔
                        intervalX = intervalX > length.x ? projectX  / length.x : (projectX - intervalX) / length.x;
                        intervalY = intervalY > length.y ? projectY  / length.y : (projectY - intervalY) / length.y;
                        intervalZ = intervalZ > length.z ? projectZ  / length.z : (projectZ - intervalZ) / length.z;

                        //Debug.Log(intervalX + " / " + intervalY + " / " + intervalZ);
						//Debug.Log("==========");


                        //위에서 계산한 X, Y, Z상 간격을 고려하여 각격이 가장 큰 쪽을 충돌한 축이라 판단
                        // (간격의 최대값은 위에서 0.05이하로 이미 걸러서 옴)
                        // 충돌한 축에 따라 외력의 방향을 설정
						if (intervalX > intervalY && intervalX > intervalZ)
                        {
                            resultDir = (p - centerPoints[0]).magnitude < (p - centerPoints[1]).magnitude
                                ? this.transform.right : -this.transform.right;

							Overlap = projectX;
                            contactLength = length.x;
						}
                        else if (intervalY > intervalX && intervalY > intervalZ)
                        {
                            resultDir = (p - centerPoints[2]).magnitude < (p - centerPoints[3]).magnitude
                                ? this.transform.up : -this.transform.up;


							Overlap = projectY;
                            contactLength = length.y;
                        }
                        else if (intervalZ > intervalY && intervalZ > intervalX)
                        {
                            resultDir = (p - centerPoints[4]).magnitude < (p - centerPoints[5]).magnitude
                                ? this.transform.forward : -this.transform.forward;

							Overlap = projectZ;
                            contactLength = length.z;
                        }
                        else
                        {
                            resultDir = rigid.velocity.normalized;
                            //Debug.Log("SFDGA");
                        }


                        //Debug.Log(this.name + " => " + isCheck + " : " + resultDir + " : " + intervalX + " / " + intervalY + " / " + intervalZ);

                        //충돌에 고려한 Rigidbody.cs 상 points의 인덱스
                        contactPointNumber = i;

                        //충돌에 고려한 Rigidbody.cs 상 points
                        contactPoint = p;

                        //충돌에 의한 외력의 방향
                        contactNormal = resultDir;

                        if (Mathf.Abs(Overlap) >= 0.2f)
                        {
                            //충돌 시 겹쳐진 경우 다시 이동을 되돌림
                            rigid.transform.position -= resultDir * Overlap;
                        }

                    }
                    //충돌 여부
                    result = true;
                    break;
				}
                i++;
			}


		}

        //충돌 여부를 참조 형식으로 되돌려 줌
        isColl = result;

        //충돌 true시 현재 스크립트(BoxColliderCS.cs)를 ColliderCS.cs형식으로 리턴
        if (result)
            return this;
        else
            return null;
	}

}
