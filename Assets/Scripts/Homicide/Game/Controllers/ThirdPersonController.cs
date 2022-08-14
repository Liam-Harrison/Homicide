using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Homicide.Game.Controllers
{
    [RequireComponent(typeof(CharacterController))]
    public class ThirdPersonController : GameBehaviour, InputActions.IPlayerActions, IUpdate, ILateUpdate
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float moveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float sprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float rotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float speedChangeRate = 10.0f;

        public AudioClip[] footstepAudioClips;
        [Range(0, 1)] public float footstepAudioVolume = 0.5f;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;

        // animation IDs
        private readonly int _animIDSpeed = Animator.StringToHash("Speed");
        private readonly int _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");

        private PlayerInput _playerInput;
        private CharacterController _controller;
        private Animator _animator;
        private Camera _camera;

        private bool _sprinting;
        private Vector2 _movement;
        
        private InputActions.PlayerActions _actions;

        protected void Awake()
        {
            _actions = new InputActions.PlayerActions(SettingsManager.InputActions);
            _actions.SetCallbacks(this);
            
            _controller = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
            _camera = Camera.main;
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
            Move();
        }

        public void GameLateUpdate()
        {
            CameraUpdate();
        }

        private void CameraUpdate()
        {
            
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
    }
}