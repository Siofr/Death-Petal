using UnityEngine;

namespace State_Machine
{
    public class PlayerReloadState : PlayerBaseState
    {
        public PlayerReloadState(PlayerManager player, Animator animator) : base(player, animator) { }


        public override void OnEnter()
        {
            InputHandler.HotkeyEvent += AddBullet;
            EventBus<StartLongReload>.Raise(new StartLongReload());
        }

        public override void Update()
        {
            player.HandleMovement();
        }

        public override void OnExit()
        {
            InputHandler.HotkeyEvent -= AddBullet;
            EventBus<EndLongReload>.Raise(new EndLongReload());
        }

        public void AddBullet(Vector2 axis)
        {
            if (axis.x < 0)
            {
                EventBus<AddBulletEvent>.Raise(new AddBulletEvent(player.bulletTypes[0]));
            }
            if (axis.x > 0)
            {
                EventBus<AddBulletEvent>.Raise(new AddBulletEvent(player.bulletTypes[1]));
            }

            if (axis.y > 0)
            {
                EventBus<AddBulletEvent>.Raise(new AddBulletEvent(player.bulletTypes[2]));
            }
            if (axis.y < 0)
            {
                EventBus<RemoveBulletEvent>.Raise(new RemoveBulletEvent(-1, -1));
            }
        }
    }
}
