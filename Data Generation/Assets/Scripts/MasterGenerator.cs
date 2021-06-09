using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System.IO;

public class MasterGenerator : MonoBehaviour
{
    Camera camera;
    GameObject[] obj;
    int len_objs;
    public int no_env_obj = 20;
    public bool manual;
    GameObject temp; 
    public GameObject background, floor;
    GameObject[] gameObjects;
    GameObject[] lights;
    public GameObject Volume;
    public Transform SpawnMainLight;
    public Transform SpawnSmallLight;
    public Transform SpawnNearObj;
    public Transform SpawnMidObj;
    public Transform SpawnFarObj;
    Texture2D tex;
    float distance;
    float frustumHeight;
    float frustumWidth;
    float fogDis;
    int idx_obj;
    public Light Main_Light;
    public HDAdditionalLightData Main_Light_HD;
    public float rot_x_max, rot_y_max, rot_z_max;
    public float rot_x_min, rot_y_min, rot_z_min;
    Cubemap[] skyDays, skyNight;
    Fog fog;
    GradientSky sky;
    List<string> txs;
    string[] arrtxs, arrNightSky, arrDaySky;
    float timer = 2.0f;
    bool timerSet = false;
    float time;
    int start = 0;
    bool haze = false;
    int txsLen;
    byte[] fileData;
    float clr;
    int lenSkyDays;
    ColorAdjustments  clrAdj;
    float meanFreePath;
    void Start()
    {   Volume volume = Volume.GetComponent<Volume>();
        //print(volume.profile.components.Count);
        
        if(!volume.profile.TryGet<Fog>(out fog)){
            fog = volume.profile.Add<Fog>(false);
        }
        if(!volume.profile.TryGet<ColorAdjustments >(out clrAdj)){
            clrAdj = volume.profile.Add<ColorAdjustments >(false);
        }
        if(!volume.profile.TryGet<GradientSky>(out sky)){
            sky = volume.profile.Add<GradientSky>(false);
        }
        // fog.albedo.value = new Color((float)Random.Range(0f, 1f), 
        //                                                 (float)Random.Range(0f, 1f), 
        //                                                 (float)Random.Range(0f, 1f),
        //                                                 1);
        string path = "D:/SynData/Textures";
        txs = new List<string>();
        foreach (string file in System.IO.Directory.GetFiles(path,"*.*", SearchOption.AllDirectories))
        { txs.Add(file);}
        txsLen = txs.Count;
        arrtxs = txs.ToArray();
        //print(txsLen);
        camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        obj = Resources.LoadAll<GameObject>("Models");
        lights = Resources.LoadAll<GameObject>("Lights");
        Main_Light = GameObject.FindWithTag("Main_Light").GetComponent<Light>();
        skyDays = Resources.LoadAll<Cubemap>("SkyBoxDay");
        skyNight = Resources.LoadAll<Cubemap>("SkyBoxNight");
        lenSkyDays = skyDays.Length;
        len_objs = (obj.Length);
        print(obj.Length);
        print(lights.Length);
        print(skyDays.Length);
    }


    void Update(){
        
        if(timerSet){
            timer = timer - Time.deltaTime;
            if(timer < 0f){
                timerSet = false;
                if(!haze){
                    string z = "D:/SynData/HazeData/Twelth/X/" + start + "_0_0.png";
                    ScreenCapture.CaptureScreenshot(z);
                }
                else{
                    string z = "D:/SynData/HazeData/Twelth/Y/" + start + ".png";
                    ScreenCapture.CaptureScreenshot(z);
                    start++;
                }
            }
        }
        else{   if(manual){        
                if (Input.GetButton("Fire1"))
                {
                    removeHaze();
                }
                if (Input.GetButton("Fire2"))
                {     
                    addHaze();
                    destroyAll();
                    generateBackground();  
                    
                }
            }
            else{
                    if(haze)
                    {   
                        addHaze();
                        destroyAll();
                        generateBackground();
                        for(int i = 0; i < 100000000; i++){
                            i++;
                        }

                        for(int i = 0; i < 1000000; i++){
                            i++;
                        }
                        timer = 0.5f;
                        timerSet = true;
                        haze = false;
                        }
                    else{
                        for(int i = 0; i < 100000000; i++){
                            i++;
                        }

                        for(int i = 0; i < 1000000; i++){
                            i++;
                        }
                        removeHaze();
                        timer = 0.5f;
                        timerSet = true; 
                        for(int i = 0; i < 1000000; i++){
                            i++;
                        }
                        haze = true;
                        }
                            
            }}
        // while(true){
        //     destroyAll();
        //     generateBackground();
        //     StartCoroutine( Order() );
        // }
    }

