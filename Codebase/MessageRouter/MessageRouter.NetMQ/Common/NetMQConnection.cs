﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;
using MessageRouter.Serialization;
using NetMQ;

namespace MessageRouter.NetMQ.Common
{
    /// <summary>
    /// Common implementation for all NetMQ <see cref="INetMQConnection"/>s
    /// </summary>
    public abstract class NetMQConnection : INetMQConnection
    {
        private readonly ISocketPollable pollableSocket;
        protected readonly ISerializer serializer;
        private readonly HashSet<IAddress> addresses = new HashSet<IAddress>();
        private bool isConnected = false;


        /// <summary>
        /// Gets an enumerable of <see cref="IAddress"/> that the receiver is listening to
        /// </summary>
        public IEnumerable<IAddress> Addresses => addresses;


        /// <summary>
        /// Gets a bool that returns true when the <see cref="NetMQConnection"/> is connected; otherwise false
        /// </summary>
        public bool IsConnected => isConnected;


        /// <summary>
        /// The NetMQ socket that a <see cref="NetMQPoller"/> will actively monitor for incoming requests
        /// </summary>
        public ISocketPollable PollableSocket => pollableSocket;


        /// <summary>
        /// Initializes a new instance of <see cref="NetMQConnection"/>
        /// </summary>
        /// <param name="pollableSocket">Inner socket connection that is monitored by <see cref="INetMQPoller"/></param>
        /// <param name="serializer">A serializer that will convert data into a binary format for transmission</param>
        public NetMQConnection(ISocketPollable pollableSocket, ISerializer serializer)
        {
            this.pollableSocket = pollableSocket ?? throw new ArgumentNullException(nameof(pollableSocket));
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }


        /// <summary>
        /// Adds the <see cref="IAddress"/> to the set of adresses the <see cref="NetMQConnection"/> will listen to
        /// for incoming <see cref="Packages.Package"/>s
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> to add</param>
        public void AddAddress(IAddress address)
        {
            if (null == address)
                throw new ArgumentNullException(nameof(address));

            if (addresses.Contains(address))
                return;

            addresses.Add(address);

            if (isConnected)
                SocketAdd(address);
        }


        /// <summary>
        /// Removes the <see cref="IAddress"/> from the set of addresses the <see cref="NetMQConnection"/> will listen to
        /// for incoming <see cref="Packages.Package"/>s
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> to remove</param>
        public void RemoveAddress(IAddress address)
        {
            if (null == address || !addresses.Contains(address))
                return;

            if (isConnected)
                SocketRemove(address);

            addresses.Remove(address);
        }


        /// <summary>
        /// Removes all <see cref="IAddress"/> from the set of addresses the <see cref="NetMQConnection"/> will listen to
        /// for incoming <see cref="Packages.Package"/>s
        /// </summary>
        public void RemoveAllAddresses()
        {
            if (isConnected)
                foreach (var address in addresses)
                    SocketRemove(address);

            addresses.Clear();
        }


        /// <summary>
        /// Initializes the connections to all added <see cref="IAddress"/>es
        /// </summary>
        public void InitializeConnection()
        {
            if (isConnected)
                return;

            foreach (var address in addresses)
                SocketAdd(address);

            isConnected = true;
        }


        /// <summary>
        /// Terminates the connection to all added <see cref="IAddress"/>es
        /// </summary>
        public void TerminateConnection()
        {
            if (!isConnected)
                return;

            foreach (var address in addresses)
                SocketRemove(address);

            isConnected = false;
        }


        public abstract void SocketAdd(IAddress address);


        public abstract void SocketRemove(IAddress address);
    }
}