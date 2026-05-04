using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SavingUI : MonoBehaviour
{
    private Scene _self;

    [SerializeField] private Image _lurkerIcon;
    [SerializeField] private TextMeshProUGUI _text;
    
    [SerializeField] private float _fadeTime;
    [SerializeField] private float _pauseTime;
    
    private void Awake()
    {
        _self = SceneManager.GetSceneByName("SavingIcon");
    }

    private void Start()
    {
        StartCoroutine(StartDecay());
    }

    private IEnumerator StartDecay()
    {
        yield return FadeUI(false);
        
        yield return new WaitForSeconds(_pauseTime);

        yield return FadeUI(true);
        
        SceneManager.UnloadSceneAsync(_self);
    }

    private IEnumerator FadeUI(bool isFadingOut)
    {
        var target = isFadingOut ? 0f : 1f;
        var alpha = isFadingOut ? 1f : 0f;

        if (!isFadingOut)
        {
            while (alpha < target)
            {
                alpha += Time.deltaTime / _fadeTime;

                var tempColor = _lurkerIcon.color;
                tempColor.a = alpha;
                
                _lurkerIcon.color = tempColor;
                _text.color = tempColor;
                
                yield return null;
            }
        }
        else
        {
            while (alpha > target)
            {
                alpha -= Time.deltaTime / _fadeTime;

                var tempColor = _lurkerIcon.color;
                tempColor.a = alpha;
                
                _lurkerIcon.color = tempColor;
                _text.color = tempColor;
                
                yield return null;
            }
        }

        var targetColor = _lurkerIcon.color;
        targetColor.a = target;
            
        _lurkerIcon.color = targetColor;
        _text.color = targetColor;
    }
}
