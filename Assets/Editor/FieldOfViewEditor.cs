using System;
using UnityEditor;using UnityEngine;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor  // Start is called once before the first execution of Update after the MonoBehaviour is created
{
    private void OnSceneGUI()
    {
        FieldOfView fov = (FieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position,Vector3.up,Vector3.forward,360,fov.GetViewRadius());
        Vector3 viewAngleA = fov.DirFromAngle(-fov.GetViewAngle() / 2, false);
        Vector3 viewAngleB = fov.DirFromAngle(fov.GetViewAngle() / 2, false);
        
        Handles.DrawLine(fov.transform.position,fov.transform.position + viewAngleA * fov.GetViewRadius());
        Handles.DrawLine(fov.transform.position,fov.transform.position + viewAngleB * fov.GetViewRadius());
    }
}
