using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

namespace Edu.Multiplayer.Core.Scene
{
    [Serializable]
    public sealed class LoginSceneLayoutController
    {
        [SerializeField]
        private Button _btnExit = default;

        [SerializeField]
        private Button _btnConnect = default;

        [SerializeField]
        private TMP_InputField _infName = default;

        public void OnAwake()
        {
            _btnExit.onClick.AddListener(() => Application.Quit());
            _btnConnect.onClick.AddListener(() => PhotonNetwork.ConnectUsingSettings());
        }
    }
}
