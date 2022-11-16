using TMPro;
using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Edu.Multiplayer.Core;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Edu.Multiplayer.Network
{
    public sealed class ConnectManager : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private TMP_InputField _usernameInputField = default;

        [SerializeField]
        private TMP_Text _feedbackText = default;

        [SerializeField]
        private Button _btnConnect = default;

        [SerializeField]
        private Button _btnExit = default;

        private void Awake()
        {
            _btnConnect.onClick.AddListener(() =>
            {
                _feedbackText.text = String.Empty;
                if(_usernameInputField.text.Length < 3)
                {
                    _feedbackText.text = "Username min 3 character";
                    return;
                }

                PlayerPrefs.SetString("Nickname", _usernameInputField.text);
                PhotonNetwork.NickName = _usernameInputField.text;
                PhotonNetwork.AutomaticallySyncScene = true;

                PhotonNetwork.ConnectUsingSettings();
                _feedbackText.text = "Connecting...";
            });

            _usernameInputField.text = PlayerPrefs.GetString("Nickname");

            _btnExit.onClick.AddListener(() => Application.Quit());
        }

        public override void OnConnectedToMaster()
        {
            _feedbackText.text = "Connected to Master";
            StartCoroutine(LoadLevelAfterConnectAndReady());
        }

        private IEnumerator LoadLevelAfterConnectAndReady()
        {
            while (!PhotonNetwork.IsConnectedAndReady)
                yield return null;

            SceneManager.LoadScene(GameManager.Instance.SceneObjects.Lobby);
        }
    }
}
