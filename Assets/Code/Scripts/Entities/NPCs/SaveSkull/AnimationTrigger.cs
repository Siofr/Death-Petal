using System;
using UnityEngine;

public class AnimationTrigger : MonoBehaviour
{
    [SerializeField] private Animator _triggerAnimator;
    [SerializeField] private string _boolParameterName;


    private void OnTriggerEnter(Collider other)
    {
        _triggerAnimator.SetBool(_boolParameterName, true);
    }

    private void OnTriggerExit(Collider other)
    {
        _triggerAnimator.SetBool(_boolParameterName, false);
    }
}
