﻿#region License

/* **************************************************************************************
 * Copyright (c) Librame Pong All rights reserved.
 * 
 * https://github.com/librame
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace Librame.Extensions.Data
{
    using Resources;

    /// <summary>
    /// 抽象状态。
    /// </summary>
    /// <typeparam name="TStatus">指定的状态类型（兼容不支持枚举类型的实体框架）。</typeparam>
    public abstract class AbstractState<TStatus> : IState<TStatus>
        where TStatus : struct
    {
        /// <summary>
        /// 状态。
        /// </summary>
        [Display(Name = nameof(Status), GroupName = "DataGroup", ResourceType = typeof(AbstractEntityResource))]
        public virtual TStatus Status { get; set; }


        /// <summary>
        /// 状态类型。
        /// </summary>
        public virtual Type StatusType
            => typeof(TStatus);


        /// <summary>
        /// 获取对象状态。
        /// </summary>
        /// <returns>返回状态（兼容不支持枚举类型的实体框架）。</returns>
        public virtual object GetObjectStatus()
            => Status;

        /// <summary>
        /// 异步获取对象状态。
        /// </summary>
        /// <param name="cancellationToken">给定的 <see cref="CancellationToken"/>（可选）。</param>
        /// <returns>返回一个包含状态（兼容不支持枚举类型的实体框架）的异步操作。</returns>
        public virtual ValueTask<object> GetObjectStatusAsync(CancellationToken cancellationToken = default)
            => cancellationToken.RunOrCancelValueAsync(() => (object)Status);


        /// <summary>
        /// 设置对象状态。
        /// </summary>
        /// <param name="newStatus">给定的新状态对象。</param>
        /// <returns>返回状态（兼容不支持枚举类型的实体框架）。</returns>
        public virtual object SetObjectStatus(object newStatus)
        {
            Status = newStatus.CastTo<object, TStatus>(nameof(newStatus));
            return newStatus;
        }

        /// <summary>
        /// 异步设置对象状态。
        /// </summary>
        /// <param name="newStatus">给定的新状态对象。</param>
        /// <param name="cancellationToken">给定的 <see cref="CancellationToken"/>（可选）。</param>
        /// <returns>返回一个包含状态（兼容不支持枚举类型的实体框架）的异步操作。</returns>
        public virtual ValueTask<object> SetObjectStatusAsync(object newStatus,
            CancellationToken cancellationToken = default)
        {
            var realNewStatus = newStatus.CastTo<object, TStatus>(nameof(newStatus));

            return cancellationToken.RunOrCancelValueAsync(() =>
            {
                Status = realNewStatus;
                return newStatus;
            });
        }

    }
}
