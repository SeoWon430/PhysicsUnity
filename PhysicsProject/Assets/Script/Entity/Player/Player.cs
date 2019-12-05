using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Movement playerMovement;
    public GameObject backCamera;

    public GameObject endPosition;


    public GameObject canvasPlay;
    public Text textReady;
    public Text textTrackName;
    public Text textSpeed;
    public Text textTrackCurrent;
    public Text textBestTime;
    public Text textTime;
    public Text textFps;
    public Image imageSpeed;

    private bool isTrack;
    public bool isEnd { get; private set; }

    private bool isInit;
    private float[] trackTime;
    private float bestTime;

    private int trackCount;
    private int trackCurrent;

    private int speed;

    private float fps = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        isInit = false;
        GameManager.instance.players.Add(this);
        canvasPlay.SetActive(false);
        isTrack = true;
        isEnd = false;

    }

    public bool Init(Track track, int index)
    {
        if (!isInit)
        {
            trackTime = new float[track.trackCount];
            for (int i = 0; i < trackTime.Length; i++)
            {
                trackTime[i]=600f;
            }



            textTrackName.text = track.trackName;
            this.transform.position = track.startPoints[index].transform.position;
            isInit = true;
            trackCount = track.trackCount;
            bestTime = 0;
            trackCurrent = 0;
            ShowInformationUI();
        }

        return true;
    }



    void FixedUpdate()
    {
        if (!GameManager.instance.isReady && !GameManager.instance.isStart)
        {
            textReady.text = "Waiting Players...\n" + GameManager.instance.players.Count + "/" + GameManager.instance.track.playerCount;

        }
        else if (GameManager.instance.isReady && !GameManager.instance.isStart)
        {
            textReady.fontSize = 160;
            textReady.text = ((int)GameManager.instance.timer + 1).ToString();

        }
        else if (GameManager.instance.isReady && GameManager.instance.isStart)
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                if (backCamera.activeSelf)
                    backCamera.SetActive(false);
                else
                    backCamera.SetActive(true);
            }

        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            this.transform.position = endPosition.transform.position;

        }
    }



    // Update is called once per frame
    void LateUpdate()
    {
        speed = (int)playerMovement.rigid.velocity.magnitude;
        if (GameManager.instance.isStart && !isEnd)
        {
            ShowInformationUI();
        }
        if(!isEnd && trackCount == trackCurrent && speed < 0.5f)
        {
            isEnd = true;
        }

    }


    void ShowInformationUI()
    {
        textBestTime.text = "Best Time : " + bestTime.ToString("F1") + "s";
        textTime.text = "Play Time : " + GameManager.instance.timer.ToString("F1") + "s";
        textFps.text = "Fps : " + fps.ToString("F1") + "s";
        if (Time.time % 2 < 0.1f)
            fps = 1 / Time.deltaTime;

        textSpeed.text = speed.ToString();
        textTrackCurrent.text = "Track : " + trackCurrent + "/" + trackCount;

        imageSpeed.fillAmount = speed / 120.0f;
    }



    void PlaySound()
    {

    }


    void OnCollisionEnterF(ColliderCS coll)
    {
    }

    void OnTriggerEnterF(ColliderCS coll)
    {
        
        if (coll.gameObject.CompareTag("StartLine"))
        {
            if (GameManager.instance.isStart && !isTrack)
            {
                isTrack = true;
                trackCurrent++;

                float sumTime = 0;
                for (int i = 0; i < trackTime.Length; i++)
                {
                    if (trackTime[i] < 500f)
                        sumTime += trackTime[i];
                    //Debug.Log(i+" /" +sumTime);
                }
                trackTime[trackCurrent - 1] = GameManager.instance.timer - sumTime;


                bestTime = Mathf.Min(trackTime);

                if (trackCount == trackCurrent)
                {
                    playerMovement.isControll = false;
                    playerMovement.rigid.AddForce(-playerMovement.rigid.velocity * 0.5f, RigidbodyCS.ForceMode.Impulse);
                    speed = 0;
                }

                ShowInformationUI();
            }




        }
        else if (coll.gameObject.CompareTag("EndLine"))
        {
            isTrack = false;
        }
    }

}
