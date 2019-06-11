﻿using System;

using NetMQ;
using NetMQ.Sockets;

using Pigeon.Addresses;
using Pigeon.NetMQ.Common;
using Pigeon.Packages;

namespace Pigeon.NetMQ.Publishers
{
    /// <summary>
    /// NetMQ imlementation of <see cref="Pigeon.Publishers.IPublisher"/> that wraps a <see cref="PublisherSocket"/>
    /// that connects to remote <see cref="Subscribers.INetMQSubscriber"/>s to publish <see cref="Package"/>s
    /// </summary>
    public sealed class NetMQPublisher : NetMQConnection, INetMQPublisher
    {
        private readonly PublisherSocket socket;


        /// <summary>
        /// Initializes a new instance of <see cref="NetMQPublisher"/>
        /// </summary>
        /// <param name="socket">Inner <see cref="PublisherSocket"/> that sends data to remotes</param>
        /// <param name="messageFactory">Factory for creating <see cref="NetMQMessage"/>s</param>
        public NetMQPublisher(PublisherSocket socket, INetMQMessageFactory messageFactory)
            : base(socket, messageFactory)
        {
            this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
        }


        /// <summary>
        /// Transmits the events to all connected <see cref="ISubscriber"/>s
        /// </summary>
        /// <param name="topicEvent">Topic event to be sent to all remote <see cref="Subscribers.ISubscriber"/>s</param>
        public void Publish(object package)
        {
            socket.SendMultipartMessage(messageFactory.CreateTopicMessage(package));
        }


        /// <summary>
        /// Add <see cref="IAddress"/> to the socket
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> to be added</param>
        public override void SocketAdd(IAddress address)
        {
            socket.Bind(address.ToString());
        }


        /// <summary>
        /// Remote <see cref="IAddress"/> from the socket
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> to be removed</param>
        public override void SocketRemove(IAddress address)
        {
            socket.Unbind(address.ToString());
        }
    }
}
