    using System.Collections.Generic;
    using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent (typeof(BoxCollider))]
public class CameraArea : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _cam;
    
    private EventBindings<CameraChangeEvent> _onCameraChange;

    public bool isActiveArea;

    private void OnEnable()
    {
        _onCameraChange = new EventBindings<CameraChangeEvent>(OnCameraChange);
        EventBus<CameraChangeEvent>.Register(_onCameraChange);
    }

    private void OnDisable()
    {
        EventBus<CameraChangeEvent>.Unregister(_onCameraChange);
    }

    private void OnCameraChange(CameraChangeEvent ctx)
    {
        if (ctx.cam != _cam) _cam.gameObject.SetActive(false);
        else _cam.gameObject.SetActive(true);
    }
    
    /* private void OnTriggerEnter(Collider other)
    {
        print($"PlayerEntered {name}");

        isActiveArea = true;
        
        if (other.transform.CompareTag("Player"))
        {
            // Trigger Event
            EventBus<CameraChangeEvent>.Raise(new CameraChangeEvent(_cam.transform, _cam));
        }
    } */

    //Changing over to stay, as it gets called ultimatly anyway (if it tanks performance you know why) -eva
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            // Trigger Event
            EventBus<CameraChangeEvent>.Raise(new CameraChangeEvent(_cam.transform, _cam, CheckEntities()));
        }
    }

    private EntityBase[] CheckEntities()
    {
        var col =  GetComponent<BoxCollider>();
        var tempCol = Physics.OverlapBox(col.bounds.center, col.bounds.extents, col.transform.rotation);
        var tempEntities = new List<EntityBase>();

        foreach (var collider in tempCol)
        {
            if(collider.TryGetComponent(out EntityBase entity)) tempEntities.Add(entity);
        }
        
        return tempEntities.ToArray();
    }

    private void OnDrawGizmos()
    {
        foreach (var col in GetComponents<BoxCollider>())
        {
        // Draw wire box around area collider
        Gizmos.color = new Color(0.2f, 0f, 1f, 0.5f); // Purple

        //BoxCollider boxCollider = GetComponent<BoxCollider>();
        BoxCollider boxCollider = col;
        Vector3 boxSize = new Vector3(boxCollider.size.x, boxCollider.size.y, boxCollider.size.z);
        Vector3 boxPosition = new Vector3(boxCollider.bounds.center.x, boxCollider.bounds.center.y, boxCollider.bounds.center.z);

        Gizmos.DrawWireCube(boxPosition, boxSize);

        Gizmos.color = new Color(0.2f, 0f, 1f, 0.2f); // More transparent Purple

        Gizmos.DrawCube(boxPosition, boxSize);
        }

        // Draw Line from collider position to camera position

        Gizmos.color = new Color(0f, 0f, 1f, 0.5f); // Blue
    }
}
