using System;
using UnityEngine;

namespace Minecraft.Scripts.Entities.Input {
    [Serializable]
    public sealed class InputAction {
        [SerializeField]
        private int id;

        [SerializeField]
        private string name;

        public InputAction(int id, string name) {
            this.id = id;
            this.name = name;
        }

        public int ID {
            get {
                return id;
            }
        }

        public string Name {
            get {
                return name;
            }
        }
    }

    public static class InputActions {
        public static readonly InputAction Horizontal = new InputAction(0, "Horizontal");
        public static readonly InputAction Vertical = new InputAction(1, "Vertical");
        public static readonly InputAction LookX = new InputAction(2, "LookX");
        public static readonly InputAction LookY = new InputAction(3, "LookY");
    }
}