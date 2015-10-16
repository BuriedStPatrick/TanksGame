using System;
using Assets.Scripts.Objects.Tank;
using UnityEngine;

namespace Assets.Scripts.Managers.Players
{
    [Serializable]
    public class Player
    {
        public Color Color;
        public Transform SpawnPoint;
        [HideInInspector] public int Number;
        [HideInInspector] public TankMovement Movement;
        [HideInInspector] public TankShooting Shooting;

        public void Setup()
        {
            Movement.PlayerNumber = Number;
            Shooting.PlayerNumber = Number;
        }
    }
}
