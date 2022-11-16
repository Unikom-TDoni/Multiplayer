using UnityEngine;

namespace Edu.Multiplayer.Core.Scene
{
    public sealed class LoginSceneCoordinator : MonoBehaviour
    {
        [SerializeField]
        private LoginSceneLayoutController _uiController = default;

        private void Awake()
        {
            _uiController.OnAwake();
        }
    }
}