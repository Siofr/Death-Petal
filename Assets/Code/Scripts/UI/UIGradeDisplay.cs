using UnityEngine;
using TMPro;

public class UIGradeDisplay : MonoBehaviour
{
    public TMP_Text finalGrade;
    public TMP_Text enemyGrade;
    public TMP_Text puzzleGrade;
    public TMP_Text urnGrade;
    public TMP_Text petalGrade;
    public TMP_Text timeGrade;

    EventBindings<DisplayEndUI> displayEndUIEventListener;

    private void Awake()
    {
        displayEndUIEventListener = new EventBindings<DisplayEndUI>(OnDisplayUI);
    }

    private void OnEnable()
    {
        EventBus<DisplayEndUI>.Register(displayEndUIEventListener);
    }

    private void OnDisable()
    {
        EventBus<DisplayEndUI>.Unregister(displayEndUIEventListener);
    }

    private void OnDisplayUI(DisplayEndUI ctx)
    {
        EditText(finalGrade, ctx.finalGrade);
        EditText(enemyGrade, ctx.enemyGrade);
        EditText(puzzleGrade, ctx.puzzleGrade);
        EditText(urnGrade, ctx.urnGrade);
        EditText(petalGrade, ctx.petalGrade);
        EditText(timeGrade, ctx.timeGrade);
    }

    private void EditText(TMP_Text text, string grade)
    {
        text.text = text.text + " " + grade;
    }
}
