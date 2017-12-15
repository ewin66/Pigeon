﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;
using MessageRouter.Messages;
using MessageRouter.Monitors;
using MessageRouter.Subscribers;
using MessageRouter.Verbs;

namespace MessageRouter.Publishers
{
    /// <summary>
    /// Manages the state and lifecycle of <see cref="IPublisher"/>s
    /// </summary>
    public class PublisherCache : IPublisherCache, IPublish
    {
        private readonly IMonitorCache monitorCache;
        private readonly IMessageFactory messageFactory;
        private readonly Dictionary<IAddress, IPublisher> publishers = new Dictionary<IAddress, IPublisher>();
        private readonly Dictionary<Type, IPublisherFactory> factories = new Dictionary<Type, IPublisherFactory>();

        
        /// <summary>
        /// Gets a readonly collection of <see cref="IPublisherFactory"/>s for creating <see cref="IPublisher"/>s at config-time
        /// </summary>
        public IReadOnlyCollection<IPublisherFactory> PublisherFactories => factories.Values;


        /// <summary>
        /// Initializes a new instance of <see cref="PublisherCache"/>
        /// </summary>
        /// <param name="monitorCache">Stores <see cref="IMonitor"/>s that actively manage <see cref="IPublisher"/>s</param>
        /// <param name="messageFactory">reates and extracts <see cref="Message"/>s that are distributed to remote <see cref="ISubscriber"/>s</param>
        public PublisherCache(IMonitorCache monitorCache, IMessageFactory messageFactory)
        {
            this.monitorCache = monitorCache ?? throw new ArgumentNullException(nameof(monitorCache));
            this.messageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));
        }


        /// <summary>
        /// Adds a <see cref="IPublisherFactory{TPublisher}"/> to the cache for config-time creation of <see cref="IPublisher"/>s
        /// </summary>
        /// <typeparam name="TPublisher">Transport specific implementation of <see cref="IPublisher"/></typeparam>
        /// <param name="factory">Factory used to create <see cref="IPublisher"/>s at config-time</param>
        public void AddFactory<TPublisher>(IPublisherFactory<TPublisher> factory) where TPublisher : IPublisher
        {
            if (null == factory)
                throw new ArgumentNullException(nameof(factory));

            if (factories.ContainsKey(factory.PublisherType))
                return;

            factories.Add(factory.PublisherType, factory);
            monitorCache.AddMonitor(factory.PublisherMonitor);
        }


        /// <summary>
        /// Creates and adds a <see cref="IPublisher"/> to the cache that binds and distributes <see cref="Message"/>s to <see cref="ISubscriber"/>s
        /// </summary>
        /// <typeparam name="TPublisher">Transport specific implementation of <see cref="IPublisher"/> to create</typeparam>
        /// <param name="address">The <see cref="IAddress"/> to bind to on which <see cref="ISubscriber"/>s can connect to receive updates</param>
        public void AddPublisher<TPublisher>(IAddress address) where TPublisher : IPublisher
        {
            if (null == address)
                throw new ArgumentNullException(nameof(address));

            if (publishers.ContainsKey(address))
                throw new InvalidOperationException(nameof(address));

            var factory = factories[typeof(TPublisher)];
            var publisher = factory.CreatePublisher(address);
            publishers.Add(address, publisher);
        }


        /// <summary>
        /// Distributes a message to any and all connected <see cref="ISubscriber"/>s
        /// </summary>
        /// <typeparam name="TMessage">Published message type</typeparam>
        /// <param name="message">The published message to distribute</param>
        public void Publish<TMessage>(TMessage message) where TMessage : class
        {
            if (null == message)
                throw new ArgumentNullException(nameof(message));

            var wrappedMessage = messageFactory.CreateMessage(message);

            foreach (var publisher in publishers.Values)
                publisher.Publish(wrappedMessage);
        }
    }
}