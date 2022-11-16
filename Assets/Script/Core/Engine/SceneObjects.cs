using System;
using UnityEngine;
using Lncodes.Module.Unity.Editor;

namespace Edu.CrossyBox.Core
{
    [Serializable]
    public struct SceneObjects
    {
        [SerializeField]
        private SceneObject _login;

        [SerializeField]
        private SceneObject _lobby;

        [SerializeField]
        private SceneObject _gameplay;

        public SceneObject Login { get => _login; }

        public SceneObject Lobby { get => _lobby; }

        public SceneObject Gameplay { get => _gameplay; }

    }
}
