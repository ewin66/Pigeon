﻿using System;
using System.Collections.Generic;

using Pigeon.Addresses;
using Pigeon.Diagnostics;
using Pigeon.Monitors;
using Pigeon.Packages;
using Pigeon.Subscribers;
using Pigeon.Verbs;

namespace Pigeon.Publishers
{
    /// <summary>
    /// Manages the state and life-cycle of <see cref="IPublisher"/>s
    /// </summary>
    public class PublisherCache : IPublisherCache, IPublish
    {
        private readonly IMonitorCache monitorCache;
        private readonly Dictionary<IAddress, IPublisher> publishers = new Dictionary<IAddress, IPublisher>();
        private readonly Dictionary<Type, IPublisherFactory> factories = new Dictionary<Type, IPublisherFactory>();

        
        /// <summary>
        /// Gets a read-only collection of <see cref="IPublisherFactory"/>s for creating <see cref="IPublisher"/>s at configuration time
        /// </summary>
        public IReadOnlyCollection<IPublisherFactory> PublisherFactories => factories.Values;


        /// <summary>
        /// Initializes a new instance of <see cref="PublisherCache"/>
        /// </summary>
        /// <param name="monitorCache">Stores <see cref="IMonitor"/>s that actively manage <see cref="IPublisher"/>s</param>
        public PublisherCache(IMonitorCache monitorCache)
        {
            this.monitorCache = monitorCache ?? throw new ArgumentNullException(nameof(monitorCache));
        }


        /// <summary>
        /// Adds a <see cref="IPublisherFactory{TPublisher}"/> to the cache for configuration time creation of <see cref="IPublisher"/>s
        /// </summary>
        /// <param name="factory">Factory used to create <see cref="IPublisher"/>s at configuration time</param>
        public void AddFactory(IPublisherFactory factory)
        {
            if (factory is null)
                throw new ArgumentNullException(nameof(factory));

            if (factories.ContainsKey(factory.PublisherType))
                return;

            factories.Add(factory.PublisherType, factory);
            monitorCache.AddMonitor(factory.PublisherMonitor);
        }


        /// <summary>
        /// Creates and adds a <see cref="IPublisher"/> to the cache that binds and distributes <see cref="Package"/>s to <see cref="ISubscriber"/>s
        /// </summary>
        /// <typeparam name="TPublisher">Transport specific implementation of <see cref="IPublisher"/> to create</typeparam>
        /// <param name="address">The <see cref="IAddress"/> to bind to on which <see cref="ISubscriber"/>s can connect to receive updates</param>
        public void AddPublisher<TPublisher>(IAddress address) where TPublisher : IPublisher
        {
            if (address is null)
                throw new ArgumentNullException(nameof(address));

            if (publishers.ContainsKey(address))
                throw new InvalidOperationException(nameof(address));

            if (!factories.TryGetValue(typeof(TPublisher), out var factory))
                throw MissingFactoryException.For<TPublisher, PublisherCache>();
            
            var publisher = factory.CreatePublisher(address);
            publishers.Add(address, publisher);
        }


        /// <summary>
        /// Distributes a message to any and all connected <see cref="ISubscriber"/>s
        /// </summary>
        /// <typeparam name="TTopic">The topic type of the message to publish</typeparam>
        /// <param name="topicEvent">The topic message to distribute</param>
        public void Publish<TMessage>(TMessage topicEvent) where TMessage : class
        {
            if (topicEvent is null)
                throw new ArgumentNullException(nameof(topicEvent));
            
            foreach (var publisher in publishers.Values)
                publisher.Publish(topicEvent);
        }
    }
}
