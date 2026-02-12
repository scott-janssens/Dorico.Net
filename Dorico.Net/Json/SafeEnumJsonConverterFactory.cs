using System.Text.Json;
using System.Text.Json.Serialization;

namespace DoricoNet.Json;

/// <summary>
/// A <see cref="JsonConverterFactory"/> that provides resilient enum deserialization for <see cref="System.Text.Json"/>.
/// </summary>
/// <remarks>
/// <para>
/// This factory produces converters for any enum type (including nullable enums). During deserialization it:
/// </para>
/// <list type="bullet">
/// <item><description>Accepts string tokens and parses them case-insensitively.</description></item>
/// <item><description>Accepts numeric tokens only when the numeric value maps to a defined enum member.</description></item>
/// <item><description>Returns <c>default</c> for unknown/invalid values instead of throwing.</description></item>
/// <item><description>For nullable enums, JSON <c>null</c> is preserved as <c>null</c>.</description></item>
/// </list>
/// <para>
/// Optional callbacks can be assigned via <see cref="UnknownEnumStringToken"/> and <see cref="UnknownEnumNumberToken"/>
/// to observe unknown values without failing deserialization.
/// </para>
/// </remarks>
public sealed class SafeEnumJsonConverterFactory : JsonConverterFactory
{
	/// <summary>
	/// Optional callback invoked when an enum value is provided as a string token but cannot be parsed.
	/// </summary>
	/// <remarks>
	/// The first argument is the enum <see cref="Type"/>; the second is the raw string token value (may be <c>null</c>).
	/// </remarks>
	public static Action<Type, string?>? UnknownEnumStringToken { get; set; }

	/// <summary>
	/// Optional callback invoked when an enum value is provided as a number token but is not a defined enum value.
	/// </summary>
	/// <remarks>
	/// The first argument is the enum <see cref="Type"/>; the second is the raw numeric value if it fits in an <see cref="int"/>,
	/// otherwise <c>null</c>.
	/// </remarks>
	public static Action<Type, int?>? UnknownEnumNumberToken { get; set; }

    /// <summary>
    /// Determines whether this factory can create a converter for <paramref name="typeToConvert"/>.
    /// </summary>
    /// <param name="typeToConvert">The type to be converted.</param>
    /// <returns><c>true</c> if the type is an enum or a nullable enum; otherwise <c>false</c>.</returns>
    public override bool CanConvert(Type typeToConvert)
	{
		ArgumentNullException.ThrowIfNull(typeToConvert);
		return (Nullable.GetUnderlyingType(typeToConvert) ?? typeToConvert).IsEnum;
	}

	/// <summary>
	/// Creates a converter for the specified enum (or nullable enum) type.
	/// </summary>
	/// <param name="typeToConvert">The enum type to create a converter for.</param>
	/// <param name="options">Serializer options (not used by this implementation).</param>
	/// <returns>A converter instance for the requested type.</returns>
	public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
	{
		var enumType = Nullable.GetUnderlyingType(typeToConvert) ?? typeToConvert;

		var converterType = typeof(SafeEnumJsonConverter<>).MakeGenericType(enumType);
		var converter = (JsonConverter)Activator.CreateInstance(converterType)!;

		if (Nullable.GetUnderlyingType(typeToConvert) is not null)
		{
			var wrapperType = typeof(NullableSafeEnumJsonConverter<>).MakeGenericType(enumType);
			return (JsonConverter)Activator.CreateInstance(wrapperType, converter)!;
		}

		return converter;
	}

