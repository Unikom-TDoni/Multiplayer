using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Edu.Multiplayer.Player;
using ExitGames.Client.Photon;
using Photon.Pun;
using Edu.Multiplayer.Battle;
using Photon.Realtime;

namespace Edu.Multiplayer.Card
{
    public sealed class CardGameManager : MonoBehaviour, IOnEventCallback
    {
        [SerializeField]
        private GameObject gameOverPanel = default;

        [SerializeField]
        private TMP_Text _winnerText = default;

        [SerializeField]
        private CardPlayer _playerOne = default;

        [SerializeField]
        private CardPlayer _playerTwo = default;

        [SerializeField]
        private float _restoreValue = default;

        [SerializeField]
        private float _damageValue = default;

        private CardPlayer _damagedPlayer = default;

        [SerializeField]
        private TMP_Text _txtPing = default;

        [SerializeField]
        private GameObject _netPlayerPerfab = default;

        private readonly HashSet<int> _syncReadyPlayers = new(2);

        private CardGameState _cardGameState, _nextState = CardGameState.NetPlayerInit;

        private bool _online = true;

        public enum CardGameState
        {
            Sync,
            NetPlayerInit,
            ChooseAttack,
            Attack,
            Damage,
            Draw,
            GameOver
        }

        private void Start()
        {
            if (_online)
            {
                PhotonNetwork.Instantiate(_netPlayerPerfab.name, default, default);
                _nextState = CardGameState.NetPlayerInit;
                _cardGameState = CardGameState.NetPlayerInit;
                if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("Restore", out var restore))
                    _restoreValue = (float)restore;
                if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("Damage", out var damage))
                    _damageValue = (float)damage;
            }
            else
                _cardGameState = CardGameState.ChooseAttack;

            StartCoroutine(PingCoroutine());
        }

        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        private void Update()
        {
            switch (_cardGameState)
            {
                case CardGameState.Sync:
                    if (_syncReadyPlayers.Count == 2)
                    {
                        _syncReadyPlayers.Clear();
                        _cardGameState = _nextState;
                    }
                    break;
                case CardGameState.NetPlayerInit:
                    if (CardNetPlayer.NetPlayers.Count == 2)
                    {
                        foreach (var item in CardNetPlayer.NetPlayers)
                        {
                            if (item.photonView.IsMine)
                                item.Set(_playerOne);
                            else
                                item.Set(_playerTwo);
                        }
                        ChangeState(CardGameState.ChooseAttack);
                    }
                    break;
                case CardGameState.ChooseAttack:
                    if (_playerOne.AttackValue != null && _playerTwo.AttackValue != null)
                    {
                        _playerOne.AnimateAttack();
                        _playerTwo.AnimateAttack();
                        _playerOne.IsClickable(false);
                        _playerTwo.IsClickable(false);
                        ChangeState(CardGameState.Attack);
                    }
                    break;
                case CardGameState.Attack:
                    if (!_playerOne.IsAnimating() && !_playerTwo.IsAnimating())
                    {
                        _damagedPlayer = GetDamagePlayer();
                        if (_damagedPlayer != null)
                        {
                            _damagedPlayer.AnimateDamage();
                            ChangeState(CardGameState.Damage);
                        }
                        else
                        {
                            _playerOne.AnimateDraw();
                            _playerTwo.AnimateDraw();
                            ChangeState(CardGameState.Draw);
                        }
                    }
                    break;
                case CardGameState.Damage:
                    if (!_playerOne.IsAnimating() && !_playerTwo.IsAnimating())
                    {
                        if (_damagedPlayer == _playerOne)
                        {
                            _playerOne.ChangeHealth(_damageValue);
                            _playerTwo.ChangeHealth(-_restoreValue);
                        }
                        else
                        {
                            _playerOne.ChangeHealth(-_restoreValue);
                            _playerTwo.ChangeHealth(_damageValue);
                        }

                        var winner = GetWinner();
                        if (winner == null)
                        {
                            ResetPlayers();
                            _playerOne.IsClickable(true);
                            _playerTwo.IsClickable(true);
                            ChangeState(CardGameState.ChooseAttack);
                        }
                        else
                        {
                            gameOverPanel.SetActive(true);
                            _winnerText.text = winner == _playerOne ? $"{_playerOne.TxtName.text} wins" : $"{_playerTwo.TxtName.text} wins";
                            ResetPlayers();
                            ChangeState(CardGameState.GameOver);
                        }
                    }
                    break;
                case CardGameState.Draw:
                    if (!_playerOne.IsAnimating() && !_playerTwo.IsAnimating())
                    {
                        ResetPlayers();
                        _playerOne.IsClickable(true);
                        _playerTwo.IsClickable(true);
                        ChangeState(CardGameState.ChooseAttack);
                    }
                    break;
            }
        }

        private void ChangeState(CardGameState nextState)
        {
            if (!_online)
            {
                _cardGameState = nextState;
                return;
            }

            if (_nextState == nextState)
                return;

            var eventRaise = new RaiseEventOptions();
            eventRaise.Receivers = ReceiverGroup.All;
            var id = PhotonNetwork.LocalPlayer.ActorNumber;
            PhotonNetwork.RaiseEvent(1, id, eventRaise, SendOptions.SendReliable);
            _cardGameState = CardGameState.Sync;
            _nextState = nextState; 
        }

        public void OnEvent(EventData photonEvent)
        {
            if(photonEvent.Code == 1)
            {
                var id = (int)photonEvent.CustomData;
                _syncReadyPlayers.Add(id);
            }
        }

        private IEnumerator PingCoroutine()
        {
            var wait = new WaitForSeconds(1);
            while (true)
            {
                _txtPing.text = $"Ping : {PhotonNetwork.GetPing()} ms";
                yield return wait;
            }
        }

        private void ResetPlayers()
        {
            _damagedPlayer = null;
            _playerOne.Reset();
            _playerTwo.Reset();
        }

        public CardPlayer GetDamagePlayer()
        {
            var _playerAttack1 = _playerOne.AttackValue;
            var _playerAttack2 = _playerTwo.AttackValue;

            if (_playerAttack1 == Attack.Rock && _playerAttack2 == Attack.Paper)
            {
                return _playerOne;
            }
            else if (_playerAttack1 == Attack.Rock && _playerAttack2 == Attack.Scissor)
            {
                return _playerTwo;
            }
            else if (_playerAttack1 == Attack.Paper && _playerAttack2 == Attack.Rock)
            {
                return _playerTwo;
            }
            else if (_playerAttack1 == Attack.Paper && _playerAttack2 == Attack.Scissor)
            {
                return _playerOne;
            }
            else if (_playerAttack1 == Attack.Scissor && _playerAttack2 == Attack.Rock)
            {
                return _playerOne;
            }
            else if(_playerAttack1 == Attack.Scissor && _playerAttack2 == Attack.Paper)
            {
                return _playerTwo;
            }

            return null;
        }

        private CardPlayer GetWinner()
        {
            if (_playerOne.Health == 0)
                return _playerTwo;
            else if (_playerTwo.Health == 0)
                return _playerOne;
            else return null;
        }
    } 
}
