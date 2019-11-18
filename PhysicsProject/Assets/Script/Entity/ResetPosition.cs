using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPosition : MonoBehaviour
{
    public GameObject resetpoint;
    private RigidbodyCS playerRigid;
    private Movement playerMovement;
    // Start is called before the first frame update
    void Start()
    {
        playerRigid = this.GetComponent<RigidbodyCS>();
        playerMovement = this.GetComponent<Movement>();
        resetpoint = Instantiate(resetpoint, this.transform.position, this.transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time % 5 < 0.1f && playerRigid.velocity.magnitude>10f && playerMovement.isOnRoad)
        {
            resetpoint.transform.position = this.transform.position;
            resetpoint.transform.rotation = this.transform.rotation;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            this.transform.position = resetpoint.transform.position;
            this.transform.rotation = resetpoint.transform.rotation;
        }
    }
}
