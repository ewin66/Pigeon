﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Monitors;

namespace MessageRouter.Senders
{
    /// <summary>
    /// Manages the state of <see cref="TSender"/>s that connect to different remotes
    /// </summary>
    /// <typeparam name="TSender">The type of senders that this can monitor</typeparam>
    public interface ISenderMonitor<TSender> : IMonitor where TSender : ISender
    {
        /// <summary>
        /// Adds a <see cref="TSender"/> to the internal cache of monitored senders
        /// </summary>
        /// <param name="sender"><see cref="TSender"/> to add to the monitored cache of senders</param>
        void AddSender(TSender sender);
    }
}
