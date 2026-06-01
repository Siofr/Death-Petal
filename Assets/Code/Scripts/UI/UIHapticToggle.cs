using UnityEngine;

public struct ToggleHaptics : IEvent
{
    
}

public class UIHapticToggle : MonoBehaviour
{
    public void ToggleHaptics()
    {
        EventBus<ToggleHaptics>.Raise(new ToggleHaptics());
    }
}
