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
/// 定义实现 <see cref="IOptionsNotifier"/> 的签名证书选项。
/// </summary>
public class SigningCredentialsOptions : AbstractOptionsNotifier
{
    /// <summary>
    /// 构造一个独立属性通知器的 <see cref="SigningCredentialsOptions"/>（此构造函数适用于独立使用 <see cref="SigningCredentialsOptions"/> 的情况）。
    /// </summary>
    /// <param name="sourceAliase">给定的源别名（独立属性通知器必须命名实例）。</param>
    public SigningCredentialsOptions(string sourceAliase)
        : base(sourceAliase)
    {
    }

    /// <summary>
    /// 构造一个 <see cref="SigningCredentialsOptions"/>。
    /// </summary>
    /// <param name="parentNotifier">给定的父级 <see cref="IPropertyNotifier"/>。</param>
    /// <param name="sourceAliase">给定的源别名（可选）。</param>
    public SigningCredentialsOptions(IPropertyNotifier parentNotifier, string? sourceAliase = null)
        : base(parentNotifier, sourceAliase)
    {
    }


    /// <summary>
    /// 证书文件（不包含路径，通常位于应用程序根目录；默认兼容 IdentityServer4 生成的临时密钥文件）。
    /// </summary>
    public string CredentialsFile
    {
        get => Notifier.GetOrAdd(nameof(CredentialsFile), "tempkey.rsa");
        set => Notifier.AddOrUpdate(nameof(CredentialsFile), value);
    }

    /// <summary>
    /// 签名证书。
    /// </summary>
    [JsonIgnore]
    public SigningCredentials Credentials
    {
        get => Notifier.GetOrAdd(nameof(Credentials), CredentialsFile.LoadOrCreateCredentialsFromFile());
        set => Notifier.AddOrUpdate(nameof(Credentials), value);
    }


    /// <summary>
    /// 设置签名证书方法。
    /// </summary>
    /// <param name="credentialsFunc">给定的签名证书方法。</param>
    /// <returns>返回签名证书方法。</returns>
    public Func<SigningCredentials> SetCredentialsFunc(Func<SigningCredentials> credentialsFunc)
    {
        Notifier.AddOrUpdate(nameof(Credentials), credentialsFunc);
        return credentialsFunc;
    }


    /// <summary>
    /// 获取哈希码。
    /// </summary>
    /// <returns>返回 32 位整数。</returns>
    public override int GetHashCode()
        => ToString().GetHashCode();

    /// <summary>
    /// 转换为字符串。
    /// </summary>
    /// <returns>返回字符串。</returns>
    public override string ToString()
        => $"{nameof(CredentialsFile)}={CredentialsFile},{nameof(Credentials.Key.KeySize)}={Credentials.Key.KeySize},{nameof(Credentials.Key.KeyId)}={Credentials.Key.KeyId}";

}