    void generateBackground()
    {   //generateWallpaper();
        fogDis = Random.Range(18f, 50f);
        clr = (float)Random.Range(150/255f, 250/255f);
        makeSky();
        //generateFloor(floor);
        //generateWallpaper(background);
        if(Random.Range(0.0f , 1.0f) < 0.5f)
            no_env_obj = Random.Range(90, 150);
        else
            no_env_obj = Random.Range(150, 250);
        print(no_env_obj);
        for(int i = 0;i < no_env_obj;i++)
        {   
            float rand = Random.Range(0.0f , 1.0f);
            //print(rand);
            if(rand < 1f / 6f){
                spawnNear();
                //print("Near");
                continue;
            }
            if(rand > 1f / 6f && rand < 3f / 6f)
            {    spawnMid();
                //print("Mid");
            }
            if(rand > 2f / 6f)
            {    spawnFar();
                 //print("Far");
            }
        }
        //spawnFarBack();
        //spawn_target_object();
        enLighten();
        
    }

    void makeSky(){
        
        //HDRI SKY
        
        // int idx = Random.Range(0, lenSkyDays);
        // sky.hdriSky.value = skyDays[idx];
        // sky.multiplier.value = Random.Range(0.5f, 1.2f);
        // sky.rotation.value = Random.Range(0f, 360f);
        // clrAdj.hueShift.value = Random.Range(-1.0f, 1.0f);
        // clrAdj.saturation.value = Random.Range(-10.0f, 10.0f);

        //Gradient Sky
        
        sky.top.value = new Color(clr, clr, clr);//new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        sky.middle.value = new Color(clr, clr, clr);;
        sky.bottom.value = new Color(clr, clr, clr);;

    }
    void generateFloor(GameObject background){
        //background.transform.position = new Vector3(background.transform.position.x, background.transform.position.y,Random.Range(30f, 80f));
        Material backMat = background.GetComponent<MeshRenderer>().material;
        //background.transform.Rotate(0f, 0.0f, Random.Range(0f, 360f), Space.World);
        int idx_back = Random.Range(0, txsLen);
        string filepath = arrtxs[idx_back];
        fileData = File.ReadAllBytes(filepath);
        tex = new Texture2D(2, 2);
        tex.LoadImage(fileData);
        backMat.SetFloat("_Metallic", Random.Range(0f,1f));
        backMat.SetFloat("_Smoothness", Random.Range(0f,1f));
        backMat.SetTexture("_BaseColorMap",tex); 
        background.transform.position = new Vector3(background.transform.position.x, Random.Range(-10, 0), background.transform.position.x); 
    }

    void generateWallpaper(GameObject background){
        background.transform.localScale = new Vector3(background.transform.localScale.x, background.transform.localScale.y,2 * fogDis * 1 / 0.15f);
        Material backMat = background.GetComponent<MeshRenderer>().material;
        //background.transform.Rotate(0f, 0.0f, Random.Range(0f, 360f), Space.World);
        backMat.SetFloat("_Metallic", 0);
        backMat.SetFloat("_Smoothness", 1);
        backMat.SetColor("_BaseColor",new Color((float)clr, 
                                                        (float)clr, 
                                                        (float)clr,
                                                        1)); 
    }

    void spawnNear(){
        idx_obj = Random.Range(0, len_objs);
        distance = Random.Range(3f, 15f);
        frustumHeight = 1.2f * 2.0f * distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        frustumWidth = 1.0f * frustumHeight * camera.aspect;
        Vector3 obj_loc = new Vector3(Random.Range(camera.transform.position.x - frustumWidth / 2 + 1f, camera.transform.position.x + frustumWidth / 2 - 1f),
            Random.Range(camera.transform.position.y - frustumHeight / 2 + 1f, camera.transform.position.y + frustumHeight / 2 - 3f),
            1.5f + camera.transform.position.z + distance);
        place_obj(obj[idx_obj],obj_loc, Random.Range(0.2f,3f));        
    }


