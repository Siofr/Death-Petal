using TMPro.EditorUtilities;
using UnityEngine;

namespace State_Machine
{
    public abstract class PlayerBaseState : IState
    {
        protected readonly PlayerManager player;
        protected readonly Animator animator;

        protected static readonly int WalkHash = Animator.StringToHash("Walk");
        protected static readonly int SprintHash = Animator.StringToHash("Sprint");
        protected static readonly int IdleHash = Animator.StringToHash("Idle");
        protected static readonly int AimHash = Animator.StringToHash("Aim");
        protected static readonly int ReloadHash = Animator.StringToHash("Reload");

        protected PlayerBaseState(PlayerManager player, Animator animator)
        {
            this.player = player;
            this.animator = animator;
        }

        public virtual void OnEnter()
        {

        }

        public virtual void Update()
        {

        }

        public virtual void FixedUpdate()
        {

        }

        public virtual void OnExit()
        {

        }
    }
}

