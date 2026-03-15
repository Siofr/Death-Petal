using UnityEngine;

public class MenuHighlighter : MonoBehaviour
{
    public GameObject startPanel;
    
    public GameObject[] subMenuPanels;
    //Transform[] allPanels;
    private void OnEnable()
    {
        //allPanels = GetComponentsInChildren<Transform>();

        foreach (var panel in subMenuPanels)
        {
            if (panel.gameObject != gameObject)
                panel.gameObject.SetActive(false);
        }

        startPanel.SetActive(true);
    }
    public void ToggleMenuPanel(GameObject menuPanel)
    {;
        FindActiveMenuPanel().SetActive(false);
        menuPanel.SetActive(true);
    }
    private GameObject FindActiveMenuPanel()
    {
        foreach (var panel in subMenuPanels)
        {
            if (panel.gameObject != gameObject && panel.gameObject.activeSelf)
                return panel.gameObject;
        }
        Debug.LogWarning("No active menu panel found");
        return null;
    }
}
