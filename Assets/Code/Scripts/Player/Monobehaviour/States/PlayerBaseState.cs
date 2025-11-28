using UnityEngine;

namespace State_Machine
{
    public abstract class PlayerBaseState : IState
    {
        protected readonly PlayerManager player;
        protected readonly Animator animator;

        protected static readonly int SpeedHash = Animator.StringToHash("Speed");
        protected static readonly int AimHash = Animator.StringToHash("Aiming");
        protected static readonly int ShootHash = Animator.StringToHash("Shoot");

        protected PlayerBaseState(PlayerManager player, Animator animator)
        {
            this.player = player;
            this.animator = animator;
        }

        protected virtual void OnEnable()
        {

        }

        protected virtual void OnDisable()
        {

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

