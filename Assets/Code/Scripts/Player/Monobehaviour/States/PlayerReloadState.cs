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
            EventBus<EndLongReload>.Raise(new EndLongReload());
        }

        public void AddBullet(Vector2 axis)
        {
            if (axis.x < 0)
            {
                EventBus<AddBulletEvent>.Raise(new AddBulletEvent(PlayerManager.Instance.bulletTypes[0]));
            }
            if (axis.x > 0)
            {
                EventBus<AddBulletEvent>.Raise(new AddBulletEvent(PlayerManager.Instance.bulletTypes[1]));
            }

            if (axis.y > 0)
            {
                EventBus<AddBulletEvent>.Raise(new AddBulletEvent(PlayerManager.Instance.bulletTypes[2]));
            }
            if (axis.y < 0)
            {
                EventBus<RemoveBulletEvent>.Raise(new RemoveBulletEvent(-1, -1));
            }
        }
    }
}
