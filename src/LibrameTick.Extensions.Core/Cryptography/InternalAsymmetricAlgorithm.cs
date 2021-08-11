﻿#region License

/* **************************************************************************************
 * Copyright (c) Librame Pong All rights reserved.
 * 
 * https://github.com/librame
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

namespace Librame.Extensions.Core.Cryptography
{
    class InternalAsymmetricAlgorithm : AbstractAsymmetricAlgorithm
    {
        private CoreExtensionOptions _options;


        public InternalAsymmetricAlgorithm(IAlgorithmParameterGenerator parameterGenerator,
            CoreExtensionBuilder extensionBuilder)
            : base(parameterGenerator, extensionBuilder)
        {
            _options = extensionBuilder.Options;
        }


        protected override SigningCredentialsOptions DefaultRsaOptions
            => _options.Algorithms.Rsa;

    }
}