	/// <summary>
	/// A robust enum converter that tolerates unexpected JSON values.
	/// </summary>
	/// <typeparam name="TEnum">The enum type.</typeparam>
	/// <remarks>
	/// <para>
	/// Deserialization behavior:
	/// </para>
	/// <list type="bullet">
	/// <item><description>String tokens are parsed using <see cref="Enum.TryParse{TEnum}(string?, bool, out TEnum)"/> (case-insensitive).</description></item>
	/// <item><description>Number tokens are accepted only if they fit in an <see cref="int"/> and are defined by the enum.</description></item>
	/// <item><description>Unknown/invalid values return <c>default</c> and invoke the appropriate logging hook.</description></item>
	/// <item><description>Unsupported token kinds are skipped best-effort and return <c>default</c>.</description></item>
	/// </list>
	/// <para>
	/// Serialization writes enums as string values via <see cref="Enum.ToString()"/>.
	/// </para>
	/// </remarks>
	private sealed class SafeEnumJsonConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
	{
		/// <summary>
		/// Reads and converts the JSON to <typeparamref name="TEnum"/>.
		/// </summary>
		/// <param name="reader">The reader.</param>
		/// <param name="typeToConvert">The type to convert.</param>
		/// <param name="options">Serializer options.</param>
		/// <returns>
		/// The parsed enum value; or <c>default</c> when the JSON token is unknown/invalid for the enum.
		/// </returns>
		public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType == JsonTokenType.String)
			{
				var s = reader.GetString();
				if (string.IsNullOrWhiteSpace(s)) return default;

				if (Enum.TryParse<TEnum>(s, ignoreCase: true, out var parsed))
					return parsed;

				UnknownEnumStringToken?.Invoke(typeof(TEnum), s);
				return default;
			}

			if (reader.TokenType == JsonTokenType.Number)
			{
				if (reader.TryGetInt32(out var i) && Enum.IsDefined(typeof(TEnum), i))
					return (TEnum)Enum.ToObject(typeof(TEnum), i);

				int? n = reader.TryGetInt32(out var v) ? v : null;
				UnknownEnumNumberToken?.Invoke(typeof(TEnum), n);
				return default;
			}

			if (reader.TokenType == JsonTokenType.Null)
				return default;

			try { reader.Skip(); } catch { }
			return default;
		}

		/// <summary>
		/// Writes the enum value as a JSON string.
		/// </summary>
		/// <param name="writer">The writer.</param>
		/// <param name="value">The enum value.</param>
		/// <param name="options">Serializer options.</param>
		public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
			=> writer.WriteStringValue(value.ToString());
	}

	/// <summary>
	/// Wraps a non-nullable <see cref="SafeEnumJsonConverter{TEnum}"/> to support nullable enums (<typeparamref name="TEnum"/>?).
	/// </summary>
	/// <typeparam name="TEnum">The underlying enum type.</typeparam>
	/// <remarks>
	/// This converter preserves JSON <c>null</c> as <c>null</c>. For non-null tokens, it delegates to the inner
	/// <see cref="JsonConverter{T}"/> for <typeparamref name="TEnum"/>.
	/// </remarks>
	private sealed class NullableSafeEnumJsonConverter<TEnum> : JsonConverter<TEnum?> where TEnum : struct, Enum
	{
		private readonly JsonConverter inner;

		/// <summary>
		/// Initializes a new instance of the <see cref="NullableSafeEnumJsonConverter{TEnum}"/> class.
		/// </summary>
		/// <param name="inner">The inner converter for the non-nullable enum type.</param>
		public NullableSafeEnumJsonConverter(JsonConverter inner) => this.inner = inner;

		/// <summary>
		/// Reads and converts the JSON to a nullable enum.
		/// </summary>
		/// <param name="reader">The reader.</param>
		/// <param name="typeToConvert">The target type.</param>
		/// <param name="options">Serializer options.</param>
		/// <returns>
		/// <c>null</c> when the JSON token is <c>null</c>; otherwise the value produced by the inner enum converter
		/// (which may be <c>default</c> for unknown/invalid values).
		/// </returns>
		public override TEnum? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType == JsonTokenType.Null) return null;
			return ((JsonConverter<TEnum>)inner).Read(ref reader, typeof(TEnum), options);
		}

		/// <summary>
		/// Writes a nullable enum as a JSON string, or <c>null</c> when the value is <c>null</c>.
		/// </summary>
		/// <param name="writer">The writer.</param>
		/// <param name="value">The enum value.</param>
		/// <param name="options">Serializer options.</param>
		public override void Write(Utf8JsonWriter writer, TEnum? value, JsonSerializerOptions options)
		{
			if (value is null) { writer.WriteNullValue(); return; }
			((JsonConverter<TEnum>)inner).Write(writer, value.Value, options);
		}
	}
}