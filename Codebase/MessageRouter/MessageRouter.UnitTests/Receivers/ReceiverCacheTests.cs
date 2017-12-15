﻿using System.Collections.Generic;

using MessageRouter.Addresses;
using MessageRouter.Messages;
using MessageRouter.Monitors;
using MessageRouter.Receivers;
using MessageRouter.Requests;

using Moq;

using NUnit.Framework;

namespace MessageRouter.UnitTests.Receivers
{
    [TestFixture]
    public class ReceiverCacheTests
    {
        private readonly Mock<IMonitorCache> mockMonitorCache = new Mock<IMonitorCache>();
        private IMonitorCache monitorCache;

        private readonly Mock<IMessageFactory> mockMessageFactory = new Mock<IMessageFactory>();
        private IMessageFactory messageFactory;

        private readonly Mock<IRequestDispatcher> mockDispatcher = new Mock<IRequestDispatcher>();
        private IRequestDispatcher dispatcher;

        private readonly Mock<IReceiverFactory<IReceiver>> mockReceiverFactory = new Mock<IReceiverFactory<IReceiver>>();
        private IReceiverFactory<IReceiver> receiverFactory;

        private readonly Mock<IReceiver> mockReceiver = new Mock<IReceiver>();
        private IReceiver receiver;

        private readonly Mock<IAddress> mockAddress = new Mock<IAddress>();
        private IAddress address;

        private readonly Mock<IReceiverMonitor<IReceiver>> mockReceiverMonitor = new Mock<IReceiverMonitor<IReceiver>>();
        private IReceiverMonitor<IReceiver> receiverMonitor;
        
        
        [SetUp]
        public void Setup()
        {
            monitorCache = mockMonitorCache.Object;
            messageFactory = mockMessageFactory.Object;
            dispatcher = mockDispatcher.Object;
            receiverFactory = mockReceiverFactory.Object;
            receiver = mockReceiver.Object;
            address = mockAddress.Object;
            receiverMonitor = mockReceiverMonitor.Object;

            mockReceiverFactory
                .Setup(m => m.CreateReceiver(It.IsAny<IAddress>()))
                .Returns(receiver);

            mockReceiverFactory
                .SetupGet(m => m.ReceiverType)
                .Returns(typeof(IReceiver));

            mockReceiverFactory
                .SetupGet(m => m.ReceiverMonitor)
                .Returns(receiverMonitor);
        }


        [TearDown]
        public void TearDown()
        {
            mockMonitorCache.Reset();
            mockMessageFactory.Reset();
            mockDispatcher.Reset();
            mockReceiverFactory.Reset();
            mockReceiver.Reset();
            mockAddress.Reset();
            mockReceiverMonitor.Reset();
        }


        #region Constructor
        [Test]
        public void ReceiverCache_WithNullMonitorCache_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new ReceiverCache(null, messageFactory, dispatcher);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void ReceiverCache_WithNullMessageFactory_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new ReceiverCache(monitorCache, null, dispatcher);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void ReceiverCache_WithNullRequestDispatcher_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new ReceiverCache(monitorCache, messageFactory, null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }
        #endregion


        #region AddFactory
        [Test]
        public void AddFactory_WithNullFactory_ThrowsArgumentNullExecption()
        {
            // Arrange
            var cache = new ReceiverCache(monitorCache, messageFactory, dispatcher);

            // Act
            TestDelegate addFactory = () => cache.AddFactory(null);

            // Assert
            Assert.That(addFactory, Throws.ArgumentNullException);
        }


        [Test]
        public void AddFactory_WithFactory_AddsToFactoryCollection()
        {
            // Arrange
            var cache = new ReceiverCache(monitorCache, messageFactory, dispatcher);

            // Act
            cache.AddFactory(receiverFactory);

            // Assert
            CollectionAssert.Contains(cache.ReceiverFactories, receiverFactory);
            Assert.That(cache.ReceiverFactories.Count, Is.EqualTo(1));
        }


        [Test]
        public void AddFactory_WithFactory_AddsMonitorToMonitorCache()
        {
            // Arrange
            var cache = new ReceiverCache(monitorCache, messageFactory, dispatcher);

            // Act
            cache.AddFactory(receiverFactory);

            // Assert
            mockMonitorCache.Verify(m => m.AddMonitor(It.IsIn(receiverMonitor)), Times.Once);
        }


        [Test]
        public void AddFactory_WithFactoryAlreadyAdded_DoesNothing()
        {
            // Arrange
            var cache = new ReceiverCache(monitorCache, messageFactory, dispatcher);
            cache.AddFactory(receiverFactory);

            // Act
            cache.AddFactory(receiverFactory);

            // Assert
            CollectionAssert.Contains(cache.ReceiverFactories, receiverFactory);
            Assert.That(cache.ReceiverFactories.Count, Is.EqualTo(1));
            mockMonitorCache.Verify(m => m.AddMonitor(It.IsIn(receiverMonitor)), Times.Once);
        }
        #endregion


        #region AddReceiver
        [Test]
        public void AddReceiver_WithNullAddress_ThrowsArgumentNullException()
        {
            // Arrange
            var cache = new ReceiverCache(monitorCache, messageFactory, dispatcher);

            // Act
            TestDelegate addReceiver = () => cache.AddReceiver<IReceiver>(null);

            // Assert
            Assert.That(addReceiver, Throws.ArgumentNullException);
        }


        [Test]
        public void AddReceiver_WithNoMatchingFactory_ThrowsKeyNotFoundException()
        {
            // Arrange
            var cache = new ReceiverCache(monitorCache, messageFactory, dispatcher);

            // Act
            TestDelegate addReceiver = () => cache.AddReceiver<IReceiver>(address);

            // Assert
            Assert.That(addReceiver, Throws.TypeOf<KeyNotFoundException>());
        }


        [Test]
        public void AddReceiver_WithFactory_CallsFactoryCreateReceiver()
        {
            // Arrange
            var cache = new ReceiverCache(monitorCache, messageFactory, dispatcher);
            cache.AddFactory(receiverFactory);

            // Act
            cache.AddReceiver<IReceiver>(address);

            // Assert
            mockReceiverFactory.Verify(m => m.CreateReceiver(It.IsIn(address)), Times.Once);
        }


        [Test]
        public void AddReceiver_WithExistingReceiverForAddress_ThrowsInvalidOperationException()
        {
            // Arrange
            var address = TcpAddress.Wildcard(5555);
            var cache = new ReceiverCache(monitorCache, messageFactory, dispatcher);
            cache.AddFactory(receiverFactory);
            cache.AddReceiver<IReceiver>(address);

            // Act
            TestDelegate addReceiver = () => cache.AddReceiver<IReceiver>(address);

            // Assert
            Assert.That(addReceiver, Throws.InvalidOperationException);
        }
        #endregion
    }
}
