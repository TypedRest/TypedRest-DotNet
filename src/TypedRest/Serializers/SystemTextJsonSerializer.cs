using System.Text.Json;
using System.Text.Json.Serialization;

namespace TypedRest.Serializers;

/// <summary>
/// Handles serializing/deserializing from/to JSON using <see cref="System.Text.Json"/>.
/// Uses camel-case naming and does not serialize <c>null</c> by default.
/// </summary>
public class SystemTextJsonSerializer : MediaTypeFormatter
{
    public SystemTextJsonSerializer()
    {
        SupportedEncodings.Add(new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true));
        SupportedEncodings.Add(new UnicodeEncoding(bigEndian: false, byteOrderMark: true, throwOnInvalidBytes: true));

        SupportedMediaTypes.Add(new("application/json"));
        SupportedMediaTypes.Add(new("text/json"));
    }

    /// <summary>
    /// Serializer options.
    /// </summary>
    public JsonSerializerOptions Options { get; } = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public override bool CanReadType(Type type) => true;

    public override bool CanWriteType(Type type) => true;

    public override async Task<object?> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        => await JsonSerializer.DeserializeAsync(readStream, type, Options);

    public override async Task<object?> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger, CancellationToken cancellationToken)
        => await JsonSerializer.DeserializeAsync(readStream, type, Options, cancellationToken);

    public override async Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        => await JsonSerializer.SerializeAsync(writeStream, value, Options);

    public override async Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext, CancellationToken cancellationToken)
        => await JsonSerializer.SerializeAsync(writeStream, value, Options, cancellationToken);
}
