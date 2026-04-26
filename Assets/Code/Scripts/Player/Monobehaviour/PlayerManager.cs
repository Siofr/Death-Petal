using State_Machine;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace State_Machine
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        CharacterController _cc;
        public Animator _animator;
        public Transform activeCam;
        public Transform newActiveCam;
        public BulletSO[] bulletTypes;
        private Camera _mainCam;
        private float _ySpeed;

        public float playerWalkSpeed;
        public float playerSprintSpeed;
        public float playerAimSpeed;
        public float currentSpeed;
        private Vector3 _movement;
        private Vector2 _aim;
        public Vector3 lookDir;
        public GameObject pauseMenu;

        private bool _isAiming;
        public bool _isSprinting;
        private bool _isReloading;
        public Transform activeTarget;

        private bool _isDialogue;

        public StateMachine stateMachine;

        private PlayerReloadState _reloadState;
        private PlayerAimState _aimState;
        private PlayerIdleState _idleState;

        private EventBindings<CameraChangeEvent> _cameraChangeEventListener;
        private EventBindings<TriggerDialogueEvent> _dialogueEnteredListener;
        private EventBindings<ExitDialogueEvent> _exitDialogueEventListener;
        private EventBindings<PlayerDamagedEvent> _playerDamageEventListener;

        [SerializeField]
        private Material[] playerDependentMaterials;

        protected override void Awake()
        {
            base.Awake();

            currentSpeed = playerWalkSpeed;

            _cameraChangeEventListener = new EventBindings<CameraChangeEvent>(OnChangeCamera);
            _dialogueEnteredListener = new EventBindings<TriggerDialogueEvent>(OnDialogueEntered);
            _exitDialogueEventListener = new EventBindings<ExitDialogueEvent>(OnDialogueExited);
            _playerDamageEventListener = new EventBindings<PlayerDamagedEvent>(OnPlayerDamage);
        }

        private void OnEnable()
        {
            EventBus<CameraChangeEvent>.Register(_cameraChangeEventListener);
            EventBus<TriggerDialogueEvent>.Register(_dialogueEnteredListener);
            EventBus<ExitDialogueEvent>.Register(_exitDialogueEventListener);
            EventBus<PlayerDamagedEvent>.Register(_playerDamageEventListener);
            
            InputHandler.PauseEvent += OnPause;
            
            // InputHandler.AimEvent += OnAim;
            InputHandler.SprintEvent += OnSprint;
            InputHandler.LongReloadEvent += OnReloadStart;
            InputHandler.LongReloadCancelledEvent += OnReloadCancel;
            InputHandler.QuickReloadEvent += OnQuickReload;
            InputHandler.HotkeyEvent += AddBullet;
            InputHandler.RemoveBulletEvent += RemoveBullet;
            InputHandler.RotateBarrelEvent += OnRotateBarrel;
        }

        private void OnDisable()
        {
            EventBus<CameraChangeEvent>.Unregister(_cameraChangeEventListener);
            EventBus<TriggerDialogueEvent>.Unregister(_dialogueEnteredListener);
            EventBus<ExitDialogueEvent>.Unregister(_exitDialogueEventListener);
            EventBus<PlayerDamagedEvent>.Unregister(_playerDamageEventListener);

            InputHandler.PauseEvent -= OnPause;
            
            // InputHandler.AimEvent -= OnAim;
            InputHandler.SprintEvent -= OnSprint;
            InputHandler.LongReloadEvent -= OnReloadStart;
            InputHandler.LongReloadCancelledEvent -= OnReloadCancel;
            // InputHandler.HotkeyEvent -= _reloadState.AddBullet;
            InputHandler.RemoveBulletEvent -= RemoveBullet;
            InputHandler.HotkeyEvent -= AddBullet;
            InputHandler.AttackEvent -= _aimState.HandleShoot;
            InputHandler.QuickReloadEvent -= OnQuickReload;
            InputHandler.RotateBarrelEvent -= OnRotateBarrel;
        }

        void SetupStateMachine()
        {
            stateMachine = new StateMachine();

            _aimState = new PlayerAimState(this, _animator);
            var moveState = new PlayerMoveState(this, _animator);
            _idleState = new PlayerIdleState(this, _animator);
            var sprintState = new PlayerSprintState(this, _animator);
            _reloadState = new PlayerReloadState(this, _animator);
            var dialogueState = new PlayerDialogueState(this, _animator);

            At(_aimState, _idleState, new FuncPredicate(() => _aim == Vector2.zero));
            At(_aimState, _reloadState, new FuncPredicate(() => _isReloading));

            At(_idleState, _aimState, new FuncPredicate(() => _aim != Vector2.zero));
            At(_idleState, moveState, new FuncPredicate(() => _movement != Vector3.zero));
            At(_idleState, _reloadState, new FuncPredicate(() => _isReloading));

            At(moveState, _idleState, new FuncPredicate(() => _movement == Vector3.zero));
            At(moveState, sprintState, new FuncPredicate(() => _isSprinting));
            At(moveState, _aimState, new FuncPredicate(() => _aim != Vector2.zero));
            At(moveState, _reloadState, new FuncPredicate(() => _isReloading));

            At(sprintState, moveState, new FuncPredicate(() => !_isSprinting));
            At(sprintState, _idleState, new FuncPredicate(() => _movement == Vector3.zero));
            At(sprintState, _aimState, new FuncPredicate(() => _aim != Vector2.zero));
            At(sprintState, _reloadState, new FuncPredicate(() => _isReloading));

            At(_reloadState, _idleState, new FuncPredicate(() => !_isReloading));

            Any(dialogueState, new FuncPredicate(() => _isDialogue));
            At(dialogueState, _idleState, new FuncPredicate(() => !_isDialogue));
            At(dialogueState, _aimState, new FuncPredicate(() => !_isDialogue));
            At(dialogueState, _reloadState, new FuncPredicate(() => !_isDialogue));
            At(dialogueState, moveState, new FuncPredicate(() => !_isDialogue));
            At(dialogueState, sprintState, new FuncPredicate(() => !_isDialogue));
        }

        private void Start()
        {
            newActiveCam = activeCam;

            _cc = GetComponent<CharacterController>();
            _animator = GetComponentInChildren<Animator>();
            _mainCam = Camera.main;

            if (newActiveCam == null) newActiveCam = _mainCam.transform;

            EventBus<TransmitPlayerInfo>.Raise(new TransmitPlayerInfo(this.transform));

            SetupStateMachine();
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

        private bool _isPaused;
        
        private void OnPause()
        {
            _isPaused = !_isPaused;

            if (!_isPaused)
            {
                Time.timeScale = 1;
                pauseMenu.SetActive(false);
            } else
            {
                Time.timeScale = 1;
                pauseMenu.SetActive(true);
            }
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

        void OnRotateBarrel(int direction)
        {
            EventBus<RotateBarrelEvent>.Raise(new RotateBarrelEvent(direction));
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
            Vector3 camForward = new Vector3(activeCam.transform.forward.x, 0, activeCam.transform.forward.z);
            Vector3 projectCamForward = Vector3.ProjectOnPlane(camForward, GetPlaneNormal());
            Vector3 camRight = Vector3.ProjectOnPlane(activeCam.transform.right, GetPlaneNormal());

            Vector3 dir = (projectCamForward.normalized * _movement.z + camRight.normalized * _movement.x).normalized;

//            Debug.Log("Direction: " + dir);

            lookDir = dir;

            if (_cc.isGrounded) _ySpeed = 0;

            // if (lookDir == Vector3.zero) _animator.SetFloat("Speed", 0.0f);

            _ySpeed -= 9.8f * Time.deltaTime;

            var lookForward = transform.position + lookDir.normalized;
            lookForward.y = transform.position.y;

            //transform.rotation = Quaternion.Euler(lookRotation);

            transform.LookAt(lookForward);

            dir.y = _ySpeed;
            _cc.Move(dir * currentSpeed * Time.deltaTime);
        }

        public void HandleLook()
        {
            Vector3 camForward = Vector3.ProjectOnPlane(_mainCam.transform.forward, GetPlaneNormal());
            Vector3 camRight = Vector3.ProjectOnPlane(_mainCam.transform.right, GetPlaneNormal());

            Vector3 dir = (camForward.normalized * _aim.y + camRight.normalized * _aim.x).normalized;
            lookDir = dir;

            var lookForward = transform.position + lookDir.normalized;
            lookForward.y = transform.position.y;

            transform.LookAt(lookForward);
        }

        public void HandleAim()
        {
            RaycastHit hit;
            Weakness weakness;

            if (Physics.SphereCast(transform.position, 0.5f, transform.forward, out hit, 30,1 &~(1 << 6 | 1 << 12 | 1 << 10)))
            {
                if (hit.transform.TryGetComponent<Weakness>(out weakness))
                {
                    if (hit.transform != activeTarget)
                    {
                        activeTarget = hit.transform;
                        EventBus<ActiveTargetEvent>.Raise(new ActiveTargetEvent(hit.transform));
                    }

                    return;
                }
            }

            activeTarget = null;
            EventBus<ActiveTargetEvent>.Raise(new ActiveTargetEvent(null));
        }

        public void AddBullet(int index)
        {
            EventBus<AddBulletEvent>.Raise(new AddBulletEvent(bulletTypes[index]));
            // EventBus<AddBulletEvent>.Raise(new AddBulletEvent(bulletTypes[index]));
        }

        public void RemoveBullet()
        {
            EventBus<RemoveBulletEvent>.Raise(new RemoveBulletEvent(-1, -1));
        }

        private void OnChangeCamera(CameraChangeEvent ctx)
        {
            newActiveCam = ctx.cam.transform;
        }

        private void OnDialogueEntered()
        {
            _isDialogue = true;
        }

        private void OnDialogueExited()
        {
            _isDialogue = false;
        }

        private void OnPlayerDamage(PlayerDamagedEvent ctx)
        {
            Debug.Log(ctx.health);

            if (ctx.health == 1)
            {
                StartCoroutine(LowHealthEffect());
            }
        }

        IEnumerator LowHealthEffect()
        {
            EventBus<HapticFeedbackEvent>.Raise(new HapticFeedbackEvent(0.75f, 0.75f, 0.15f));

            yield return new WaitForSeconds(1.5f);

            StartCoroutine(LowHealthEffect());
        }
    }
}

