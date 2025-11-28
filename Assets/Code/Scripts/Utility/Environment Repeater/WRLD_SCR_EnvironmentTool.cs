using System;
using System.Collections.Generic;
//using TreeEditor;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class WRLD_SCR_EnvironmentTool : MonoBehaviour
{
}
/*
 [SerializeField] private List<GameObject> sequence;
 [SerializeField] private List<GameObject> activeSequence;
 [SerializeField] private BoxCollider bounding;
 [SerializeField] private bool lockActiveSequence;
 [SerializeField] private bool flip;

 private Vector3 boundingCenter;
 private Vector3 posEdgePositionLocal;
 private Vector3 posEdgePositionWorld;
 private Vector3 negEdgePositionWorld;

 [SerializeField] private Vector3 nextSlotPosition;
 private int wallIterations;
 private bool isContinueValid = true;

 private void Start()
 {
     #if UNITY_EDITOR


         for (int i = 0; i <= transform.childCount; i++)
         {
             activeSequence.Add(transform.GetChild(i).gameObject);
         }
         bounding = GetComponent<BoxCollider>();



         nextSlotPosition = Vector3.zero;
     #endif
 }

 void Update()
 {
     #if UNITY_EDITOR
         if(lockActiveSequence) return;

         GetBounds();
         DrawSequence();
     #endif
 }

 private void GetBounds()
 {
     if (bounding.center != boundingCenter)
     {
         KillActiveSequence();
     }
     boundingCenter = bounding.center;
     posEdgePositionLocal = (bounding.size.x > bounding.size.y) ? new Vector3(bounding.size.x/2, 0, 0) : new Vector3(0, 0, bounding.size.z/2);
     posEdgePositionWorld = bounding.transform.position + bounding.center +  posEdgePositionLocal;
     negEdgePositionWorld = bounding.transform.position + bounding.center + (-1 * posEdgePositionLocal);

     if(nextSlotPosition == Vector3.zero) nextSlotPosition = negEdgePositionWorld;
 }

 private void DrawSequence()
 {
     var iterations = 0;
     while (isContinueValid )
     {
         foreach (GameObject o in sequence)
         {
             // edit direction
             var swapFlowDirection = (posEdgePositionLocal.x == 0);

             nextSlotPosition += (swapFlowDirection) ?
                 new Vector3(0, 0, o.GetComponent<BoxCollider>().size.x/2) :
                 new Vector3(o.GetComponent<BoxCollider>().size.x/2, 0, 0);

             print(o.name.ToUpper() + " | " + o.GetComponent<BoxCollider>().size.x);

             nextSlotPosition.y = 0;
                                  //bounding.transform.position.y -bounding.bounds.size.y / 2 + o.GetComponent<BoxCollider>().size.x;

             isContinueValid = (swapFlowDirection) ? !(nextSlotPosition.z > posEdgePositionWorld.z) :
                 !(nextSlotPosition.x > posEdgePositionWorld.x);
             if (!isContinueValid) return;

             var newSection = Instantiate(o, nextSlotPosition, Quaternion.identity);
             newSection.transform.SetParent(transform);
             newSection.transform.localPosition =
                 new Vector3(newSection.transform.localPosition.x,
                     0,
                     newSection.transform.localPosition.z);
             if(swapFlowDirection)
                 newSection.transform.Rotate(Vector3.up, 90);
             if(flip)
                 newSection.transform.Rotate(Vector3.up, 180);

             nextSlotPosition += (swapFlowDirection) ?
                 new Vector3(0, 0, o.GetComponent<BoxCollider>().size.x/2) :
                 new Vector3(o.GetComponent<BoxCollider>().size.x/2, 0, 0);


             activeSequence.Add(newSection);
         }
         iterations++;
     }

 }

 private void KillActiveSequence()
 {
     activeSequence.ForEach(DestroyImmediate);
     activeSequence = new List<GameObject>();
     nextSlotPosition = Vector3.zero;
     isContinueValid = true;
 }

}
*/
#endif
