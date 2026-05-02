using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BossBar : MonoBehaviour
{
    private float _relativeHP;
    private float _innitMaxHP;

    public float actualHpLerpSpeed = 0.75f;
    public float laggedHpLerpSpeed = 0.25f;

    public GameObject weaknessRef;
    public BossBase bossRef;

    public Image actualHealthImage;
    public Image laggedHealthImage;

    private EventBindings<CorrectShotEvent> _onCorrectShotListener;

    private void OnEnable()
    {
        _innitMaxHP = GetCurrentHP();
        _relativeHP = GetRelativeHp();

        _onCorrectShotListener = new EventBindings<CorrectShotEvent>(OnCorrectShot);
        EventBus<CorrectShotEvent>.Register(_onCorrectShotListener);
    }

    private void OnDisable()
    {
        EventBus<CorrectShotEvent>.Unregister(_onCorrectShotListener);
    }
    private float GetCurrentHP()
    {
        float maxHp = 0f;
        maxHp = weaknessRef.GetComponentsInChildren<Weakness>().Count();
        return maxHp;
    }

    private float GetRelativeHp()
    {
        float relativeHp = 0f;
        relativeHp = _innitMaxHP / GetCurrentHP();
        return relativeHp;
    }

    //===

    private void OnCorrectShot(CorrectShotEvent ctx)
    {
        //ctx.enemy = bossRef;

        _relativeHP = GetRelativeHp();
        _lerpHp = true;
        _lerpLaggedHp = true;
    }

    private bool _lerpHp;
    private bool _lerpLaggedHp;

    private void Update()
    {
        if(_lerpHp)
        {
            actualHealthImage.fillAmount = Mathf.Lerp(actualHealthImage.fillAmount, _relativeHP, actualHpLerpSpeed);

            if (actualHealthImage.fillAmount + 0.01f <= _relativeHP) _lerpHp = false;
        }
        if(_lerpLaggedHp)
        {
            laggedHealthImage.fillAmount = Mathf.Lerp(laggedHealthImage.fillAmount, _relativeHP, laggedHpLerpSpeed);

            if (laggedHealthImage.fillAmount + 0.01f <= _relativeHP) _lerpLaggedHp = false;
        }
    }

}
