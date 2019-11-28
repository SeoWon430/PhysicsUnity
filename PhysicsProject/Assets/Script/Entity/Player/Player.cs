using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int trackCount;
    public int rank;
    public int bestTime;

    public GameObject collParticle;
    private ParticleSystem[] particles;

    // Start is called before the first frame update
    void Start()
    {
        trackCount=0;
        rank=0;
        bestTime=0;
        particles = collParticle.GetComponentsInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnCollisionEnterF(ColliderCS coll)
    {
        if (coll.CompareTag("Wall") || coll.CompareTag("Obstacle"))
        {
            foreach(ParticleSystem particle in particles)
            {
                particle.transform.position = coll.contactPoint + new Vector3(0, 0.5f, 0);
                particle.Play();
            }
        }
    }
}
