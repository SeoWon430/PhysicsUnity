using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    public bool isMulitPlay;
    public GameObject ImageControlKey;
    public Text textTitle;
    public Color textColor;

    private float colorTime;
    private int add;

    // Start is called before the first frame update
    void Start()
    {
        isMulitPlay = false;
        ImageControlKey.SetActive(false);
        DontDestroyOnLoad(this.gameObject);
        colorTime = 0;
        add = 1;
    }

    // Update is called once per frame
    void Update()
    {
        colorTime += Time.deltaTime/2 * add;
        if (colorTime >= 1f || colorTime <= 0f )
            add *= -1;

        textColor = Color.Lerp(Color.red, Color.yellow, colorTime);
        if(textTitle != null)
            textTitle.color = textColor;
    }

    public void ShowControlKey(bool show)
    {

        ImageControlKey.SetActive(show);
    }



    public void Play(bool isMulti)
    {
        isMulitPlay = isMulti;
        SceneManager.LoadScene(1);
        Destroy(this.gameObject, 3.0f);
    }


    public void Exit()
    {
        Application.Quit();
    }
}
