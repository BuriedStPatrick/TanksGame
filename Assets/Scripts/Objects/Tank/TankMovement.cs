using Assets.Scripts.Enums;
using UnityEngine;

namespace Assets.Scripts.Objects.Tank
{
    public class TankMovement : MonoBehaviour
    {
        public int PlayerNumber = 1;         
        public float Speed = 12f;            
        public float TurnSpeed = 180f;
        public AudioSource MovementAudio;
        public AudioClip EngineIdling;
        public AudioClip EngineDriving;
        public float PitchRange = 0.2f;

        private string _movementAxisName;
        private string _turnAxisName;
        private Rigidbody _rigidbody;
        private float _movementInputValue;
        private float _turnInputValue;
        private float _originalPitch;
        private TankStates State;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            State = TankStates.Idle;
        }

        private void OnEnable ()
        {
            _rigidbody.isKinematic = false;
            _movementInputValue = 0f;
            _turnInputValue = 0f;
        }

        private void OnDisable ()
        {
            _rigidbody.isKinematic = true;
        }

        private void Start()
        {
            _movementAxisName = "Vertical" + PlayerNumber;
            _turnAxisName = "Horizontal" + PlayerNumber;

            _originalPitch = MovementAudio.pitch;
        }

        private void Update()
        {
            HandleInput();
            SetState();
            EngineAudio();
        }

        private void HandleInput()
        {
            _movementInputValue = Input.GetAxis(_movementAxisName);
            _turnInputValue = Input.GetAxis(_turnAxisName);
        }

        private void EngineAudio()
        {
            // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.
            if (Mathf.Abs(_movementInputValue) < 0.1f && Mathf.Abs(_turnInputValue) < 0.1f)
            {
                if (MovementAudio.clip == EngineDriving)
                {
                    MovementAudio.clip = EngineIdling;
                    MovementAudio.pitch = Random.Range(_originalPitch - PitchRange, _originalPitch + PitchRange);
                    MovementAudio.Play();
                }
            }
            else if (MovementAudio.clip == EngineIdling)
            {
                MovementAudio.clip = EngineDriving;
                MovementAudio.pitch = Random.Range(_originalPitch - PitchRange, _originalPitch + PitchRange);
                MovementAudio.Play();
            }
        }

        private void FixedUpdate()
        {
            // Move and turn the tank.
            Move();
            Turn();
        }

        private void Move()
        {
            // Adjust the position of the tank based on the player's input.
            var movement = transform.forward * _movementInputValue * Speed * Time.deltaTime;
            _rigidbody.MovePosition(_rigidbody.position + movement);
            
        }

        private void Turn()
        {
            // Adjust the rotation of the tank based on the player's input.
            var turnAmount = _turnInputValue * TurnSpeed * Time.deltaTime;
            var turnRotation = Quaternion.Euler(0f, turnAmount, 0);
            _rigidbody.MoveRotation(_rigidbody.rotation * turnRotation);
        }

        private void SetState()
        {
            if (_movementInputValue > 0f)
            {
                State = TankStates.Moving;
            }
            else
            {
                State = TankStates.Idle;
            }
        }
    }
}