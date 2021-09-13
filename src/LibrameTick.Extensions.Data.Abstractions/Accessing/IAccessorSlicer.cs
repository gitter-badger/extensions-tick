﻿#region License

/* **************************************************************************************
 * Copyright (c) Librame Pong All rights reserved.
 * 
 * https://github.com/librame
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

namespace Librame.Extensions.Data.Accessing
{
    /// <summary>
    /// <see cref="IAccessor"/> 切片器接口。
    /// </summary>
    public interface IAccessorSlicer
    {
        /// <summary>
        /// 访问器类型。
        /// </summary>
        Type AccessorType { get; }


        /// <summary>
        /// 切片访问器集合。
        /// </summary>
        /// <param name="descriptors">给定的 <see cref="IReadOnlyList{IAccessor}"/>。</param>
        /// <param name="interaction">给定的 <see cref="AccessMode"/>（支持读/写分库）。</param>
        /// <param name="customSliceFunc">给定的自定义切片方法（可选；支持参数分库）。</param>
        /// <returns>返回 <see cref="IAccessor"/>。</returns>
        IAccessor? SliceAccessors(IReadOnlyList<AccessorDescriptor> descriptors,
            AccessMode interaction, Func<IAccessor, bool>? customSliceFunc = null);
    }
}
