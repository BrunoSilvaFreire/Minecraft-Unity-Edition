using System;

namespace Minecraft.Scripts.Utility {
    public class AbstractRange<T> {
        public T Start, End;
    }

    [Serializable]
    public sealed class IntRange : AbstractRange<int> { }

    [Serializable]
    public sealed class UInt8Range : AbstractRange<byte> {
        public byte Evaluate() {
            return (byte) UnityEngine.Random.Range(Start, End);
        }
    }

    [Serializable]
    public sealed class UInt16Range : AbstractRange<ushort> { }

    [Serializable]
    public sealed class UInt32Range : AbstractRange<uint> { }

    [Serializable]
    public sealed class UInt64Range : AbstractRange<ulong> { }
}