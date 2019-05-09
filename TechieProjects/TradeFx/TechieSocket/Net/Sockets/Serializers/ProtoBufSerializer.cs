//   ===================================================================================
//   <copyright file="TechieSerializer.cs" company="TechieNotes">
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
//      The TechieSerializer.cs file.
//   </summary>
//   ===================================================================================

using System.IO;

using ProtoBuf;

namespace TechieSocket.Net.Sockets.Serializers
{
    public class ProtoBufSerializer : ITechieSerializer
    {
        #region Public Methods and Operators

        public T Deserialize<T>(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                return Serializer.Deserialize<T>(stream);
            }
        }

        public byte[] Serialize<T>(T item)
        {
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, item);
                return stream.ToArray();
            }
        }

        #endregion
    }
}