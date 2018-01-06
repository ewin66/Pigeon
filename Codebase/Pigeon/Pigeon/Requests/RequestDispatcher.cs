﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

using Pigeon.Diagnostics;

namespace Pigeon.Requests
{
    /// <summary>
    /// Stores and resolves registered handlers for an incoming request to invoke to prepare responses and respond to
    /// </summary>
    public class RequestDispatcher : IRequestDispatcher
    {
        protected readonly Dictionary<Type, RequestHandlerFunction> requestHandlers = new Dictionary<Type, RequestHandlerFunction>();


        /// <summary>
        /// Finds and invokes a registered handler for the reuqest to prepare a response
        /// </summary>
        /// <param name="request">Request message</param>
        /// <returns>Response message</returns>
        public Task<object> Handle(object request)
        {
            if (null == request)
                throw new ArgumentNullException(nameof(request));

            var requestType = request.GetType();
            if (!requestHandlers.TryGetValue(requestType, out var handler))
                throw new RequestHandlerNotFoundException(requestType);

            return handler(request);
        }


        /// <summary>
        /// Registers an <see cref="IRequestHandler{TRequest, TResponse}"/>
        /// </summary>
        /// <typeparam name="TRequest">Type of request message</typeparam>
        /// <typeparam name="TResponse">Type of response message</typeparam>
        /// <param name="handler">Request handler instance</param>
        public void Register<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler)
        {
            ValidateTypes<TRequest, TResponse>();
            requestHandlers.Add(typeof(TRequest), request => Task.Run(() => (object)handler.Handle((TRequest)request)));
        }


        /// <summary>
        /// Registers a <see cref="RequestHandlerDelegate{TRequest, TResponse}"/>
        /// </summary>
        /// <typeparam name="TRequest">Type of request object</typeparam>
        /// <typeparam name="TResponse">Type of response object</typeparam>
        /// <param name="handler">Request handler instance</param>
        public void Register<TRequest, TResponse>(RequestHandlerDelegate<TRequest, TResponse> handler)
        {
            ValidateTypes<TRequest, TResponse>();
            requestHandlers.Add(typeof(TRequest), request => Task.Run(() => (object)handler((TRequest)request)));
        }


        /// <summary>
        /// Registers an <see cref="AsyncRequestHandlerDelegate{TRequest, TResponse}"/>
        /// </summary>
        /// <typeparam name="TRequest">Type of request object</typeparam>
        /// <typeparam name="TResponse">Type of response object</typeparam>
        /// <param name="handler">Request handler instance</param>
        public void RegisterAsync<TRequest, TResponse>(AsyncRequestHandlerDelegate<TRequest, TResponse> handler)
        {
            ValidateTypes<TRequest, TResponse>();

            requestHandlers.Add(typeof(TRequest), request => Task.Run(async () => (object)(await handler((TRequest)request))));
        }


        /// <summary>
        /// Initializes a new instance of a <see cref="RequestDispatcher"/> used for fluent construction
        /// </summary>
        /// <returns>An empty <see cref="RequestDispatcher"/></returns>
        public static RequestDispatcher Create()
        {
            return new RequestDispatcher();
        }


        protected void ValidateTypes<TRequest, TResponse>()
        {
            if (null == typeof(TRequest).GetCustomAttribute<SerializableAttribute>())
                throw new UnserializableTypeException(typeof(TRequest));

            if (null == typeof(TResponse).GetCustomAttribute<SerializableAttribute>())
                throw new UnserializableTypeException(typeof(TResponse));
        }
    }
}
