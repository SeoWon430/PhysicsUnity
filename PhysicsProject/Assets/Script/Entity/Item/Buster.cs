using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buster : MonoBehaviour
{
    private GameObject particleHit;
    private ParticleSystem[] particles;
    private Vector3 direction = Vector3.zero;

    public enum Direction
    {
        NONE,
        RIGHT,
        LEFT,
        UP,
        DOWN,
        FORWARD,
        BACK
    }
    public Direction busterDirection = Direction.NONE;
    public float busterForce;

    // Start is called before the first frame update
    void Start()
    {

        switch (busterDirection)
        {
            case Direction.RIGHT:
                direction = this.transform.right;
                break;
            case Direction.LEFT:
                direction = -this.transform.right;
                break;
            case Direction.UP:
                direction = this.transform.up;
                break;
            case Direction.DOWN:
                direction = -this.transform.up;
                break;
            case Direction.FORWARD:
                direction = this.transform.forward;
                break;
            case Direction.BACK:
                direction = -this.transform.forward;
                break;
        }

        particleHit = Instantiate(Resources.Load("HitEffect") as GameObject);
        particleHit.transform.parent = this.transform;
        particleHit.transform.position = this.transform.position + direction * 2 + this.transform.up;
        particles = particleHit.GetComponentsInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnterF(ColliderCS coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {

            Movement move= coll.gameObject.GetComponent<Movement>();
            if(move != null)
            {
                //Debug.Log(dir);
                move.StartCoroutine(move.Boost(direction, busterForce));
            }


            foreach (ParticleSystem particle in particles)
            {
                particle.Play();
            }

        }

        //c =StartCoroutine(RotateForce(coll.transform.eulerAngles, coll.contactPoint));
        //Log("Enter : " + coll.gameObject.name + "/" + contactObjects.Count);
    }
}
