using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class Bishop_AttackPatternSpawner : MonoBehaviour
{
    public GameObject projectilePrefab;
    private Vector3 _playerPosition = Vector3.zero;
    private BossBase _bossController;

    [Header("Target Spit Settings")] public float targetspit_timeBetweenShots;
    [SerializeField] private Vector3 targetspit_shootForce;
    [SerializeField] private float targetspit_attackLength;
    
    [Header("Radial Spit Settings")] public float radialspit_timeBetweenShots;
    [SerializeField] private Vector3 radialspit_shootForce;
    [SerializeField] private float radialspit_degrees;
    [SerializeField] private float radialspit_attackLength;

    private void Start()
    {
        Initialise();
    }

    public void Initialise()
    {
        _bossController = GetComponent<BossBase>();
        if(_bossController.target)
            _playerPosition = _bossController.target.transform.position;
    }

    public void StartTargetSpit()
    {
        StartCoroutine(TargetSpit());
    }

    public void StartRadialSpit(bool dual = false)
    {
        StartCoroutine(RadialSpit(dual));
    }

    private IEnumerator TargetSpit()
    {
        _bossController.isAttackReady = false;
        print("Targetspit begun");
        float startTime = Time.time;

        while (Time.time - startTime < radialspit_attackLength)
        {
            _playerPosition = _bossController.target.transform.position;
            print("shoot");
            var newProjectile = Instantiate(projectilePrefab, transform.position, quaternion.identity, transform).GetComponent<ProjectileBase>();
            var attackVector = (_playerPosition - transform.position).normalized;
            attackVector = new Vector3(
                attackVector.x * targetspit_shootForce.x,
                attackVector.y * targetspit_shootForce.y,
                attackVector.z * targetspit_shootForce.z);
            newProjectile.SendProjectile(attackVector, 4, true, _bossController);
            yield return new WaitForSeconds(targetspit_timeBetweenShots);
        }
        _bossController.isAttackReady = true;
    }

    public IEnumerator RadialSpit(bool dual = false)
    {
        _bossController.isAttackReady = false;
        print("RadialSpit begun");
        float startTime = Time.time;

        while (Time.time - startTime < radialspit_attackLength)
        {
            transform.eulerAngles = new Vector3(
                transform.eulerAngles.x,
                transform.eulerAngles.y + radialspit_degrees,
                transform.eulerAngles.z
            );

            print("shoot");
            var newProjectile = Instantiate(projectilePrefab, transform.position, quaternion.identity)
                .GetComponent<ProjectileBase>();
            var attackVector = transform.forward;
            

            attackVector = new Vector3(
                attackVector.x * radialspit_shootForce.x,
                attackVector.y * radialspit_shootForce.y,
                attackVector.z * radialspit_shootForce.z);

            newProjectile.SendProjectile(attackVector, 4, true, _bossController);

            if (dual)
            {
                var newProjectileB = Instantiate(projectilePrefab, transform.position, quaternion.identity)
                    .GetComponent<ProjectileBase>();
                attackVector.x *= -1;
                attackVector.z *= -1;
                // cheap man's way to prevent the projectiles from colliding
                attackVector.y += 3.5f;
                
                newProjectileB.SendProjectile(attackVector, 4, true, _bossController);
            }

            yield return new WaitForSeconds(radialspit_timeBetweenShots);
            
        }
        _bossController.isAttackReady = true;
    }
    
}
