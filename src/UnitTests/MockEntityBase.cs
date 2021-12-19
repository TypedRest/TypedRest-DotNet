using System.ComponentModel.DataAnnotations;

namespace TypedRest;

public class MockEntityBase
{
    [Key]
    public virtual long Id { get; set; }
}