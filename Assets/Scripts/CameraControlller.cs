
/************************************************************
* COMPONENT OF: Camera
* SCRIPT ATTACHED TO: Main Camera
* DESCRIPTION: It is used to controll the main camera
* AUTHOR: EEdward
* DATE WRITTEN: Sep 18 2026
* VERSION: 1.0
*************************************************************/

using UnityEngine;

public class CameraControlller : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Vector3 offset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = playerTransform.position + offset; 
    }
}
