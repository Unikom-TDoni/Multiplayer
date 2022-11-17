using ExitGames.Client.Photon;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Edu.Multiplayer.Setting
{
    public sealed class PropertySettings : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private Slider _slider = default;

        [SerializeField]
        private TMP_InputField _inputField = default;

        [SerializeField]
        private string _propertyKey = default;

        [SerializeField]
        private float _initialValue = 50;

        [SerializeField]
        private float _minValue = 0;

        [SerializeField]
        private float _maxValue = 100;

        [SerializeField]
        private bool _wholeNumber = default;


        private void Start()
        {
            _slider.minValue = _minValue;
            _slider.maxValue = _maxValue;
            _slider.wholeNumbers = _wholeNumber;
            _inputField.contentType = _wholeNumber ? TMP_InputField.ContentType.IntegerNumber : TMP_InputField.ContentType.DecimalNumber;

            if (!PhotonNetwork.IsMasterClient)
            {
                _slider.interactable = default;
                _inputField.interactable = default;
            }

            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(_propertyKey, out var value))
                UpdateSliderInputField((float)value);
            else
                UpdateSliderInputField(_initialValue);

            _slider.onValueChanged.AddListener(value => {
                if (!PhotonNetwork.IsMasterClient)
                    return;

                UpdateSliderInputField(value);
                SetValue(value);
            });

            _inputField.onEndEdit.AddListener(value =>
            {
                if (!PhotonNetwork.IsMasterClient)
                    return;

                if (float.TryParse(value, out var floatValue))
                {
                    floatValue = Mathf.Clamp(floatValue, _slider.minValue, _slider.maxValue);
                    UpdateSliderInputField(floatValue);
                    SetValue(floatValue);
                }
            });
        }

        public void UpdateNetworkValue()
        {
            var property = new Hashtable();
            property.Add(_propertyKey, _slider.value);
            PhotonNetwork.CurrentRoom.SetCustomProperties(property);
        }

        private void SetValue(float value)
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            var property = new Hashtable();
            property.Add(_propertyKey, value);
            PhotonNetwork.CurrentRoom.SetCustomProperties(property);
        }

        public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            if (propertiesThatChanged.TryGetValue(_propertyKey, out var value) && !PhotonNetwork.IsMasterClient)
                UpdateSliderInputField((float)value);
        }

        private void UpdateSliderInputField(float value)
        {
            _slider.value = value;
            if(_wholeNumber)
                _inputField.text = (Mathf.RoundToInt(value).ToString("D"));
            else
                _inputField.text = (value.ToString("F2"));
        }
    }
}
