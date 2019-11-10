#nullable disable
// Resta.UriTemplates
// Copyright(c) 2013 Pavel Shkarin
// Source: https://github.com/a7b0/uri-templates
// License: The MIT License(MIT)

using System;
using System.Collections.Generic;
using System.Text;

namespace TypedRest.UriTemplates
{
    internal class Literal : IUriComponent
    {
        private readonly string value;

        public Literal(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            this.value = value;
        }

        public string Value
        {
            get { return value; }
        }

        public void Resolve(StringBuilder builder, IDictionary<string, object> variables)
        {
            builder.Append(value);
        }

        public IEnumerable<IUriComponent> ResolveTemplate(IDictionary<string, object> variables)
        {
            return new IUriComponent[] {this};
        }

        public override string ToString()
        {
            return value;
        }
    }
}
