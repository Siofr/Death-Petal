public class AOEFreeze : AOEEffect
{
    public override void StartEffect()
    {
        ToggleFreeze(true);
        StartPlaceHolderVFX(true);
    }

    public override void EndEffect()
    {
        ToggleFreeze(false);
        StartPlaceHolderVFX(false);
    }
    
    public void ToggleFreeze(bool toggle)
    {
        __isActive = toggle;
        
        for (int i = __targets.Count - 1; i >= 0; i--)
        {
            if (__targets[i] == null || __targets[i].Weaknesses.Count < 1)
            {
                __targets.RemoveAt(i);
                continue;
            }

            if (toggle && (__targets[i].Weaknesses[0].WeakType == __aoeTargetType || (int)__aoeTargetType == -1))
            {
                __targets[i].FreezeEnemy(toggle);
            }
            
            if(!toggle) __targets[i].FreezeEnemy(toggle);
        }
    }
}