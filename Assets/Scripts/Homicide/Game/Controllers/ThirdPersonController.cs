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
        [Tooltip("Acceleration and deceleration")]
        public float gravity = 10.0f;

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
        [SerializeField] private LayerMask aimMask = new();

        // Cinemachine
        private float cinemachineTargetYaw;
        private float cinemachineTargetPitch;

        // Player
        private float speed;
        private float animationBlend;
        private float targetRotation = 0.0f;
        private float rotationVelocity;
        private float verticalVelocity = -6f;

        // Animation IDs
        private readonly int animIDSpeed = Animator.StringToHash("Speed");
        private readonly int animIDMotionSpeed = Animator.StringToHash("MotionSpeed");

        // References
        private PlayerInput playerInput;
        private CharacterController controller;
        private Animator animator;
        private new Camera camera;
        
        // Input
        private Vector2 look;
        private Vector2 movement;
        private bool sprinting;
        private bool aiming;
        
        private const float Threshold = 0.01f;
        
        private InputActions.PlayerActions actions;

        protected void Awake()
        {
            actions = new(SettingsManager.InputActions);
            actions.SetCallbacks(this);
            
            controller = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            camera = Camera.main;
            
            SetPlayerControl(hasControl);
        }

        private void OnEnable()
        {
            actions.Enable();
        }

        private void OnDisable()
        {
            actions.Disable();
        }

        public void GameUpdate()
        {
            if (!hasControl)
            {
                movement = Vector2.zero;
                sprinting = false;
                aiming = false;
            }

            Move();
            if (aiming) Aiming();
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
            var ray = camera.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));
            return Physics.Raycast(ray, out var hit, 1000f, aimMask) ? hit.point : ray.GetPoint(1000f);
        }

        private void CameraRotation()
        {
            if (look.sqrMagnitude >= Threshold && !lockCameraPosition)
            {
                look *= sensitivity;
                cinemachineTargetYaw += look.x;
                cinemachineTargetPitch += SettingsManager.Inverted ? look.y : -look.y;
            }

            cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
            cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, bottomClamp, topClamp);

            cinemachineCameraTarget.transform.rotation = Quaternion.Euler(cinemachineTargetPitch + cameraAngleOverride,
                cinemachineTargetYaw, 0.0f);
        }
        
        private void Move()
        {
            var targetSpeed = sprinting ? sprintSpeed : moveSpeed;
            
            if (movement == Vector2.zero) targetSpeed = 0.0f;

            var velocity = controller.velocity;
            var currentHorizontalSpeed = new Vector3(velocity.x, 0.0f, velocity.z).magnitude;
            
            const float speedOffset = 0.1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed,Time.deltaTime * speedChangeRate);
                speed = Mathf.Round(speed * 1000f) / 1000f;
            }
            else
            {
                speed = targetSpeed;
            }

            animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
            if (animationBlend < 0.01f) animationBlend = 0f;

            var inputDirection = new Vector3(movement.x, 0.0f, movement.y).normalized;

            if (movement != Vector2.zero)
            {
                targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  camera.transform.eulerAngles.y;
                var rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity,
                    rotationSmoothTime);

                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            var targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

            controller.Move(targetDirection.normalized * (speed * Time.deltaTime) +
                             new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);

            animator.SetFloat(animIDSpeed, animationBlend);
            animator.SetFloat(animIDMotionSpeed, 1);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (!(animationEvent.animatorClipInfo.weight > 0.5f)) return;
            
            var index = Random.Range(0, footstepAudioClips.Length);
            AudioSource.PlayClipAtPoint(footstepAudioClips[index], transform.TransformPoint(controller.center), footstepAudioVolume);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            movement = context.ReadValue<Vector2>();
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            sprinting = context.performed;
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            look = context.ReadValue<Vector2>();
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            if (context.performed)
                aiming = true;
            else if (context.canceled)
                aiming = false;
            
            aimCamera.gameObject.SetActive(aiming);
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