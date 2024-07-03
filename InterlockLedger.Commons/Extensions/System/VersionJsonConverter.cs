// ******************************************************************************************************************************
// ****
// ****      Copyright (c) 2017-2024 InterlockLedger Network
// ****
// ******************************************************************************************************************************

using System.Text.Json;

namespace System;

public class VersionJsonConverter : JsonConverter<Version>
{
#pragma warning disable IDE0072 // Add missing cases
    public override Version? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.TokenType switch {
            JsonTokenType.String => Version.Parse(reader.GetString()!),
            JsonTokenType.Null => null,
            JsonTokenType.None => null,
            _ => throw new NotSupportedException(),
        };
#pragma warning restore IDE0072 // Add missing cases

    public override void Write(Utf8JsonWriter writer, Version value, JsonSerializerOptions options) =>
        WriteVersion(writer.Required(), value);

    private static void WriteVersion(Utf8JsonWriter writer, Version value) {
        if (value is null)
            writer.WriteNullValue();
        else
            writer.WriteStringValue(value.ToString());
    }
}
