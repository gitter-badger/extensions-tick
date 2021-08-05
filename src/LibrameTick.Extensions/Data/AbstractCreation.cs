﻿#region License

/* **************************************************************************************
 * Copyright (c) Librame Pong All rights reserved.
 * 
 * https://github.com/librame
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

using Librame.Extensions.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;
using System.Threading.Tasks;

namespace Librame.Extensions.Data
{
    /// <summary>
    /// 定义抽象实现 <see cref="ICreation{TCreatedBy}"/>。
    /// </summary>
    /// <typeparam name="TCreatedBy">指定的创建者类型。</typeparam>
    [NotMapped]
    public abstract class AbstractCreation<TCreatedBy>
        : AbstractCreation<TCreatedBy, DateTimeOffset>, ICreation<TCreatedBy>
        where TCreatedBy : IEquatable<TCreatedBy>
    {
        /// <summary>
        /// 构造一个 <see cref="AbstractCreation{TCreatedBy}"/>。
        /// </summary>
        protected AbstractCreation()
        {
            CreatedTime = DateTimeExtensions.GetUtcNow();
            CreatedTimeTicks = CreatedTime.Ticks;
        }


        /// <summary>
        /// 创建时间周期数。
        /// </summary>
        [Display(Name = nameof(CreatedTimeTicks), ResourceType = typeof(DataResource))]
        public virtual long CreatedTimeTicks { get; set; }


        /// <summary>
        /// 设置对象创建时间。
        /// </summary>
        /// <param name="newCreatedTime">给定的新创建时间对象。</param>
        /// <returns>返回日期与时间（兼容 <see cref="DateTime"/> 或 <see cref="DateTimeOffset"/>）。</returns>
        public override object SetObjectCreatedTime(object newCreatedTime)
        {
            CreatedTime = ToCreatedTime(newCreatedTime, nameof(newCreatedTime));
            CreatedTimeTicks = CreatedTime.Ticks;

            return newCreatedTime;
        }

        /// <summary>
        /// 异步设置创建时间。
        /// </summary>
        /// <param name="newCreatedTime">给定的新创建时间对象。</param>
        /// <param name="cancellationToken">给定的 <see cref="CancellationToken"/>（可选）。</param>
        /// <returns>返回一个包含日期与时间（兼容 <see cref="DateTime"/> 或 <see cref="DateTimeOffset"/>）的异步操作。</returns>
        public override ValueTask<object> SetObjectCreatedTimeAsync(object newCreatedTime,
            CancellationToken cancellationToken = default)
        {
            var createdTime = ToCreatedTime(newCreatedTime, nameof(newCreatedTime));

            return cancellationToken.RunValueTask(() =>
            {
                CreatedTime = createdTime;
                CreatedTimeTicks = CreatedTime.Ticks;

                return newCreatedTime;
            });
        }


        /// <summary>
        /// 转换为键值对字符串。
        /// </summary>
        /// <returns>返回字符串。</returns>
        public override string ToString()
            => $"{base.ToString()};{nameof(CreatedTimeTicks)}={CreatedTimeTicks}";

    }


    /// <summary>
    /// 定义抽象实现 <see cref="ICreation{TCreatedBy, TCreatedTime}"/>。
    /// </summary>
    /// <typeparam name="TCreatedBy">指定的创建者类型。</typeparam>
    /// <typeparam name="TCreatedTime">指定的创建时间类型（提供对 <see cref="DateTime"/> 或 <see cref="DateTimeOffset"/> 的支持）。</typeparam>
    [NotMapped]
    public abstract class AbstractCreation<TCreatedBy, TCreatedTime> : ICreation<TCreatedBy, TCreatedTime>
        where TCreatedBy : IEquatable<TCreatedBy>
        where TCreatedTime : struct
    {
        /// <summary>
        /// 创建者。
        /// </summary>
        [Display(Name = nameof(CreatedBy), ResourceType = typeof(DataResource))]
        public virtual TCreatedBy? CreatedBy { get; set; }

        /// <summary>
        /// 创建时间。
        /// </summary>
        [Display(Name = nameof(CreatedTime), ResourceType = typeof(DataResource))]
        public virtual TCreatedTime CreatedTime { get; set; }


        /// <summary>
        /// 创建者类型。
        /// </summary>
        [NotMapped]
        public virtual Type CreatedByType
            => typeof(TCreatedBy);

        /// <summary>
        /// 创建时间类型。
        /// </summary>
        [NotMapped]
        public virtual Type CreatedTimeType
            => typeof(TCreatedTime);


        /// <summary>
        /// 转换为创建者。
        /// </summary>
        /// <param name="createdBy">给定的创建者对象。</param>
        /// <param name="paramName">给定的参数名称。</param>
        /// <returns>返回 <typeparamref name="TCreatedBy"/>。</returns>
        public virtual TCreatedBy ToCreatedBy(object? createdBy, string? paramName)
            => createdBy.AsNotNull<TCreatedBy>(paramName);

        /// <summary>
        /// 转换为创建时间。
        /// </summary>
        /// <param name="createdTime">给定的创建时间对象。</param>
        /// <param name="paramName">给定的参数名称。</param>
        /// <returns>返回 <typeparamref name="TCreatedTime"/>。</returns>
        public virtual TCreatedTime ToCreatedTime(object createdTime, string? paramName)
            => createdTime.AsNotNull<TCreatedTime>(paramName);


        /// <summary>
        /// 获取对象创建者。
        /// </summary>
        /// <returns>返回创建者（兼容标识或字符串）。</returns>
        public virtual object? GetObjectCreatedBy()
            => CreatedBy;

        /// <summary>
        /// 异步获取对象创建者。
        /// </summary>
        /// <param name="cancellationToken">给定的 <see cref="CancellationToken"/>（可选）。</param>
        /// <returns>返回一个包含创建者（兼容标识或字符串）的异步操作。</returns>
        public virtual ValueTask<object?> GetObjectCreatedByAsync(CancellationToken cancellationToken = default)
            => cancellationToken.RunValueTask(GetObjectCreatedBy);


        /// <summary>
        /// 获取对象创建时间。
        /// </summary>
        /// <returns>返回日期与时间（兼容 <see cref="DateTime"/> 或 <see cref="DateTimeOffset"/>）。</returns>
        public virtual object GetObjectCreatedTime()
            => CreatedTime;

        /// <summary>
        /// 异步获取对象创建时间。
        /// </summary>
        /// <param name="cancellationToken">给定的 <see cref="CancellationToken"/>（可选）。</param>
        /// <returns>返回一个包含日期与时间（兼容 <see cref="DateTime"/> 或 <see cref="DateTimeOffset"/>）的异步操作。</returns>
        public virtual ValueTask<object> GetObjectCreatedTimeAsync(CancellationToken cancellationToken = default)
            => cancellationToken.RunValueTask(GetObjectCreatedTime);


        /// <summary>
        /// 设置对象创建者。
        /// </summary>
        /// <param name="newCreatedBy">给定的新创建者对象。</param>
        /// <returns>返回创建者（兼容标识或字符串）。</returns>
        public virtual object? SetObjectCreatedBy(object? newCreatedBy)
        {
            CreatedBy = ToCreatedBy(newCreatedBy, nameof(newCreatedBy));
            return newCreatedBy;
        }

        /// <summary>
        /// 异步设置对象创建者。
        /// </summary>
        /// <param name="newCreatedBy">给定的新创建者对象。</param>
        /// <param name="cancellationToken">给定的 <see cref="CancellationToken"/>（可选）。</param>
        /// <returns>返回一个包含创建者（兼容标识或字符串）的异步操作。</returns>
        public virtual ValueTask<object?> SetObjectCreatedByAsync(object? newCreatedBy,
            CancellationToken cancellationToken = default)
        {
            var createdBy = ToCreatedBy(newCreatedBy, nameof(newCreatedBy));

            return cancellationToken.RunValueTask(() =>
            {
                CreatedBy = createdBy;
                return newCreatedBy;
            });
        }


        /// <summary>
        /// 设置对象创建时间。
        /// </summary>
        /// <param name="newCreatedTime">给定的新创建时间对象。</param>
        /// <returns>返回日期与时间（兼容 <see cref="DateTime"/> 或 <see cref="DateTimeOffset"/>）。</returns>
        public virtual object SetObjectCreatedTime(object newCreatedTime)
        {
            CreatedTime = ToCreatedTime(newCreatedTime, nameof(newCreatedTime));
            return newCreatedTime;
        }

        /// <summary>
        /// 异步设置对象创建时间。
        /// </summary>
        /// <param name="newCreatedTime">给定的新创建时间对象。</param>
        /// <param name="cancellationToken">给定的 <see cref="CancellationToken"/>（可选）。</param>
        /// <returns>返回一个包含日期与时间（兼容 <see cref="DateTime"/> 或 <see cref="DateTimeOffset"/>）的异步操作。</returns>
        public virtual ValueTask<object> SetObjectCreatedTimeAsync(object newCreatedTime,
            CancellationToken cancellationToken = default)
        {
            var createdTime = ToCreatedTime(newCreatedTime, nameof(newCreatedTime));

            return cancellationToken.RunValueTask(() =>
            {
                CreatedTime = createdTime;
                return newCreatedTime;
            });
        }


        /// <summary>
        /// 转换为键值对字符串。
        /// </summary>
        /// <returns>返回字符串。</returns>
        public override string ToString()
            => $"{nameof(CreatedBy)}={CreatedBy};{nameof(CreatedTime)}={CreatedTime}";

    }
}
