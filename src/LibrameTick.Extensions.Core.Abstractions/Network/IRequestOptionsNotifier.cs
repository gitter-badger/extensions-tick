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

/// <summary>
/// 定义继承 <see cref="IOptionsNotifier"/> 的请求选项通知器接口。
/// </summary>
public interface IRequestOptionsNotifier : IOptionsNotifier
{
    /// <summary>
    /// 基础 URL。
    /// </summary>
    public string BaseUrl { get; }

    /// <summary>
    /// 验证标识。
    /// </summary>
    public string VerifyId { get; }
}
