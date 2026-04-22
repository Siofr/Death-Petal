using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Event Bus Utils
///
/// Helper class for the Event Bus
/// Initialises all Event Buses before game starts
/// </summary>
public static class EventBusUtils
{
    private static IReadOnlyCollection<Type> EventTypes { get; set; }
    private static IReadOnlyCollection<Type> EventBusTypes { get; set; } 
    
        //Cleanup methods to clear Event Buses when exiting out of Playmode
#if UNITY_EDITOR
    
    public static PlayModeStateChange State {get ; set;}

    [InitializeOnLoadMethod]
    public static void InitialiseEditor()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChange;
        EditorApplication.playModeStateChanged += OnPlayModeStateChange;
    }

    private static void OnPlayModeStateChange(PlayModeStateChange state)
    {
        State = state;
        
        if(state == PlayModeStateChange.ExitingPlayMode)
            ClearAllBuses();
    }
    
#endif
    
    //Initialisation done before game scene is loaded to avoid scene runtime overhead
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialise()
    {
        Debug.Log("Initialising Event Buses...");
        
        EventTypes = PredefinedAssemblyUtils.GetType(typeof(IEvent));
        EventBusTypes = InitialialiseAllBuses();
    }
    
    //Creates buses for each existing Event Type found
    private static List<Type> InitialialiseAllBuses()
    {
        List<Type> eventBuses = new List<Type>();
        
        var typedef  = typeof(EventBus<>);

        foreach (var type in EventTypes)
        {
            var busType =  typedef.MakeGenericType(type);
            eventBuses.Add(busType);
            Debug.Log($"Initialised {type.Name} Bus");
        }
        
        return eventBuses;
    }
    
    //Cleanup Method
    public static void ClearAllBuses()
    {
        Debug.Log("Clearing all buses...");
        foreach (var type in EventBusTypes)
        {
            var clearMethod = type.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);
            clearMethod.Invoke(null, null);
        }
    }
}