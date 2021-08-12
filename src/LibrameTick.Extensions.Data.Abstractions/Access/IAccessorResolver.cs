﻿#region License

/* **************************************************************************************
 * Copyright (c) Librame Pong All rights reserved.
 * 
 * https://github.com/librame
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

namespace Librame.Extensions.Data.Access
{
    /// <summary>
    /// 定义 <see cref="IAccessor"/> 解析器接口。
    /// </summary>
    public interface IAccessorResolver
    {
        /// <summary>
        /// 解析访问器描述符列表。
        /// </summary>
        /// <returns>返回 <see cref="IReadOnlyList{AccessorDescriptor}"/>。</returns>
        IReadOnlyList<AccessorDescriptor> ResolveDescriptors();
    }
}
