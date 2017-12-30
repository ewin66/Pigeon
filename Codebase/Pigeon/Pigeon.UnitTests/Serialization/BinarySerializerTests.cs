﻿using Pigeon.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pigeon.UnitTests.Serialization
{
    [TestFixture]
    public class BinarySerializerTests
    {
        [Test]
        public void Serialize_WithString_ProducesByteArray()
        {
            // Arrange
            var serializer = new DotNetSerializer();
            var str = "Some string";

            // Act
            var bytes = serializer.Serialize<string>(str);

            // Assert
            Assert.That(bytes, Is.Not.Null);
            CollectionAssert.IsNotEmpty(bytes);
        }


        [Test]
        public void Deserialize_WithStringData_ReproducesString()
        {
            // Arrange
            var serializer = new DotNetSerializer();
            var str = "Some string";
            var bytes = serializer.Serialize<string>(str);

            // Act
            var deserializedStr = serializer.Deserialize<string>(bytes);

            // Assert
            Assert.That(deserializedStr, Is.EqualTo(str));
        }
    }
}