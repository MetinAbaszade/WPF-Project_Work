﻿#pragma checksum "..\..\..\Views\EvaluationWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "742D960FF11AC543BCC6008836520F394152857759112438E363834A0C87B0DC"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Converters;
using MaterialDesignThemes.Wpf.Transitions;
using Project_Work_WPF.Views;
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


namespace Project_Work_WPF.Views {
    
    
    /// <summary>
    /// EvaluationWindow
    /// </summary>
    public partial class EvaluationWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 41 "..\..\..\Views\EvaluationWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Button1;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\..\Views\EvaluationWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Button2;
        
        #line default
        #line hidden
        
        
        #line 49 "..\..\..\Views\EvaluationWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Button3;
        
        #line default
        #line hidden
        
        
        #line 53 "..\..\..\Views\EvaluationWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Button4;
        
        #line default
        #line hidden
        
        
        #line 57 "..\..\..\Views\EvaluationWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Button5;
        
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
            System.Uri resourceLocater = new System.Uri("/Project_Work_WPF;component/views/evaluationwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Views\EvaluationWindow.xaml"
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
            this.Button1 = ((System.Windows.Controls.Button)(target));
            
            #line 41 "..\..\..\Views\EvaluationWindow.xaml"
            this.Button1.MouseRightButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Star_Button_MouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 2:
            this.Button2 = ((System.Windows.Controls.Button)(target));
            
            #line 45 "..\..\..\Views\EvaluationWindow.xaml"
            this.Button2.MouseRightButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Star_Button_MouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 3:
            this.Button3 = ((System.Windows.Controls.Button)(target));
            
            #line 49 "..\..\..\Views\EvaluationWindow.xaml"
            this.Button3.MouseRightButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Star_Button_MouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 4:
            this.Button4 = ((System.Windows.Controls.Button)(target));
            
            #line 53 "..\..\..\Views\EvaluationWindow.xaml"
            this.Button4.MouseRightButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Star_Button_MouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 5:
            this.Button5 = ((System.Windows.Controls.Button)(target));
            
            #line 57 "..\..\..\Views\EvaluationWindow.xaml"
            this.Button5.MouseRightButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Star_Button_MouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 61 "..\..\..\Views\EvaluationWindow.xaml"
            ((System.Windows.Controls.Button)(target)).MouseRightButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Delete_Button_MouseRightButtonDown);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
