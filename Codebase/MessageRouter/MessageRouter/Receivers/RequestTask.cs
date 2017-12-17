﻿using MessageRouter.Packages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Receivers
{
    /// <summary>
    /// Struct to combine an incoming request with a handler to return the response
    /// </summary>
    public struct RequestTask
    {
        /// <summary>
        /// Stores a readonly reference to an incoming <see cref="Package"/>
        /// </summary>
        public readonly Package Request;


        /// <summary>
        /// Stores a readonly reference to an action that will send a response <see cref="Package"/>
        /// </summary>
        public readonly Action<Package> ResponseHandler;
        

        /// <summary>
        /// Initializes a new instance of a RequestTask composed of the supplied request <see cref="Package"/> and handler
        /// </summary>
        /// <param name="request"></param>
        /// <param name="responseHandler"></param>
        public RequestTask(Package request, Action<Package> responseHandler)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
            ResponseHandler = responseHandler ?? throw new ArgumentNullException(nameof(responseHandler));
        }
    }
}
