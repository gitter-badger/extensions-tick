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
    /// <summary>
    /// 算法接口。
    /// </summary>
    public interface IAlgorithm
    {
        /// <summary>
        /// 参数生成器。
        /// </summary>
        IAlgorithmParameterGenerator ParameterGenerator { get; }

        /// <summary>
        /// 扩展构建器。
        /// </summary>
        IExtensionBuilder ExtensionBuilder { get; }
    }
}
