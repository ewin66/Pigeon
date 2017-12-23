﻿using Pigeon.Addresses;
using Pigeon.NetMQ.Common;
using Pigeon.Receivers;
using Pigeon.Senders;

namespace Pigeon.NetMQ.Receivers
{
    /// <summary>
    /// Interface that encapsulates a NetMQ <see cref="IReceiver"/> that is able to bind to <see cref="IAddress"/>es 
    /// to receive and reply to incoming messages from remote <see cref="ISender"/>
    /// </summary>
    public interface INetMQReceiver : IReceiver, INetMQConnection
    {
        /// <summary>
        /// Gets the <see cref="RequestTaskHandler"/> delegate the <see cref="INetMQReceiver"/> calls when
        /// an incoming request message is received
        /// </summary>
        RequestTaskHandler Handler { get; }
    }
}