using System.ComponentModel.DataAnnotations;

namespace TypedRest
{
    public class MockEntity
    {
        [Key]
        public long Id { get; set; }

        public string Name { get; set; }

        public MockEntity()
        {
        }

        public MockEntity(long id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}