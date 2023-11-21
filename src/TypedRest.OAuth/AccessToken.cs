namespace TypedRest.OAuth;

internal class AccessToken(string value, DateTime expiration)
{
    public string Value { get; } = value;

    public bool IsExpired
        => DateTime.Now >= expiration;
}
