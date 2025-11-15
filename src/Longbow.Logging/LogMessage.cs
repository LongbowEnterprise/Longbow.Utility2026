// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

namespace Longbow.Logging;

/// <summary>
/// 
/// </summary>
struct LogMessage
{
    /// <summary>
    /// 时间戳
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// 消息体
    /// </summary>
    public string Message { get; set; }
}
