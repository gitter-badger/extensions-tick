﻿#region License

/* **************************************************************************************
 * Copyright (c) Librame Pong All rights reserved.
 * 
 * https://github.com/librame
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

namespace Librame.Extensions.Core.Cryptography
{
    /// <summary>
    /// 算法选项。
    /// </summary>
    public class AlgorithmOptions
    {
        /// <summary>
        /// 构造一个 <see cref="AlgorithmOptions"/>。
        /// </summary>
        /// <param name="notifyProperty">给定的 <see cref="INotifyProperty"/>。</param>
        public AlgorithmOptions(INotifyProperty notifyProperty)
        {
            NotifyProperty = notifyProperty;

            Aes = InitializeAesOptions(notifyProperty);
            AesCcm = InitializeAesCcmOptions(notifyProperty);
            AesGcm = InitializeAesGcmOptions(notifyProperty);
        }


        /// <summary>
        /// 通知属性。
        /// </summary>
        protected INotifyProperty NotifyProperty { get; }


        /// <summary>
        /// AES 选项。
        /// </summary>
        public KeyNonceOptions Aes { get; private set; }

        /// <summary>
        /// AES-CCM 选项。
        /// </summary>
        public KeyNonceTagOptions AesCcm { get; private set; }

        /// <summary>
        /// AES-GCM 选项。
        /// </summary>
        public KeyNonceTagOptions AesGcm { get; private set; }


        /// <summary>
        /// 初始化 AES 选项。
        /// </summary>
        /// <param name="notifyProperty">给定的 <see cref="INotifyProperty"/>。</param>
        /// <returns>返回 <see cref="KeyNonceOptions"/>。</returns>
        public static KeyNonceOptions InitializeAesOptions(INotifyProperty notifyProperty)
        {
            var options = new KeyNonceOptions(notifyProperty);

            options.KeyMaxSize = 256;
            options.NonceMaxSize = 128;

            options.Key = RandomExtensions.GenerateByteArray(options.KeyMaxSize);
            options.Nonce = RandomExtensions.GenerateByteArray(options.NonceMaxSize);

            return options;
        }

        /// <summary>
        /// 初始化 AES-CCM 选项。
        /// </summary>
        /// <param name="notifyProperty">给定的 <see cref="INotifyProperty"/>。</param>
        /// <returns>返回 <see cref="KeyNonceTagOptions"/>。</returns>
        public static KeyNonceTagOptions InitializeAesCcmOptions(INotifyProperty notifyProperty)
        {
            var options = new KeyNonceTagOptions(notifyProperty);

            // 参数长度不能是 16、24 或 32 字节（128、192 或 256 位）
            options.KeyMaxSize = 255;
            options.NonceMaxSize = System.Security.Cryptography.AesCcm.NonceByteSizes.MaxSize;
            options.TagMaxSize = System.Security.Cryptography.AesCcm.TagByteSizes.MaxSize;

            options.Key = RandomExtensions.GenerateByteArray(options.KeyMaxSize);
            options.Nonce = RandomExtensions.GenerateByteArray(options.NonceMaxSize);
            options.Tag = RandomExtensions.GenerateByteArray(options.TagMaxSize);

            return options;
        }

        /// <summary>
        /// 初始化 AES-GCM 选项。
        /// </summary>
        /// <param name="notifyProperty">给定的 <see cref="INotifyProperty"/>。</param>
        /// <returns>返回 <see cref="KeyNonceTagOptions"/>。</returns>
        public static KeyNonceTagOptions InitializeAesGcmOptions(INotifyProperty notifyProperty)
        {
            var options = new KeyNonceTagOptions(notifyProperty);

            // 参数长度不能是 16、24 或 32 字节（128、192 或 256 位）
            options.KeyMaxSize = 255;
            options.NonceMaxSize = System.Security.Cryptography.AesGcm.NonceByteSizes.MaxSize;
            options.TagMaxSize = System.Security.Cryptography.AesGcm.TagByteSizes.MaxSize;

            options.Key = RandomExtensions.GenerateByteArray(options.KeyMaxSize);
            options.Nonce = RandomExtensions.GenerateByteArray(options.NonceMaxSize);
            options.Tag = RandomExtensions.GenerateByteArray(options.TagMaxSize);

            return options;
        }

    }
}
