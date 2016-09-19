using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapecker.Agent
{
    internal static class AgentConstants
    {
        //config
        public const string CONFIG_SECTION_NAME = "datapecker";
        public const string CONFIG_CATCH_ALL_NAME = "*";

        //app settings
        public const string SERVICE_URI_OVERRIDE_KEY = "DatapeckerServiceUri";
        public const string SERVICE_ENDPOINT_NAME_KEY = "DatapeckerEndpointName";
        public const string DEBUG_MODE_SETTINGS_KEY = "DatapeckerDebugMode";


        //ExceptionEvent creator
        public const int MAX_INNER_EXCEPTIONS = 10;
        public const int MAX_EVENT_KEY_LENGTH = 215;
        public const int MAX_STRING_LENGTH = 500;
        public const int MAX_STACKTRACE_LENGTH = 10000;


        //Transfer
        public static readonly TimeSpan REPORT_INTERVAL = TimeSpan.FromMinutes(5);
        public static readonly Uri SERVICE_URI = new Uri("net.tcp://rs.datapecker.net:4502/reporting.svc/v1");
        public const string SERVICE_DNS = "support.datapecker.net";
        public static readonly TimeSpan SERVICE_MANAGER_TIMER_INTERVAL = TimeSpan.FromMilliseconds(500);
        public const int EXCEPTION_CASES_SEND_BLOCK = 200;
        public const int CUSTOM_EVENT_CASES_SEND_BLOCK = 10;


        //Storage
        public const string STORAGE_DEFAULT_PATH = "\\logs\\datapecker";
        public const string STORAGE_AGENT_SETTINGS_FILE = "settings.txt";
        public const string STORAGE_REPORTING_STATE_FILE = "state.txt";
        public const string STORAGE_ERRORS_FILE = "ERRORS.README.txt";
        public const string STORAGE_EXCEPTION_EVENTS_FILE = "exception events.bin";
        public const string STORAGE_CASES_FILE = "exception cases.txt";
        public const string STORAGE_KNOWN_KEYS_FILE = "event keys.txt";
        public const string STORAGE_CUSTOM_EVENTS_FILE = "custom events.bin";
    }
}
