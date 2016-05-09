namespace TypedRestSample.Model
{
    /// <summary>
    /// A resource that can be deployed to a specific target.
    /// </summary>
    public class Resource : NamedEntity
    {
        /// <summary>
        /// A target to deploy the resource to.
        /// </summary>
        public virtual Target Target { get; set; }
    }
}