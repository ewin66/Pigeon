﻿using MessageRouter.Addresses;
using MessageRouter.NetMQ.Receivers;
using MessageRouter.Receivers;
using MessageRouter.Serialization;
using Moq;
using NetMQ.Sockets;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.NetMQ.UnitTests.Receivers
{
    [TestFixture]
    public class NetMQReceiverTests
    {
        private readonly Mock<ISerializer> mockSerializer = new Mock<ISerializer>();
        private ISerializer serializer;

        private readonly RequestTaskHandler handler = (rec, task) => { };


        [SetUp]
        public void Setup()
        {
            serializer = mockSerializer.Object;
        }


        #region Constructor
        [Test]
        public void NetMQReceiver_WithNullRouterSocket_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate test = () => new NetMQReceiver(null, serializer, handler);

            // Assert
            Assert.That(test, Throws.ArgumentNullException);
        }


        [Test]
        public void NetMQReceiver_WithNullSerializer_ThrowsArgumentNullException()
        {
            // Arrange
            var routerSocket = new RouterSocket();

            // Act
            TestDelegate test = () => new NetMQReceiver(routerSocket, null, handler);

            // Assert
            Assert.That(test, Throws.ArgumentNullException);
        }


        [Test]
        public void NetMQReceiver_WithNullHandler_ThrowsArgumentNullException()
        {
            // Arrange
            var routerSocket = new RouterSocket();

            // Act
            TestDelegate construct = () => new NetMQReceiver(routerSocket, serializer, null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void NetMQReceiver_WithHandler_HandlerPropertyReturnsSameHandler()
        {
            // Arrange
            var routerSocket = new RouterSocket();

            // Act
            var receiver = new NetMQReceiver(routerSocket, serializer, handler);

            // Assert
            Assert.That(receiver.Handler, Is.SameAs(handler));
        }
        #endregion


        #region IsConnected
        [Test]
        public void IsConnected_BeforeBindIsCalled_IsFalse()
        {
            // Arrange
            var routerSocket = new RouterSocket();
            var receiver = new NetMQReceiver(routerSocket, serializer, handler);

            // Act
            var isConnected = receiver.IsConnected;

            // Assert
            Assert.That(isConnected, Is.False);
        }


        [Test]
        public void IsConnected_OnceBindIsCalled_IsTrue()
        {
            // Arrange
            var routerSocket = new RouterSocket();
            var receiver = new NetMQReceiver(routerSocket, serializer, handler);
            receiver.InitializeConnection();

            // Act
            var isConnected = receiver.IsConnected;

            // Assert
            Assert.That(isConnected, Is.True);
        }


        [Test]
        public void IsConnected_OnceUnbindAllIsCalled_IsFalse()
        {
            // Arrange
            var routerSocket = new RouterSocket();
            var receiver = new NetMQReceiver(routerSocket, serializer, handler);
            receiver.InitializeConnection();
            receiver.TerminateConnection();

            // Act
            var isConnected = receiver.IsConnected;

            // Assert
            Assert.That(isConnected, Is.False);
        }
        #endregion


        #region Addresses
        [Test]
        public void Addresses_WhenNoAddressesAdded_IsEmpty()
        {
            // Arrange
            var routerSocket = new RouterSocket();
            var receiver = new NetMQReceiver(routerSocket, serializer, handler);

            // Act
            var any = receiver.Addresses.Any();

            // Assert
            Assert.That(any, Is.False);
        }
        #endregion


        #region AddAddress
        [Test]
        public void AddAddress_WithNullAddress_ThrowsArgumentNullException()
        {
            // Arrange
            var routerSocket = new RouterSocket();
            var receiver = new NetMQReceiver(routerSocket, serializer, handler);

            // Act
            TestDelegate addAddress = () => receiver.AddAddress(null);

            // Assert
            Assert.That(addAddress, Throws.ArgumentNullException);
        }


        [Test]
        public void AddAddress_WithNewAddress_AddsToAddresses()
        {
            // Arrange
            var routerSocket = new RouterSocket();
            var receiver = new NetMQReceiver(routerSocket, serializer, handler);
            var address = TcpAddress.Wildcard(5555);

            // Act
            receiver.AddAddress(address);

            // Assert
            CollectionAssert.Contains(receiver.Addresses, address);
        }


        [Test]
        public void AddAddress_WithAlreadyAddedAddress_DoesNotAddTwice()
        {
            // Arrange
            var routerSocket = new RouterSocket();
            var receiver = new NetMQReceiver(routerSocket, serializer, handler);
            var address = TcpAddress.Wildcard(5555);
            receiver.AddAddress(address);

            // Act
            receiver.AddAddress(address);

            // Assert
            Assert.That(receiver.Addresses, Has.Count.EqualTo(1));
        }
        #endregion


        #region RemoveAddress

        #endregion


        #region RemoveAllAddresses
        [Test]
        public void RemoveAllAddresses_WithAddedAddresses_ClearsAddresses()
        {
            // Arrange
            var routerSocket = new RouterSocket();
            var receiver = new NetMQReceiver(routerSocket, serializer, handler);
            receiver.AddAddress(TcpAddress.Wildcard(5555));

            // Act
            receiver.RemoveAllAddresses();

            // Assert
            CollectionAssert.IsEmpty(receiver.Addresses);
        }
        #endregion


        #region Remove
        [Test]
        public void Remove_WithAddedAddress_RemovesAddress()
        {
            // Arrange
            var routerSocket = new RouterSocket();
            var receiver = new NetMQReceiver(routerSocket, serializer, handler);
            var address = TcpAddress.Wildcard(5555);
            receiver.AddAddress(address);

            // Act
            receiver.RemoveAddress(address);

            // Assert
            CollectionAssert.DoesNotContain(receiver.Addresses, address);
        }


        [Test]
        public void Remove_WithUnaddedAddress_DoesNothing()
        { 
            // Arrange
            var routerSocket = new RouterSocket();
            var receiver = new NetMQReceiver(routerSocket, serializer, handler);

            // Act
            TestDelegate test = () => receiver.RemoveAddress(TcpAddress.Wildcard(5555));

            // Assert
            Assert.That(test, Throws.Nothing);
        }
        #endregion


        #region Bind
        [Test]
        public void Bind_WithNoAddresses_DoesNothing()
        {
            // Arrange
            var routerSocket = new RouterSocket();
            var receiver = new NetMQReceiver(routerSocket, serializer, handler);

            // Act
            TestDelegate test = () => receiver.InitializeConnection();

            // Assert
            Assert.That(test, Throws.Nothing);
        }


        [Test]
        public void Bind_WhenCalled_SetsIsBound()
        {
            // Arrange
            var routerSocket = new RouterSocket();
            var receiver = new NetMQReceiver(routerSocket, serializer, handler);

            // Act
            receiver.InitializeConnection();

            // Assert
            Assert.That(receiver.IsConnected, Is.True);
        }
        #endregion


        #region UnbindAll
        [Test]
        public void UnbindAll_BeforeBindCall_DoesNothing()
        {
            // Arrange
            var routerSocket = new RouterSocket();
            var receiver = new NetMQReceiver(routerSocket, serializer, handler);

            // Act
            TestDelegate test = () => receiver.TerminateConnection();

            // Assert
            Assert.That(test, Throws.Nothing);
        }
        #endregion
    }
}
