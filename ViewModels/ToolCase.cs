using System.Collections.Generic;
using System.Linq;
using Lieferliste_WPF.ViewModels.Base;
using System.Windows;
using System.Windows.Input;
using Lieferliste_WPF.Commands;
using System;
using System.Xml.Linq;
using System.Reflection;
using Lieferliste_WPF.ViewModels.Support;

namespace Lieferliste_WPF.ViewModels
{
    class ToolCase : Base.ViewModelBase
    {
        static ToolCase _this = new ToolCase();
        private Perspective _activePerspective;
        private List<Perspective> _perspectives = new List<Perspective>();
        public ICommand toArchive { get; private set; }
        public List<Perspective> Perspectives { get { return _perspectives; } }
        protected ToolCase()
        {

            DataSetEL4.PrespectivesDataTable pers = DbManager.Instance().getPerspectives(MainWindowViewModel.currentUser);

            foreach (DataSetEL4.PrespectivesRow row in pers)
            {
                Perspective p = new Perspective();
                p.Name = row.PerspectName;

                try
                {
                    var xml = XDocument.Load(@"Perspective/" + row.PerspectFileName.Trim() + ".xml");

                    var queryD = from c in xml.Root.Elements("DocumentPanes") select new { Children = c.Descendants("Item") };
                    string t = this.GetType().Namespace + ".";
                    foreach (var Childs in queryD)
                    {
                        foreach (var item in Childs.Children)
                        {
                            Type itemType = this.GetType().Assembly.GetType(t + item.Attribute("Type").Value);
                            if (item.Attribute("Param") != null)
                            {
                                p.SubType = Convert.ToInt32(item.Attribute("Param").Value);
                                p.LeaderPanes.Add((ViewModelBase)Activator.CreateInstance(itemType, new object[] { p }));
                            }
                            else
                            {
                                if (item.Attribute("Static") != null)
                                {
                                    PropertyInfo method = itemType.GetProperty("This",BindingFlags.Public | BindingFlags.Static);
                                    var vm = (CrudVM)method.GetValue(null, null);
                                    
                                    p.LeaderPanes.Add(vm);
                                }
                                else
                                {
                                    p.LeaderPanes.Add((ViewModelBase)Activator.CreateInstance(itemType));
                                }
                            }
                        }

                    }
                    var queryA = from c in xml.Root.Descendants("AnchorablePanes") select new { Children = c.Descendants("Item") };
                    foreach (var Childs in queryA)
                    {
                        foreach (var item in Childs.Children)
                        {
                            Type itemType = this.GetType().Assembly.GetType(t + item.Attribute("Type").Value);
                            if (item.Attribute("Param") != null)
                            {
                                p.SubType = Convert.ToInt32(item.Attribute("Param").Value);
                                p.AttatchedPanes.Add((ViewModelBase)Activator.CreateInstance(itemType, new object[] { p }));
                                
                            }
                            else
                            {
                                if (item.Attribute("Static") != null)
                                {
                                    PropertyInfo prop = itemType.GetProperty("This",BindingFlags.Public | BindingFlags.Static);
                                    p.AttatchedPanes.Add((ViewModelBase)prop.GetValue(null,null));
                                }
                                else
                                {
                                    p.AttatchedPanes.Add((ViewModelBase)Activator.CreateInstance(itemType));
                                }
                            }
                        }
                    }
                }
                catch (FieldAccessException e)
                {
                    throw e;
                }
                catch (TypeLoadException e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    throw e;
                }
                //switch (row.PerspectType)
                //{
                //    case 1:
                //        p.SubType = 1;
                //        foreach (DataSetEL4.PerspectModelRow PMRow in PM.Where(x => x.PerspectName == row.PerspectName))
                //        {
                //            switch (PMRow.DocumentPane)
                //            {
                //                case "MACO1000":
                //                    p.LeaderPanes.Add(new MachineContainerViewModel(p));
                //                    break;
                //            }
                //            switch (PMRow.Anchorable)
                //            {
                //                case "MASH1000":
                //                    MachineContainerViewModel mc = p.LeaderPanes.FirstOrDefault(
                //                        x => x.GetType() == typeof(MachineContainerViewModel)) as MachineContainerViewModel;
                //                    if (mc != null)
                //                    {

                //                        p.AttatchedPanes.Add(new MachineWrapper { MachineViewModel = mc.SelectedMachine });
                //                    }
                //                    break;
                //                case "ORDE1000":
                //                    p.AttatchedPanes.Add(OrderViewModel.This);
                //                    break;
                //            }
                //        }

                //        break;
                //}
                _perspectives.Add(p);
            }

            _activePerspective = _perspectives.ElementAt(0);
            toArchive = new ActionCommand(OnArchiveExecuted, OnArchiveCanExecute);
        }
        public static ToolCase This { get { return _this; } }
        public Perspective ActivePerspective
        {
            get { return _activePerspective; }
            set { _activePerspective = value; }
        }
        public void ChangeActivePerpective(string Name)
        {
            if (_perspectives.Exists(x => x.Name == Name))
            {
                _activePerspective = _perspectives.Find(x => x.Name == Name);
                RaisePropertyChanged("ActivePerspective");
                RaisePropertyChanged("LeaderPanes");
                RaisePropertyChanged("AttachedPanes");
            }
        }
        public void PropertyModifieded()
        {
            RaisePropertyChanged("AttachedPanes");
        }
        public ObservableList<ViewModelBase> LeaderPanes
        {
            get { return _activePerspective.LeaderPanes; }
        }
        public ObservableList<ViewModelBase> AttachedPanes
        {
            get { return _activePerspective.AttatchedPanes; }
        }



        public void InitCommandBindings(Window win)
        {

            //win.CommandBindings.Add(new CommandBinding((_perspectives[0].LeaderPanes[0] as DeliveryListViewModel).SortAscCommand));
            //win.CommandBindings.Add(new CommandBinding((_perspectives[0].LeaderPanes[0] as DeliveryListViewModel).SortDescCommand));

        }

        void OnArchiveExecuted(object parameter)
        {
            if (parameter != null)
            {
            }
        }

        bool OnArchiveCanExecute(object parameter) { return true; }
    }
}
