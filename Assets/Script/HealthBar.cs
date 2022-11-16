using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Edu.Multiplayer.Layout
{
    public sealed class HealthBar : MonoBehaviour
    {
        [SerializeField]
        private Image _image = default;

        private Slider _slider = default;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
        }

        public void UpdateBar(float value)
        {
            DOTween.To(() => _slider.value, x => _slider.value = x, value, 0.1f);
            if (value > 0.6f)
                _image.DOColor(Color.green, 0.5f);
            else if (value > 0.4f)
                _image.DOColor(Color.yellow, 0.5f);
            else
                _image.DOColor(Color.red, 0.5f);
        }
    }
}
