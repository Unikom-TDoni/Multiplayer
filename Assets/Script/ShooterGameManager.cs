using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Edu.Multiplayer.Network
{
    public sealed class ShooterGameManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _playerPerfab = default;

        private void Start()
        {
            PhotonNetwork.Instantiate(_playerPerfab.name, Vector2.zero, Quaternion.identity);
        }
    }
}
