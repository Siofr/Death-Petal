using UnityEngine;

namespace State_Machine
{
    public class PlayerAimState : PlayerBaseState
    {
        public PlayerAimState(PlayerManager player, Animator animator) : base(player, animator) { }

        public override void OnEnter()
        {
            EventBus<ActivateCircleEvent>.Raise(new ActivateCircleEvent());
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
            EventBus<ActivateCircleEvent>.Raise(new ActivateCircleEvent());
            EventBus<ActiveTargetEvent>.Raise(new ActiveTargetEvent(null));
            player.currentSpeed = player.playerWalkSpeed;
        }
    }
}

