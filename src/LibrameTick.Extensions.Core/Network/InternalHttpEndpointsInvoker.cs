﻿#region License

/* **************************************************************************************
 * Copyright (c) Librame Pong All rights reserved.
 * 
 * https://github.com/librame
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

namespace Librame.Extensions.Core.Network;

class InternalHttpEndpointsInvoker : AbstractHttpEndpointsInvoker
{
    public InternalHttpEndpointsInvoker(ILoggerFactory loggerFactory,
        IHttpClientInvokerFactory factory, IOptionsMonitor<CoreExtensionOptions> options)
        : base(loggerFactory.CreateLogger<InternalHttpEndpointsInvoker>(), factory,
            options.CurrentValue.Request, options.CurrentValue.Algorithm)
    {
    }

}
