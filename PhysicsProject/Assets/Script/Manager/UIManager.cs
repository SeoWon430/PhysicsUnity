using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public RigidbodyCS player;
    public RigidbodyCS enemy;


    public Text textSpeed;
    public Text textTime;
    public Text textFps;


    public float deltaTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    void FixedUpdate()
    {

    }
    // Update is called once per frame
    void LateUpdate()
    {
        if (Time.time % 2 < 0.1f)
            deltaTime = 1 / Time.deltaTime;
        textSpeed.text = ((int) player.velocity.magnitude).ToString("d3")+"Km/h";
        textTime.text = "Time : "+Time.time.ToString("F1")+"s";
        textFps.text = "Fps : " + deltaTime.ToString("F1") + "s";
    }

}
