using System;

namespace Datapecker.Agent
{
    internal interface IInternalLogger : IDisposable
    {
        void Error(ExceptionType type, string message);
        void Error(ExceptionType type, string message, params object[] parameters);
        void Exception(ExceptionType type, Exception exception);
        void Exception(ExceptionType type, Exception exception, string message);
        void Exception(ExceptionType type, Exception exception, string message, params object[] parameters);
    }
}