using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Player Movement")]
        public float MoveSpeed = 4.0f;
        public float SprintSpeed = 6.0f;
        public float RotationSpeed = 1.0f;
        public float SpeedChangeRate = 10.0f;

        [Header("Jumping & Gravity")]
        public float JumpHeight = 1.2f;
        public float Gravity = -15.0f;
        public float JumpTimeout = 0.1f;
        public float FallTimeout = 0.15f;

        [Header("Ground Check")]
        public bool Grounded = true;
        public float GroundedOffset = -0.14f;
        public float GroundedRadius = 0.5f;
        public LayerMask GroundLayers;

        [Header("Camera Settings")]
        public GameObject CinemachineCameraTarget;
        public float TopClamp = 90.0f;
        public float BottomClamp = -90.0f;
        [SerializeField, Range(0f, 10f)] private float _tiltMultiplier;

        [Header("Head Bobbing")]
        public float BobAmplitude = 0.05f;
        public float BobFrequency = 10f;
        public float LandBobIntensity = 0.1f;
        public float LandBobRecoverySpeed = 5f;

        [Header("Audio Clips")]
        [SerializeField] private AudioClip walking;
        [SerializeField] private AudioClip sprint;
        [SerializeField] private AudioClip jump;
        [SerializeField] private AudioClip land;

        // Control state
        private bool _controlsLocked = false;

        // Movement Variables
        private float _speed;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // Camera Rotation
        private float _cinemachineTargetPitch = 0f;

        // Components
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        // Head Bobbing
        private Vector3 _headStartPosition;
        private float _bobTimer = 0f;
        private bool _footstepPlayedCycle = false;
        private bool _landed = false;

        #region Camera Variables
        private float _currentTilt = 0f;
        private float _tiltVelocity;
        #endregion

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
        private bool IsCurrentDeviceMouse => _playerInput.currentControlScheme == "KeyboardMouse";
#endif

        private void Awake()
        {
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#endif

            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
            _headStartPosition = CinemachineCameraTarget.transform.localPosition;
        }

        private void Update()
        {
            if (!_controlsLocked)
            {
                JumpAndGravity();
                GroundedCheck();
                Move();
                HandleHeadBob();
            }
        }

        private void LateUpdate()
        {
            if (!_controlsLocked)
            {
                CameraRotation();
            }
        }

        // Public method to lock/unlock player controls
        public void LockControls(bool locked)
        {
            _controlsLocked = locked;

            // Reset input values when controls are locked
            if (locked)
            {
                _input.move = Vector2.zero;
                _input.look = Vector2.zero;
                _input.jump = false;
                _input.sprint = false;
            }
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            bool wasGrounded = Grounded;
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

            // Detect Landing
            if (!wasGrounded && Grounded)
            {
                _landed = true;
                SoundFXManager.Instance.PlaySoundFXClip(land, transform, 1f);
            }
        }

        private void CameraRotation()
        {
            if (_input.look.sqrMagnitude >= 0.01f)
            {
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                // Update camera pitch and rotation
                _cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
                _rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

                _cinemachineTargetPitch = Mathf.Clamp(_cinemachineTargetPitch, BottomClamp, TopClamp);

                // Implement camera tilt
                float targetTilt = -_input.look.x * _tiltMultiplier;
                _currentTilt = Mathf.SmoothDamp(_currentTilt, targetTilt, ref _tiltVelocity, 0.1f);

                // Apply rotation and tilt
                CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, _currentTilt);
                transform.Rotate(Vector3.up * _rotationVelocity);
            }
        }

        private void Move()
        {
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            float currentSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            _speed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * SpeedChangeRate);

            Vector3 moveDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
            _controller.Move(moveDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }

        private void HandleHeadBob()
        {
            if (!Grounded) return;

            if (_landed)
            {
                // Landing bob effect
                CinemachineCameraTarget.transform.localPosition = _headStartPosition + new Vector3(0f, -LandBobIntensity, 0f);
                _landed = false;
            }
            else if (_input.move.sqrMagnitude > 0.1f)
            {
                _bobTimer += Time.deltaTime;
                float bobOffsetY = Mathf.Sin(_bobTimer * BobFrequency) * BobAmplitude;
                CinemachineCameraTarget.transform.localPosition = _headStartPosition + new Vector3(0f, bobOffsetY, 0f);

                if (bobOffsetY < -BobAmplitude * 0.9f && !_footstepPlayedCycle)
                {
                    PlayFootstepSound();
                    _footstepPlayedCycle = true;
                }

                if (bobOffsetY > 0f) _footstepPlayedCycle = false;
            }
            else
            {
                CinemachineCameraTarget.transform.localPosition = Vector3.Lerp(CinemachineCameraTarget.transform.localPosition, _headStartPosition, Time.deltaTime * LandBobRecoverySpeed);
            }
        }

        private void PlayFootstepSound()
        {
            if (!Grounded) return;

            AudioClip clipToPlay = _input.sprint ? sprint : walking;
            SoundFXManager.Instance.PlaySoundFXClip(clipToPlay, transform, 1f);
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                _fallTimeoutDelta = FallTimeout;

                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                    SoundFXManager.Instance.PlaySoundFXClip(jump, transform, 1f);
                }

                _jumpTimeoutDelta = _jumpTimeoutDelta > 0.0f ? _jumpTimeoutDelta - Time.deltaTime : JumpTimeout;
            }
            else
            {
                _jumpTimeoutDelta = JumpTimeout;
                _fallTimeoutDelta -= Time.deltaTime;
                _input.jump = false;

                if (_verticalVelocity < _terminalVelocity)
                {
                    _verticalVelocity += Gravity * Time.deltaTime;
                }
            }
        }
    }
}