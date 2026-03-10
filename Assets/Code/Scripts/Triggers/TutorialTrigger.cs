using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class TutorialTrigger : MonoBehaviour
{

    [System.Serializable]
    public struct TutorialInfo
    {
        public List<string> tutorialText;
    }

    public List<TutorialInfo> tutorialSteps = new List<TutorialInfo>();
    private int _currentTutorialStep;
    private int _currentStep;

    void AdvanceTutorial()
    {
        // Step
    }

    void TriggerTutorial()
    {
        TriggerNextStep(0);
    }

    void TriggerNextStep(int step)
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            TriggerTutorial();
        }
    }
}
