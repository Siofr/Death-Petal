using UnityEngine;
using UnityEngine.SceneManagement;

public class TempSceneManager : MonoBehaviour
{
    public void reloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        EventBus<LevelLoadEvent>.Raise(new LevelLoadEvent(false));
    }

    public void loadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
