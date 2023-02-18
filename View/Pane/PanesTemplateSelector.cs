namespace Lieferliste_WPF.View.Pane
{
    using Lieferliste_WPF.ViewModels;
    using System.Windows;
    using System.Windows.Controls;

    class PanesTemplateSelector : DataTemplateSelector
    {
        public PanesTemplateSelector()
        {

        }


        public DataTemplate OrderViewTemplate
        {
            get;
            set;
        }

        public DataTemplate DeliveryListViewTemplate
        {
            get;
            set;
        }
        public DataTemplate MachineContainerViewTemplate
        {
            get;
            set;
        }
        public DataTemplate MachineViewTemplate
        {
            get;
            set;
        }
        public DataTemplate MachineWrapperTemplate
        {
            get;
            set;
        }

        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {


            if (item is OrderViewModel)
                return OrderViewTemplate;
            if (item is DeliveryListViewModel)
                return DeliveryListViewTemplate;
            if (item is MachineContainerViewModel)
                return MachineContainerViewTemplate;
            if (item is MachineViewModel)
                return MachineViewTemplate;
            if (item is MachineWrapper)
                return MachineWrapperTemplate;

            return base.SelectTemplate(item, container);
        }
    }
}
