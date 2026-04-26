using UnityEngine;

namespace State_Machine
{
    public class PlayerAimState : PlayerBaseState
    {
        public PlayerAimState(PlayerManager player, Animator animator) : base(player, animator) { }

        public override void OnEnter()
        {
            animator.SetBool(AimHash, true);
            EventBus<AimEvent>.Raise(new AimEvent());
            player.currentSpeed = player.playerAimSpeed;
            InputHandler.AttackEvent += HandleShoot;
        }

        public override void Update()
        {
            player.HandleMovement();
            player.HandleLook();
            player.HandleAim();

            if (player._movement != Vector3.zero) animator.SetFloat("Speed", 0.1f);
            else animator.SetFloat("Speed", 0.0f);
        }

        public override void OnExit()
        {
            animator.SetBool(AimHash, false);
            EventBus<AimEvent>.Raise(new AimEvent());
            EventBus<ActiveTargetEvent>.Raise(new ActiveTargetEvent(null));
            player.activeTarget = null;
            InputHandler.AttackEvent -= HandleShoot;
            player.currentSpeed = player.playerWalkSpeed;
        }

        public void HandleShoot()
        {
            animator.SetTrigger("Shoot");
            if (player.activeTarget == null) 
            {
                EventBus<ShootEvent>.Raise(new ShootEvent(null));
                return;
            }

            Weakness weakness = player.activeTarget.GetComponent<Weakness>();
            EventBus<ShootEvent>.Raise(new ShootEvent(weakness));
        }
    }
}