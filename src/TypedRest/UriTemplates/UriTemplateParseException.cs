// Resta.UriTemplates
// Copyright(c) 2013 Pavel Shkarin
// Source: https://github.com/a7b0/uri-templates
// License: The MIT License(MIT)

using System;

namespace TypedRest.UriTemplates
{
    public sealed class UriTemplateParseException : UriTemplateException
    {
        private readonly int position;
        private readonly string template;

        public UriTemplateParseException(string message, string template, int position)
            : base(message)
        {
            this.position = position;
            this.template = template;
        }

        public UriTemplateParseException(string message, string template, int position, Exception innerException)
            : base(message, innerException)
        {
            this.position = position;
            this.template = template;
        }

        public int Position
        {
            get { return position; }
        }

        public string Template
        {
            get { return template; }
        }
    }
}
