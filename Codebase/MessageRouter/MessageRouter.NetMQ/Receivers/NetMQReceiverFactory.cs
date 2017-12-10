﻿using MessageRouter.Receivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;
using MessageRouter.Serialization;
using NetMQ.Sockets;

namespace MessageRouter.NetMQ.Receivers
{
    /// <summary>
    /// Factory for NetMQ <see cref="IReceiver"/>s
    /// </summary>
    public class NetMQReceiverFactory : IReceiverFactory
    {
        private readonly ISerializer<byte[]> serializer = new BinarySerializer();


        /// <summary>
        /// Initializes a new instance of a NetMQReceiverFactory
        /// </summary>
        /// <param name="serializer">A serializer that will convert request and response messages to a binary format for transport along the wire</param>
        public NetMQReceiverFactory(ISerializer<byte[]> serializer)
        {
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }


        /// <summary>
        /// Creates a new instance of a <see cref="IReceiver"/> bound to the supplied <see cref="IAddress"/>
        /// </summary>
        /// <param name="address">Address of local bound endpoint</param>
        /// <returns>Receiver bound to the address</returns>
        public IReceiver Create(IAddress address)
        {
            var socket = new RouterSocket();
            var receiver = new NetMQReceiver(socket, serializer);

            receiver.Add(address);

            return receiver;
        }
    }
}