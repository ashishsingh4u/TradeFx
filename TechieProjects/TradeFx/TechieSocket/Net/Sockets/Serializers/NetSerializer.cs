//   ===================================================================================
//   <copyright file="TechieNetSerializer.cs" company="TechieNotes">
//   ===================================================================================
//    TechieNotes Utilities & Best Practices
//    Samples and Guidelines for Winform & ASP.net development
//   ===================================================================================
//    Copyright (c) TechieNotes.  All rights reserved.
//    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//    OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//    LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
//    FITNESS FOR A PARTICULAR PURPOSE.
//   ===================================================================================
//    The example companies, organizations, products, domain names,
//    e-mail addresses, logos, people, places, and events depicted
//    herein are fictitious.  No association with any real company,
//    organization, product, domain name, email address, logo, person,
//    places, or events is intended or should be inferred.
//   ===================================================================================
//   </copyright>
//   <author>Ashish Singh</author>
//   <email>mailto:ashishsingh4u@gmail.com</email>
//   <date>24-05-2015</date>
//   <summary>
//      The TechieNetSerializer.cs file.
//   </summary>
//   ===================================================================================

using System;
using System.Collections.Generic;
using System.IO;

using NetSerializer;

namespace TechieSocket.Net.Sockets.Serializers
{
    public class NetSerializer : ITechieSerializer
    {
        #region Fields

        private readonly Serializer _serializer;

        #endregion

        #region Constructors and Destructors

        public NetSerializer(IEnumerable<Type> types)
        {
            _serializer = new Serializer(types);
        }

        #endregion

        #region Public Methods and Operators

        public T Deserialize<T>(byte[] data)
        {
            using (var memoryStream = new MemoryStream(data))
            {
                var value = (T)_serializer.Deserialize(memoryStream);
                return value;
            }
        }

        public byte[] Serialize<T>(T item)
        {
            using (var memoryStream = new MemoryStream())
            {
                _serializer.Serialize(memoryStream, item);
                return memoryStream.ToArray();
            }
        }

        #endregion
    }
}