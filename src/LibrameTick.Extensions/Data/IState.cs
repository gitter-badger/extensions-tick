﻿#region License

/* **************************************************************************************
 * Copyright (c) Librame Pong All rights reserved.
 * 
 * https://github.com/librame
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

using System;

namespace Librame.Extensions.Data
{
    /// <summary>
    /// 定义泛型状态接口。
    /// </summary>
    /// <typeparam name="TStatus">指定的状态类型（兼容不支持枚举类型的实体框架）。</typeparam>
    public interface IState<TStatus> : IEquatable<IState<TStatus>>, IObjectState
    {
        /// <summary>
        /// 状态。
        /// </summary>
        TStatus Status { get; set; }
    }
}
