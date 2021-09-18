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

namespace Librame.Extensions.Data.Accessing;

/// <summary>
/// 定义实现 <see cref="IOptions"/> 的访问选项。
/// </summary>
public class AccessOptions : AbstractOptions
{
    /// <summary>
    /// 构造一个独立属性通知器的 <see cref="AccessOptions"/>（此构造函数适用于独立使用 <see cref="AccessOptions"/> 的情况）。
    /// </summary>
    /// <param name="sourceAliase">给定的源别名（独立属性通知器必须命名实例）。</param>
    public AccessOptions(string sourceAliase)
        : base(sourceAliase)
    {
    }

    /// <summary>
    /// 构造一个 <see cref="AccessOptions"/>。
    /// </summary>
    /// <param name="parentNotifier">给定的父级 <see cref="IPropertyNotifier"/>。</param>
    public AccessOptions(IPropertyNotifier parentNotifier)
        : base(parentNotifier, sourceAliase: null)
    {
    }


    /// <summary>
    /// 当连接数据库时，如果数据库已存在，则可以确保将可能已存在的数据库删除（默认未启用功能。注：务必慎用此功能，推荐用于测试环境，不可用于正式环境）。
    /// </summary>
    public bool EnsureDatabaseDeleted
    {
        get => Notifier.GetOrAdd(nameof(EnsureDatabaseDeleted), false);
        set => Notifier.AddOrUpdate(nameof(EnsureDatabaseDeleted), value);
    }

    /// <summary>
    /// 当连接数据库时，如果数据库不存在，则可以确保新建数据库（默认启用此功能）。
    /// </summary>
    public bool EnsureDatabaseCreated
    {
        get => Notifier.GetOrAdd(nameof(EnsureDatabaseCreated), true);
        set => Notifier.AddOrUpdate(nameof(EnsureDatabaseCreated), value);
    }

    /// <summary>
    /// 自动迁移数据库（默认启用此功能）。
    /// </summary>
    public bool AutomaticMigration
    {
        get => Notifier.GetOrAdd(nameof(AutomaticMigration), true);
        set => Notifier.AddOrUpdate(nameof(AutomaticMigration), value);
    }

}
