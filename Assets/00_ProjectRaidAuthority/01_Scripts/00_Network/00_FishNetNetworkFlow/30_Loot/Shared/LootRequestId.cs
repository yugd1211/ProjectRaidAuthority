using System;

namespace ProjectRaidAuthority.Networking
{
    /// <summary>
    /// 서버 권한 Loot 스모크에서 중복 요청을 구분하는 클라이언트 생성 요청 식별자입니다.
    /// </summary>
    [Serializable]
    public readonly struct LootRequestId : IEquatable<LootRequestId>
    {
        /// <summary>비어 있는 요청 식별자입니다.</summary>
        public static readonly LootRequestId Empty = new(string.Empty);

        /// <summary>요청 식별자 문자열을 보관합니다.</summary>
        public string Value { get; }

        /// <summary>요청 식별자를 생성합니다.</summary>
        /// <param name="value">요청마다 달라야 하는 식별자입니다.</param>
        public LootRequestId(string value)
        {
            Value = value;
        }

        /// <summary>요청 식별자가 비어 있는지 반환합니다.</summary>
        public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

        /// <inheritdoc />
        public bool Equals(LootRequestId other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is LootRequestId other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(ToString());

        /// <inheritdoc />
        public override string ToString() => IsEmpty ? "<empty-loot-request>" : Value;
    }
}
