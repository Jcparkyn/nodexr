﻿namespace Nodexr.Api.Functions.IntegrationTests;
using NUnit.Framework;

using static Nodexr.Api.Functions.IntegrationTests.Testing;

public class TestBase
{
    [SetUp]
    public async Task TestSetUp()
    {
        await ResetState();
    }
}
