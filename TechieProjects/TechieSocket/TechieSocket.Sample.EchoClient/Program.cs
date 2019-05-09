﻿//   ===================================================================================
//   <copyright file="Program.cs" company="TechieNotes">
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
//   <date>31-05-2015</date>
//   <summary>
//      The Program.cs file.
//   </summary>
//   ===================================================================================

using System;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading;

using log4net;
using log4net.Config;

using Microsoft.Practices.Unity;

using TechieSocket.Net.Sockets.Client;
using TechieSocket.Net.Sockets.Response;
using TechieSocket.Net.Sockets.Serializers;

namespace TechieSocket.Sample.EchoClient
{
    internal class Program
    {
        #region Static Fields

        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static UnityContainer TechieContainer;

        #endregion

        #region Public Methods and Operators

        public static void InitializeBootstraper()
        {
            TechieContainer = new UnityContainer();
            TechieContainer.RegisterType<ITechieSerializer, ProtoBufSerializer>();
        }

        public static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            InitializeBootstraper();
            var clientService = TechieContainer.Resolve<TechieClientService>();
            clientService.Subscribe<ServerReponse>(
                eventArgs =>
                    {
                        var response = eventArgs.Item;
                        Console.WriteLine(response.Data);
                        Logger.Info(response.Data);
                    },
                Console.WriteLine);
            Thread.Sleep(5000);
            Observable.Interval(TimeSpan.FromMilliseconds(1))
                .Subscribe(x => { clientService.Publish(new ClientResponse { Data = "Hello Server" }); });
            Console.ReadLine();
        }

        #endregion
    }
}