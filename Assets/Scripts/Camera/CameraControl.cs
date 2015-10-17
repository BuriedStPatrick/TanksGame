using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Extensions;
using UnityEngine;

namespace Assets.Scripts.Camera
{
    public class CameraControl : MonoBehaviour
    {
        public float DampTime = 0.2f;
        public float ScreenEdgeBuffer = 4f;
        public float MinSize = 6.5f;
        
        [HideInInspector] public Transform[] Targets;

        private UnityEngine.Camera _camera
        {
            get {return GetComponentInChildren<UnityEngine.Camera>();}
        }

        private float _zoomSpeed;
        private Vector3 _moveVelocity;
        private Vector3 _desiredPosition;

        private void FixedUpdate()
        {
            Move();
            Zoom();
        }

        private void Move()
        {
            FindAveragePosition();
            transform.position = Vector3.SmoothDamp(transform.position, _desiredPosition, ref _moveVelocity, DampTime);
        }

        private void FindAveragePosition()
        {
            var averagePos = new Vector3();
            var numTargets = 0;
            foreach (var target in Targets.Where(t => t.IsGameObjectActive()))
            {
                averagePos += target.position;
                numTargets++;
            }

            if (numTargets > 0)
            {
                averagePos /= numTargets;
            }
            averagePos.y = transform.position.y;
            _desiredPosition = averagePos;
        }

        private void Zoom()
        {
            var requiredSize = FindRequiredSize();
            _camera.orthographicSize = Mathf.SmoothDamp(_camera.orthographicSize, requiredSize, ref _zoomSpeed, DampTime);
        }

        private float FindRequiredSize()
        {
            var desiredLocalPos = transform.InverseTransformPoint(_desiredPosition);
            var size = CalculateSizeForEachTarget(desiredLocalPos);
            return CalculateScreenBuffer(size);
        }

        private float CalculateSizeForEachTarget(Vector3 desiredLocalPos)
        {
            var size = 0f;
            foreach (var desiredPosToTarget in GetDesiredTargetPositions(desiredLocalPos))
            {
                size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));
                size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / _camera.aspect);
            }
            return size;
        }

        private IEnumerable<Vector3> GetDesiredTargetPositions(Vector3 desiredLocalPos)
        {
            return Targets.Where(t => t.gameObject.activeSelf)
                    .Select(target => transform.InverseTransformPoint(target.position))
                    .Select(targetLocalPos => targetLocalPos - desiredLocalPos);
        }

        private float CalculateScreenBuffer(float size)
        {
            return Mathf.Max(size + ScreenEdgeBuffer, MinSize);
        }

        public void SetStartPositionAndSize()
        {
            FindAveragePosition();
            transform.position = _desiredPosition;
            _camera.orthographicSize = FindRequiredSize();
        }
    }
}