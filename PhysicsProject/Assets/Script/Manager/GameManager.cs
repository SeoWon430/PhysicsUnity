using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject[] trackMaps;
    private GameObject track;



    // Start is called before the first frame update
    void Start()
    {
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init()
    {

    }



    public Track SetTrack(int trackNumber)
    {
        if (trackNumber < 0 || trackNumber >= trackMaps.Length)
            return null;

        GameObject track = Instantiate(trackMaps[trackNumber]);

        return track.GetComponent<Track>();
    }

}
