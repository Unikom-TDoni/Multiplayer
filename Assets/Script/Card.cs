using Edu.Multiplayer.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Edu.Multiplayer.Player
{
    public sealed class Card : MonoBehaviour
    {
        public Attack AttackValue = default;

        [SerializeField]
        private CardPlayer _player = default;

        public Vector2 OriginalPos = default;

        private Vector2 _originalScale = default;

        private Color _originalColor = default;

        private bool _isClickable = true;

        private void Start()
        {
            OriginalPos = transform.position;
            _originalScale = transform.localScale;
            _originalColor = GetComponent<Image>().color;
            GetComponent<Button>().onClick.AddListener(() =>
            {
                if (_isClickable)
                {
                    OriginalPos = transform.position;
                    _player.SetChoosenCard(this);
                }
            });
        }

        internal void Reset()
        {
            transform.position = OriginalPos;
            transform.localScale = _originalScale;
            GetComponent<Image>().color = _originalColor;
        }

        public void SetClickable(bool value) =>
            _isClickable = value;
        
    }
}
