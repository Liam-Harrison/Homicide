using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Homicide.Game.Controllers
{
    [RequireComponent(typeof(CharacterController))]
    public class ThirdPersonController : GameBehaviour, InputActions.IPlayerActions, IUpdate, ILateUpdate
    {
        [TabGroup("Locomotion")]
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float moveSpeed = 2.0f;

        [TabGroup("Locomotion")]
        [Tooltip("Sprint speed of the character in m/s")]
        public float sprintSpeed = 5.335f;

        [TabGroup("Locomotion")]
        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float rotationSmoothTime = 0.12f;

        [TabGroup("Locomotion")]
        [Tooltip("Acceleration and deceleration")]
        public float speedChangeRate = 10.0f;

        [TabGroup("Locomotion")]
        public AudioClip[] footstepAudioClips;
        [Range(0, 1)] public float footstepAudioVolume = 0.5f;

        [SerializeField, TabGroup("Locomotion")]
        private bool hasControl = false;
        
        [TabGroup("Camera")]
        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject cinemachineCameraTarget;
        
        [TabGroup("Camera")]
        [Tooltip("How far in degrees can you move the camera up")]
        public float topClamp = 70.0f;

        [TabGroup("Camera")]
        [Tooltip("How far in degrees can you move the camera down")]
        public float bottomClamp = -30.0f;

        [TabGroup("Camera")]
        [Tooltip("Additional degrees to override the camera. Useful for fine tuning camera position when locked")]
        public float cameraAngleOverride = 0.0f;

        [TabGroup("Camera")]
        [Tooltip("For locking the camera position on all axis")]
        public bool lockCameraPosition = false;
        
        [TabGroup("Camera"), PropertyRange(0.1, 2)]
        public float sensitivity;
        
        [TabGroup("Camera"), Title("Aim Camera")]
        [SerializeField] private Cinemachine.CinemachineVirtualCamera aimCamera;

        [TabGroup("Camera")]
        [SerializeField] private Transform aimTarget;
        
        [TabGroup("Camera")]
        [SerializeField] private float aimOffset;
        
        [TabGroup("Camera")]
        [SerializeField] private LayerMask aimMask = new LayerMask();

        // Cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // Player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;

        // Animation IDs
        private readonly int _animIDSpeed = Animator.StringToHash("Speed");
        private readonly int _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");

        // References
        private PlayerInput _playerInput;
        private CharacterController _controller;
        private Animator _animator;
        private Camera _camera;
        
        // Input
        private Vector2 _look;
        private Vector2 _movement;
        private bool _sprinting;
        private bool _aiming;
        
        private const float Threshold = 0.01f;
        
        private InputActions.PlayerActions _actions;

        protected void Awake()
        {
            _actions = new InputActions.PlayerActions(SettingsManager.InputActions);
            _actions.SetCallbacks(this);
            
            _controller = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
            _camera = Camera.main;
            
            SetPlayerControl(hasControl);
        }

        private void OnEnable()
        {
            _actions.Enable();
        }

        private void OnDisable()
        {
            _actions.Disable();
        }

        public void GameUpdate()
        {
            if (!hasControl)
            {
                _movement = Vector2.zero;
                _sprinting = false;
                _aiming = false;
            }

            Move();
            if (_aiming) Aiming();
        }

        public void GameLateUpdate()
        {
            CameraRotation();
        }

        private void Aiming()
        {
            var point = GetAimPoint();
            aimTarget.position = point;
            point.y = transform.position.y;

            var dir = (point - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, Quaternion.Euler(0, aimOffset, 0) * dir, Time.smoothDeltaTime * 20f);
        }
        
        private Vector3 GetAimPoint()
        {
            var ray = _camera.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));
            return Physics.Raycast(ray, out var hit, 1000f, aimMask) ? hit.point : ray.GetPoint(1000f);
        }

        private void CameraRotation()
        {
            if (_look.sqrMagnitude >= Threshold && !lockCameraPosition)
            {
                _look *= sensitivity;
                _cinemachineTargetYaw += _look.x;
                _cinemachineTargetPitch += SettingsManager.Inverted ? _look.y : -_look.y;
            }

            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);

            cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + cameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            var targetSpeed = _sprinting ? sprintSpeed : moveSpeed;
            
            if (_movement == Vector2.zero) targetSpeed = 0.0f;

            var velocity = _controller.velocity;
            var currentHorizontalSpeed = new Vector3(velocity.x, 0.0f, velocity.z).magnitude;
            
            const float speedOffset = 0.1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed,Time.deltaTime * speedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            var inputDirection = new Vector3(_movement.x, 0.0f, _movement.y).normalized;

            if (_movement != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _camera.transform.eulerAngles.y;
                var rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    rotationSmoothTime);

                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            var targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, 1);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (!(animationEvent.animatorClipInfo.weight > 0.5f)) return;
            
            var index = Random.Range(0, footstepAudioClips.Length);
            AudioSource.PlayClipAtPoint(footstepAudioClips[index], transform.TransformPoint(_controller.center), footstepAudioVolume);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _movement = context.ReadValue<Vector2>();
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            _sprinting = context.performed;
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            _look = context.ReadValue<Vector2>();
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            if (context.performed)
                _aiming = true;
            else if (context.canceled)
                _aiming = false;
            
            aimCamera.gameObject.SetActive(_aiming);
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        public void SetPlayerControl(bool state)
        {
            hasControl = state;
            Cursor.lockState = hasControl ? CursorLockMode.Locked : CursorLockMode.Confined;
        }
    }
}