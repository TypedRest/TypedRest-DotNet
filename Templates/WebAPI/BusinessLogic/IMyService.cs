using System.Collections.Generic;
using XProjectNamespaceX.Model;

namespace XProjectNamespaceX.BusinessLogic
{
    public interface IMyService
    {
        IEnumerable<MyEntity> GetAll();
        MyEntity Get(long id);
        void Add(MyEntity entity);
        void Update(MyEntity entity);
        void Remove(long id);
    }
}