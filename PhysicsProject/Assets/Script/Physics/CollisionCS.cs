using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCS 
{
    public GameObject collisionObject;
    public Vector3 collisionDirecion;

    public float boundness;
    public float fricion;

    public CollisionCS(GameObject obj, Vector3 dir)
    {
        BoxColliderCS coll = obj.GetComponent<BoxColliderCS>();
        boundness = coll.boundness;
        fricion = coll.fricion;

        collisionObject = obj;
        collisionDirecion = dir;
    }


}
