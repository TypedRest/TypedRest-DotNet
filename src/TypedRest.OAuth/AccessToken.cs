namespace TypedRest.OAuth;

internal class AccessToken
{
    public string Value { get; }

    private readonly DateTime _expiration;

    public bool IsExpired
        => DateTime.Now >= _expiration;

    public AccessToken(string value, DateTime expiration)
    {
        Value = value;
        _expiration = expiration;
    }
}