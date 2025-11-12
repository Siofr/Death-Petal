using UnityEngine;

namespace State_Machine
{
    public class PlayerSprintState : PlayerBaseState
    {
        public PlayerSprintState(PlayerManager player, Animator animator) : base(player, animator) { }

        public override void OnEnter()
        {
            Debug.Log("Enter Sprint State");
            animator.SetFloat(SpeedHash, 0.3f);
            player.currentSpeed = player.playerSprintSpeed;
        }

        public override void Update()
        {
            player.HandleMovement();
        }

        public override void OnExit()
        {
            player.currentSpeed = player.playerWalkSpeed;
        }
    }
}

