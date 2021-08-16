﻿#region License

/* **************************************************************************************
 * Copyright (c) Librame Pong All rights reserved.
 * 
 * https://github.com/librame
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

using Librame.Extensions.Core;

namespace Librame.Extensions.Drawing
{
    /// <summary>
    /// 图画扩展构建器。
    /// </summary>
    public class DrawingExtensionBuilder : AbstractExtensionBuilder<DrawingExtensionOptions, DrawingExtensionBuilder>
    {
        /// <summary>
        /// 构造一个 <see cref="DrawingExtensionBuilder"/>。
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="parentBuilder"/> 或 <paramref name="options"/> 为空。
        /// </exception>
        /// <param name="parentBuilder">给定的父级 <see cref="IExtensionBuilder"/>。</param>
        /// <param name="options">给定的 <see cref="DrawingExtensionOptions"/>。</param>
        public DrawingExtensionBuilder(IExtensionBuilder parentBuilder, DrawingExtensionOptions options)
            : base(parentBuilder, options)
        {
            // Base: IdentificationGenerator
            TryAddOrReplaceService<IIdentificationGeneratorFactory, InternalIdentificationGeneratorFactory>();

            // Access
            TryAddOrReplaceService<IAccessorAggregator, InternalAccessorAggregator>();
            TryAddOrReplaceService<IAccessorManager, InternalAccessorManager>();
            TryAddOrReplaceService<IAccessorResolver, InternalAccessorResolver>();
            TryAddOrReplaceService<IAccessorSlicer, InternalAccessorSlicer>();

            // Store
            TryAddOrReplaceService(typeof(IStore<>), typeof(BaseStore<>));

            // ValueConversion
            TryAddOrReplaceService<IEncryptionConverterFactory, InternalEncryptionConverterFactory>();
        }

    }
}
