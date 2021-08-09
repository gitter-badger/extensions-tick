﻿using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace Librame.Extensions.Data
{
    public class IdentityGeneratorFactoryTests
    {

        [Fact]
        public void AllTest()
        {
            var factory = DataExtensionBuilderHelper.CurrentServices
                .GetRequiredService<IIdentificationGeneratorFactory>();

            var combId = factory.GetNewId<Guid>();
            Assert.NotEqual(Guid.Empty, combId);

            var snowflakeId = factory.GetNewId<long>();
            Assert.True(snowflakeId > 0);

            var mongoId = factory.GetNewId<string>();
            Assert.NotEmpty(mongoId);
        }

    }
}
