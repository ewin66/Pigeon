﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Messages;
using MessageRouter.Addresses;

namespace MessageRouter.Receivers
{
    /// <summary>
    /// Factory for <see cref="IReceiver"/>s and <see cref="IAsyncReceiver"/>s
    /// </summary>
    public interface IReceiverFactory
    {
        /// <summary>
        /// Creates a new instance of a <see cref="IReceiver"/> bound to the supplied <see cref="IAddress"/>
        /// </summary>
        /// <param name="address">Address of local bound endpoint</param>
        /// <returns>Receiver bound to the address</returns>
        IReceiver Create(IAddress address);


        /// <summary>
        /// Creates a new instance of a <see cref="IAsyncReceiver"/> bound to the supplied <see cref="IAddress"/>
        /// </summary>
        /// <param name="address">Address of local bound enpoint</param>
        /// <returns>AsyncReceiver bound to the address</returns>
        IAsyncReceiver CreateAsync(IAddress address);
    }
}