// Resta.UriTemplates
// Copyright(c) 2013 Pavel Shkarin
// Source: https://github.com/a7b0/uri-templates
// License: The MIT License(MIT)

using System;

namespace TypedRest.UriTemplates
{
    public class UriTemplateException : Exception
    {
        public UriTemplateException(string message)
            : base(message)
        {
        }

        public UriTemplateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
