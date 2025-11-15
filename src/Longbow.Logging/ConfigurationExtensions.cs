// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

using System.Runtime.InteropServices;

namespace Microsoft.Extensions.Configuration;

/// <summary>
/// IConfiguration 扩展类
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// 获得 环境变量中的 OS 属性值 近 Windows 平台有效
    /// </summary>
    /// <returns></returns>
    public static string GetOS()
    {
        var os = "";
        if (string.IsNullOrEmpty(os))
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                os = RuntimeInformation.OSDescription;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                os = "OSX";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            {
                os = "FreeBSD";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                os = "Linux";
            }
            else
            {
                os = "Unknown";
            }
        }
        return os;
    }

    /// <summary>
    /// 获得 环境变量中的 UserName 属性值
    /// </summary>
    /// <param name="config"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static string GetUserName(this IConfiguration config, string defaultValue = "")
    {
        var userName = config.GetValue("USERNAME", "");

        // Mac CentOs 系统
        if (string.IsNullOrEmpty(userName))
        {
            userName = config.GetValue("LOGNAME", defaultValue);
        }

        return userName!;
    }

    /// <summary>
    /// 获得 环境变量中的 ASPNETCORE_ENVIRONMENT 属性值
    /// </summary>
    /// <param name="config"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static string GetEnvironmentName(this IConfiguration config, string defaultValue = "") => config.GetValue("ASPNETCORE_ENVIRONMENT", defaultValue)!;

    /// <summary>
    /// 获得 环境变量中的 ASPNETCORE_IIS_PHYSICAL_PATH 属性值
    /// </summary>
    /// <param name="config"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static string GetIISPath(this IConfiguration config, string defaultValue = "") => config.GetValue("ASPNETCORE_IIS_PHYSICAL_PATH", defaultValue)!;

    /// <summary>
    /// 获得 环境变量中的 VisualStudioEdition 属性值
    /// </summary>
    /// <param name="config"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static string GetVisualStudioVersion(this IConfiguration config, string defaultValue = "")
    {
        var edition = config.GetValue("VisualStudioEdition", "");
        var version = config.GetValue("VisualStudioVersion", "");

        var ret = $"{edition} {version}";
        if (ret == " ")
        {
            ret = defaultValue;
        }

        return ret;
    }
}
