﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Datapecker.Agent.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class MessageResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal MessageResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Datapecker.Agent.Resources.MessageResources", typeof(MessageResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Exception occurred in type {0} method {1} line {2}..
        /// </summary>
        internal static string Caller_ExceptionMethod {
            get {
                return ResourceManager.GetString("Caller_ExceptionMethod", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error loading Datapecker configuration..
        /// </summary>
        internal static string ConfigManager_ConfigLoadError {
            get {
                return ResourceManager.GetString("ConfigManager_ConfigLoadError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Configuration file could not be found or loaded..
        /// </summary>
        internal static string ConfigManager_ConfigNotFoundError {
            get {
                return ResourceManager.GetString("ConfigManager_ConfigNotFoundError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Configuration must be initialized before using. There were exceptions while loading configuration. If you want to procede with another configuration, then use UpdateConfiguration method to initialize again..
        /// </summary>
        internal static string ConfigManager_ConfigNotInitialized {
            get {
                return ResourceManager.GetString("ConfigManager_ConfigNotInitialized", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Default Target not found in configuration..
        /// </summary>
        internal static string ConfigManager_DefaultTargetNotFound {
            get {
                return ResourceManager.GetString("ConfigManager_DefaultTargetNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} element name {1} is already used..
        /// </summary>
        internal static string ConfigManager_ElementNameInUse {
            get {
                return ResourceManager.GetString("ConfigManager_ElementNameInUse", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Section &lt;{0}&gt; is not defined in application config file..
        /// </summary>
        internal static string ConfigManager_NoSectionException {
            get {
                return ResourceManager.GetString("ConfigManager_NoSectionException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to IStateProvider with name &lt;{0}&gt; not found..
        /// </summary>
        internal static string ConfigManager_StateProviderNotFound {
            get {
                return ResourceManager.GetString("ConfigManager_StateProviderNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Storage path &lt;{0}&gt; is already used for another target..
        /// </summary>
        internal static string ConfigManager_StoragePathInUse {
            get {
                return ResourceManager.GetString("ConfigManager_StoragePathInUse", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Target with name &lt;{0}&gt; not found..
        /// </summary>
        internal static string ConfigManager_TargetNotFound {
            get {
                return ResourceManager.GetString("ConfigManager_TargetNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Config parameter {0} of {1} element  is not specified..
        /// </summary>
        internal static string ConfigModel_ParameterMissing {
            get {
                return ResourceManager.GetString("ConfigModel_ParameterMissing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Exception occurred when accessing local storage file..
        /// </summary>
        internal static string CustomEvents_StorageException {
            get {
                return ResourceManager.GetString("CustomEvents_StorageException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to convert value {0} to property of {1} class..
        /// </summary>
        internal static string GroupEntryExtensions_ValueConvertException {
            get {
                return ResourceManager.GetString("GroupEntryExtensions_ValueConvertException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to set value {0} to property of {1} class..
        /// </summary>
        internal static string GroupEntryExtensions_ValueSetException {
            get {
                return ResourceManager.GetString("GroupEntryExtensions_ValueSetException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Internal exception occurred. See exception details..
        /// </summary>
        internal static string InternalCaller_DebugException {
            get {
                return ResourceManager.GetString("InternalCaller_DebugException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to create instance of {0}..
        /// </summary>
        internal static string StateProviderConfig_ActivationFail {
            get {
                return ResourceManager.GetString("StateProviderConfig_ActivationFail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to StateProvider is not specified..
        /// </summary>
        internal static string StateProviderConfig_NotSet {
            get {
                return ResourceManager.GetString("StateProviderConfig_NotSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to IStateProvider of type &lt;{0}&gt; not found..
        /// </summary>
        internal static string StateProviderConfig_TypeNotFound {
            get {
                return ResourceManager.GetString("StateProviderConfig_TypeNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to StateProvider type {0} does not implement {1} interface..
        /// </summary>
        internal static string StateProviderConfig_TypeWrongInterface {
            get {
                return ResourceManager.GetString("StateProviderConfig_TypeWrongInterface", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ApplicationID must be a hexadecimal number 24 digits long..
        /// </summary>
        internal static string TargetConfig_AppliationIDRequired {
            get {
                return ResourceManager.GetString("TargetConfig_AppliationIDRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Exception occurred when loading application settings from local storage file..
        /// </summary>
        internal static string TargetContext_AppSettingsLoadException {
            get {
                return ResourceManager.GetString("TargetContext_AppSettingsLoadException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Close exception while communicating to the server..
        /// </summary>
        internal static string WcfSafeCall_CloseException {
            get {
                return ResourceManager.GetString("WcfSafeCall_CloseException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Communication exception while trying to connect to the server..
        /// </summary>
        internal static string WcfSafeCall_CommunicationException {
            get {
                return ResourceManager.GetString("WcfSafeCall_CommunicationException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Connection to the server failed..
        /// </summary>
        internal static string WcfSafeCall_ConnectionFailed {
            get {
                return ResourceManager.GetString("WcfSafeCall_ConnectionFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not establish connection to the server..
        /// </summary>
        internal static string WcfSafeCall_OpenFailed {
            get {
                return ResourceManager.GetString("WcfSafeCall_OpenFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Internal server exception..
        /// </summary>
        internal static string WcfSafeCall_ServerInternalException {
            get {
                return ResourceManager.GetString("WcfSafeCall_ServerInternalException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Timeout exception while communicating to the server..
        /// </summary>
        internal static string WcfSafeCall_TimeoutException {
            get {
                return ResourceManager.GetString("WcfSafeCall_TimeoutException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unknown exception while communicating to the server..
        /// </summary>
        internal static string WcfSafeCall_UnknownException {
            get {
                return ResourceManager.GetString("WcfSafeCall_UnknownException", resourceCulture);
            }
        }
    }
}
