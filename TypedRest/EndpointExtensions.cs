using System;
using System.Collections.Generic;

namespace TypedRest
{
    /// <summary>
    /// Provides extension methods for <seealso cref="IEndpoint"/>.
    /// </summary>
    public static class EndpointExtensions
    {
        /// <summary>
        /// Helper method that retrieves a link template with a specific relation type and expands it using a single variable.
        /// </summary>
        /// <param name="endpoint">The endpoint to operate on.</param>
        /// <param name="rel">The relation type of the template to look for. "-template" is appended implicitly for HTTP Link Headers.</param>
        /// <param name="variableName">The name of the variable to insert.</param>
        /// <param name="value">The value to insert for the variable.</param>
        /// <returns>The href of the resolved template.</returns>
        /// <exception cref="KeyNotFoundException">No link template with the specified <paramref name="rel"/> could be found.</exception>
        /// <seealso cref="IEndpoint.LinkTemplate"/>
        public static Uri LinkTemplateExpanded(this IEndpoint endpoint, string rel, string variableName, string value)
        {
            var template = endpoint.LinkTemplate(rel);
            return new Uri(endpoint.Uri, template.Resolve(new Dictionary<string, object> {{variableName, value}}));
        }
    }
}