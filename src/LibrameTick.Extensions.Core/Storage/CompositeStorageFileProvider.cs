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
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Librame.Extensions.Core.Storage
{
    sealed class CompositeStorageFileProvider : IStorageFileProvider
    {
        private readonly IStorageFileProvider[] _providers;


        public CompositeStorageFileProvider(IEnumerable<IStorageFileProvider> providers)
        {
            _providers = providers.ToArray();
        }


        #region Private

        private void ChainingProvidersByException(Action<IStorageFileProvider> action, int accessorIndex = 0)
        {
            try
            {
                action.Invoke(_providers[accessorIndex]);
            }
            catch (Exception)
            {
                if (accessorIndex < _providers.Length - 1)
                    ChainingProvidersByException(action, accessorIndex++); // 链式处理

                throw; // 所有访问器均出错时则抛出异常
            }
        }

        private TResult ChainingProvidersByException<TResult>(Func<IStorageFileProvider, TResult> func, int accessorIndex = 0)
        {
            try
            {
                return func.Invoke(_providers[accessorIndex]);
            }
            catch (Exception)
            {
                if (accessorIndex < _providers.Length - 1)
                    return ChainingProvidersByException(func, accessorIndex++); // 链式处理

                throw; // 所有访问器均出错时则抛出异常
            }
        }

        #endregion


        public IStorageDirectoryContents GetDirectoryContents(string subpath)
            => ChainingProvidersByException(p => p.GetDirectoryContents(subpath));

        public IStorageFileInfo GetFileInfo(string subpath)
            => ChainingProvidersByException(p => p.GetFileInfo(subpath));

        public IChangeToken Watch(string filter)
            => ChainingProvidersByException(p => p.Watch(filter));

        IDirectoryContents IFileProvider.GetDirectoryContents(string subpath)
            => GetDirectoryContents(subpath);

        IFileInfo IFileProvider.GetFileInfo(string subpath)
            => GetFileInfo(subpath);

    }
}
