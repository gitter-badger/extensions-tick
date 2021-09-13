﻿#region License

/* **************************************************************************************
 * Copyright (c) Librame Pong All rights reserved.
 * 
 * https://github.com/librame
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

using Librame.Extensions.Data.Storing;
using Microsoft.EntityFrameworkCore;

namespace Librame.Extensions.Data.Accessing
{
    /// <summary>
    /// 定义实现 <see cref="IAccessor"/> 的数据访问器接口。
    /// </summary>
    public interface IDataAccessor : IAccessor
    {
        /// <summary>
        /// 审计数据集。
        /// </summary>
        DbSet<Audit> Audits { get; set; }

        /// <summary>
        /// 表格数据集。
        /// </summary>
        DbSet<Tabulation> Tabulations { get; set; }
    }
}
