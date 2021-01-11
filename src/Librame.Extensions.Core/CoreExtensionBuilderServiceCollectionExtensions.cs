﻿#region License

/* **************************************************************************************
 * Copyright (c) Librame Pong All rights reserved.
 * 
 * https://github.com/librame
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

using Microsoft.Extensions.DependencyInjection;
using System;

namespace Librame.Extensions.Core
{
    /// <summary>
    /// <see cref="CoreExtensionBuilder"/> 的 <see cref="ServiceCollection"/> 静态扩展。
    /// </summary>
    public static class CoreExtensionBuilderServiceCollectionExtensions
    {
        /// <summary>
        /// 添加 Librame 扩展。
        /// </summary>
        /// <param name="services">给定的 <see cref="IServiceCollection"/>。</param>
        /// <param name="setupAction">给定的配置选项动作（可选）。</param>
        /// <returns>返回 <see cref="CoreExtensionBuilder"/>。</returns>
        public static CoreExtensionBuilder AddLibrame(this IServiceCollection services,
            Action<CoreExtensionOptions>? setupAction = null)
        {
            var options = new CoreExtensionOptions();
            setupAction?.Invoke(options);

            return new CoreExtensionBuilder(services, options);
        }

    }
}
