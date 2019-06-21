﻿using System;

using Pigeon.Diagnostics;

namespace Pigeon.Topics
{
    public class DITopicDispatcher : TopicDispatcher, IDITopicDispatcher
    {
        private IContainer container;


        public DITopicDispatcher(IContainer container)
        {
            this.container = container ?? throw new ArgumentNullException(nameof(container));
        }


        public void Register<TTopic, THandler>() where THandler : ITopicHandler<TTopic>
        {
            Validate<TTopic>();

            if (!container.IsRegistered<THandler>())
                throw new NotRegisteredException(typeof(THandler));

            handlers.Add(typeof(TTopic), eventMessage => container.Resolve<THandler>().Handle((TTopic)eventMessage));
        }
    }
}
