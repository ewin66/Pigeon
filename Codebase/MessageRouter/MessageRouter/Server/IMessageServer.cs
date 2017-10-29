﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageRouter.Server
{
    /// <summary>
    /// Interface controls the active server process that accepts and responds to incoming requests 
    /// </summary>
    /// <typeparam name="TServerInfo">Type of <see cref="IServerInfo"/> data object the server keeps so
    /// implemetations can control what state information they expose</typeparam>
    public interface IMessageServer<TServerInfo>
    {
        /// <summary>
        /// Gets the current state of the server
        /// </summary>
        TServerInfo ServerInfo { get; }


        /// <summary>
        /// Synchronously starts the server running
        /// </summary>
        void Run();


        /// <summary>
        /// Synchronously starts the server running with a <see cref="CancellationToken"/> to stop
        /// </summary>
        /// <param name="cancellationToken"></param>
        void Run(CancellationToken cancellationToken);
    }
}
