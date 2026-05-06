using UnityEngine;

public class EnemySpawnState<T>: EnemyBaseState<T> where T: EnemyBase
{
    public EnemySpawnState(T enemyController) : base(enemyController) { }

    private bool _ditherIsDisalbled = false;
    
    public override void OnEnter()
    {
        enemyController.Weaknesses[0].Toggle(false);
        enemyController.StopAgent(true);
    }

    public override void Update()
    {
        if (enemyController.animator.speed == 0)
        {
            if (_ditherIsDisalbled) return;
            
            GetDitherRendererMaterial(false);
            
            _ditherIsDisalbled = true;
        }
        else
        {
            if (!_ditherIsDisalbled) return;

            GetDitherRendererMaterial(true);
            
            _ditherIsDisalbled = false;
        }
    }
    
    public override void OnExit()
    {
        enemyController.Weaknesses[0].Toggle(true);
        enemyController.StopAgent(false);
        
        GetDitherRendererMaterial(true);
    }
    
    private void GetDitherRendererMaterial(bool isVisible)
    {
        var rends = enemyController.GetComponentsInChildren<SkinnedMeshRenderer>();
        if (rends == null || rends.Length < 1) return;
        
        for(int i = 0; i < rends.Length; i++)
        {
            for (int j = 0; j < rends[i].materials.Length; j++)
            {
                //if (rends[i].materials[j].shader != Shader.Find("Assets/")) continue;
                Debug.Log(rends[i].materials[j].name);
                rends[i].materials[j].SetFloat("_IsVisible", isVisible ? 1f : 0f);
            }
        }
    }
}