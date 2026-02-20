using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyFactory<>), true)]
public class EnemyFactoryEditor: Editor
{   
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    
        var factory = (EnemyFactory<EnemyBase>)target;
        
        if (GUILayout.Button("Spawn Enemy at Test Pos"))
        {
            factory.CreateEnemy(Vector3.zero, factory.transform);
        }
    }
}

/*[CustomEditor(typeof(MonoBase<>), true)]
public class ExampleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Build Object"))
        {
            IMonoBase<MonoBehaviour> test = (IMonoBase<MonoBehaviour>)target;
            IMonoBase<MonoBehaviour> example = test.ExampleMethod();
            Debug.Log(example is IMonoBase<Example>);
        }
    }
}*/