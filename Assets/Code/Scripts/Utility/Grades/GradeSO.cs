using UnityEngine;

[CreateAssetMenu(fileName = "Grade", menuName = "ScriptableObjects/GradeSO", order = 1)]
public class GradeSO : ScriptableObject
{
    public string letterGrade;
    public int timeTaken;
    public int damageTaken;
    public int enemiesKilled;
    public int puzzlesSolved;
    public int urnsBroken;
    public int petalsRemaining;
    public int bulletsReflected;
}
