using System;
using Assets.Scripts.Objects.Tank;
using UnityEngine;

namespace Assets.Scripts.Managers.Players
{
    [Serializable]
    public class PlayerManager
    {
        public Player Player;

        [HideInInspector] public string ColoredPlayerText;
        [HideInInspector] public GameObject Instance;
        [HideInInspector] public int Wins;
        
        private GameObject _canvasGameObject;

        public void Setup()
        {
            SetupPlayer();
            SetupCanvas();
            SetupRenderers();
        }

        private void SetupRenderers()
        {
            var renderers = Instance.GetComponentsInChildren<MeshRenderer>();

            foreach (var renderer in renderers)
            {
                renderer.material.color = Player.Color;
            }
        }

        private void SetupCanvas()
        {
            _canvasGameObject = Instance.GetComponentInChildren<Canvas>().gameObject;
            ColoredPlayerText = string.Format("<color=#{0}>PLAYER {1}</color>",
                ColorUtility.ToHtmlStringRGB(Player.Color),
                Player.Number);
        }

        private void SetupPlayer()
        {
            Player.Movement = Instance.GetComponent<TankMovement>();
            Player.Shooting = Instance.GetComponent<TankShooting>();
            Player.Setup();
        }

        public void DisableControl()
        {
            Player.Movement.enabled = false;
            Player.Shooting.enabled = false;

            _canvasGameObject.SetActive(false);
        }

        public void EnableControl()
        {
            Player.Movement.enabled = true;
            Player.Shooting.enabled = true;

            _canvasGameObject.SetActive(true);
        }

        public void Reset()
        {
            Instance.transform.position = Player.SpawnPoint.position;
            Instance.transform.rotation = Player.SpawnPoint.rotation;

            Instance.SetActive(false);
            Instance.SetActive(true);
        }
    }
}
