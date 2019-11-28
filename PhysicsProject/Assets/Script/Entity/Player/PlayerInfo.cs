using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    public Movement player;
    public GameObject backCamera;

    public bool isStart=false;
    public Text textSpeed;
    public Text textTrackCurrent;
    public Text textBestTime;
    public Text textTime;
    public Text textFps;
    public Image imageSpeed;

    private float timer;
    private float bestTime;

    private int trackCount;
    private int trackCurrent;

    private int speed;

    private float deltaTime = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        ShowInformationUI();
        bestTime = 0f;
        trackCount = 3;
        trackCurrent = 0;
    }



    void FixedUpdate()
    {
        if (isStart)
            timer += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if(backCamera.activeSelf)
                backCamera.SetActive(false);
            else
                backCamera.SetActive(true);
        }

    }



    // Update is called once per frame
    void LateUpdate()
    {
        speed = (int)player.rigid.velocity.magnitude;
        if (isStart)
            ShowInformationUI();
    }


    void ShowInformationUI()
    {
        if (Time.time % 2 < 0.1f)
            deltaTime = 1 / Time.deltaTime;

        textSpeed.text = speed.ToString();
        textTrackCurrent.text = "Track : " + trackCurrent + "/" + trackCount;

        textBestTime.text = "Best Time : " + bestTime.ToString("F1") + "s";
        textTime.text = "Play Time : " + timer.ToString("F1") + "s";
        textFps.text = "Fps : " + deltaTime.ToString("F1") + "s";
        imageSpeed.fillAmount = speed / 120.0f;
    }



  

    void OnTriggerEnterF(ColliderCS coll)
    {
        if (coll.gameObject.CompareTag("StartLine"))
        {
            if (isStart)
                trackCurrent++;
            else
                isStart = true;

            if (trackCurrent == 1)
            {
                bestTime = timer;
            }
            else if (trackCurrent > 1 && (timer - bestTime < bestTime))
            {
                bestTime = timer - bestTime;
            }

            if (trackCount == trackCurrent)
            {
                player.isControll = false;
                isStart = false;
                player.rigid.AddForce(-player.rigid.velocity*0.5f, RigidbodyCS.ForceMode.Impulse);
                speed = 0;
                ShowInformationUI();
            }



        }
    }
}
