using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//기본적인 콜라이더
//RigidbodyCS넣을 데이터만 구성
public class ColliderCS : MonoBehaviour
{
    //충돌한 물체(RigidbodyCS)에 적용할 외력의 방향
    //방향 = 출돌한 면의 normal
    public Vector3 contactNormal { get; protected set; }

    //충돌한 물체(RigidbodyCS)의 충돌 점
    public Vector3 contactPoint { get; protected set; }

    //충돌한 물체(RigidbodyCS)의 충돌 점 인덱스
    //충돌 점의 인덱스는 0~7 (box모양의 콜라이더는 점 8개로 구성)
    public int contactPointNumber { get; protected set; }

    //충돌한 면에 대해 가져야 할 거리
    //현재 물체에 대한 local의 x, y, z 스케일 값을 가짐
    //RigidbodyCS에서 물체 이동 후 겹치는 양을 체크
    public float contactLength { get; protected set; }

    //물체가 가지는 탄성계수
    public float boundness = 0;

    //물체가 가지는 마찰계수
    public float fricion = 0;

    //물체가 가지는 콜라이더의 중심점
    [HideInInspector]
    public Vector3 centerPosition;



    // Start is called before the first frame update
    void Start()
    {
        contactNormal = Vector3.zero;
        contactPoint = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void print()
    {
        Debug.Log(contactNormal + " / " + contactPoint + " / " + contactLength);
    }
}
