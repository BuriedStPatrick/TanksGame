using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UIDirectionControl : MonoBehaviour
    {
        public bool _useRelativeRotation = true;
        private Quaternion _relativeRotation;

        private void Start()
        {
            _relativeRotation = transform.parent.localRotation;
        }

        private void Update()
        {
            if (_useRelativeRotation)
                transform.rotation = _relativeRotation;
        }
    }
}
