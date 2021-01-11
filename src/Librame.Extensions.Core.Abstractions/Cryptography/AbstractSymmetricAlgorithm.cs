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
using System.Security.Cryptography;

namespace Librame.Extensions.Core.Cryptography
{
    /// <summary>
    /// 抽象对称算法（抽象实现 <see cref="ISymmetricAlgorithm"/>）。
    /// </summary>
    public abstract class AbstractSymmetricAlgorithm : ISymmetricAlgorithm
    {
        /// <summary>
        /// 构造一个 <see cref="AbstractAlgorithmParameterGenerator"/>。
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="parameterGenerator"/> 为空。
        /// </exception>
        /// <param name="parameterGenerator">给定的 <see cref="IAlgorithmParameterGenerator"/>。</param>
        /// <param name="extensionBuilder">给定的 <see cref="IExtensionBuilder"/>。</param>
        public AbstractSymmetricAlgorithm(IAlgorithmParameterGenerator parameterGenerator,
            IExtensionBuilder extensionBuilder)
        {
            ParameterGenerator = parameterGenerator.NotNull(nameof(parameterGenerator));
            ExtensionBuilder = extensionBuilder.NotNull(nameof(extensionBuilder));
        }


        /// <summary>
        /// 参数生成器。
        /// </summary>
        public IAlgorithmParameterGenerator ParameterGenerator { get; private set; }

        /// <summary>
        /// 扩展构建器。
        /// </summary>
        public IExtensionBuilder ExtensionBuilder { get; private set; }


        #region AES

        /// <summary>
        /// 加密 AES。
        /// </summary>
        /// <param name="buffer">给定待加密的字节数组。</param>
        /// <param name="options">给定的 <see cref="KeyNonceOptions"/>（可选；默认使用选项配置）。</param>
        /// <returns>返回经过加密的字节数组。</returns>
        public virtual byte[] EncryptAes(byte[] buffer, KeyNonceOptions? options = null)
        {
            EnsureAesOptions(options);

            return AlgorithmUtility.RunAes(aes =>
            {
                var transform = aes.CreateEncryptor();
                return transform.TransformFinalBlock(buffer, 0, buffer.Length);
            });
        }

        /// <summary>
        /// 解密 AES。
        /// </summary>
        /// <param name="buffer">给定的字节数组。</param>
        /// <param name="options">给定的 <see cref="KeyNonceOptions"/>（可选；默认使用选项配置）。</param>
        /// <returns>返回经过解密的字节数组。</returns>
        public virtual byte[] DecryptAes(byte[] buffer, KeyNonceOptions? options = null)
        {
            EnsureAesOptions(options);

            return AlgorithmUtility.RunAes(aes =>
            {
                var transform = aes.CreateDecryptor();
                return transform.TransformFinalBlock(buffer, 0, buffer.Length);
            });
        }

        /// <summary>
        /// 确保 AES 选项。
        /// </summary>
        /// <param name="options">给定的 <see cref="KeyNonceOptions"/>。</param>
        protected virtual void EnsureAesOptions(KeyNonceOptions? options)
        {
            if (options is null)
                options = new KeyNonceOptions(ExtensionBuilder);

            options.Key = AlgorithmUtility.RunAes(aes =>
            {
                var newKey = ParameterGenerator.GenerateKey(aes.KeySize);
                return aes.Key = newKey;
            });

            options.Nonce = AlgorithmUtility.RunAes(aes =>
            {
                var newNonce = ParameterGenerator.GenerateNonce(aes.BlockSize, aes.Key);
                return aes.IV = newNonce;
            });
        }

        #endregion


        #region AES-CCM

        /// <summary>
        /// 加密 AES-CCM。
        /// </summary>
        /// <param name="buffer">给定待加密的字节数组。</param>
        /// <param name="options">给定的 <see cref="KeyNonceTagOptions"/>（可选；默认使用选项配置）。</param>
        /// <returns>返回经过加密的字节数组。</returns>
        public virtual byte[] EncryptAesCcm(byte[] buffer, KeyNonceTagOptions? options = null)
        {
            EnsureAesCcmOptions(options);

            var ciphertext = new byte[buffer.Length];

#pragma warning disable CS8602 // 解引用可能出现空引用。
#pragma warning disable CS8604 // 可能的 null 引用参数。
            var aes = new AesCcm(options.Key);
            aes.Encrypt(options.Nonce, buffer, ciphertext, options.Tag);
#pragma warning restore CS8604 // 可能的 null 引用参数。
#pragma warning restore CS8602 // 解引用可能出现空引用。

            return ciphertext;
        }

