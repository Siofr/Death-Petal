using System;
using System.Collections;
using UnityEngine;

public class Bishop_AttackPatternSpawner : MonoBehaviour
{
    public GameObject projectilePrefab;
    private Vector3 _playerPosition = Vector3.zero;
    private BossBase _bossController;
    public float phaseLength = 6f;

    [Header("Target Spit Settings")] public float targetspit_timeBetweenShots;

    private void Start()
    {
        Initialise();
        StartCoroutine(TargetSpit());
    }

    public void Initialise()
    {
        _bossController = GetComponent<BossBase>();
        if(_bossController.target)
            _playerPosition = _bossController.target.transform.position;
    }
    
    public IEnumerator TargetSpit()
    {
        print("Targetspit begun");
        float startTime = Time.time;

        while (Time.time - startTime < phaseLength)
        {
            print("shoot");
            var newProjectile = Instantiate(projectilePrefab).GetComponent<ProjectileBase>();
            newProjectile.SendProjectile(_playerPosition - transform.position, 4, true, _bossController);
            yield return new WaitForSeconds(targetspit_timeBetweenShots);
        }
    }
}
