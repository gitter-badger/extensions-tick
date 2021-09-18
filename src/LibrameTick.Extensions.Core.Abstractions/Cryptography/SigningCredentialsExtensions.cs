﻿#region License

/* **************************************************************************************
 * Copyright (c) Librame Pong All rights reserved.
 * 
 * https://github.com/librame
 * 
 * You must not remove this notice, or any other, from this software.
 * **************************************************************************************/

#endregion

namespace Librame.Extensions.Core.Cryptography;

/// <summary>
/// <see cref="SigningCredentials"/> 静态扩展。
/// </summary>
public static class SigningCredentialsExtensions
{

    /// <summary>
    /// 当作证书。
    /// </summary>
    /// <param name="credentials">给定的 <see cref="SigningCredentials"/>。</param>
    /// <returns>返回 <see cref="X509Certificate2"/>。</returns>
    public static X509Certificate2 AsCertificate(this SigningCredentials credentials)
    {
        if (credentials.Key is X509SecurityKey x509Key)
            return x509Key.Certificate;

        throw new NotSupportedException($"Not supported signing credentials.");
    }

    /// <summary>
    /// 当作 RSA。
    /// </summary>
    /// <param name="credentials">给定的 <see cref="SigningCredentials"/>。</param>
    /// <returns>返回 <see cref="RSA"/>。</returns>
    public static RSA AsRsa(this SigningCredentials credentials)
    {
        if (credentials.Key is X509SecurityKey x509Key)
            return (RSA)x509Key.PrivateKey;

        if (credentials.Key is RsaSecurityKey rsaKey)
        {
            if (rsaKey.Rsa is null)
            {
                var rsa = RSA.Create();
                rsa.ImportParameters(rsaKey.Parameters);

                return rsa;
            }

            return rsaKey.Rsa;
        }

        throw new NotSupportedException($"Not supported signing credentials.");
    }


    /// <summary>
    /// 从临时密钥文件加载或创建签名证书。
    /// </summary>
    /// <param name="credentialsFile">给定的证书文件。</param>
    /// <returns>返回 <see cref="SigningCredentials"/>。</returns>
    public static SigningCredentials LoadOrCreateCredentialsFromFile(this string credentialsFile)
    {
        if (string.IsNullOrEmpty(credentialsFile))
            throw new ArgumentNullException(nameof(credentialsFile));

        // 临时密钥文件通常存放在应用根目录
        var tempRsaKey = TemporaryRsaKey.LoadOrCreateFile(credentialsFile.SetBasePath());
        return new SigningCredentials(tempRsaKey.AsRsaKey(), SecurityAlgorithms.RsaSha256);
    }


    /// <summary>
    /// 验证签名证书。
    /// </summary>
    /// <param name="credentials">给定的 <see cref="SigningCredentials"/>。</param>
    /// <returns>返回 <see cref="SigningCredentials"/>。</returns>
    public static SigningCredentials Verify(this SigningCredentials credentials)
    {
        if (!(credentials.Key is AsymmetricSecurityKey
            || credentials.Key is JsonWebKey && ((JsonWebKey)credentials.Key).HasPrivateKey))
        {
            throw new InvalidOperationException("Invalid signing key.");
        }

        return credentials;
    }

}
