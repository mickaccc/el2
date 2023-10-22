using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Lieferliste_WPF.Views

{
    sealed class CloseableTabControl:TabControl
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CloseableTabItem();
        }
    }
}
