using UnityEngine;

/// <summary>
/// Abstract Singleton Class
///
/// Used to create basic Singletons for any MonoBehaviour Objects
/// For the creation of a singleton please inherit as such
/// Example:
/// public class PlayerManager: Singleton<PlayerManager> {...}
/// </summary>
/// <typeparam name="T"> Specifies MonoBehaviour Type</typeparam>
public abstract class Singleton<T> : MonoBehaviour where T: Component
{
    protected static T __instance;
    
    //Property reference for the Singleton, is in ReadOnly
    public static T Instance => __instance;

    //Awake method made Virtual
    //If awake method needs to be modified please invoke base.Awake() in function
    //Example:
    //    protected override void Awake()
    //    {
    //    base.Awake();
    //    //Logic..
    //    }
    //
    protected virtual void Awake()
    {
        if (__instance == null)
            __instance = FindAnyObjectByType<T>();
        else
            Destroy(gameObject);
    }
}
