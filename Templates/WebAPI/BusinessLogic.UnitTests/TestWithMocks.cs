using System;
using Moq;
using NUnit.Framework;

namespace XProjectNamespaceX.BusinessLogic
{
    /// <summary>
    /// Common base class for test fixtures that use a <see cref="Moq.MockRepository"/>.
    /// </summary>
    public abstract class TestWithMocks
    {
        protected MockRepository MockRepository;

        /// <summary>
        /// Creates a new <see cref="Mock"/> for a specific type. Multiple requests for the same type return new mock instances each time.
        /// </summary>
        /// <remarks>All created <see cref="Mock"/>s are automatically <see cref="Mock.Verify"/>d after the test completes.</remarks>
        protected Mock<T> CreateMock<T>()
            where T : class
        {
            return MockRepository.Create<T>();
        }

        [SetUp]
        public virtual void SetUp()
        {
            MockRepository = new MockRepository(MockBehavior.Strict);
        }

        [TearDown]
        public virtual void TearDown()
        {
            // Prevent Mock verify failures from hiding underlying test failures
            try
            {
                if (TestContext.CurrentContext.Result.State != TestState.Success)
                    return;
            }
            catch (NullReferenceException)
            {
                // Bug in NUnit prevents some test contexts from being detected
            }

            MockRepository.VerifyAll();
        }
    }
}
