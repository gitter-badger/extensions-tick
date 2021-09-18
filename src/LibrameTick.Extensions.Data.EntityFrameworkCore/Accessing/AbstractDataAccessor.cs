﻿#region License

/* **************************************************************************************
 * Copyright (c) Librame Pong All rights reserved.
 * 
 * https://github.com/librame
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

using Librame.Extensions.Data.Auditing;
using Librame.Extensions.Data.Storing;
using Librame.Extensions.Data.ValueConversion;

namespace Librame.Extensions.Data.Accessing;

/// <summary>
/// 定义抽象 <see cref="AbstractDataAccessor"/> 的泛型实现。
/// </summary>
/// <typeparam name="TAccessor">指定实现 <see cref="AbstractDataAccessor"/> 的访问器类型。</typeparam>
public abstract class AbstractDataAccessor<TAccessor> : AbstractDataAccessor
    where TAccessor : AbstractDataAccessor
{
    /// <summary>
    /// 使用指定的数据库上下文选项构造一个 <see cref="AbstractDataAccessor{TAccessor}"/> 实例。
    /// </summary>
    /// <remarks>
    /// 备注：如果需要注册多个 <see cref="DbContext"/> 扩展，参数必须使用泛型 <see cref="DbContextOptions{TAccessor}"/> 形式，
    /// 不能使用非泛型 <see cref="DbContextOptions"/> 形式，因为 <paramref name="options"/> 参数也会注册到容器中以供使用。
    /// </remarks>
    /// <param name="options">给定的 <see cref="DbContextOptions{TAccessor}"/>。</param>
    protected AbstractDataAccessor(DbContextOptions<TAccessor> options)
        : base(options)
    {
    }


    /// <summary>
    /// 访问器类型。
    /// </summary>
    public override Type AccessorType
        => typeof(TAccessor);
}


/// <summary>
/// 定义抽象 <see cref="AbstractAccessor"/> 与 <see cref="IDataAccessor"/> 的实现。
/// </summary>
public abstract class AbstractDataAccessor : AbstractAccessor, IDataAccessor
{
    private IReadOnlyList<Audit>? _audits;


#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

    /// <summary>
    /// 使用指定的数据库上下文选项构造一个 <see cref="AbstractDataAccessor"/> 实例。
    /// </summary>
    /// <param name="options">给定的 <see cref="DbContextOptions"/>。</param>
    protected AbstractDataAccessor(DbContextOptions options)
        : base(options)
    {
        SavingChanges += AbstractDataAccessor_SavingChanges;
        SavedChanges += AbstractDataAccessor_SavedChanges;
    }


    private void AbstractDataAccessor_SavedChanges(object? sender, SavedChangesEventArgs e)
    {
        var accessor = (sender as AbstractDataAccessor)!;
        if (accessor._audits is not null)
            accessor.DataOptions.Audit.NotificationAction?.Invoke(accessor._audits);
    }

    private void AbstractDataAccessor_SavingChanges(object? sender, SavingChangesEventArgs e)
    {
        var accessor = (sender as AbstractDataAccessor)!;
        var auditOptions = accessor.DataOptions.Audit;
        var auditingManager = accessor.GetRequiredScopeService<IAuditingManager>();

#pragma warning disable EF1001 // Internal EF Core API usage.

        var entityEntries = ((IDbContextDependencies)accessor).StateManager
            .GetEntriesForState(auditOptions.AddedState, auditOptions.ModifiedState,
                auditOptions.DeletedState, auditOptions.UnchangedState);

        accessor._audits = auditingManager.GetAudits(entityEntries.Select(s => new EntityEntry(s)));

#pragma warning restore EF1001 // Internal EF Core API usage.

        // 保存审计数据
        if (accessor._audits.Count > 0 && auditOptions.SaveAudits)
        {
            accessor.Audits.AddRange(accessor._audits);
            accessor.AuditProperties.AddRange(accessor._audits.SelectMany(s => s.Properties));
        }
    }

#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。


    /// <summary>
    /// 审计数据集。
    /// </summary>
    public DbSet<Audit> Audits { get; set; }

    /// <summary>
    /// 审计属性数据集。
    /// </summary>
    public DbSet<AuditProperty> AuditProperties { get; set; }


    /// <summary>
    /// 模型创建后置动作。
    /// </summary>
    public Action<IMutableEntityType>? ModelCreatingPostAction { get; set; }


    /// <summary>
    /// 开始模型创建。
    /// </summary>
    /// <param name="modelBuilder">给定的 <see cref="ModelBuilder"/>。</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // DbContext.OnModelCreating()
        base.OnModelCreating(modelBuilder);

        OnDataModelCreating(modelBuilder);

        var converterFactory = GetRequiredScopeService<IEncryptionConverterFactory>();
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            entityType.UseEncryption(converterFactory, AccessorType);
            entityType.UseQueryFilters(DataOptions.QueryFilters, this);

            ModelCreatingPostAction?.Invoke(entityType);
        }
    }

    /// <summary>
    /// 开始数据模型创建。
    /// </summary>
    /// <param name="modelBuilder">给定的 <see cref="ModelBuilder"/>。</param>
    protected virtual void OnDataModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.CreateDataModel(this);
    }

}
