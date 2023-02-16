namespace Lieferliste_WPF.View.Pane
{
  using System.Windows;
  using System.Windows.Controls;
    using Lieferliste_WPF.ViewModels.Base;
    using Lieferliste_WPF.ViewModels;

  class PanesStyleSelector : StyleSelector
  {
    public Style ToolStyle
    {
      get;
      set;
    }


    public Style DocumentStyle
    {
      get;
      set;
    }
    public Style DeliveryStyle
    {
        get;
        set;
    }
    public Style MachineWrapperStyle
    {
        get;
        set;
    }

    public override System.Windows.Style SelectStyle(object item, System.Windows.DependencyObject container)
    {
      if (item is OrderViewModel)
        return DocumentStyle;

		if (item is ToolViewModel)
        return ToolStyle;
        if (item is DeliveryListViewModel)
            return DeliveryStyle;
        if (item is MachineContainerViewModel)
            return DocumentStyle;
        if (item is MachineWrapper)
            return MachineWrapperStyle;


      return base.SelectStyle(item, container);
    }
  }
}
