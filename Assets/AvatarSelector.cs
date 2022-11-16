using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Edu.Multiplayer.Player
{
    public sealed class AvatarSelector : MonoBehaviour
    {
        [SerializeField]
        private Image _image = default;

        [SerializeField]
        private Sprite[] _avatarSprites = default;

        [SerializeField]
        private Button _btnNext = default;

        [SerializeField]
        private Button _btnPrevious = default;

        private int _selectedIndex = default;

        private void Awake()
        {
            _btnNext.onClick.AddListener(() => ShiftSelectedIndex(1));
            _btnPrevious.onClick.AddListener(() => ShiftSelectedIndex(-1));
        }

        private void Start()
        {
            _selectedIndex = PlayerPrefs.GetInt("AvatarIndex", 0);
            _image.sprite = _avatarSprites[_selectedIndex];
            Save();
        }

        private void ShiftSelectedIndex(int shift)
        {
            _selectedIndex += shift;

            while (_selectedIndex >= _avatarSprites.Length)
                _selectedIndex -= _avatarSprites.Length;

            while (_selectedIndex < 0)
                _selectedIndex += _avatarSprites.Length;

            _image.sprite = _avatarSprites[_selectedIndex];
            Save();
        }

        private void Save()
        {
            PlayerPrefs.SetInt("AvatarIndex", _selectedIndex);
            var property = new Hashtable
            {
                { "AvatarIndex", _selectedIndex }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(property);
        }
    }
}
