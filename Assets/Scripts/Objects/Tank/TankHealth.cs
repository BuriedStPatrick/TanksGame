using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Objects.Tank
{
    public class TankHealth : MonoBehaviour
    {
        public float StartingHealth = 100f;
        public Slider Slider;
        public Image FillImage;
        public Color FullHealthColor = Color.green;
        public Color ZeroHealthColor = Color.red;
        public GameObject ExplosionPrefab;

        private AudioSource _explosionAudio;
        private ParticleSystem _explosionParticles;
        private float _currentHealth;
        private bool _isDead;

        private void Awake()
        {
            _explosionParticles = Instantiate(ExplosionPrefab).GetComponent<ParticleSystem>();
            _explosionAudio = _explosionParticles.GetComponent<AudioSource>();

            _explosionParticles.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _currentHealth = StartingHealth;
            _isDead = false;

            SetHealthUI();
        }

        public void TakeDamage(float amount)
        {
            // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.
            _currentHealth -= amount;
            SetHealthUI();
            if (_currentHealth <= 0f && !_isDead)
            {
                OnDeath();
            }
        }

        private void SetHealthUI()
        {
            // Adjust the value and colour of the slider.
            Slider.value = _currentHealth;
            FillImage.color = Color.Lerp(ZeroHealthColor, FullHealthColor, _currentHealth/StartingHealth);
        }

        private void OnDeath()
        {
            // Play the effects for the death of the tank and deactivate it.
            _isDead = true;
            PlayExplosionParticles();
            PlayExplosionAudio();
            gameObject.SetActive(false);
        }

        private void PlayExplosionParticles()
        {
            _explosionParticles.transform.position = transform.position;
            _explosionParticles.gameObject.SetActive(true);
            _explosionParticles.Play();
        }

        private void PlayExplosionAudio()
        {
            _explosionAudio.Play();
        }
    }
}