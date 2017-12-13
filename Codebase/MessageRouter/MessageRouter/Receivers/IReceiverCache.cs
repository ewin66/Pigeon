﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Receivers
{
    /// <summary>
    /// Manages the lifecycle of <see cref="IReceiver"/>s
    /// </summary>
    public interface IReceiverCache
    {
        /// <summary>
        /// Gets a readonly collection of <see cref="IReceiverFactory"/>s for creating <see cref="IReceiver"/>s at runtime
        /// </summary>
        IReadOnlyCollection<IReceiverFactory> ReceiverFactories { get; }
    }
}
