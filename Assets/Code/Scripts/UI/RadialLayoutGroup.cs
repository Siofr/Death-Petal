using System;
using Unity.Mathematics;
using UnityEngine;


[ExecuteInEditMode]
public class RadialLayoutGroup : MonoBehaviour
{
    public Vector2 centerOffset;
    public float rotationOffset;
    public float radius;
    public float distance;

    public bool ignoreInactive = false;
    public bool rotateChildren = false;

    private float internalRadius;
    private float eligibleChildCount = 0;
    
    public void SetPointOnCircle(Transform childObject, int childCount)
    {
        //var distanceAdjustment = (distance * childCount / (Mathf.PI*2)) + (rotationOffset + transform.eulerAngles.z);
        
        var distanceAdjustment= ((distance/transform.childCount * -childCount) + rotationOffset + transform.eulerAngles.z)* (Mathf.PI/180);
        var x = transform.position.x + centerOffset.x + internalRadius * Mathf.Cos(distanceAdjustment);
        var y = transform.position.y + centerOffset.y + internalRadius * Mathf.Sin(distanceAdjustment);


        childObject.position = new Vector3(x, y, 0);
        
        if(!rotateChildren) return;
        
        var angleStep = distance /  transform.childCount;
        childObject.eulerAngles = new Vector3(0, 0, angleStep * childCount *-1);
        childObject.name = childCount.ToString();
    }

    public void Update()
    {
        internalRadius = Screen.width * (radius/50);
        for (int i = 0; i <= transform.childCount-1; i++)
        {
            SetPointOnCircle(transform.GetChild(i), i);
        }
    }

}
