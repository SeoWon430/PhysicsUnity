using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class CameraEffect : MonoBehaviour
{
    public Shader curShader;
    public float grayScale;
    public float depthPower;
    public bool isEffect;
    private Material screenMaterial;

    Material ScreenMaterial
    {
        get
        {
            if (screenMaterial == null)
            {
                screenMaterial = new Material(curShader);
                screenMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return screenMaterial;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        isEffect = false;

        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }
        if(!curShader && !curShader.isSupported)
        {
            enabled = false;
        }
    }


    void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
    {
        if(curShader != null)
        {
            ScreenMaterial.SetFloat("_Luminosity", grayScale);
            Graphics.Blit(sourceTexture, destTexture, ScreenMaterial);

            ScreenMaterial.SetFloat("_DepthPower", depthPower);
            Graphics.Blit(sourceTexture, destTexture, ScreenMaterial);
        }
        else
        {
            Graphics.Blit(sourceTexture, destTexture);
        }
    }


    // Update is called once per frame
    void Update()
    {
        grayScale = Mathf.Clamp(grayScale, 0f, 1f);
        Camera.main.depthTextureMode = DepthTextureMode.Depth;
        depthPower = Mathf.Clamp(depthPower, 0f, 1f);
    }

    void OnDisable()
    {
        DestroyImmediate(screenMaterial);
    }
}
