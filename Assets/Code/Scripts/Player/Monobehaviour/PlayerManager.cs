using State_Machine;
using UnityEngine;

namespace State_Machine
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        CharacterController _cc;
        public Animator _animator;
        public Transform activeCam;
        public BulletSO[] bulletTypes;
        private Camera _mainCam;

        public float playerWalkSpeed;
        public float playerSprintSpeed;
        public float playerAimSpeed;
        public float currentSpeed;
        private Vector3 _movement;
        private Vector2 _aim;
        public Vector3 lookDir;

        private bool _isAiming;
        private bool _isSprinting;
        private bool _isReloading;
        public Transform activeTarget;

        public StateMachine stateMachine;

        private PlayerReloadState _reloadState;
        private PlayerAimState _aimState;
        private PlayerIdleState _idleState;

        [SerializeField]
        private Material[] playerDependentMaterials;

        protected override void Awake()
        {
            base.Awake();
            _cc = GetComponent<CharacterController>();
            _animator = GetComponentInChildren<Animator>();
            _mainCam = Camera.main;

            currentSpeed = playerWalkSpeed;

            SetupStateMachine();
        }

        private void OnEnable()
        {
            InputHandler.AimEvent += OnAim;
            InputHandler.SprintEvent += OnSprint;
            InputHandler.LongReloadEvent += OnReloadStart;
            InputHandler.LongReloadCancelledEvent += OnReloadCancel;
            InputHandler.QuickReloadEvent += OnQuickReload;
        }

        private void OnDisable()
        {
            InputHandler.AimEvent -= OnAim;
            InputHandler.SprintEvent -= OnSprint;
            InputHandler.LongReloadEvent -= OnReloadStart;
            InputHandler.LongReloadCancelledEvent -= OnReloadCancel;
            InputHandler.HotkeyEvent -= _reloadState.AddBullet;
            InputHandler.AttackEvent -= _aimState.HandleShoot;
            InputHandler.QuickReloadEvent -= OnQuickReload;
        }

        void SetupStateMachine()
        {
            stateMachine = new StateMachine();

            _aimState = new PlayerAimState(this, _animator);
            var moveState = new PlayerMoveState(this, _animator);
            _idleState = new PlayerIdleState(this, _animator);
            var sprintState = new PlayerSprintState(this, _animator);
            _reloadState = new PlayerReloadState(this, _animator);

            At(_aimState, _idleState, new FuncPredicate(() => _aim == Vector2.zero));

            At(_idleState, _aimState, new FuncPredicate(() => _aim != Vector2.zero));
            At(_idleState, moveState, new FuncPredicate(() => _movement != Vector3.zero));

            At(moveState, _idleState, new FuncPredicate(() => _movement == Vector3.zero));
            At(moveState, sprintState, new FuncPredicate(() => _isSprinting));
            At(moveState, _aimState, new FuncPredicate(() => _aim != Vector2.zero));

            At(sprintState, moveState, new FuncPredicate(() => !_isSprinting));
            At(sprintState, _idleState, new FuncPredicate(() => _movement == Vector3.zero));
            At(sprintState, _aimState, new FuncPredicate(() => _aim != Vector2.zero));

            Any(_reloadState, new FuncPredicate(() => _isReloading));
            At(_reloadState, _idleState, new FuncPredicate(() => !_isReloading));
        }

        private void Start()
        {
            stateMachine.SetState(_idleState);
        }

        void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        private void Update()
        {
            var movementDirection = InputHandler.MOVE.ReadValue<Vector2>();
            var aimDirection = InputHandler.LOOK.ReadValue<Vector2>();

            _movement = new Vector3(movementDirection.x, 0, movementDirection.y);
            _aim = aimDirection;
            
            foreach (var playerDependentMaterial in playerDependentMaterials)
            {
                playerDependentMaterial.SetVector("_PlayerPosition", transform.position);
            }

            stateMachine.Update();
        }

        private void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }

        void OnAim()
        {
            if (!_isAiming)
            {
                _isAiming = true;
                return;
            }
            _isAiming = false;
        }

        void OnReloadStart()
        {
            _isReloading = true;
        }

        void OnReloadCancel()
        {
            if (_isReloading) _isReloading = false;
        }

        void OnQuickReload()
        {
            EventBus<QuickReload>.Raise(new QuickReload());
        }

        void OnSprint()
        {
            if (!_isSprinting)
            {
                _isSprinting = true;
                return;
            }
            _isSprinting = false;
        }
        private Vector3 GetPlaneNormal()
        {
            Ray ray = new Ray(transform.position, -transform.up);
            RaycastHit hit;

            Physics.SphereCast(ray, .45f, out hit, 1f, 1 << 3, QueryTriggerInteraction.Ignore);

            if (hit.collider != null)
            {
                return hit.normal;
            }

            return Vector3.negativeInfinity;
        }

        public void HandleMovement()
        {
            Vector3 camForward = Vector3.ProjectOnPlane(activeCam.transform.forward, GetPlaneNormal());
            Vector3 camRight = Vector3.ProjectOnPlane(activeCam.transform.right, GetPlaneNormal());

            Vector3 dir = camForward * _movement.z + camRight * _movement.x;
            lookDir = dir;

            if (lookDir == Vector3.zero) _animator.SetFloat("Speed", 0.0f);
            else _animator.SetFloat("Speed", Mathf.Clamp(currentSpeed / 10, 0.0f, 1.0f));

            transform.LookAt(transform.position + lookDir.normalized);

            _cc.Move(dir.normalized * currentSpeed * Time.deltaTime);
        }

        public void HandleLook()
        {
            Vector3 camForward = Vector3.ProjectOnPlane(_mainCam.transform.forward, GetPlaneNormal());
            Vector3 camRight = Vector3.ProjectOnPlane(_mainCam.transform.right, GetPlaneNormal());

            Vector3 dir = camForward * _aim.y + camRight * _aim.x;
            lookDir = dir;

            transform.LookAt(transform.position + lookDir.normalized);
        }

        public void HandleAim()
        {
            RaycastHit hit;
            Weakness weakness;

            if (Physics.SphereCast(transform.position, 0.5f, transform.forward, out hit, 30))
            {
                if (hit.transform.TryGetComponent<Weakness>(out weakness))
                {
                    activeTarget = hit.transform;
                    EventBus<ActiveTargetEvent>.Raise(new ActiveTargetEvent(hit.transform));
                    return;
                }
            }

            EventBus<ActiveTargetEvent>.Raise(new ActiveTargetEvent(null));
        }
    }
}

