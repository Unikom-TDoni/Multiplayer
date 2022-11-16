using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Edu.Multiplayer.Network
{
    public sealed class RoomItem : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _txtRoomName = default;

        [SerializeField]
        private Button _btnNavigation = default;

        private LobbyManager _manager = default;

        private RoomInfo _roomInfo = default;

        private void Awake()
        {
            _btnNavigation.onClick.AddListener(() =>
                _manager.JoinRoom(_roomInfo.Name)
            );
        }

        public void Set(LobbyManager manager, RoomInfo roomInfo)
        {
            _manager = manager;
            _roomInfo = roomInfo;
            _txtRoomName.text = $"{roomInfo.Name} ({roomInfo.PlayerCount}/{roomInfo.MaxPlayers})";
            if (!roomInfo.IsOpen)
                _btnNavigation.interactable = default;
        }
    }
}
