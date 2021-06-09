using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGizmos : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    void OnDrawGizmosSelected(){
        Gizmos.color = new Color(0, 1.0f, 0, 0.3f);
        Gizmos.DrawCube(this.transform.position, this.transform.lossyScale);
    }

}
