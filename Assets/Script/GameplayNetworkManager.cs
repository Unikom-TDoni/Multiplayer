using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Edu.Multiplayer.Core
{
    public sealed class GameplayNetworkManager : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private Button _btnBackToLogin = default;

        [SerializeField]
        private Button _btnBackToLobby = default;

        [SerializeField]
        private Button _btnReplay = default;

        [SerializeField]
        private Button _btnQuit = default;

        [SerializeField]
        private Button _btnBackToLoginGameOver = default;

        private IEnumerator BackToLobby()
        {
            PhotonNetwork.LeaveRoom();
            while (PhotonNetwork.InRoom || !PhotonNetwork.IsConnectedAndReady)
                yield return null;
            SceneManager.LoadScene(GameManager.Instance.SceneObjects.Lobby);
        }

        private IEnumerator BackToLogin()
        {
            PhotonNetwork.Disconnect();
            while (PhotonNetwork.IsConnected)
                yield return null;
            SceneManager.LoadScene(GameManager.Instance.SceneObjects.Login);
        }

        private IEnumerator Quit()
        {
            PhotonNetwork.Disconnect();
            while (PhotonNetwork.IsConnected)
                yield return null;
            Application.Quit();
        }

        private void Awake()
        {
            _btnBackToLobby.onClick.AddListener(() => StartCoroutine(BackToLobby()));
            _btnBackToLobby.onClick.AddListener(() => StartCoroutine(BackToLogin()));
            _btnBackToLoginGameOver.onClick.AddListener(() => StartCoroutine(BackToLogin()));
            _btnQuit.onClick.AddListener(() => StartCoroutine(Quit()));
            _btnReplay.onClick.AddListener(() =>
            {
                if (PhotonNetwork.IsMasterClient)
                    SceneManager.LoadScene(GameManager.Instance.SceneObjects.Gameplay);
            });
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
                PhotonNetwork.LeaveRoom();
        }
    }
}
