﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Lieferliste_WPF.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.6.0.0")]
    public sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("229, 220, 220")]
        public global::System.Drawing.Color Pause {
            get {
                return ((global::System.Drawing.Color)(this["Pause"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("58, 94, 218")]
        public global::System.Drawing.Color Stripe1 {
            get {
                return ((global::System.Drawing.Color)(this["Stripe1"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("58, 161, 86")]
        public global::System.Drawing.Color Stripe2 {
            get {
                return ((global::System.Drawing.Color)(this["Stripe2"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Red")]
        public global::System.Drawing.Color outOfDate {
            get {
                return ((global::System.Drawing.Color)(this["outOfDate"]));
            }
            set {
                this["outOfDate"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("White")]
        public global::System.Drawing.Color inDate {
            get {
                return ((global::System.Drawing.Color)(this["inDate"]));
            }
            set {
                this["inDate"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("255, 128, 0")]
        public global::System.Drawing.Color Stripe3 {
            get {
                return ((global::System.Drawing.Color)(this["Stripe3"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <string>#FFAA00</string>
  <string>#AAAA00</string>
  <string>#92a8d1</string>
  <string>#034f84</string>
  <string>#f7cac9</string>
  <string>#f7786b</string>
  <string>#deeaee</string>
  <string>#b1cbbb</string>
  <string>#eea29a</string>
  <string>#c94c4c</string>
  <string>#ffef96</string>
  <string>#80ced6</string>
  <string>#4040a1</string>
  <string>#b0aac0</string>
  <string>#009933</string>
  <string>#990000</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection ColorPallette {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["ColorPallette"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Data Source=DESKTOP-MEMDFDP\\SQLEXPRESS;Initial Catalog=DB_COS_LIEFERLISTE_SQL;Int" +
            "egrated Security=True;Connect Timeout=5;Encrypt=False;TrustServerCertificate=Tru" +
            "e")]
        public string mgsch_Server {
            get {
                return ((string)(this["mgsch_Server"]));
            }
            set {
                this["mgsch_Server"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Data Source=HL0VM00069;Initial Catalog=DB_COS_LIEFERLISTE_SQL;Connect Timeout=5;E" +
            "ncrypt=False;TrustServerCertificate=True;User Id=EL2;Password=SCM7777scm!$")]
        public string scm2hl_Server {
            get {
                return ((string)(this["scm2hl_Server"]));
            }
            set {
                this["scm2hl_Server"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("EMEA\\")]
        public string Domain {
            get {
                return ((string)(this["Domain"]));
            }
            set {
                this["Domain"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.ConnectionString)]
        [global::System.Configuration.DefaultSettingValueAttribute("Data Source=HL0VM00069;Initial Catalog=DB_COS_LIEFERLISTE_SQL;Connect Timeout=5;E" +
            "ncrypt=False;TrustServerCertificate=True;User Id=EL2;Password=SCM7777scm!$")]
        public string ConnectionString {
            get {
                return ((string)(this["ConnectionString"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("^(\\w{4})(\\w{3})(\\w+),[TTNR],,[AID]")]
        public string ExplorerPath {
            get {
                return ((string)(this["ExplorerPath"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <string>Dokumente|*.pdf;*.doc</string>
  <string>|Bild Dateien|*.jpg;*.jpeg;*.png;*bmp;*.tiff</string>
  <string>|Excel Dateien|*.xls;*xlsx;*xlsm</string>
  <string>|alles|*.*</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection ExplorerFilter {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["ExplorerFilter"]));
            }
            set {
                this["ExplorerFilter"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("*.pdf")]
        public string ExplorerExt {
            get {
                return ((string)(this["ExplorerExt"]));
            }
            set {
                this["ExplorerExt"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Q:\\ZproE\\COS_Messdaten")]
        public string ExplorerRoot {
            get {
                return ((string)(this["ExplorerRoot"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Q:\\ZproE\\COS_Messdaten")]
        public string Setting {
            get {
                return ((string)(this["Setting"]));
            }
            set {
                this["Setting"] = value;
            }
        }
    }
}
