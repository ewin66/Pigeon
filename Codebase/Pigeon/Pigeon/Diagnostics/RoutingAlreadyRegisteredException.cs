﻿using System;
using System.Runtime.Serialization;

namespace Pigeon.Diagnostics
{
    /// <summary>
    /// Exception that is thrown when a overriding routing is added to a router
    /// </summary>
    [Serializable]
    public class RoutingAlreadyRegisteredException<T> : PigeonException
    {
        /// <summary>
        /// Gets the overriding routing that was attempted to be registered
        /// </summary>
        public T OverridingRouting { get; private set; }


        /// <summary>
        /// Gets the pre-existing routing that was already registered
        /// </summary>
        public T ExistingRouting { get; private set; }


        /// <summary>
        /// Initializes a new instance of <see cref="RoutingAlreadyRegisteredException"/>
        /// </summary>
        /// <param name="overridingRouting">New routing that was attempted to be registered</param>
        /// <param name="existingRouting">Pre-existing routing that was already registered</param>
        public RoutingAlreadyRegisteredException(T overridingRouting, T existingRouting)
            : this(overridingRouting, existingRouting, $"New mapping {overridingRouting.ToString()} already registered with {existingRouting.ToString()}", null)
        { }


        /// <summary>
        /// Initializes a new instance of <see cref="RoutingAlreadyRegisteredException"/>
        /// </summary>
        /// <param name="overridingRouting">New routing that was attempted to be registered</param>
        /// <param name="existingRouting">Pre-existing routing that was already registered</param>
        /// <param name="message">Message that describes the exception</param>
        public RoutingAlreadyRegisteredException(T overridingRouting, T existingRouting, string message) 
            : this(overridingRouting, existingRouting, message, null)
        { }


        /// <summary>
        /// Initializes a new instance of <see cref="RoutingAlreadyRegisteredException"/>
        /// </summary>
        /// <param name="overridingRouting">New routing that was attempted to be registered</param>
        /// <param name="existingRouting">Pre-existing routing that was already registered</param>
        /// <param name="message">Message that describes the exception</param>
        /// <param name="inner">Inner exception</param>
        public RoutingAlreadyRegisteredException(T overridingRouting, T existingRouting, string message, Exception inner) 
            : base(message, inner)
        {
            OverridingRouting = overridingRouting;
            ExistingRouting = existingRouting;
        }


        /// <summary>
        /// Initializes a new instance of <see cref="RoutingAlreadyRegisteredException{T}"/> with serialized data
        /// </summary>
        /// <param name="info">Holds the serialized object data about the exception</param>
        /// <param name="context">Contains contextual information about the source or destination</param>
        protected RoutingAlreadyRegisteredException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        { }
    }
}
