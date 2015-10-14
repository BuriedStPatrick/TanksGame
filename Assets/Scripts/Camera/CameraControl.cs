using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Camera
{
    public class CameraControl : MonoBehaviour
    {
        public float DampTime = 0.2f;
        public float ScreenEdgeBuffer = 4f;
        public float MinSize = 6.5f;
        
        [HideInInspector]
        public Transform[] Targets;

        private UnityEngine.Camera _camera;
        private float _zoomSpeed;
        private Vector3 _moveVelocity;
        private Vector3 _desiredPosition;

        private void Awake()
        {
            _camera = GetComponentInChildren<UnityEngine.Camera>();
        }

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

            foreach (var target in Targets.Where(t => t.gameObject.activeSelf))
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
            var size = 0f;

            foreach (var target in Targets.Where(t => t.gameObject.activeSelf))
            {
                var targetLocalPos = transform.InverseTransformPoint(target.position);
                var desiredPosToTarget = targetLocalPos - desiredLocalPos;
                size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.y));
                size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.x) / _camera.aspect);
            }
        
            size += ScreenEdgeBuffer;
            size = Mathf.Max(size, MinSize);
            return size;
        }

        public void SetStartPositionAndSize()
        {
            FindAveragePosition();
            transform.position = _desiredPosition;
            _camera.orthographicSize = FindRequiredSize();
        }
    }
}