    void spawnMid(){
        idx_obj = Random.Range(0, len_objs);
        distance = Random.Range(15, 40);
        frustumHeight = 1.2f * 2.0f * distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        frustumWidth = 1.0f * frustumHeight * camera.aspect;
        Vector3 obj_loc = new Vector3(Random.Range(camera.transform.position.x - frustumWidth / 2 + 1f, camera.transform.position.x + frustumWidth / 2 - 1f),
            Random.Range(camera.transform.position.y - frustumHeight / 2 + 1f, camera.transform.position.y + frustumHeight / 2 - 3f),
            1.5f + camera.transform.position.z + distance);
        place_obj(obj[idx_obj],obj_loc,Random.Range(3f,10f));
    }


    void spawnFar(){
        idx_obj = Random.Range(0, len_objs);
        distance = Random.Range(40f, 150f);
        frustumHeight = 1.2f * 2.0f * distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        frustumWidth = 1.0f * frustumHeight * camera.aspect;
        Vector3 obj_loc = new Vector3(Random.Range(camera.transform.position.x - frustumWidth / 2 + 1f, camera.transform.position.x + frustumWidth / 2 + 1f),
            Random.Range(camera.transform.position.y - frustumHeight / 2 + 1f, camera.transform.position.y + frustumHeight / 2 - 3f),
            1.5f + camera.transform.position.z + distance);
        place_obj(obj[idx_obj],obj_loc, Random.Range(15f,30f));
    }

    void spawnFarBack(){
        idx_obj = Random.Range(0, len_objs);
        distance = Random.Range(80f, 200f);
        frustumHeight = 1.2f * 2.0f * distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        frustumWidth = 1.0f * frustumHeight * camera.aspect;
        Vector3 obj_loc = new Vector3(Random.Range(camera.transform.position.x - frustumWidth / 2 + 1f, camera.transform.position.x + frustumWidth / 2 + 1f),
            Random.Range(camera.transform.position.y - frustumHeight / 2 + 1f, camera.transform.position.y + frustumHeight / 2 - 3f),
            1.5f + camera.transform.position.z + distance);
        place_obj(obj[idx_obj],obj_loc, Random.Range(400f,500f));
    }

    void place_obj(GameObject new_obj, Vector3 loc, float scale_fact){
        temp = Instantiate(new_obj, loc, Random.rotation);   
        //scale_fact =  Random.Range(0.2f,5f);
        temp.transform.localScale = new Vector3(scale_fact,scale_fact,scale_fact);
        MeshRenderer mesh = temp.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        //temp.isStatic = true;
        temp.tag = "Scene_object";
        MeshRenderer[] rend;
        Material[] mats;
        foreach (Transform child in temp.transform){
            rend = child.gameObject.GetComponents<MeshRenderer>();
            foreach(MeshRenderer r  in rend){
                mats = r.materials;
                foreach(Material m in mats){
                    //print("Gosh");
                    float rand1 = Random.Range(0.0f, 1.0f);
                    if(rand1 < 0.85f){
                        //print("randomised");
                        m.SetColor("_BaseColor", new Color((float)Random.Range(0f, 1f), 
                                                        (float)Random.Range(0f, 1f), 
                                                        (float)Random.Range(0f, 1f),
                                                        1));}
                    else{
                        m.SetColor("_BaseColor", new Color((float)Random.Range(0.9f, 1f), 
                                                        (float)Random.Range(0.9f, 1f), 
                                                        (float)Random.Range(0.9f, 1f),
                                                        1));
                    }
                    m.SetFloat("_Metallic", Random.Range(0f,0.1f));
                    m.SetFloat("_Smoothness", Random.Range(0f,1f));
                }
            }
        }
        //mat1 = temp.transform.GetChild(0).GetComponentsInChildren<Material>();
        // mat1[0].SetColor("_Color", Color.red);
//
    }


    private static Quaternion Change(float x, float y, float z)
    {
        Quaternion newQuaternion = new Quaternion();
        newQuaternion.Set(x, y, z, 1);
        //Return the new Quaternion
        return newQuaternion;
    }

