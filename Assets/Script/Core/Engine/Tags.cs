using System;
using UnityEngine;
using Lncodes.Module.Unity.Editor;

namespace Edu.CrossyBox.Core
{
    [Serializable]
    public struct Tags
    {
        [TagSelector]
        [SerializeField]
        private string _car;

        public string Car { get => _car; }
    }
}
