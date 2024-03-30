using AnalyticsServer.Models;
using Assets.Scripts.Data.Constants;
using Assets.Scripts.DataAccess;
using System;
using System.Threading;

public class LogManager
{
    private const int MaxAnalyticsInSession = 10;
    private const int MaxLogsInSession = 10;
    private const int MaxExceptionsInSession = 10;
    private const string ApplicationVersion = "2024-03-25_1";
    private const bool LogsEnabled = true;
    private static readonly object _lock = new object();
    private static string _sessionIdenfier;
    private static int analyticsSendedInSession = 0;
    private static int logsSendedInSession = 0;
    private static int exceptionsSendedInSession = 0;

    public static void SendLog(WebServiceCaller<LogDto, bool> logSender, string content)
    {
        if (logsSendedInSession < MaxLogsInSession)
        {
            Interlocked.Increment(ref logsSendedInSession);
            LogDto logDto = new LogDto()
            {
                Application = LogDto.ApplicationEnum.HeartsOfInk,
                ApplicationVersion = ApplicationVersion,
                Content = content,
                SessionIdentifier = GetSessionId(),
                UserName = "Pending implementation"
            };

            if (LogsEnabled)
            {
                logSender.GenericWebServiceCaller(ApiConfig.LoggingServerUrl, Method.POST, "Log", logDto);
            }
        }
    }

    public static void SendException(WebServiceCaller<LogExceptionDto, bool> errorSender, Exception ex)
    {
        if (exceptionsSendedInSession < MaxExceptionsInSession)
        {
            Interlocked.Increment(ref exceptionsSendedInSession);

            if (LogsEnabled)
            {
                SendException(errorSender, ex, string.Empty);
            }
        }
    }

    public static void LogAnalytic(WebServiceCaller<LogAnalyticsDto, bool> analyticSender, string tag, string log)
    {
        if (analyticsSendedInSession < MaxAnalyticsInSession)
        {
            Interlocked.Increment(ref analyticsSendedInSession);
            LogAnalyticsDto logAnalyticsDto = new LogAnalyticsDto()
            {
                AnalyticTag = tag,
                Content = log,
                Application = LogDto.ApplicationEnum.HeartsOfInk,
                SessionIdentifier = GetSessionId(),
                ApplicationVersion = ApplicationVersion,
                UserName = "Pending implementation"
            };

            if (LogsEnabled)
            {
                analyticSender.GenericWebServiceCaller(ApiConfig.LoggingServerUrl, Method.POST, "Analytics", logAnalyticsDto);
            }
        }
    }

    public static void SendException(WebServiceCaller<LogExceptionDto, bool> errorSender, Exception ex, string addittionalInfo)
    {
        LogExceptionDto logExceptionDto = new LogExceptionDto()
        {
            Exception = ex,
            Content = addittionalInfo,
            Application = LogDto.ApplicationEnum.HeartsOfInk,
            SessionIdentifier = GetSessionId(),
            ApplicationVersion = ApplicationVersion,
            UserName = "Pending implementation"
        };

        if (LogsEnabled)
        {
            errorSender.GenericWebServiceCaller(ApiConfig.LoggingServerUrl, Method.POST, "Exception", logExceptionDto);
        }
    }

    private static string GetSessionId()
    {
        lock (_lock)
        {
            if (string.IsNullOrEmpty(_sessionIdenfier))
            {
                _sessionIdenfier = DateTime.Now.ToString();
            }
        }

        return _sessionIdenfier;
    } 
}
