using System.ComponentModel.DataAnnotations;

namespace TypedRest.Samples.Library.Models
{
    /// <summary>
    /// A software package like an application or a library.
    /// </summary>
    public class Package
    {
        [Key]
        public long Id { get; set; }

        public string Name { get; set; }
    }
}