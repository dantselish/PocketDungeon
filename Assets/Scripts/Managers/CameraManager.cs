using System;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera MainCamera;

    public bool RaycastFromCamera(float maxDistance, int layerMask, out RaycastHit raycastHit)
    {
        Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
        
        return Physics.Raycast(ray, out raycastHit, maxDistance, layerMask);
    }
}
