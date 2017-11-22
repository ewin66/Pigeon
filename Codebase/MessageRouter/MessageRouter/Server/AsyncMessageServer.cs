﻿using MessageRouter.Messages;
using MessageRouter.Receivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Server
{
    /// <summary>
    /// Basic implementation of an active server process that accepts and responds asynchronously to incoming requests
    /// </summary>
    public class AsyncMessageServer : IAsyncMessageServer<ServerInfo>
    {
        private readonly IMessageFactory messageFactory;
        private readonly IAsyncReceiverManager receiverManager;
        private readonly IRequestDispatcher requestDispatcher;
        private readonly ServerInfo serverInfo;
        private readonly object runLock = new object();
        private bool running = false;


        /// <summary>
        /// Gets the current state of the server
        /// </summary>
        public ServerInfo ServerInfo => throw new NotImplementedException();


        /// <summary>
        /// Initializes a new instance of AsyncMessageServer
        /// </summary>
        /// <param name="messageFactory"><see cref="IMessageFactory"/> dependency for constructing <see cref="Message"/>s and extracting request objects</param>
        /// <param name="receiverManager"><see cref="IAsyncReceiverFactory"/> dependency for managing <see cref="IAsyncReceiver"/>s</param>
        /// <param name="requestDispatcher"><see cref="IRequestDispatcher"/> dependency for routing and handling incoming requests</param>
        /// <param name="name">Name identifying the server</param>
        public AsyncMessageServer(IMessageFactory messageFactory, IAsyncReceiverManager receiverManager, IRequestDispatcher requestDispatcher, string name)
        {
            this.messageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));
            this.receiverManager = receiverManager ?? throw new ArgumentNullException(nameof(receiverManager));
            this.requestDispatcher = requestDispatcher ?? throw new ArgumentNullException(nameof(requestDispatcher));

            serverInfo = new ServerInfo
            {
                Name = name,
                Running = false,
                StartUpTimeStamp = null
            };

            receiverManager.RequestReceived += HandleAndRespond;
        }


        /// <summary>
        /// Asynchronously starts the server running
        /// </summary>
        public void Start()
        {
            lock (runLock)
            {
                if (running)
                    throw new InvalidOperationException($"Server {serverInfo.Name} is already running");

                running = true;
                serverInfo.StartUpTimeStamp = DateTime.Now;

                receiverManager.Start();
            }
        }


        /// <summary>
        /// Stops the asynchronous server
        /// </summary>
        public void Stop()
        {
            lock (runLock)
            {
                if (!running)
                    throw new InvalidOperationException($"Server {serverInfo.Name} is not currently running");

                running = false;
                serverInfo.StartUpTimeStamp = null;

                receiverManager.Stop();
            }
        }


        /// <summary>
        /// Extracts and responds to requests
        /// </summary>
        /// <param name="requestTask">Incoming request task</param>
        private void HandleAndRespond(object sender, RequestTask requestTask)
        {
            Task.Run(() =>
            {
                var requestObject = messageFactory.ExtractRequest(requestTask.Request);
                var responseObject = requestDispatcher.Handle(requestObject);
                var responseMessage = CreateResponse(responseObject);
                requestTask.ResponseHandler(responseMessage);
            });
        }

        
        /// <summary>
        /// Wraps a response object in a <see cref="Message"/> for returning to remote source
        /// </summary>
        /// <param name="responseObject">Response object</param>
        /// <returns>Response <see cref="Message"/></returns>
        public Message CreateResponse(object responseObject)
        {
            var createResponse = typeof(IMessageFactory)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Single(m => m.Name == "CreateResponse");

            var genericMethod = createResponse.MakeGenericMethod(responseObject.GetType());

            return (Message)genericMethod.Invoke(messageFactory, new object[] { responseObject });
        }
    }
}