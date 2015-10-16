using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Objects.Tank;
using UnityEngine;

namespace Assets.Scripts.Shell
{
    public class ShellExplosion : MonoBehaviour
    {
        public LayerMask TankMask;
        public ParticleSystem ExplosionParticles;
        public AudioSource ExplosionAudio;
        public float MaxDamage = 100f;
        public float ExplosionForce = 1000f;
        public float MaxLifeTime = 2f;
        public float ExplosionRadius = 5f;

        private void Start()
        {
            Destroy(gameObject, MaxLifeTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            // Find all the tanks in an area around the shell and damage them.
            var colliders = Physics.OverlapSphere(transform.position, ExplosionRadius, TankMask);
            HandleCollisions(colliders);
            PlayParticles();
            PlaySound();
            DestroyExplosion();
        }

        private void HandleCollisions(IEnumerable<Collider> colliders)
        {
            foreach (var targetRigidbody in ExtractTargetRigidbodies(colliders))
            {
                targetRigidbody.AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius);
                var targetHealth = targetRigidbody.GetComponent<TankHealth>();
                DealDamage(targetHealth, CalculateDamage(targetRigidbody.position));
            }
        }

        private static IEnumerable<Rigidbody> ExtractTargetRigidbodies(IEnumerable<Collider> colliders)
        {
            return colliders
                .Where(c =>
                    c.GetComponent<Rigidbody>() &&
                    c.GetComponent<Rigidbody>().GetComponent<TankHealth>())
                .Select(col => col.GetComponent<Rigidbody>());
        }

        private static void DealDamage(TankHealth targetHealth, float damage)
        {
            targetHealth.TakeDamage(damage);
        }

        private void PlayParticles()
        {
            ExplosionParticles.transform.SetParent(null);
            ExplosionParticles.Play();
        }

        private void PlaySound()
        {
            ExplosionAudio.Play();
        }

        private void DestroyExplosion()
        {
            Destroy(ExplosionParticles.gameObject, ExplosionParticles.duration);
            Destroy(gameObject);
        }

        private float CalculateDamage(Vector3 targetPosition)
        {
            // Calculate the amount of damage a target should take based on it's position.
            return Mathf.Max(0f, CalculateRelativeDistance(targetPosition) * MaxDamage);
        }

        private float CalculateRelativeDistance(Vector3 targetPosition)
        {
            var explosionToTarget = targetPosition - transform.position;
            var explosionDistance = explosionToTarget.magnitude;
            var relativeDistance = (ExplosionRadius - explosionDistance) / ExplosionRadius;
            return relativeDistance;
        }
    }
}