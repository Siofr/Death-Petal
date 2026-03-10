using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Tutorial Data", menuName = "ScriptableObjects/Tutorial Data", order = 1)]
public class TutorialSO : ScriptableObject
{
    public string tutorialText;
    public List<string> tutorialSteps;
}
