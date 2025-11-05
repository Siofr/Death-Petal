using UnityEngine;

namespace State_Machine
{
    public class PlayerAimState : PlayerBaseState
    {
        public PlayerAimState(PlayerManager player, Animator animator) : base(player, animator) { }

        public override void OnEnter()
        {
            Debug.Log("Enter Attack State");
            player.currentSpeed = player.playerAimSpeed;
        }

        public override void Update()
        {
            player.HandleMovement();
            player.HandleLook();
        }

        public override void OnExit()
        {
            player.currentSpeed = player.playerWalkSpeed;
        }
    }
}

