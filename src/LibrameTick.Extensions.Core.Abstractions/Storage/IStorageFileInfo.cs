﻿#region License

/* **************************************************************************************
 * Copyright (c) Librame Pong All rights reserved.
 * 
 * https://github.com/librame
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

namespace Librame.Extensions.Core.Storage;

/// <summary>
/// 定义存储文件信息。
/// </summary>
public interface IStorageFileInfo : IFileInfo
{
    /// <summary>
    /// 创建写入流。
    /// </summary>
    /// <returns>返回 <see cref="Stream"/>。</returns>
    Stream CreateWriteStream();
}
