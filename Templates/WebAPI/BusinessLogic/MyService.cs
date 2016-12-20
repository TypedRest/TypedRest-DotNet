using System.Collections.Generic;
using NLog;
using XProjectNamespaceX.Model;

namespace XProjectNamespaceX.BusinessLogic
{
    public class MyService : IMyService
    {
        private readonly ILogger _logger = LogManager.GetLogger("XProjectNamespaceX");

        private readonly MyServiceConfiguration _configuration;

        public MyService(MyServiceConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<MyEntity> GetAll()
        {
            return new[] {new MyEntity {Id = 1}};
        }

        public MyEntity Get(long id)
        {
            if (id != 1) throw new KeyNotFoundException($"Entity with ID {id} not found.");
            return new MyEntity {Id = 1};
        }

        public void Add(MyEntity entity)
        {
        }

        public void Update(MyEntity entity)
        {
        }

        public void Remove(long id)
        {
        }
    }
}