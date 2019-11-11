using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxColliderCS : MonoBehaviour
{
	public bool isTrigger=false;
	public Vector3 center = Vector3.zero;
	public Vector3 size = Vector3.one;
	public Vector3 length = Vector3.one;

    public Vector3 contactPoint=Vector3.zero;

	//[HideInInspector]
	public float maxLength { get; private set; }  = 0;

	[HideInInspector]
	public Vector3 centerPosition;

	private bool isStatic;
	public Vector3[] points;
    private Vector3[] centerPoints;


    public float boundness=0;
    public float fricion=0;

    public Plane p;

	private void Awake()
	{
		if (GetComponent<RigidbodyCS>())
			isStatic = false;
		else
			isStatic = true;


		points = new Vector3[8];
        centerPoints = new Vector3[6];
        SetBoundaryPoint();

        length = new Vector3(size.x * this.transform.lossyScale.x / 2,
                                size.y * this.transform.lossyScale.y / 2,
                                size.z * this.transform.lossyScale.z / 2);

        maxLength = length.magnitude;

        if(boundness<0)
            boundness = 0;

        if (fricion < 0)
            fricion = 0;

    }

	void Start()
	{

	}

	void FixedUpdate()
	{
		if (!isStatic)
			SetBoundaryPoint();

        //Debug.Log(this.name + " : " + this.transform.up);
        //Debug.DrawRay(this.transform.position, this.transform.up * 100, Color.red, 100);
    }

	void SetBoundaryPoint()
	{
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


	public bool CheckCollision(RigidbodyCS rigid, out Vector3 dir, bool isCheck, bool isCheckNext)
	{
		
		bool result = false;
		Vector3 resultDir = Vector3.zero;
        Vector3 distance = centerPosition - rigid.colliderCS.centerPosition;
		float Overlap = 0;

		if ( (maxLength + rigid.colliderCS.maxLength) * 1.5f > distance.magnitude)
        {

			foreach ( Vector3 point in rigid.colliderCS.points)
			{
				Vector3 p = Vector3.zero;
                if (isCheckNext)
                    p = point + rigid.velocity * Time.deltaTime/2;
                else
                    p = point;

                float projectX = (Vector3.Project(p - centerPosition, this.transform.right)).magnitude ;
                float projectY = (Vector3.Project(p - centerPosition, this.transform.up)).magnitude ;
                float projectZ = (Vector3.Project(p - centerPosition, this.transform.forward)).magnitude ;
				
				
                projectX -= length.x;
                projectY -= length.y;
                projectZ -= length.z;


                //Debug.Log(this.name + " : " + p + "/" + projectX + " / " + projectY + " / " + projectZ);
                if (projectX <= 0.05f && projectY <= 0.05f && projectZ <= 0.05f)
                {
                    
                    if (!isCheck)
                    {

                        float interval = (centerPosition - rigid.transform.position).magnitude;
                        Vector3 prevRigidPosition = rigid.transform.position - rigid.velocity * Time.deltaTime;
                        float intervalX = (Vector3.Project(prevRigidPosition - centerPosition, this.transform.right)).magnitude;
                        float intervalY = (Vector3.Project(prevRigidPosition - centerPosition, this.transform.up)).magnitude;
                        float intervalZ = (Vector3.Project(prevRigidPosition - centerPosition, this.transform.forward)).magnitude;
                        //rigid.transform.position -= new Vector3(intervalX, intervalY, intervalZ);
                        //Debug.Log(distanceX + " / " + distanceY + " / " + distanceZ);

                        
                        intervalX = intervalX > length.x ? projectX  / length.x : (projectX - intervalX) / length.x;
                        intervalY = intervalY > length.y ? projectY  / length.y : (projectY - intervalY) / length.y;
                        intervalZ = intervalZ > length.z ? projectZ  / length.z : (projectZ - intervalZ) / length.z;

                        /*
						float intervalX = projectX ;
						float intervalY = projectY ;
						float intervalZ = projectZ ;
						*/

                        //Debug.Log(intervalX + " / " + intervalY + " / " + intervalZ);
						//Debug.Log("==========");


						if (intervalX > intervalY && intervalX > intervalZ)
                        {
                            
                            resultDir = (p - centerPoints[0]).magnitude < (p - centerPoints[1]).magnitude
                                ? this.transform.right : -this.transform.right;

							//resultDir = this.transform.right;

							//rigid.transform.position += resultDir * projectX;
							//Debug.Log(this.name + "x" + Overlap);
							Overlap = projectX;
							//Debug.Log(Overlap);
						}
                        else if (intervalY > intervalX && intervalY > intervalZ)
                        {
                            
                            resultDir = (p - centerPoints[2]).magnitude < (p - centerPoints[3]).magnitude
                                ? this.transform.up : -this.transform.up;


							//resultDir = this.transform.up;
							//rigid.transform.position += resultDir * projectY;
							//Debug.Log(this.name +"y" + Overlap);
							Overlap = projectY;
                            //Debug.Log(Overlap);
                        }
                        else if (intervalZ > intervalY && intervalZ > intervalX)
                        {
                            
                            resultDir = (p - centerPoints[4]).magnitude < (p - centerPoints[5]).magnitude
                                ? this.transform.forward : -this.transform.forward;

							//resultDir = this.transform.forward;

							//Debug.Log(this.name + "z" + Overlap);
							Overlap = projectZ;
							//Debug.Log(Overlap);
						}
                        else
                        {
                            resultDir = rigid.velocity.normalized;
                            Debug.Log("SFDGA");
                        }


                        //Debug.Log(this.name + " => " + isCheck + " : " + resultDir + " : " + intervalX + " / " + intervalY + " / " + intervalZ);

                    }


                    if (Overlap != 0)
                    {
						/*
                        float minMove = rigid.velocity.magnitude * Time.deltaTime * 2f;
                        Overlap = Mathf.Abs(Overlap) < minMove ? -minMove : Overlap;
                        float maxMove = rigid.velocity.magnitude * Time.deltaTime * 2.5f;
                        Overlap = Mathf.Abs(Overlap) < minMove ? -minMove : Overlap;
                        Overlap = Mathf.Abs(Overlap) > maxMove ? -maxMove : Overlap;
						*/

                        //rigid.velocity *= -1;
                        //result = false;
						rigid.transform.position -= resultDir * Overlap;
						//Debug.Log(this.name + " => "+isCheck+" : " + resultDir + " : " + projectX + " / " + projectY + " / " + projectZ);
						//Debug.Log("==========");
					}


                    //float dirX = Vector3.Project(resultDir, this.transform.right).magnitude;
                    result = true;
                    contactPoint = p;
                    //Debug.Log(resultDir);
                    break;
				}
			}
		/*
			if (Mathf.Abs(Overlap) > 0.1f)
				goto recheck;
				*/
			//Debug.Log("====");


		}



		dir = resultDir;
		return result;
	}
}
