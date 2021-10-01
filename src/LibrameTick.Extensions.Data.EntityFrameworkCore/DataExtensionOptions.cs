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
using Librame.Extensions.Data.Accessing;
using Librame.Extensions.Data.Auditing;
using Librame.Extensions.Data.Sharding;
using Librame.Extensions.Data.Storing;
using Librame.Extensions.Data.ValueConversion;

namespace Librame.Extensions.Data;

/// <summary>
/// 数据扩展选项。
/// </summary>
public class DataExtensionOptions : AbstractExtensionOptions<DataExtensionOptions>
{
    private readonly List<IObjectIdentificationGenerator> _idGenerators
        = new List<IObjectIdentificationGenerator>();


    /// <summary>
    /// 构造一个 <see cref="DataExtensionOptions"/>。
    /// </summary>
    /// <param name="parentOptions">给定的父级 <see cref="IExtensionOptions"/>。</param>
    public DataExtensionOptions(IExtensionOptions parentOptions)
        : base(parentOptions, parentOptions.Directories)
    {
        CoreOptions = parentOptions.GetRequiredOptions<CoreExtensionOptions>();
        Access = new(Notifier);
        Audit = new(Notifier);
        Store = new(Notifier);

        // Base: IdentificationGenerator
        AddIdGenerator(new MongoIdentificationGenerator(CoreOptions.Clock));
        AddIdGenerator(new SnowflakeIdentificationGenerator(CoreOptions.MachineId,
            CoreOptions.DataCenterId, CoreOptions.Clock));
        // 异构数据源数据同步功能的标识必须使用统一的生成方案
        AddIdGenerator(CombIdentificationGenerator.ForSqlServer(CoreOptions.Clock));

        ServiceCharacteristics.AddSingleton<IAuditingManager>();
        ServiceCharacteristics.AddSingleton<IIdentificationGeneratorFactory>();

        // Accessing
        ServiceCharacteristics.AddScope<IAccessorManager>();
        ServiceCharacteristics.AddScope<IAccessorMigrator>();
        ServiceCharacteristics.AddScope<IAccessorResolver>();

        ServiceCharacteristics.AddScope<IAccessorSeeder>(addImplementationType: true);
        ServiceCharacteristics.AddScope<IAccessorInitializer>();

        // Sharding
        ShardingStrategies.Add(new DateTimeShardingStrategy(CoreOptions.Clock));
        ShardingStrategies.Add(new DateTimeOffsetShardingStrategy(CoreOptions.Clock));
        ShardingStrategies.Add(new CultureInfoShardingStrategy());

        ServiceCharacteristics.AddSingleton<IShardingManager>();

        // Storing
        ServiceCharacteristics.AddScope(typeof(IStore<>));

        // ValueConversion
        ServiceCharacteristics.AddSingleton<IEncryptionConverterFactory>();
    }


    /// <summary>
    /// 核心扩展选项。
    /// </summary>
    [JsonIgnore]
    public CoreExtensionOptions CoreOptions { get; init; }

    /// <summary>
    /// 访问选项。
    /// </summary>
    public AccessOptions Access { get; init; }

    /// <summary>
    /// 审计选项。
    /// </summary>
    public AuditOptions Audit { get; init; }

    /// <summary>
    /// 存储选项。
    /// </summary>
    public StoreOptions Store {  get; init; }


    /// <summary>
    /// 标识生成器列表集合（默认已集成 <see cref="string"/> “MongoDB”、<see cref="long"/> “雪花”、<see cref="Guid"/> “COMB for SQL Server” 等标识类型的生成器）。
    /// </summary>
    [JsonIgnore]
    public IReadOnlyList<IObjectIdentificationGenerator> IdGenerators
        => _idGenerators;

    /// <summary>
    /// 分片策略列表集合（默认已集成 <see cref="CultureInfoShardingStrategy"/>、<see cref="DateTimeShardingStrategy"/>、<see cref="DateTimeOffsetShardingStrategy"/> 等分片策略）。
    /// </summary>
    [JsonIgnore]
    public List<IShardingStrategy> ShardingStrategies { get; init; }
        = new();

    /// <summary>
    /// 查询过滤器列表集合。
    /// </summary>
    [JsonIgnore]
    public List<IQueryFilter> QueryFilters { get; init; }
        = new();


    /// <summary>
    /// 添加实现 <see cref="IIdentificationGenerator{TId}"/> 的标识生成器（推荐从 <see cref="AbstractIdentificationGenerator{TId}"/> 派生）。
    /// </summary>
    /// <param name="idGenerator">给定的 <see cref="IIdentificationGenerator{TId}"/>。</param>
    public void AddIdGenerator<TId>(IIdentificationGenerator<TId> idGenerator)
        where TId : IEquatable<TId>
        => _idGenerators.Add(idGenerator);

}
