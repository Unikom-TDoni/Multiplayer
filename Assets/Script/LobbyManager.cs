using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Photon.Realtime;
using Edu.Multiplayer.Core;

namespace Edu.Multiplayer.Network
{
    public sealed class LobbyManager : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private TMP_InputField _newRoomInputField = default;

        [SerializeField]
        private TMP_Text _txtFeedback = default;

        [SerializeField]
        private Button _btnCreateRoom = default;

        [SerializeField]
        private GameObject _roomListObject = default;

        [SerializeField]
        private GameObject _playerListObj = default;

        [SerializeField]
        private GameObject _room = default;

        [SerializeField]
        private GameObject _lobby = default;

        [SerializeField]
        private RoomItem _roomItemPerfab = default;

        [SerializeField]
        private PlayerItem _playerItemPerfab = default;

        [SerializeField]
        private TMP_Text _txtRoomName = default;

        [SerializeField]
        private Button _btnStartGame = default;

        [SerializeField]
        private Button _btnLeave = default;

        private readonly List<RoomItem> _roomItemList = new();

        private readonly List<PlayerItem> _playerItemList = new();

        private readonly Dictionary<string, RoomInfo> _roomInfoCache = new();

        private void Start()
        {
            _txtFeedback.text = "Joinning Lobby";

            PhotonNetwork.JoinLobby();

            _btnCreateRoom.onClick.AddListener(() =>
            {
                _txtFeedback.text = String.Empty;
                if (_newRoomInputField.text.Length < 3)
                {
                    _txtFeedback.text = "Room Name min 3 characters";
                    return;
                }

                var roomOptions = new RoomOptions
                {
                    MaxPlayers = 2,
                };
                PhotonNetwork.CreateRoom(_newRoomInputField.text, roomOptions);
            });

            _btnStartGame.onClick.AddListener(() =>
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.CurrentRoom.IsOpen = false;
                    PhotonNetwork.LoadLevel(GameManager.Instance.SceneObjects.Gameplay);
                }
            });
        }

        public void JoinRoom(string roomName)
        {
            PhotonNetwork.JoinRoom(roomName);
        }

        public override void OnCreatedRoom()
        {
            _txtFeedback.text = $"Created room {PhotonNetwork.CurrentRoom.Name}";
        }

        private void SetStartGameButton()
        {
            _btnStartGame.interactable = PhotonNetwork.CurrentRoom.PlayerCount > 1;
            _btnStartGame.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        }

        public override void OnJoinedRoom()
        {
            _txtFeedback.text = $"Created room : {PhotonNetwork.CurrentRoom}";
            _txtRoomName.text = PhotonNetwork.CurrentRoom.Name;
            _room.SetActive(true);
            _lobby.SetActive(false);
            UpdatePlayerlist();
            SetStartGameButton();
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            UpdatePlayerlist();
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            UpdatePlayerlist();
        }

        private void UpdatePlayerlist()
        {
            foreach (var item in _playerItemList)
                Destroy(item.gameObject);

            _playerItemList.Clear();

            foreach (var (key, value) in PhotonNetwork.CurrentRoom.Players)
            {
                var playerItem = Instantiate(_playerItemPerfab, _playerListObj.transform);
                playerItem.Set(value);
                _playerItemList.Add(playerItem);

                if (value == PhotonNetwork.LocalPlayer)
                    playerItem.transform.SetAsFirstSibling();
            }

            SetStartGameButton();
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            _txtFeedback.text = $"{returnCode} : {message}";
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            foreach (var item in roomList)
                _roomInfoCache[item.Name] = item;

            foreach (var item in _roomItemList)
                Destroy(item.gameObject);

            _roomItemList.Clear();


            var roomInfoList = new List<RoomInfo>(_roomInfoCache.Count);
            foreach (var item in _roomInfoCache.Values)
            {
                if (item.IsOpen)
                    roomInfoList.Add(item);
            }

            foreach (var item in _roomInfoCache.Values)
            {
                if (!item.IsOpen)
                    roomInfoList.Add(item);
            }

            foreach (var item in roomInfoList)
            {
                if (!item.IsVisible || item.MaxPlayers == 0)
                    continue;
                var newRoomItem = Instantiate(_roomItemPerfab, _roomListObject.transform);
                newRoomItem.Set(this, item);
                _roomItemList.Add(newRoomItem);
            }
        }
    }
}
