using System;
using Moq;
using NUnit.Framework;

namespace XProjectNamespaceX.BusinessLogic
{
    /// <summary>
    /// Common base class for test fixtures that use <see cref="AutoMockContainer"/>.
    /// </summary>
    /// <typeparam name="TTarget">The type of the object to be instantiated and tested.</typeparam>
    public class TestWithContainer<TTarget> : TestWithMocks
        where TTarget : class
    {
        private AutoMockContainer _container;

        /// <summary>
        /// The object to be tested.
        /// </summary>
        protected TTarget Target { get; private set; }

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            _container = new AutoMockContainer(MockRepository);
            Register(_container);

            Target = _container.Create<TTarget>();
        }

        /// <summary>
        /// Creates or retrieves a <see cref="Mock"/> for a specific type. Multiple requests for the same type return the same mock instance.
        /// These are the same mocks that are injected into the <see cref="Target"/>.
        /// </summary>
        /// <remarks>All created <see cref="Mock"/>s are automatically <see cref="Mock.Verify()"/>d after the test completes.</remarks>
        protected Mock<T> GetMock<T>()
            where T : class
        {
            return _container.GetMock<T>();
        }

        /// <summary>
        /// Provides an instance of a specific type.
        /// Will usually be a <see cref="Mock"/> as provided by <see cref="GetMock{T}"/> unless a custom instance has been registered in <see cref="Register"/>.
        /// </summary>
        protected T Resolve<T>() where T : class
        {
            return _container.Resolve<T>();
        }

        /// <summary>
        /// Hook that can be used to register objects in the <see cref="AutoMockContainer"/> before the <see cref="Target"/> is constructed.
        /// </summary>
        protected virtual void Register(AutoMockContainer container)
        {
        }

        [TearDown]
        public override void TearDown()
        {
            (Target as IDisposable)?.Dispose();

            base.TearDown();
        }
    }
}
