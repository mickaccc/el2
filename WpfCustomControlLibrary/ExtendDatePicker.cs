using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace WpfCustomControlLibrary
{
    public class ExtendDatePicker : DatePicker
    {
        static ExtendDatePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ExtendDatePicker), new FrameworkPropertyMetadata(typeof(ExtendDatePicker)));
        }


    }
}
