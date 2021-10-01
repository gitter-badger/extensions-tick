﻿#region License

/* **************************************************************************************
 * Copyright (c) Librame Pong All rights reserved.
 * 
 * https://github.com/librame
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

namespace Librame.Extensions.Data;

/// <summary>
/// 定义泛型父标识符接口。
/// </summary>
/// <typeparam name="TId">指定的标识类型（兼容各种引用与值类型标识）。</typeparam>
public interface IParentIdentifier<TId> : IIdentifier<TId>, IObjectParentIdentifier
    where TId : IEquatable<TId>
{
    /// <summary>
    /// 父标识。
    /// </summary>
    TId? ParentId { get; set; }
}
