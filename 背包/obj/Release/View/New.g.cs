﻿

#pragma checksum "F:\bajinsheng\Documents\Visual Studio 2013\Projects\背包\背包\View\New.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "072B8CEF8C6E69BF14732E59CE8D856F"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace 背包.View
{
    partial class New : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 38 "..\..\View\New.xaml"
                ((global::Windows.UI.Xaml.Controls.ComboBox)(target)).DropDownClosed += this.MyTravel_DropDownClosed;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 56 "..\..\View\New.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.Save_Click;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 57 "..\..\View\New.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.VoiceInput_Click;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


