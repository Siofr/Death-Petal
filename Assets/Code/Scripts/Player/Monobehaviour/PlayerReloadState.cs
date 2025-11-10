using UnityEngine;

namespace State_Machine
{
    public class PlayerReloadState : PlayerBaseState
    {
        public PlayerReloadState(PlayerManager player, Animator animator) : base(player, animator) { }


        public override void OnEnter()
        {
            
        }

        public override void Update()
        {
            player.HandleMovement();
        }

        public override void OnExit()
        {

        }

        public void AddBullet(Vector2 axis)
        {
            if (axis.x < 0)
            {
                EventBus<AddBulletEvent>.Raise(new AddBulletEvent(PlayerManager.Instance.bulletType));
            }
            if (axis.x > 0)
            {
                EventBus<AddBulletEvent>.Raise(new AddBulletEvent(PlayerManager.Instance.bulletType));
            }

            if (axis.y > 0)
            {
                EventBus<AddBulletEvent>.Raise(new AddBulletEvent(PlayerManager.Instance.bulletType));
            }
            if (axis.y < 0)
            {
                EventBus<RemoveBullet>.Raise(new RemoveBullet());
            }
        }
    }
}
