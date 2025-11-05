using State_Machine;
using UnityEngine;

namespace State_Machine
{
    public class PlayerManager : MonoBehaviour
    {
        CharacterController _cc;
        Animator _animator;
        Camera _cam;

        public float playerWalkSpeed;
        public float playerSprintSpeed;
        public float playerAimSpeed;
        public float currentSpeed;
        private Vector3 _movement;
        private Vector2 _aim;
        public Vector3 lookDir;

        private bool _isAiming;
        private bool _isSprinting;

        public StateMachine stateMachine;

        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
            _cam = Camera.main;

            currentSpeed = playerWalkSpeed;

            SetupStateMachine();
        }

        private void OnEnable()
        {
            InputHandler.AimEvent += OnAim;
            InputHandler.SprintEvent += OnSprint;
        }

        private void OnDisable()
        {
            InputHandler.AimEvent -= OnAim;
            InputHandler.SprintEvent -= OnSprint;
        }

        void SetupStateMachine()
        {
            stateMachine = new StateMachine();

            var aimState = new PlayerAimState(this, _animator);
            var moveState = new PlayerMoveState(this, _animator);
            var idleState = new PlayerIdleState(this, _animator);
            var sprintState = new PlayerSprintState(this, _animator);

            At(aimState, idleState, new FuncPredicate(() => _aim == Vector2.zero));

            At(idleState, aimState, new FuncPredicate(() => _aim != Vector2.zero));
            At(idleState, moveState, new FuncPredicate(() => _movement != Vector3.zero));

            At(moveState, idleState, new FuncPredicate(() => _movement == Vector3.zero));
            At(moveState, sprintState, new FuncPredicate(() => _isSprinting));
            At(moveState, aimState, new FuncPredicate(() => _aim != Vector2.zero));

            At(sprintState, moveState, new FuncPredicate(() => !_isSprinting));
            At(sprintState, idleState, new FuncPredicate(() => _movement == Vector3.zero));
            At(sprintState, aimState, new FuncPredicate(() => _aim != Vector2.zero));

            stateMachine.SetState(idleState);
        }

        void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        private void Update()
        {
            var movementDirection = InputHandler.MOVE.ReadValue<Vector2>();
            var aimDirection = InputHandler.LOOK.ReadValue<Vector2>();

            _movement = new Vector3(movementDirection.x, 0, movementDirection.y);
            _aim = aimDirection;

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

            Debug.Log("No collider hit");
            return Vector3.negativeInfinity;
        }

        public void HandleMovement()
        {
            Vector3 camForward = Vector3.ProjectOnPlane(_cam.transform.forward, GetPlaneNormal());
            Vector3 camRight = Vector3.ProjectOnPlane(_cam.transform.right, GetPlaneNormal());

            Vector3 dir = camForward * _movement.z + camRight * _movement.x;
            lookDir = dir;

            transform.LookAt(transform.position + lookDir.normalized);

            _cc.Move(dir.normalized * currentSpeed * Time.deltaTime);
        }

        public void HandleLook()
        {
            Vector3 camForward = Vector3.ProjectOnPlane(_cam.transform.forward, GetPlaneNormal());
            Vector3 camRight = Vector3.ProjectOnPlane(_cam.transform.right, GetPlaneNormal());

            Vector3 dir = camForward * _aim.y + camRight * _aim.x;
            lookDir = dir;

            transform.LookAt(transform.position + lookDir.normalized);
        }
    }
}

