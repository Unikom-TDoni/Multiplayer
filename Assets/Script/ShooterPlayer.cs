using Photon.Pun;
using TMPro;
using UnityEngine;
using DG.Tweening;

namespace Edu.Multiplayer.Network
{
    public sealed class ShooterPlayer : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private float _speed = default;

        [SerializeField]
        private int _health = default;

        [SerializeField]
        private TMP_Text _txtName = default;

        private SpriteRenderer _spriteRenderer = default;

        private void Awake()
        {
            _txtName.text = $"{photonView.Owner.NickName} ({_health})";
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (!photonView.IsMine)
                return;

            var moveDir = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            );

            transform.Translate(moveDir * Time.deltaTime * _speed);

            if (Input.GetKeyDown(KeyCode.Space))
                photonView.RPC("TakeDamage", RpcTarget.All, 1);
        }

        [PunRPC]
        public void TakeDamage(int ammount)
        {
            _health -= ammount;
            _txtName.text = $"{photonView.Owner.NickName} ({_health})";
            _spriteRenderer.DOColor(Color.red, 0.2f).SetLoops(1, LoopType.Yoyo).From();
        }

    }
}
