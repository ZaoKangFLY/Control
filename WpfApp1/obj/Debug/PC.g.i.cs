﻿#pragma checksum "..\..\PC.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "3088995396DF83555E7DD2EE59EE81D9B51631CD804817426ABD7424D1F275FC"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace gangway_controller {
    
    
    /// <summary>
    /// NewWindow
    /// </summary>
    public partial class NewWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 10 "..\..\PC.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button PCdown;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\PC.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Candown;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\PC.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Reapp;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\PC.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button PCre;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\PC.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Fastdown;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/WpfApp1;component/pc.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\PC.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.PCdown = ((System.Windows.Controls.Button)(target));
            
            #line 10 "..\..\PC.xaml"
            this.PCdown.Click += new System.Windows.RoutedEventHandler(this.PCDown_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.Candown = ((System.Windows.Controls.Button)(target));
            
            #line 11 "..\..\PC.xaml"
            this.Candown.Click += new System.Windows.RoutedEventHandler(this.CancellDown_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.Reapp = ((System.Windows.Controls.Button)(target));
            
            #line 12 "..\..\PC.xaml"
            this.Reapp.Click += new System.Windows.RoutedEventHandler(this.restartAPP_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.PCre = ((System.Windows.Controls.Button)(target));
            
            #line 13 "..\..\PC.xaml"
            this.PCre.Click += new System.Windows.RoutedEventHandler(this.restartPC_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.Fastdown = ((System.Windows.Controls.Button)(target));
            
            #line 14 "..\..\PC.xaml"
            this.Fastdown.Click += new System.Windows.RoutedEventHandler(this.FastDown_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

