using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Pattern
    {
        NONE,
        GRAY,
        BACK,
        COUNT
    }

    public Pattern pattern;
    private Renderer render;
    private BoxColliderCS collider;



    // Start is called before the first frame update
    void Start()
    {
        render = GetComponent<Renderer>();
        collider = GetComponent<BoxColliderCS>();
        //Init();
    }



    void Init()
    {
        if (pattern == Pattern.NONE)
        {
            pattern = (Pattern)Random.Range(0, (int)Pattern.COUNT);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void OnTriggerEnterF(ColliderCS coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Hide());

            switch (pattern)
            {
                case Pattern.NONE:
                    break;
                case Pattern.GRAY:

                    CameraEffect effect = coll.gameObject.GetComponentInChildren<CameraEffect>();
                    if (effect != null && !effect.isEffect)
                    {
                        StartCoroutine(SetEffect(effect));
                    }
                    break;
                case Pattern.BACK:
                    ResetPosition back = coll.gameObject.GetComponentInChildren<ResetPosition>();
                    if (back != null)
                        back.ReturnPosition();
                    break;
                default:
                    break;
            }

        }
    }

    IEnumerator Hide()
    {
        render.enabled = false;
        collider.enabled = false;
        yield return new WaitForSeconds(5f);
        render.enabled = true;
        collider.enabled = true;
    }
    IEnumerator SetEffect(CameraEffect effect)
    {
        effect.isEffect = true;
        effect.grayScale = Random.Range(0.5f, 1f);
        int depthEffect = Random.Range(0, 5);
        if(depthEffect >= 2)
            effect.depthPower = (float)depthEffect / 10f;

        yield return new WaitForSeconds(3f);

        effect.isEffect = false;
        effect.grayScale = 0;
        effect.depthPower = 0;
    }
}
