using Photon.Pun;
using UnityEngine.UI;
using Edu.Multiplayer.Battle;
using System.Collections.Generic;

namespace Edu.Multiplayer.Player
{
    public class CardNetPlayer : MonoBehaviourPun
    {
        public static List<CardNetPlayer> NetPlayers = new(2);

        private Card[] cards = default;

        private void OnEnable()
        {
            NetPlayers.Add(this);
        }

        public void RemoteClickButton(Attack value)
        {
            if (photonView.IsMine)
                photonView.RPC("RemoteClickButtonRPC", RpcTarget.Others, (int) value);
        }

        [PunRPC]
        private void RemoteClickButtonRPC(int value)
        {
            foreach (var item in cards)
            {
                if(item.AttackValue == (Attack)value)
                {
                    var button = item.GetComponent<Button>();
                    button.onClick.Invoke();
                    break;
                }
            }
        }

        private void OnDisable()
        {
            foreach (var item in cards)
            {
                var btn = item.GetComponent<Button>();
                btn.onClick.RemoveListener(() => RemoteClickButton(item.AttackValue));
            }
            NetPlayers.Remove(this);
        }

        public void Set(CardPlayer player)
        {
            player.TxtName.text = photonView.Owner.NickName;
            cards = player.GetComponentsInChildren<Card>();
            foreach (var item in cards)
            {
                var btn = item.GetComponent<Button>();
                btn.onClick.AddListener(() => RemoteClickButton(item.AttackValue));
                if (!photonView.IsMine)
                    btn.interactable = default;
            }
        }
    }
}
