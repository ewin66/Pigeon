﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Packages;
using MessageRouter.Serialization;
using NetMQ;

namespace MessageRouter.NetMQ
{
    /// <summary>
    /// Creates and extracts <see cref="NetMQMessage"/>s
    /// </summary>
    public class MessageFactory : IMessageFactory
    {
        private readonly ISerializer serializer;
        private readonly IPackageFactory packageFactory;


        /// <summary>
        /// Initializes a new instance of <see cref="MessageFactory"/>
        /// </summary>
        /// <param name="serializer">A serializer that will convert data into a binary format for transmission</param>
        /// <param name="packageFactory">Wraps objects in a packages</param>
        public MessageFactory(ISerializer serializer, IPackageFactory packageFactory)
        {
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            this.packageFactory = packageFactory ?? throw new ArgumentNullException(nameof(packageFactory));
        }


        /// <summary>
        /// Creates a <see cref="NetMQMessage"/> wrapping a topic event
        /// </summary>
        /// <param name="topicEvent">Topic event to be wrapped in a <see cref="NetMQMessage"/></param>
        /// <returns><see cref="NetMQMessage"/> wrapping a topic event</returns>
        public NetMQMessage CreateTopicMessage(object topicEvent)
        {
            var package = packageFactory.Pack(topicEvent);
            var message = new NetMQMessage(2);
            message.Append(topicEvent.GetType().FullName);
            message.Append(serializer.Serialize(package));
            return message;
        }


        /// <summary>
        /// Extracts a topic event from the <see cref="NetMQMessage"/>
        /// </summary>
        /// <param name="message"><see cref="NetMQMessage"/> wrapping a topic evnet</param>
        /// <returns>Topic event contained within the <see cref="NetMQMessage"/></returns>
        public object ExtractTopic(NetMQMessage message)
        {
            var package = serializer.Deserialize<Package>(message[1].ToByteArray());
            return packageFactory.Unpack(package);
        }


        /// <summary>
        /// Creates a <see cref="NetMQMessage"/> wapping a request object
        /// </summary>
        /// <param name="request">Request object to be wrapped in a <see cref="NetMQMessage"/></param>
        /// <param name="requestId">An <see cref="int"/> identifier for matching asynchronous requests and responses</param>
        /// <returns><see cref="NetMQMessage"/> wrapping the request object</returns>
        public NetMQMessage CreateRequestMessage(object request, int requestId)
        {
            var package = packageFactory.Pack(request);
            var message = new NetMQMessage(4);
            message.AppendEmptyFrame();
            message.Append(requestId);
            message.AppendEmptyFrame();
            message.Append(serializer.Serialize(package));
            return message;
        }

        
        /// <summary>
        /// Extracts a request from the <see cref="NetMQMessage"/>
        /// </summary>
        /// <param name="message"><see cref="NetMQMessage"/> wrapping a request object</param>
        /// <returns>Request object contained withing the <see cref="NetMQMessage"/>, address of remote sender, and request identifier</returns>
        public (object request, byte[] address, int requestId) ExtractRequest(NetMQMessage message)
        {
            var address = message[0].ToByteArray();
            var requestId = message[2].ConvertToInt32();
            var package = serializer.Deserialize<Package>(message[4].ToByteArray());
            var request = packageFactory.Unpack(package);
            return (request, address, requestId);
        }


        /// <summary>
        /// Creates a <see cref="NetMQMessage"/> wrapping a response object
        /// </summary>
        /// <param name="response">Response object to be wrapped in a <see cref="NetMQMessage"/></param>
        /// <param name="address">Address of the remote</param>
        /// <param name="requestId">An <see cref="int"/> identifier for matching asynchronous requests and responses</param>
        /// <returns><see cref="NetMQMessage"/> wrapping the response object</returns>
        public NetMQMessage CreateResponseMessage(object response, byte[] address, int requestId)
        {
            var package = packageFactory.Pack(response);
            var message = new NetMQMessage(5);
            message.Append(address);
            message.AppendEmptyFrame();
            message.Append(requestId);
            message.AppendEmptyFrame();
            message.Append(serializer.Serialize(package));
            return message;
        }


        /// <summary>
        /// Extracts a response from the <see cref="NetMQMessage"/>
        /// </summary>
        /// <param name="message"><see cref="NetMQMessage"/> wrapping a response object</param>
        /// <returns>Response object contained within the <see cref="NetMQMessage"/></returns>
        public object ExtractResponse(NetMQMessage message)
        {
            var package = serializer.Deserialize<Package>(message[1].ToByteArray());
            return packageFactory.Unpack(package);
        }


        /// <summary>
        /// Checks to see whether the <see cref="NetMQMessage"/> request is valid
        /// </summary>
        /// <param name="requestMessage"><see cref="NetMQMessage"/> request to check for validity</param>
        /// <returns>True if the request <see cref="NetMQMessage"/> is valid; false otherwise</returns>
        public bool IsValidRequestMessage(NetMQMessage requestMessage)
        {
            return null != requestMessage && requestMessage.FrameCount == 5;
        }
    }
}