namespace TypedRest.Serializers;

/// <summary>
/// Handles serializing/deserializing from/to XML using <see cref="XmlSerializer"/>.
/// </summary>
public class XmlSerializer : XmlMediaTypeFormatter
{
    public XmlSerializer()
    {
        UseXmlSerializer = true;
    }
}
