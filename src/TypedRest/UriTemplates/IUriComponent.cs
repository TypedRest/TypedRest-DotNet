// Resta.UriTemplates
// Copyright(c) 2013 Pavel Shkarin
// Source: https://github.com/a7b0/uri-templates
// License: The MIT License(MIT)

using System.Collections.Generic;
using System.Text;

namespace TypedRest.UriTemplates
{
    internal interface IUriComponent
    {
        void Resolve(StringBuilder builder, IDictionary<string, object> variables);

        IEnumerable<IUriComponent> ResolveTemplate(IDictionary<string, object> variables);
    }
}
