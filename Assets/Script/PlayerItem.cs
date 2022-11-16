using TMPro;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

namespace Edu.Multiplayer.Network
{
    public sealed class PlayerItem : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _txtPlayerName = default;

        [SerializeField]
        private Image _image = default;

        [SerializeField]
        private Sprite[] _avatarSprites = default;

        public void Set(Photon.Realtime.Player player)
        {
            if(player.CustomProperties.TryGetValue("AvatarIndex", out var value))
                _image.sprite = _avatarSprites[(int)value];



            _txtPlayerName.text = player.NickName;
            if (player == PhotonNetwork.MasterClient)
                _txtPlayerName.text = $"{player.NickName} (Master)";
        }
    }
}
