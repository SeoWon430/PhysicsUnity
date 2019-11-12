﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//현재 Scene의 물리 엔진을 구현
//RigidbodyCS와 BoxColliderCS에 대해 충돌을 판단
public class PhysicsManager : MonoBehaviour
{
    //현재 스크립트는 static으로 관리
    public static PhysicsManager instance;

    //현재 Scene에 존재하는 모든 BoxColliderCS 물체
    public List<BoxColliderCS> colliderList;

    //현재 Scene에 존재하는 모든 RigidbodyCS 물체
    public List<RigidbodyCS> rigidbodyList;

    //현재 Scene에 적용할 중력
    public Vector3 gravity = new Vector3(0, -9.81f, 0);

    //float deltaTime = 0.0f;


    //현재 객체 static 저장
    void Awake()
    {
        if (instance == null)
            instance = this;
    }



	void Start()
    {
        //현재 Scene에 존재하는 모든 RigidbodyCS 물체 파악
        RigidbodyCS[] rigids = FindObjectsOfType<RigidbodyCS>();
		foreach (var r in rigids)
		{
			rigidbodyList.Add(r);
		}
         
        //현재 Scene에 존재하는 모든 BoxColliderCS 물체 파악
        BoxColliderCS[] colls = FindObjectsOfType<BoxColliderCS>();
		foreach(var c in colls)
		{
			colliderList.Add(c);
		}


    }



    //물리 계산을 위해 고정된 프레임인 FixedUpdate()에서 실행
    void FixedUpdate()
    {
        //모든 RigidbodyCS 물체에 대하여
        foreach ( RigidbodyCS rigidObject in rigidbodyList)
        {

            //일단 RigidbodyCS 물체에 중력 적용
            if (rigidObject.useGravity && !rigidObject.isKinematic)
            {
                rigidObject.AddForceNormal(gravity * Time.deltaTime);
            }

            //모든 BoxColliderCS 물체와 충돌 체크
            foreach (BoxColliderCS collObject in colliderList)
            {
                //RigidbodyCS 물체는 BoxColliderCS도 가지고 있으므로 자기 자신에 대해 충돌 체크를 방지
                if (rigidObject.gameObject.Equals(collObject.gameObject))
                    continue;


                //RigidbodyCS물체와 BoxColliderCS물체의 트리거 체크
                bool isTrigger = rigidObject.colliderCS.isTrigger || collObject.isTrigger;

                //RigidbodyCS물체와 BoxColliderCS물체의 충돌 체크
                //길어서 따로 함수로 구현
                CheckCollision(rigidObject, collObject, isTrigger);


            }

            //RigidbodyCS물체가 isKinematic이 false면 외력에 의한 움직임 계산
            //외력은 기본적으로 수직항력
            if (!rigidObject.isKinematic)
                rigidObject.MovementForce();
        }

	}


    //각 RigidbodyCS물체에서 모든 외력을 계산하여 속도가 결정이 되고 난 후 물체가 움직이도록 실행
    private void LateUpdate()
    {
        foreach (RigidbodyCS rigidObject in rigidbodyList)
        {
            rigidObject.VelocityMove();
        }
    }


    //RigidbodyCS와 BoxColliderCS 물체에 대해 충돌을 계산
    //isTrigger가 ture면 충돌 했는지만 계산하고 외력을 계산x
    private void CheckCollision(RigidbodyCS rigidObject, BoxColliderCS collObject, bool isTrigger)
    {

        //충돌한 물체의 정보
        //현재 모든 물체는 BoxColliderCS로 구현 되어 있지만 다형성으로 부분만 넘겨줌
        ColliderCS coll = null;

        //충돌 여부
        bool isCollision = false;

        //충돌체크 할 물체가 이미 충돌 계산되어 있는지 판단
        bool isContain = false;

        //트리거가 아닌 경우 : 충돌되어 있는 물체는 RigidbodyCS.cs의 contactObjects에 저장되어 있음
        //트리거인 경우 : 충돌되어 있는 물체는 RigidbodyCS.cs의 triggerObjects 저장되어 있음
        if (!isTrigger)
            isContain = rigidObject.contactObjects.Contains(collObject);
        else
            isContain = rigidObject.triggerObjects.Contains(collObject);


        //충돌 체크
        //BoxColliderCS기준에서 RigidbodyCS의 points를 통해 충돌을 계산
        //리턴 값은 자기 자신 객체(BoxColliderCS이지만 ColliderCS로 리턴)
        if (rigidObject.collisionDetection == RigidbodyCS.Detection.Default)
            coll = collObject.CheckCollision(rigidObject, isContain, false, out isCollision, isTrigger);
        else
            coll = collObject.CheckCollision(rigidObject, isContain, true, out isCollision, isTrigger);


        //트리거가 아닌 경우 : 충돌 물체를 contactObjects에 저장하고 OnCollision 함수 호출
        //충돌 물체 : ColliderCS.cs / contactObjects를 가진 객체 : RigidbodyCS.cs
        if (!isTrigger)
        {
            if (isCollision && !isContain)
            {
                rigidObject.contactObjects.Add(collObject);
                rigidObject.SendMessage("OnCollisionEnterF", collObject, SendMessageOptions.DontRequireReceiver);
                //rigidObject.ForceRotation(collObject.transform.rotation);

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
            else
            {
                //Debug.Log("none");

            }

        }

        //트리거인 경우 : 충돌 물체를 triggerObjects 저장하고 OnTrigger 함수 호출
        //충돌 물체 : ColliderCS.cs / triggerObjects 가진 객체 : RigidbodyCS.cs
        //저장하여 충돌 여부만 판단하고 외력 계산은 안함
        //외력 계산은 RigidbodyCS.cs내의 함수로 실행
        else
        {
            if (isCollision && !isContain)
            {
                rigidObject.triggerObjects.Add(collObject);
                rigidObject.SendMessage("OnTriggerEnterF", collObject, SendMessageOptions.DontRequireReceiver);
            }
            else if (!isCollision && isContain)
            {
                rigidObject.triggerObjects.Remove(collObject);
                rigidObject.SendMessage("OnTriggerExitF", collObject, SendMessageOptions.DontRequireReceiver);
            }
            else if (isCollision && isContain)
            {
                rigidObject.SendMessage("OnTriggerStayF", collObject, SendMessageOptions.DontRequireReceiver);
            }

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