    void enLighten(){
        int no_lights = Random.Range(2, 4);
        for(int k = 0;k < no_lights;k++){

            idx_obj = Random.Range(0, lights.Length);
            distance = Random.Range(3f , 15.0f);
            frustumHeight = 2.0f * distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            frustumWidth = frustumHeight * camera.aspect;
            Vector3 obj_loc = new Vector3(Random.Range(SpawnSmallLight.position.x - SpawnSmallLight.localScale.x / 2, SpawnSmallLight.position.x + SpawnSmallLight.localScale.x / 2),
                                                                            Random.Range(SpawnSmallLight.position.y - SpawnSmallLight.localScale.y / 2, SpawnSmallLight.position.y + SpawnSmallLight.localScale.y / 2),
                                                                            Random.Range(SpawnSmallLight.position.z - SpawnSmallLight.localScale.z / 2, SpawnSmallLight.position.z + SpawnSmallLight.localScale.z / 2));

            Quaternion target = Quaternion.Euler(Random.Range(90 , 135), 0, 0);
            temp = Instantiate(lights[0], obj_loc, target); //PLEASE CHANGE
            temp.tag = "Scene_object";
            temp.GetComponent<Light>().color = new Color((float)Random.Range(0f, 1f), 
                                                        (float)Random.Range(0f, 1f), 
                                                        (float)Random.Range(0f, 1f),
                                                        1);
            temp.GetComponent<Light>().intensity = Random.Range(40, 4000);
            temp.GetComponent<Light>().range = Random.Range(10f,30f);
        }
        //Main_Light.color =  new Color((float)Random.Range(0f, 1f), 
                                                        // (float)Random.Range(0f, 1f), 
                                                        // (float)Random.Range(0f, 1f),
                                                        // 1);
        Main_Light.colorTemperature = Random.Range(3000, 12000);
        //sky.rotation.value = Random.Range(0,360);
        GameObject.FindWithTag("Main_Light").GetComponent<Light>().intensity = Random.Range(0,1f);
        Main_Light_HD.intensity =  Random.Range(0,10f);
        GameObject.FindWithTag("Main_Light").transform.position = new Vector3(Random.Range(SpawnMainLight.position.x - SpawnMainLight.localScale.x / 2, SpawnMainLight.position.x + SpawnMainLight.localScale.x / 2),
                                                                            Random.Range(SpawnMainLight.position.y - SpawnMainLight.localScale.y / 2, SpawnMainLight.position.y + SpawnMainLight.localScale.y / 2),
                                                                            Random.Range(SpawnMainLight.position.z - SpawnMainLight.localScale.z / 2, SpawnMainLight.position.z + SpawnMainLight.localScale.z / 2));
        
        // float rotX = Random.Range(45f, 135f);
        // float rotY = Random.Range(45f, 135f);
        GameObject.FindWithTag("Main_Light").transform.localRotation = Quaternion.Euler(Random.Range(45f, 135f),Random.Range(45f, 135f),0f);
    }

    void destroyAll()
    {
        gameObjects = GameObject.FindGameObjectsWithTag("Scene_object");

        for (var i = 0; i < gameObjects.Length; i++)
        {
            Destroy(gameObjects[i]);
        }
        Resources.UnloadUnusedAssets();
    }

    void addHaze(){
        //print("Hello There");
        // RenderSettings.fog = true;
        // RenderSettings.fogDensity = (float)Random.Range(0.08f, 0.25f);
        // RenderSettings.fogMode = FogMode.ExponentialSquared;
        fog.active = true;
        
        // RenderSettings.fogColor = new Color(clr, clr, clr);
        fog.baseHeight.value = Random.Range(0.1f, 5f);
        fog.active = true;
        fog.albedo.value = new Color(clr, clr, clr);
        fog.color.value = new Color(clr, clr, clr);
        fog.anisotropy.value = Random.Range(-1.0f, 1.0f);
        fog.meanFreePath.value = fogDis;
        //clrAdj.postExposure.value = 0.8f;
    }
    void removeHaze(){
        //fog.active = false;
        //clrAdj.postExposure.value = 1f;
        fog.meanFreePath.value = fogDis * 10;
        // RenderSettings.fog = true;
        // RenderSettings.fogDensity = (float)Random.Range(0.8f, 0.25f);
        // RenderSettings.fogMode = FogMode.Linear;
        // RenderSettings.fogStartDistance = 8f;
        // RenderSettings.fogEndDistance = 20f;
        // float clr = (float)Random.Range(180/255f, 210/255f);
        // RenderSettings.fogColor = new Color(clr, clr, clr);
    }
}
