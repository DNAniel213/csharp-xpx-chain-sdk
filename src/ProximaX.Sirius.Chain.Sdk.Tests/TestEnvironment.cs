﻿using System;
using System.Collections.Generic;
using System.Text;
using ProximaX.Sirius.Chain.Sdk.Model.Accounts;
using ProximaX.Sirius.Chain.Sdk.Tests.Utils;

namespace ProximaX.Sirius.Chain.Sdk.Tests
{
    public class TestEnvironment
    {
        public string Host { get; set; }
        public string Protocol { get; set; }

        public int Port { get; set; }

        public string GenerationHash { get; set; }

        public string SeedAccountPK { get; set; }

        public string BaseUrl => $"{Protocol}://{Host}:{Port}";

        public EnvironmentSelection EnvironmentSelection { get; set; }

        public TestEnvironment(string host = null, string protocol = null, int port = default,string generationHash = null, string seedAccountPK=null,
            EnvironmentSelection selection = EnvironmentSelection.DEV)
        {
            Host = host;
            Protocol = protocol;
            Port = port;
            GenerationHash = generationHash;
            SeedAccountPK = seedAccountPK;
            EnvironmentSelection = selection;
        }
    }
}
