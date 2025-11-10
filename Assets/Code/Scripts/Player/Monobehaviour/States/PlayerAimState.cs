using UnityEngine;

namespace State_Machine
{
    public class PlayerAimState : PlayerBaseState
    {
        public PlayerAimState(PlayerManager player, Animator animator) : base(player, animator) { }

        public override void OnEnter()
        {
            EventBus<AimEvent>.Raise(new AimEvent());
            player.currentSpeed = player.playerAimSpeed;
        }

        public override void Update()
        {
            player.HandleMovement();
            player.HandleLook();
            player.HandleAim();
        }

        public override void OnExit()
        {
            EventBus<AimEvent>.Raise(new AimEvent());
            EventBus<ActiveTargetEvent>.Raise(new ActiveTargetEvent(null));
            player.currentSpeed = player.playerWalkSpeed;
        }

        public void HandleShoot()
        {
            if (PlayerManager.Instance.activeTarget == null) { OnMiss(); }

            Weakness weakness = PlayerManager.Instance.activeTarget.GetComponent<Weakness>();
            EventBus<ShootEvent>.Raise(new ShootEvent(weakness));
        }

        public void OnMiss()
        {
            EventBus<ShootEvent>.Raise(new ShootEvent(null));
        }
    }
}

