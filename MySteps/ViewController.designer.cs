﻿// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace MySteps
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel AvarageLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIDatePicker EndDatePicker { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIDatePicker StartDatePicker { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel TotalLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (AvarageLabel != null) {
                AvarageLabel.Dispose ();
                AvarageLabel = null;
            }

            if (EndDatePicker != null) {
                EndDatePicker.Dispose ();
                EndDatePicker = null;
            }

            if (StartDatePicker != null) {
                StartDatePicker.Dispose ();
                StartDatePicker = null;
            }

            if (TotalLabel != null) {
                TotalLabel.Dispose ();
                TotalLabel = null;
            }
        }
    }
}