﻿using Microsoft.Extensions.Hosting;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication();

var startup = new Startup();
startup.Configure(host);

var app = host.Build();

app.Run();
