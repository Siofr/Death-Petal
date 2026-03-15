using UnityEngine;

public class RebindGlobalReset : MonoBehaviour
{
    public void ResetAllBindings()
    {
        RebindKeyUI[] allRebinds = GameObject.FindObjectsByType<RebindKeyUI>(FindObjectsSortMode.None);

        foreach (var bind in allRebinds)
        {
            bind.resetBinding();
        }
    }
}
