using System;
using IdentityModel.Client;

namespace TypedRest.Http
{
    internal class AccessToken
    {
        public string Value { get; }

        private readonly DateTime _expiration;

        public bool IsExpired
            => _expiration >= DateTime.Now;

        public AccessToken(string value, DateTime expiration)
        {
            Value = value;
            _expiration = expiration;
        }
    }
}
