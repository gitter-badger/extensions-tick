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
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Librame.Extensions.Data
{
    /// <summary>
    /// 锁定期静态扩展。
    /// </summary>
    public static class LockoutExtensions
    {
        /// <summary>
        /// 设置锁定期结束时间。
        /// </summary>
        /// <typeparam name="TLockoutTime">指定的锁定期时间类型（提供对 <see cref="DateTime"/> 或 <see cref="DateTimeOffset"/> 的支持）。</typeparam>
        /// <param name="lockout">给定的 <see cref="ILockout{TLockoutTime}"/>。</param>
        /// <param name="newLockoutFactory">给定的新 <typeparamref name="TLockoutTime"/> 工厂方法。</param>
        /// <returns>返回一个包含锁定期结束时间（提供对 <see cref="DateTime"/> 或 <see cref="DateTimeOffset"/> 的支持）的异步操作。</returns>
        [SuppressMessage("Design", "CA1062:验证公共方法的参数", Justification = "<挂起>")]
        public static TLockoutTime? SetLockoutEnd<TLockoutTime>(this ILockout<TLockoutTime> lockout,
            Func<TLockoutTime?, TLockoutTime?> newLockoutFactory)
            where TLockoutTime : struct
        {
            lockout.NotNull(nameof(lockout));
            newLockoutFactory.NotNull(nameof(newLockoutFactory));

            return lockout.LockoutEnd = newLockoutFactory.Invoke(lockout.LockoutEnd);
        }

        /// <summary>
        /// 异步设置锁定期结束时间。
        /// </summary>
        /// <typeparam name="TLockoutTime">指定的锁定期时间类型（提供对 <see cref="DateTime"/> 或 <see cref="DateTimeOffset"/> 的支持）。</typeparam>
        /// <param name="lockout">给定的 <see cref="ILockout{TLockoutTime}"/>。</param>
        /// <param name="newLockoutFactory">给定的新 <typeparamref name="TLockoutTime"/> 工厂方法。</param>
        /// <param name="cancellationToken">给定的 <see cref="CancellationToken"/>（可选）。</param>
        /// <returns>返回一个包含锁定期结束时间（提供对 <see cref="DateTime"/> 或 <see cref="DateTimeOffset"/> 的支持）的异步操作。</returns>
        public static ValueTask<TLockoutTime?> SetLockoutEndAsync<TLockoutTime>(this ILockout<TLockoutTime> lockout,
            Func<TLockoutTime?, TLockoutTime?> newLockoutFactory, CancellationToken cancellationToken = default)
            where TLockoutTime : struct
        {
            lockout.NotNull(nameof(lockout));
            newLockoutFactory.NotNull(nameof(newLockoutFactory));

            return cancellationToken.RunOrCancelValueAsync(()
                => lockout.LockoutEnd = newLockoutFactory.Invoke(lockout.LockoutEnd));
        }


        /// <summary>
        /// 设置对象锁定期结束时间。
        /// </summary>
        /// <param name="lockout">给定的 <see cref="IObjectLockout"/>。</param>
        /// <param name="newLockoutEndFactory">给定的新对象锁定期结束时间工厂方法。</param>
        /// <returns>返回锁定期结束时间（提供对 <see cref="DateTime"/> 或 <see cref="DateTimeOffset"/> 的支持）。</returns>
        [SuppressMessage("Design", "CA1062:验证公共方法的参数", Justification = "<挂起>")]
        public static object SetObjectLockoutEnd(this IObjectLockout lockout,
            Func<object, object> newLockoutEndFactory)
        {
            lockout.NotNull(nameof(lockout));
            newLockoutEndFactory.NotNull(nameof(newLockoutEndFactory));

            var newLockoutEnd = lockout.GetObjectLockoutEnd();
            return lockout.SetObjectLockoutEnd(newLockoutEndFactory.Invoke(newLockoutEnd));
        }

        /// <summary>
        /// 异步设置对象锁定期结束时间。
        /// </summary>
        /// <param name="lockout">给定的 <see cref="IObjectLockout"/>。</param>
        /// <param name="newLockoutEndFactory">给定的新对象锁定期结束时间工厂方法。</param>
        /// <param name="cancellationToken">给定的 <see cref="CancellationToken"/>（可选）。</param>
        /// <returns>返回一个包含锁定期结束时间（提供对 <see cref="DateTime"/> 或 <see cref="DateTimeOffset"/> 的支持）的异步操作。</returns>
        [SuppressMessage("Design", "CA1062:验证公共方法的参数", Justification = "<挂起>")]
        public static async ValueTask<object> SetObjectLockoutEndAsync(this IObjectLockout lockout,
            Func<object, object> newLockoutEndFactory, CancellationToken cancellationToken = default)
        {
            lockout.NotNull(nameof(lockout));
            newLockoutEndFactory.NotNull(nameof(newLockoutEndFactory));

            var newLockoutEnd = await lockout.GetObjectLockoutEndAsync(cancellationToken).ConfigureAwait();
            return await lockout.SetObjectLockoutEndAsync(newLockoutEndFactory.Invoke(newLockoutEnd), cancellationToken)
                .ConfigureAwait();
        }

    }
}
