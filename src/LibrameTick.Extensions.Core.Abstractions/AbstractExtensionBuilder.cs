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
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Librame.Extensions.Core
{
    /// <summary>
    /// 抽象扩展构建器（抽象实现 <see cref="IExtensionBuilder"/>）。
    /// </summary>
    public abstract class AbstractExtensionBuilder : AbstractExtensionInfo, IExtensionBuilder
    {
        /// <summary>
        /// 构造一个 <see cref="AbstractExtensionBuilder"/>。
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="services"/> 或 <paramref name="options"/> 为空。
        /// </exception>
        /// <param name="services">给定的 <see cref="IServiceCollection"/>。</param>
        /// <param name="options">给定的 <see cref="IExtensionOptions"/>。</param>
        /// <param name="parent">给定的可空 <see cref="IExtensionBuilder"/>。</param>
        public AbstractExtensionBuilder(IServiceCollection services, IExtensionOptions options,
            IExtensionBuilder? parent)
            : base(parent)
        {
            Services = services.NotNull(nameof(services));
            Options = options.NotNull(nameof(options));
        }


        /// <summary>
        /// 服务集合。
        /// </summary>
        public IServiceCollection Services { get; private set; }

        /// <summary>
        /// 扩展选项。
        /// </summary>
        public IExtensionOptions Options { get; private set; }


        #region AddOrReplaceByCharacteristic

        /// <summary>
        /// 通过服务特征实现添加或替换服务（支持扩展选项的替换服务字典集合）。
        /// </summary>
        /// <typeparam name="TService">指定的服务类型。</typeparam>
        /// <typeparam name="TImplementation">指定的实现类型。</typeparam>
        /// <returns>返回 <see cref="IExtensionBuilder"/>。</returns>
        public virtual IExtensionBuilder AddOrReplaceByCharacteristic<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
            => AddOrReplaceByCharacteristic(typeof(TService), typeof(TImplementation));

        /// <summary>
        /// 通过服务特征实现添加或替换服务（支持扩展选项的替换服务字典集合）。
        /// </summary>
        /// <typeparam name="TService">指定的服务类型。</typeparam>
        /// <param name="implementationType">给定的实现类型。</param>
        /// <returns>返回 <see cref="IExtensionBuilder"/>。</returns>
        public virtual IExtensionBuilder AddOrReplaceByCharacteristic<TService>(Type implementationType)
            where TService : class
            => AddOrReplaceByCharacteristic(typeof(TService), implementationType);

        /// <summary>
        /// 通过服务特征实现添加或替换服务（支持扩展选项的替换服务字典集合）。
        /// </summary>
        /// <param name="serviceType">给定的服务类型。</param>
        /// <param name="implementationType">给定的实现类型。</param>
        /// <returns>返回 <see cref="IExtensionBuilder"/>。</returns>
        public virtual IExtensionBuilder AddOrReplaceByCharacteristic(Type serviceType, Type implementationType)
        {
            if (!Options.ServiceCharacteristics.TryGetValue(serviceType, out var characteristic))
                characteristic = ServiceCharacteristic.Singleton();

            if (Options.ReplacedServices.TryGetValue(serviceType, out var replacedType))
                implementationType = replacedType;

            var descriptor = ServiceDescriptor.Describe(serviceType, implementationType, characteristic.Lifetime);
            return AddOrReplaceByCharacteristic(descriptor, characteristic);
        }


        /// <summary>
        /// 通过服务特征实现添加或替换服务。
        /// </summary>
        /// <typeparam name="TService">指定的服务类型。</typeparam>
        /// <param name="factory">给定的服务方法。</param>
        /// <returns>返回 <see cref="IExtensionBuilder"/>。</returns>
        public virtual IExtensionBuilder AddOrReplaceByCharacteristic<TService>(Func<IServiceProvider, TService> factory)
            where TService : class
            => AddOrReplaceByCharacteristic(typeof(TService), factory);

        /// <summary>
        /// 通过服务特征实现添加或替换服务。
        /// </summary>
        /// <param name="serviceType">给定的服务类型。</param>
        /// <param name="factory">给定的服务方法。</param>
        /// <returns>返回 <see cref="IExtensionBuilder"/>。</returns>
        public virtual IExtensionBuilder AddOrReplaceByCharacteristic(Type serviceType, Func<IServiceProvider, object> factory)
        {
            if (!Options.ServiceCharacteristics.TryGetValue(serviceType, out var characteristic))
                characteristic = ServiceCharacteristic.Singleton();

            var descriptor = ServiceDescriptor.Describe(serviceType, factory, characteristic.Lifetime);
            return AddOrReplaceByCharacteristic(descriptor, characteristic);
        }


        /// <summary>
        /// 通过服务特征实现添加或替换服务。
        /// </summary>
        /// <typeparam name="TService">指定的服务类型。</typeparam>
        /// <param name="instance">给定的服务实例。</param>
        /// <returns>返回 <see cref="IExtensionBuilder"/>。</returns>
        public virtual IExtensionBuilder AddOrReplaceByCharacteristic<TService>(TService instance)
            where TService : class
            => AddOrReplaceByCharacteristic(typeof(TService), instance);

        /// <summary>
        /// 通过服务特征实现添加或替换服务。
        /// </summary>
        /// <param name="serviceType">给定的服务类型。</param>
        /// <param name="instance">给定的服务实例。</param>
        /// <returns>返回 <see cref="IExtensionBuilder"/>。</returns>
        public virtual IExtensionBuilder AddOrReplaceByCharacteristic(Type serviceType, object instance)
        {
            // 虽然对象服务实例都是单例，但此处需要使用服务特征的是否替换功能
            if (!Options.ServiceCharacteristics.TryGetValue(serviceType, out var characteristic))
                characteristic = ServiceCharacteristic.Singleton();

            var descriptor = ServiceDescriptor.Singleton(serviceType, instance);
            return AddOrReplaceByCharacteristic(descriptor, characteristic);
        }


        /// <summary>
        /// 通过服务特征实现添加或替换服务。
        /// </summary>
        /// <param name="descriptor">给定的 <see cref="ServiceDescriptor"/>。</param>
        /// <param name="characteristic">给定的 <see cref="ServiceCharacteristic"/>。</param>
        /// <returns>返回 <see cref="IExtensionBuilder"/>。</returns>
        protected virtual IExtensionBuilder AddOrReplaceByCharacteristic(ServiceDescriptor descriptor,
            ServiceCharacteristic characteristic)
        {
            // 如果需要替换已存在的服务
            if (characteristic.ReplaceIfExists)
                Services.Replace(descriptor);
            else
                Services.TryAdd(descriptor);
            
            return this;
        }

        #endregion

    }
}
