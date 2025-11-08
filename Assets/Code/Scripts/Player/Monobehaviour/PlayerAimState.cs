using UnityEngine;

namespace State_Machine
{
    public class PlayerAimState : PlayerBaseState
    {
        public PlayerAimState(PlayerManager player, Animator animator) : base(player, animator) { }

        public override void OnEnter()
        {
            EventBus<PlayerAimEvent>.Raise(new PlayerAimEvent());
            player.currentSpeed = player.playerAimSpeed;
        }

        public override void Update()
        {
            player.HandleMovement();
            player.HandleLook();
        }

        public override void OnExit()
        {
            // EventBus<PlayerAimCancelEvent>.Raise(new PlayerAimCancelEvent());
            player.currentSpeed = player.playerWalkSpeed;
        }
    }
}

