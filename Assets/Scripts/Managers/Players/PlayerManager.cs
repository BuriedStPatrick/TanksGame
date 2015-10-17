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
        [HideInInspector] public TankMovement Movement;
        [HideInInspector] public TankShooting Shooting;
        
        private GameObject _canvasGameObject;

        public void Setup()
        {
            SetupPlayer();
            SetupCanvas();
            SetupRenderers();
        }

        private void SetupRenderers()
        {
            foreach (var renderer in Instance.GetComponentsInChildren<MeshRenderer>())
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
            Movement = Instance.GetComponent<TankMovement>();
            Shooting = Instance.GetComponent<TankShooting>();
            Movement.PlayerNumber = Player.Number;
            Shooting.PlayerNumber = Player.Number;
        }

        public void DisableControl()
        {
            Movement.enabled = false;
            Shooting.enabled = false;

            _canvasGameObject.SetActive(false);
        }

        public void EnableControl()
        {
            Movement.enabled = true;
            Shooting.enabled = true;

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
