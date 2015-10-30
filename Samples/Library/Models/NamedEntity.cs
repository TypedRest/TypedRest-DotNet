using System.ComponentModel.DataAnnotations;

namespace TypedRest.Samples.Library.Models
{
    /// <summary>
    /// An entity with a unique numeric identifier and a human-readable name.
    /// </summary>
    public abstract class NamedEntity
    {
        [Key]
        public long Id { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
