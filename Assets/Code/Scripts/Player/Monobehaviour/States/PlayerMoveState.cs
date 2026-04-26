using UnityEngine;

namespace State_Machine
{
    public class PlayerMoveState : PlayerBaseState
    {
        public PlayerMoveState(PlayerManager player, Animator animator) : base(player, animator) { }

        public override void OnEnter()
        {
            Debug.Log("Enter Move State");
            animator.SetFloat("Speed", 0.4f);
        }

        public override void Update()
        {
            player.HandleMovement();
        }

        public override void OnExit()
        {

        }
    }
}

