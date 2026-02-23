using UnityEngine;

namespace State_Machine
{
    public class PlayerIdleState : PlayerBaseState
    {
        public PlayerIdleState(PlayerManager player, Animator animator) : base(player, animator) { }

        public override void OnEnter()
        {
            animator.SetFloat(SpeedHash, 0);
            player.activeCam = player.newActiveCam;
            player.transform.LookAt(player.transform.position, player.lookDir);
            //Debug.Log("Enter Idle State");
        }

        public override void OnExit()
        {

        }
    }
}

