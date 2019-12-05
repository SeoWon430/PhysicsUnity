using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool isMultiPlay;
    public static GameManager instance { get; private set; }
    public List<Player> players;
    public Track[] trackMaps;
    public Track track { get; private set; }

    public bool isReady;
    public bool isStart;
    public float timer { get; private set; }



    void Awake()
    {
        if(instance ==null)
            instance = this;

        StartManager start = FindObjectOfType<StartManager>();
        if (start != null)
            isMultiPlay = start.isMulitPlay;
        else
            isMultiPlay = false;
    }



    void Start()
    {
        int randomTrack = Random.Range(0, trackMaps.Length);
        track = SetTrack(randomTrack);
        isReady = false;
        isStart = false;
        timer = 2.95f;
    }



    void FixedUpdate()
    {
        if (!isReady)
        {
            if (track.playerCount <= players.Count || !isMultiPlay)
            {
                isReady = PlayReady();
            }
        }
        else if(isReady && !isStart)
        {
            timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                timer = 0;
                    isStart = true;
                    PlayStart();
                }
        }
        else if (isReady && isStart)
        {
            timer += Time.deltaTime;

            if (PlayEnd())
                LoadMainScene();

        }

    }



    bool PlayReady()
    {
        bool playReady = true;
        for (int i = 0; i < players.Count; i++)
        {
            playReady = playReady && players[i].Init(track, i);
        }

        return playReady;
    }



    void PlayStart()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].playerMovement.isControll = true;
            players[i].textReady.gameObject.SetActive(false);
            players[i].canvasPlay.SetActive(true);
        }
    }


    bool PlayEnd()
    {
        bool playEnd = true;
        for (int i = 0; i < players.Count; i++)
        {
            playEnd = playEnd && players[i].isEnd;
        }

        return playEnd;
    }

    public Track SetTrack(int trackNumber)
    {
        if (trackNumber < 0 || trackNumber >= trackMaps.Length)
            return null;

        GameObject track = Instantiate(trackMaps[trackNumber].gameObject);

        return track.GetComponent<Track>();
    }


    public void LoadMainScene()
    {
        SceneManager.LoadScene(0);
    }

}
