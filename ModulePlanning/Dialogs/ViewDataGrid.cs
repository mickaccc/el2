using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace ModulePlanning.Dialogs
{
    public class ViewDataGrid : DataGrid
    {
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            if (AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementSelected) ||
                AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementAddedToSelection) ||
                AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection))
            {
                if (SelectedItem == null)
                {
                    return;
                }
            }
            base.OnSelectionChanged(e);
        }
    }
}
