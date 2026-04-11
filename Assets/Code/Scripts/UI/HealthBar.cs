
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image actualBarImage;
    public Image echoBarImage;
    public float echoBarTime = 0.25f;
    public LeanTweenType echoAnimationType;

    private float _barValue;
    public float BarValue
    {
        get
        {
            return _barValue;
        }
        set
        {
            

            if(value != _barValue)
            {
                actualBarImage.fillAmount = value;
                EchoHealth(value);

                //StartCoroutine(LerpValue(value));
            }

            _barValue = value;
        }
    }

    void EchoHealth(float target)
    {
        if (LeanTween.isTweening(echoBarImage.gameObject))
        {
            LeanTween.cancel(echoBarImage.gameObject);
        }

        LeanTween.value(echoBarImage.gameObject ,echoBarImage.fillAmount, target, echoBarTime).setOnUpdate((float val) => {  echoBarImage.fillAmount = val; }).setEase(echoAnimationType);

        
    }

    /* float lerpErrorOffset = 0.005f;
    public float lerpSpeed = 0.5f;

    IEnumerator LerpValue(float target)
    {
        while (echoBarImage.fillAmount > target + lerpErrorOffset)
        {
            echoBarImage.fillAmount = Mathf.Lerp(echoBarImage.fillAmount, target, lerpSpeed);

            yield return null;
        }

        echoBarImage.fillAmount = target;
    } */
}
