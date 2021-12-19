namespace TypedRest;

public class MockEntity : MockEntityBase
{
    // NOTE: [Key] is inherited
    public override long Id { get; set; }

    public string? Name { get; set; }

    public MockEntity()
    {}

    public MockEntity(long id, string name)
    {
        // ReSharper disable once VirtualMemberCallInConstructor
        Id = id;
        Name = name;
    }

    protected bool Equals(MockEntity other) => Id == other.Id && string.Equals(Name, other.Name);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((MockEntity)obj);
    }

    public override int GetHashCode() => 0;
}