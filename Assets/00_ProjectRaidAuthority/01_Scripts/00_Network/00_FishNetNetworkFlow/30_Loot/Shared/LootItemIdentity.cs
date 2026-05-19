using System;

namespace ProjectRaidAuthority.Networking
{
    /// <summary>
    /// 서버 권한 Loot 스모크에서 전리품 prefab 인스턴스를 구분하는 값입니다.
    /// </summary>
    [Serializable]
    public readonly struct LootItemIdentity : IEquatable<LootItemIdentity>
    {
        /// <summary>비어 있는 아이템 식별자입니다.</summary>
        public static readonly LootItemIdentity Empty = new(string.Empty);

        /// <summary>식별자 문자열을 보관합니다.</summary>
        public string Value { get; }

        /// <summary>아이템 식별자를 생성합니다.</summary>
        /// <param name="value">서버가 관리할 아이템 인스턴스 식별자입니다.</param>
        public LootItemIdentity(string value)
        {
            Value = value;
        }

        /// <summary>식별자가 비어 있는지 반환합니다.</summary>
        public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

        /// <inheritdoc />
        public bool Equals(LootItemIdentity other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is LootItemIdentity other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(ToString());

        /// <inheritdoc />
        public override string ToString() => IsEmpty ? "<empty-loot-item>" : Value;
    }
}
