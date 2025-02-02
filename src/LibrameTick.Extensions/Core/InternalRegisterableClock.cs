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

class InternalRegisterableClock : IRegisterableClock
{
    // 定义时钟回流偏移量
    private readonly TimeSpan _refluxOffsetMilliseconds;


    public InternalRegisterableClock()
    {
        // 默认为 100
        _refluxOffsetMilliseconds = TimeSpan.FromMilliseconds(100);
    }


    public DateTime GetNow(DateTime? timestamp = null)
    {
        var localNow = DateTime.Now;

        if (timestamp.HasValue && timestamp.Value > localNow)
        {
            // 计算时间差并添加补偿以解决时钟回流
            var offset = (timestamp.Value - localNow).Add(_refluxOffsetMilliseconds);
            localNow.Add(offset);
        }

        return localNow;
    }

    public Task<DateTime> GetNowAsync(DateTime? timestamp = null,
        CancellationToken cancellationToken = default)
        => cancellationToken.RunTask(() => GetNow(timestamp));


    public DateTimeOffset GetUtcNow(DateTimeOffset? timestamp = null)
    {
        var localNow = DateTimeOffset.UtcNow;

        if (timestamp.HasValue && timestamp.Value > localNow)
        {
            // 计算时间差并添加补偿以解决时钟回流
            var offset = (timestamp.Value - localNow).Add(_refluxOffsetMilliseconds);
            localNow.Add(offset);
        }

        return localNow;
    }

    public Task<DateTimeOffset> GetUtcNowAsync(DateTimeOffset? timestamp = null,
        CancellationToken cancellationToken = default)
        => cancellationToken.RunTask(() => GetUtcNow(timestamp));

}
