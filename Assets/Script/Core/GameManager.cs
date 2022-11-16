using UnityEngine;
using Edu.CrossyBox.Core;
using Lncodes.Module.Unity.Template;
using UnityEngine.SceneManagement;

namespace Edu.Multiplayer.Core
{
    public sealed class GameManager : SingletonMonoBehavior<GameManager>
    {
        public Tags Tags = default;

        public SceneObjects SceneObjects = default;

        protected override void Awake()
        {
            base.Awake();
            SceneManager.LoadScene(SceneObjects.Login);
        }

    }
}
