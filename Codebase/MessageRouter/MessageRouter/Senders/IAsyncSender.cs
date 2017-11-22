﻿using MessageRouter.Messages;
using MessageRouter.Receivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Senders
{
    /// <summary>
    /// Extends the <see cref="ISender"/> interface for connecting and sending messages to a remote with a task based
    /// async send-and-receive method
    /// </summary>
    public interface IAsyncSender : ISender
    {
        /// <summary>
        /// Asynchronously sends a <see cref="Message"/> to the connected remote <see cref="IReceiver"/> and returns the reponse <see cref="Message"/>
        /// Default 1 hour timeout
        /// </summary>
        /// <param name="message">Request message</param>
        /// <returns>Response message</returns>
        Task<Message> SendAndReceiveAsync(Message message);


        /// <summary>
        /// Asynchronously sends a <see cref="Message"/> to the connected remote <see cref="IReceiver"/> and returns the reponse <see cref="Message"/>
        /// </summary>
        /// <param name="message">Request message</param>
        /// <param name="timeout">Time to wait without a response before throwing an exception</param>
        /// <returns>Response message</returns>
        Task<Message> SendAndReceiveAsync(Message message, TimeSpan timeout);
    }
}
