using System.Collections;
using System.Linq;
using Assets.Scripts.Camera;
using Assets.Scripts.Managers.Players;
using UnityEngine;

namespace Assets.Scripts.Managers.Games
{
    public class GameManager : MonoBehaviour
    {
        public Game Game;
        public CameraControl CameraControl;
        public GameObject TankPrefab;
        public PlayerManager[] PlayerManagers;

        private int _roundNumber;
        private WaitForSeconds _startWait;
        private WaitForSeconds _endWait;
        private PlayerManager _roundWinner;
        private PlayerManager _gameWinner;

        private void Awake()
        {
            _startWait = new WaitForSeconds(Game.StartDelay);
            _endWait = new WaitForSeconds(Game.EndDelay);
            SpawnAllTanks();
            SetCameraTargets();
            StartCoroutine(GameLoop());
        }

        private void SpawnAllTanks()
        {
            for (var i = 0; i < PlayerManagers.Length; i++)
            {
                PlayerManagers[i].Instance =
                    Instantiate(TankPrefab, PlayerManagers[i].Player.SpawnPoint.position, PlayerManagers[i].Player.SpawnPoint.rotation) as GameObject;
                PlayerManagers[i].Player.Number = i + 1;
                PlayerManagers[i].Setup();
            }
        }

        private void SetCameraTargets()
        {
            var targets = new Transform[PlayerManagers.Length];
            for (var i = 0; i < targets.Length; i++)
            {
                targets[i] = PlayerManagers[i].Instance.transform;
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
            Game.MessageText.text = "ROUND " + _roundNumber;
            yield return _startWait;
        }

        private IEnumerator RoundPlaying()
        {
            EnableTankControl();
            Game.MessageText.text = string.Empty;
            while (!IsOnePlayerLeft())
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
            Game.MessageText.text = EndMessage();
            yield return _endWait;
        }

        private bool IsOnePlayerLeft()
        {
            return PlayerManagers.Count(t => t.Instance.activeSelf) <= 1;
        }

        private PlayerManager GetRoundWinner()
        {
            return PlayerManagers.FirstOrDefault(t => t.Instance.activeSelf);
        }

        private PlayerManager GetGameWinner()
        {
            return PlayerManagers.FirstOrDefault(tankManager => tankManager.Wins == Game.NumRoundsToWin);
        }

        private string EndMessage()
        {
            var message = "DRAW!";

            if (_roundWinner != null)
                message = _roundWinner.ColoredPlayerText + " WINS THE ROUND!";

            message += "\n\n\n\n";

            message = PlayerManagers.Aggregate(message, (current, tankManager) =>
                current + (tankManager.ColoredPlayerText + ": " + tankManager.Wins + " WINS\n"));

            if (_gameWinner != null)
                message = _gameWinner.ColoredPlayerText + " WINS THE GAME!";

            return message;
        }

        private void ResetAllTanks()
        {
            foreach (var tankManager in PlayerManagers)
            {
                tankManager.Reset();
            }
        }

        private void EnableTankControl()
        {
            foreach (var tankManager in PlayerManagers)
            {
                tankManager.EnableControl();
            }
        }

        private void DisableTankControl()
        {
            foreach (var tankManager in PlayerManagers)
            {
                tankManager.DisableControl();
            }
        }
    }
}