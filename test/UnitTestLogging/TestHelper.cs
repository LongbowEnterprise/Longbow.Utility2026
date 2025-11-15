// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Longbow.Logging;

public static class TestHelper
{
    /// <summary>
    /// 
    /// </summary>
    public static void TriggerAppSettingsFileChange(IConfiguration configuration, Action changeCallback)
    {
        ChangeToken.OnChange(() => configuration.GetReloadToken(), () => changeCallback.Invoke());
        var root = (ConfigurationRoot)configuration;
        root.Reload();
    }
}
