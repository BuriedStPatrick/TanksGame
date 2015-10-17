using System;
using UnityEngine;

namespace Assets.Scripts.Managers.Players
{
    [Serializable]
    public class Player
    {
        public Color Color;
        public Transform SpawnPoint;
        [HideInInspector] public int Number;
    }
}
