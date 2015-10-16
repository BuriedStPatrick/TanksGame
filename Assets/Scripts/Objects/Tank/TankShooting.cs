using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Objects.Tank
{
    public class TankShooting : MonoBehaviour
    {
        public int PlayerNumber = 1;
        public Rigidbody Shell;
        public Transform FireTransform;
        public Slider AimSlider;
        public AudioSource ShootingAudio;
        public AudioClip ChargingClip;
        public AudioClip FireClip;
        public float MinLaunchForce = 15f;
        public float MaxLaunchForce = 30f;
        public float MaxChargeTime = 0.75f;

        private string _fireButton;
        private float _currentLaunchForce;
        private float _chargeSpeed;
        private bool _hasFired;

        private void OnEnable()
        {
            _currentLaunchForce = MinLaunchForce;
            AimSlider.value = MinLaunchForce;
        }

        private void Start()
        {
            _fireButton = "Fire" + PlayerNumber;
            _chargeSpeed = (MaxLaunchForce - MinLaunchForce) / MaxChargeTime;
        }

        private void Update()
        {
            // Track the current state of the fire button and make decisions based on the current launch force.
            AimSlider.value = MinLaunchForce;
            if (_currentLaunchForce >= MaxLaunchForce && !_hasFired)
            {
                // at max charge, not fired
                _currentLaunchForce = MaxLaunchForce;
                Fire();
            }
            else if (Input.GetButtonDown(_fireButton))
            {
                // have we pressed fire for the first time?
                _hasFired = false;
                _currentLaunchForce = MinLaunchForce;
                PlayChargingAudio();
            }
            else if (Input.GetButton(_fireButton) && !_hasFired)
            {
                // holding the fire button, not yet fired
                _currentLaunchForce += _chargeSpeed * Time.deltaTime;
                AimSlider.value = _currentLaunchForce;
            }
            else if (Input.GetButtonUp(_fireButton) && !_hasFired)
            {
                // we released the button, having not fired yet
                Fire();
            }
        }

        private void PlayChargingAudio()
        {
            ShootingAudio.clip = ChargingClip;
            ShootingAudio.Play();
        }

        private void PlayFireAudio()
        {
            ShootingAudio.clip = FireClip;
            ShootingAudio.Play();
        }

        private void Fire()
        {
            // Instantiate and launch the shell.
            _hasFired = true;
            var shellInstance = Instantiate(Shell, FireTransform.position, FireTransform.rotation) as Rigidbody;
            shellInstance.velocity = _currentLaunchForce * FireTransform.forward;
            PlayFireAudio();
            _currentLaunchForce = MinLaunchForce;
        }
    }
}