// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

using System.Collections.Specialized;
using System.Text;

namespace System;

/// <summary>
/// 格式化异常方法扩展类
/// </summary>
public static class FormatExceptionExtensions
{
    /// <summary>
    /// 格式化异常方法
    /// </summary>
    /// <param name="ex">Exception 实例</param>
    /// <param name="nv">Exception 附加信息集合</param>
    /// <returns></returns>
    public static string FormatException(this Exception ex, NameValueCollection? nv = null)
    {
        // Create StringBuilder to maintain publishing information.
        var strInfo = new StringBuilder();
        var additionalInfo = new NameValueCollection();

        #region Load the AdditionalInformation Collection with environment data.
        additionalInfo["TimeStamp"] = DateTime.Now.ToString();
        additionalInfo["MachineName"] = Environment.MachineName;
        additionalInfo["AppDomainName"] = AppDomain.CurrentDomain.FriendlyName;
        if (nv != null) additionalInfo.Add(nv);
        #endregion

        #region Record the contents of the AdditionalInfo collection
        // Record the contents of the AdditionalInfo collection.
        // Record General information.
        var generalSeparator = new string('*', 45);
        var subSeparator = new string('-', 45);

        strInfo.AppendFormat("General Information {0}{1}{0}Additional Info{0}", Environment.NewLine, generalSeparator);
        foreach (string? i in additionalInfo)
        {
            if (!string.IsNullOrEmpty(i)) strInfo.AppendFormat("{1}: {2}{0}", Environment.NewLine, i, additionalInfo.Get(i));
        }
        #endregion

        #region Loop through each exception class in the chain of exception objects
        // Loop through each exception class in the chain of exception objects.
        var currentException = ex;    // Temp variable to hold InnerException object during the loop.
        var intExceptionCount = 1;              // Count variable to track the number of exceptions in the chain.
        if (currentException is AggregateException ex1)
        {
            FormatInnerException(ref intExceptionCount, strInfo, generalSeparator, subSeparator, currentException);
            foreach (var innerEx in ex1.InnerExceptions) FormatInnerExceptionLoop(ref intExceptionCount, strInfo, generalSeparator, subSeparator, innerEx);
        }
        else FormatInnerExceptionLoop(ref intExceptionCount, strInfo, generalSeparator, subSeparator, currentException);
        #endregion

        strInfo.AppendLine();
        return strInfo.ToString();
    }

    private static void FormatInnerExceptionLoop(ref int intExceptionCount, StringBuilder strInfo, string generalSeparator, string subSeparator, Exception currentException)
    {
        Exception? ex = currentException;

        while (ex != null)
        {
            FormatInnerException(ref intExceptionCount, strInfo, generalSeparator, subSeparator, ex);

            // Reset the temp exception object and iterate the counter.
            ex = ex.InnerException;
        };
    }

    private static void FormatInnerException(ref int intExceptionCount, StringBuilder strInfo, string generalSeparator, string subSeparator, Exception currentException)
    {
        // Write title information for the exception object.
        strInfo.AppendFormat(null, "{0}{1}) Exception Information{0}{2}{0}", Environment.NewLine, intExceptionCount.ToString(), subSeparator);
        strInfo.AppendFormat(null, "Exception Type: {1}{0}", Environment.NewLine, currentException.GetType().FullName);

        #region Loop through the public properties of the exception object and record their value
        var aryPublicProperties = currentException.GetType().GetProperties();
        foreach (var p in aryPublicProperties)
        {
            // Do not log information for the InnerException or StackTrace. This information is
            // captured later in the process.
            if (p.Name != "InnerException" && p.Name != "StackTrace")
            {
                var propertyValue = p.GetValue(currentException, null);
                if (p.Name == "AdditionalInformation")
                {
                    // Verify the collection is not null.
                    if (propertyValue != null)
                    {
                        #region Loop through the public properties of the exception object and record their value
                        // Loop through the public properties of the exception object and record their value.
                        // Cast the collection into a local variable.
                        // Check if the collection contains values.
                        if (propertyValue is NameValueCollection currentAdditionalInfo && currentAdditionalInfo.Count > 0)
                        {
                            strInfo.AppendFormat("AdditionalInformation{0}", Environment.NewLine);
                            // Loop through the collection adding the information to the string builder.
                            foreach (string? infoKey in currentAdditionalInfo)
                            {
                                if (!string.IsNullOrEmpty(infoKey)) strInfo.AppendFormat("{1}: {2}{0}", Environment.NewLine, infoKey, currentAdditionalInfo[infoKey]);
                            }
                        }
                        #endregion
                    }
                }
                else
                {
                    strInfo.AppendFormat("{1}: {2}{0}", Environment.NewLine, p.Name, propertyValue);
                }
            }
        }
        #endregion

        #region Record the Exception StackTrace
        // Record the StackTrace with separate label.
        if (currentException.StackTrace != null)
        {
            strInfo.AppendFormat("{0}StackTrace Information{0}{1}{0}", Environment.NewLine, generalSeparator);
            strInfo.AppendFormat("{1}{0}", Environment.NewLine, currentException.StackTrace);
        }
        #endregion

        intExceptionCount++;
    }
}