        /// <summary>
        /// 解密 AES-CCM。
        /// </summary>
        /// <param name="buffer">给定的字节数组。</param>
        /// <param name="options">给定的 <see cref="KeyNonceTagOptions"/>（可选；默认使用选项配置）。</param>
        /// <returns>返回经过解密的字节数组。</returns>
        public virtual byte[] DecryptAesCcm(byte[] buffer, KeyNonceTagOptions? options = null)
        {
            EnsureAesCcmOptions(options);

            var plaintext = new byte[buffer.Length];

#pragma warning disable CS8602 // 解引用可能出现空引用。
#pragma warning disable CS8604 // 可能的 null 引用参数。
            var aes = new AesCcm(options.Key);
            aes.Decrypt(options.Nonce, buffer, options.Tag, plaintext);
#pragma warning restore CS8604 // 可能的 null 引用参数。
#pragma warning restore CS8602 // 解引用可能出现空引用。

            return plaintext;
        }

        /// <summary>
        /// 确保 AES-CCM 选项。
        /// </summary>
        /// <param name="options">给定的 <see cref="KeyNonceTagOptions"/>。</param>
        protected virtual void EnsureAesCcmOptions(KeyNonceTagOptions? options)
        {
            if (options is null)
                options = new KeyNonceTagOptions(ExtensionBuilder);

            // Use AES KeySize
            options.Key = AlgorithmUtility.RunAes(aes =>
            {
                var newKey = ParameterGenerator.GenerateKey(aes.KeySize);
                return aes.Key = newKey;
            });

            options.Nonce = ParameterGenerator.GenerateNonce(AesCcm.NonceByteSizes.MaxSize, options.Key);
            options.Tag = ParameterGenerator.GenerateTag(AesCcm.TagByteSizes.MaxSize, options.Key);
        }

        #endregion


        #region AES-GCM

        /// <summary>
        /// 加密 AES-GCM。
        /// </summary>
        /// <param name="buffer">给定待加密的字节数组。</param>
        /// <param name="options">给定的 <see cref="KeyNonceTagOptions"/>（可选；默认使用选项配置）。</param>
        /// <returns>返回经过加密的字节数组。</returns>
        public virtual byte[] EncryptAesGcm(byte[] buffer, KeyNonceTagOptions? options = null)
        {
            EnsureAesGcmOptions(options);

            var ciphertext = new byte[buffer.Length];

#pragma warning disable CS8602 // 解引用可能出现空引用。
#pragma warning disable CS8604 // 可能的 null 引用参数。
            var aes = new AesGcm(options.Key);
            aes.Encrypt(options.Nonce, buffer, ciphertext, options.Tag);
#pragma warning restore CS8604 // 可能的 null 引用参数。
#pragma warning restore CS8602 // 解引用可能出现空引用。

            return ciphertext;
        }

        /// <summary>
        /// 解密 AES-GCM。
        /// </summary>
        /// <param name="buffer">给定的字节数组。</param>
        /// <param name="options">给定的 <see cref="KeyNonceTagOptions"/>（可选；默认使用选项配置）。</param>
        /// <returns>返回经过解密的字节数组。</returns>
        public virtual byte[] DecryptAesGcm(byte[] buffer, KeyNonceTagOptions? options = null)
        {
            EnsureAesGcmOptions(options);

            var plaintext = new byte[buffer.Length];

#pragma warning disable CS8602 // 解引用可能出现空引用。
#pragma warning disable CS8604 // 可能的 null 引用参数。
            var aes = new AesGcm(options.Key);
            aes.Decrypt(options.Nonce, buffer, options.Tag, plaintext);
#pragma warning restore CS8604 // 可能的 null 引用参数。
#pragma warning restore CS8602 // 解引用可能出现空引用。

            return plaintext;
        }

        /// <summary>
        /// 确保 AES-GCM 选项。
        /// </summary>
        /// <param name="options">给定的 <see cref="KeyNonceTagOptions"/>。</param>
        protected virtual void EnsureAesGcmOptions(KeyNonceTagOptions? options)
        {
            if (options is null)
                options = new KeyNonceTagOptions(ExtensionBuilder);

            // Use AES KeySize
            options.Key = AlgorithmUtility.RunAes(aes =>
            {
                var newKey = ParameterGenerator.GenerateKey(aes.KeySize);
                return aes.Key = newKey;
            });

            options.Nonce = ParameterGenerator.GenerateNonce(AesGcm.NonceByteSizes.MaxSize, options.Key);
            options.Tag = ParameterGenerator.GenerateTag(AesGcm.TagByteSizes.MaxSize, options.Key);
        }

        #endregion

    }
}
