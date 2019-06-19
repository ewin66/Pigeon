﻿using System;
using System.ComponentModel;

namespace ExampleContracts.Topics
{
    [Serializable]
    [ImmutableObject(true)]
    public class UserConnected
    {
        public UserConnected(int userId, string userName, DateTime timestamp)
        {
            UserId = userId;
            UserName = userName;
            Timestamp = timestamp;
        }


        public int UserId { get; }
        public string UserName { get; }
        public DateTime Timestamp { get; }
    }
}