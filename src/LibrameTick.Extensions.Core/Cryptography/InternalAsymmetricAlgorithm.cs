﻿#region License

/* **************************************************************************************
 * Copyright (c) Librame Pong All rights reserved.
 * 
 * https://github.com/librame
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

namespace Librame.Extensions.Core.Cryptography;

class InternalAsymmetricAlgorithm : AbstractAsymmetricAlgorithm
{
    public InternalAsymmetricAlgorithm(IAlgorithmParameterGenerator parameterGenerator,
        IOptionsMonitor<CoreExtensionOptions> options)
        : base(parameterGenerator, options.CurrentValue.Algorithm)
    {
    }

}
