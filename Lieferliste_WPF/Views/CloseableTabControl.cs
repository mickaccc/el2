﻿using System.Windows;
using System.Windows.Controls;

namespace Lieferliste_WPF.Views

{
    internal sealed class CloseableTabControl : TabControl
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CloseableTabItem();
        }
    }
}
