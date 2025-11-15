// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;

namespace Longbow.Logging;

/// <summary>
/// 日志配置辅助类
/// </summary>
/// <typeparam name="TOptions"></typeparam>
/// <typeparam name="TProvider"></typeparam>
/// <remarks>
/// 默认构造函数
/// </remarks>
/// <param name="providerConfiguration"></param>
public class LoggerProviderConfigureOptions<TOptions, TProvider>(ILoggerProviderConfiguration<TProvider> providerConfiguration) : ConfigureFromConfigurationOptions<TOptions>(providerConfiguration.Configuration) where TOptions : class
{
}
