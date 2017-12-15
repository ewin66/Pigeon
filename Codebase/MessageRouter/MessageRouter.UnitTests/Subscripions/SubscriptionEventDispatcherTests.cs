﻿using MessageRouter.Subscriptions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.UnitTests.Subscripions
{
    [TestFixture]
    public class SubscriptionEventDispatcherTests
    {
        public class Topic { }
        public class SubTopic : Topic { }

        private readonly Mock<ISubscriptionHandler<Topic>> mockHandler = new Mock<ISubscriptionHandler<Topic>>();
        private ISubscriptionHandler<Topic> handler;
        

        [SetUp]
        public void Setup()
        {
            handler = mockHandler.Object;
        }


        [TearDown]
        public void Teardown()
        {
            mockHandler.Reset();
        }


        #region Handle
        [Test]
        public void Handle_WithNullMessage_DoesNothing()
        {
            // Arrange
            var dispatcher = SubscriptionEventDispatcher.Create();

            // Act
            TestDelegate handle = () => dispatcher.Handle(null);

            // Assert
            Assert.That(handle, Throws.Nothing);
        }


        [Test]
        public void Handle_WithNoHandlerRegistered_DoesNothing()
        {
            // Arrange
            var dispatcher = SubscriptionEventDispatcher.Create();
            var message = new Topic();

            // Act
            TestDelegate handle = () => dispatcher.Handle(message);

            // Assert
            Assert.That(handle, Throws.Nothing);
        }


        [Test]
        public void Handle_WithHandlerRegistered_CallsHandler()
        {
            // Arrange
            var dispatcher = SubscriptionEventDispatcher.Create().Register(handler);
            var message = new Topic();

            // Act
            dispatcher.Handle(message);

            // Assert
            mockHandler.Verify(m => m.Handle(It.IsIn(message)), Times.Once);
        }


        [Test]
        public void Handle_WithHandlerDelegateRegistered_CallsHandler()
        {
            // Arrange
            var handled = false;
            var dispatcher = SubscriptionEventDispatcher.Create().Register<Topic>(e => { handled = true; });
            var message = new Topic();

            // Act
            dispatcher.Handle(message);

            // Assert
            Assert.That(handled, Is.True);
        }


        [Test]
        public void Handle_WithBaseClassHandlerRegistered_DoesNothing()
        {
            // Arrange
            var dispatcher = SubscriptionEventDispatcher.Create().Register(handler);
            var message = new SubTopic();

            // Act
            TestDelegate handle = () => dispatcher.Handle(message);

            // Assert
            Assert.That(handle, Throws.Nothing);
            mockHandler.Verify(m => m.Handle(It.IsAny<Topic>()), Times.Never);
        }
        #endregion
    }
}
