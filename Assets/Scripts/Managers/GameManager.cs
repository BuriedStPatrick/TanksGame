using System.Collections;
using System.Linq;
using Assets.Scripts.Camera;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        public int NumRoundsToWin = 5;
        public float StartDelay = 3f;
        public float EndDelay = 3f;
        public CameraControl CameraControl;
        public Text MessageText;
        public GameObject TankPrefab;
        public TankManager[] Tanks;

        private int _roundNumber;
        private WaitForSeconds _startWait;
        private WaitForSeconds _endWait;
        private TankManager _roundWinner;
        private TankManager _gameWinner;

        private void Start()
        {
            _startWait = new WaitForSeconds(StartDelay);
            _endWait = new WaitForSeconds(EndDelay);

            SpawnAllTanks();
            SetCameraTargets();
            StartCoroutine(GameLoop());
        }


        private void SpawnAllTanks()
        {
            for (var i = 0; i < Tanks.Length; i++)
            {
                Tanks[i].Instance =
                    Instantiate(TankPrefab, Tanks[i].SpawnPoint.position, Tanks[i].SpawnPoint.rotation) as GameObject;
                Tanks[i].PlayerNumber = i + 1;
                Tanks[i].Setup();
            }
        }

        private void SetCameraTargets()
        {
            var targets = new Transform[Tanks.Length];
            for (var i = 0; i < targets.Length; i++)
            {
                targets[i] = Tanks[i].Instance.transform;
            }

            CameraControl.Targets = targets;
        }

        private IEnumerator GameLoop()
        {
            yield return StartCoroutine(RoundStarting());
            yield return StartCoroutine(RoundPlaying());
            yield return StartCoroutine(RoundEnding());

            if (_gameWinner != null)
            {
                Application.LoadLevel(Application.loadedLevel);
            }
            else
            {
                StartCoroutine(GameLoop());
            }
        }

        private IEnumerator RoundStarting()
        {
            ResetAllTanks();
            DisableTankControl();
            CameraControl.SetStartPositionAndSize();
            _roundNumber++;
            MessageText.text = "ROUND " + _roundNumber;
            yield return _startWait;
        }

        private IEnumerator RoundPlaying()
        {
            EnableTankControl();
            MessageText.text = string.Empty;
            while (!IsOneTankLeft())
            {
                yield return null;
            }
        }

        private IEnumerator RoundEnding()
        {
            DisableTankControl();
            _roundWinner = null; 
            _roundWinner = GetRoundWinner();

            if (_roundWinner != null)
            {
                _roundWinner.Wins++;
            }
            _gameWinner = GetGameWinner();
            MessageText.text = EndMessage();
            yield return _endWait;
        }

        private bool IsOneTankLeft()
        {
            return Tanks.Count(t => t.Instance.activeSelf) <= 1;
        }

        private TankManager GetRoundWinner()
        {
            return Tanks.FirstOrDefault(t => t.Instance.activeSelf);
        }

        private TankManager GetGameWinner()
        {
            return Tanks.FirstOrDefault(tankManager => tankManager.Wins == NumRoundsToWin);
        }

        private string EndMessage()
        {
            var message = "DRAW!";

            if (_roundWinner != null)
                message = _roundWinner.ColoredPlayerText + " WINS THE ROUND!";

            message += "\n\n\n\n";

            message = Tanks.Aggregate(message, (current, tankManager) =>
                current + (tankManager.ColoredPlayerText + ": " + tankManager.Wins + " WINS\n"));

            if (_gameWinner != null)
                message = _gameWinner.ColoredPlayerText + " WINS THE GAME!";

            return message;
        }

        private void ResetAllTanks()
        {
            foreach (var tankManager in Tanks)
            {
                tankManager.Reset();
            }
        }

        private void EnableTankControl()
        {
            foreach (var tankManager in Tanks)
            {
                tankManager.EnableControl();
            }
        }

        private void DisableTankControl()
        {
            foreach (var tankManager in Tanks)
            {
                tankManager.DisableControl();
            }
        }
    }
}