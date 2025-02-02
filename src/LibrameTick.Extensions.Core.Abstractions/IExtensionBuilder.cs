﻿#region License

/* **************************************************************************************
 * Copyright (c) Librame Pong All rights reserved.
 * 
 * https://github.com/librame
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

namespace Librame.Extensions.Core;

/// <summary>
/// 定义实现 <see cref="IExtensionInfo"/> 的扩展构建器接口。
/// </summary>
public interface IExtensionBuilder : IExtensionInfo
{
    /// <summary>
    /// 父级构建器。
    /// </summary>
    IExtensionBuilder? ParentBuilder { get; }

    /// <summary>
    /// 扩展选项类型。
    /// </summary>
    Type ExtensionOptionsType { get; }

    /// <summary>
    /// 替换服务字典集合。
    /// </summary>
    IDictionary<Type, Type> ReplacedServices { get; }

    /// <summary>
    /// 服务集合。
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// 服务特征集合。
    /// </summary>
    ServiceCharacteristicCollection ServiceCharacteristics { get; }


    /// <summary>
    /// 将扩展选项保存为 JSON 文件。
    /// </summary>
    /// <param name="services">给定的 <see cref="IServiceProvider"/>。</param>
    /// <returns>返回保存路径。</returns>
    string SaveOptionsAsJson(IServiceProvider services);

    /// <summary>
    /// 将扩展选项保存为 JSON 文件。
    /// </summary>
    /// <param name="services">给定的 <see cref="IServiceProvider"/>。</param>
    /// <param name="options">输出 <see cref="IExtensionOptions"/>。</param>
    /// <returns>返回保存路径。</returns>
    string SaveOptionsAsJson(IServiceProvider services, out IExtensionOptions options);
}
