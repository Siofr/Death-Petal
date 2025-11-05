using UnityEngine;

namespace State_Machine
{
    public class PlayerIdleState : PlayerBaseState
    {
        public PlayerIdleState(PlayerManager player, Animator animator) : base(player, animator) { }

        public override void OnEnter()
        {
            player.transform.LookAt(player.transform.position, player.lookDir);
            Debug.Log("Enter Action Idle State");
        }

        public override void OnExit()
        {

        }
    }
}

