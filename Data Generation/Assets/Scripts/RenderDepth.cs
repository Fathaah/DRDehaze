using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class RenderDepth : MonoBehaviour
{   
    public Camera cam;
    public Material Mat;
    public Shader Depth;
    // Start is called before the first frame update
    void Start()
    {
        cam = this.GetComponent<Camera>();
        cam.depthTextureMode = DepthTextureMode.Depth;
        Mat = new Material(Depth);
    }

    private void OnPreRender(){
        Shader.SetGlobalMatrix(Shader.PropertyToID("UNITY_MATRIX_IV"), cam.cameraToWorldMatrix);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination){
        Graphics.Blit(source, destination);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
