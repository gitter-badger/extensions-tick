﻿#region License

/* **************************************************************************************
 * Copyright (c) Librame Pong All rights reserved.
 * 
 * https://github.com/librame
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Internal;
using Microsoft.Extensions.FileProviders.Physical;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Librame.Extensions.Core.Storage
{
    /// <summary>
    /// 定义实现 <see cref="IStorageDirectoryContents"/> 的物理存储目录内容集合。
    /// </summary>
    public class PhysicalStorageDirectoryContents : IStorageDirectoryContents
    {
        private readonly IEnumerable<IStorageFileInfo> _entries;
        private readonly string? _directory;
        private readonly ExclusionFilters _filters;


        /// <summary>
        /// 构造一个 <see cref="PhysicalStorageDirectoryContents"/>。
        /// </summary>
        /// <param name="directory">给定的目录。</param>
        public PhysicalStorageDirectoryContents(string directory)
            : this(directory, ExclusionFilters.Sensitive)
        {
        }

        /// <summary>
        /// 构造一个 <see cref="PhysicalStorageDirectoryContents"/>。
        /// </summary>
        /// <param name="directory">给定的目录。</param>
        /// <param name="filters">指定排除哪些文件或目录。</param>
        public PhysicalStorageDirectoryContents(string directory, ExclusionFilters filters)
        {
            Exists = Directory.Exists(directory);

            _entries = directory.EnumerateStorageFileInfos(filters);
            _directory = directory;
            _filters = filters;
        }

        /// <summary>
        /// 构造一个 <see cref="PhysicalStorageDirectoryContents"/>。
        /// </summary>
        /// <param name="contents">给定的 <see cref="IDirectoryContents"/>。</param>
        public PhysicalStorageDirectoryContents(IDirectoryContents contents)
        {
            Exists = (contents is PhysicalDirectoryContents physical) && physical.Exists;

            _entries = contents.Select<IFileInfo, IStorageFileInfo>(info =>
            {
                if (info is PhysicalFileInfo file)
                {
                    return new PhysicalStorageFileInfo(file);
                }
                else if (info is PhysicalDirectoryInfo dir)
                {
                    return new PhysicalStorageDirectoryInfo(dir);
                }

                // shouldn't happen unless BCL introduces new implementation of base type
                throw new InvalidOperationException($"Unexpected type of {nameof(FileSystemInfo)}.");
            });
        }


        /// <summary>
        /// 是否存在。
        /// </summary>
        public bool Exists { get; init; }


        /// <summary>
        /// 获取枚举器。
        /// </summary>
        /// <returns>返回 <see cref="IEnumerable{IStorageFileInfo}"/>。</returns>
        public IEnumerator<IStorageFileInfo> GetEnumerator()
            => _entries.GetEnumerator();

        /// <summary>
        /// 获取枚举器。
        /// </summary>
        /// <returns>返回 <see cref="IFileInfo"/>。</returns>
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        IEnumerator<IFileInfo> IEnumerable<IFileInfo>.GetEnumerator()
            => GetEnumerator();

    }
}
