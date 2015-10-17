using System;
using UnityEngine.UI;

namespace Assets.Scripts.Managers.Games
{
    [Serializable]
    public class Game
    {
        public int NumRoundsToWin = 5;
        public float StartDelay = 3f;
        public float EndDelay = 3f;
        public Text MessageText;
    }
}