﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18033
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SeriesSelector.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\users\\abo\\desktop\\source")]
        public string SourcePath {
            get {
                return ((string)(this["SourcePath"]));
            }
            set {
                this["SourcePath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("c:\\users\\abo\\desktop\\destination")]
        public string DestinationSeriesPath {
            get {
                return ((string)(this["DestinationSeriesPath"]));
            }
            set {
                this["DestinationSeriesPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("mkv|avi|mp4")]
        public string FileTypes {
            get {
                return ((string)(this["FileTypes"]));
            }
            set {
                this["FileTypes"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("720p|1080i|1080p|x264|DSR|HR-HDTV|HR.HDTV|HDTV|DVDMux|BluRay|BRay|Rip|DVDRip|BluR" +
            "ayRip|Blu|Ray|Xvid")]
        public string CleanList {
            get {
                return ((string)(this["CleanList"]));
            }
            set {
                this["CleanList"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("c:\\users\\abo\\desktop\\destination")]
        public string DestinationMoviesPath {
            get {
                return ((string)(this["DestinationMoviesPath"]));
            }
            set {
                this["DestinationMoviesPath"] = value;
            }
        }
    }
}
