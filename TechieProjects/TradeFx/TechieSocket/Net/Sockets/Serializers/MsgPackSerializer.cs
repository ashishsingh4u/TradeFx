//   ===================================================================================
//   <copyright file="MsgPackSerializer.cs" company="TechieNotes">
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
//   <date>30-05-2015</date>
//   <summary>
//      The MsgPackSerializer.cs file.
//   </summary>
//   ===================================================================================

using System.IO;

using MsgPack.Serialization;

namespace TechieSocket.Net.Sockets.Serializers
{
    public class MsgPackSerializer : ITechieSerializer
    {
        #region Public Methods and Operators

        public T Deserialize<T>(byte[] data)
        {
            var serializer = MessagePackSerializer.Get<T>();
            using (var stream = new MemoryStream(data))
            {
                var item = serializer.Unpack(stream);
                return item;
            }
        }

        public byte[] Serialize<T>(T item)
        {
            var serializer = MessagePackSerializer.Get<T>();
            using (var stream = new MemoryStream())
            {
                serializer.Pack(stream, item);
                return stream.ToArray();
            }
        }

        #endregion
    }
}