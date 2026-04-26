using UnityEngine;

namespace State_Machine
{
    public class PlayerDialogueState : PlayerBaseState
    {
        public PlayerDialogueState(PlayerManager player, Animator animator) : base(player, animator)
        {
        }

        public override void OnEnter()
        {
            animator.SetFloat(SpeedHash, 0);
            player.activeCam = player.newActiveCam;
            player.transform.LookAt(player.transform.position, player.lookDir);
            animator.SetFloat("Speed", 0.0f);
        }

        public override void OnExit()
        {
            
        }
    }
}
