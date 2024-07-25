using CompositeCommands.Core;
using El2Core.Constants;
using El2Core.Models;
using El2Core.Utils;
using El2Core.ViewModelBase;
using Microsoft.EntityFrameworkCore;
using Prism.Ioc;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Lieferliste_WPF.ViewModels
{
    public class ProjectEditViewModel : ViewModelBase, IDialogAware
    {
        public string Title { get; } = "Projekt Editor";

        private IContainerProvider _container;
        private IApplicationCommands _applicationCommands;
        public IApplicationCommands ApplicationCommands
        {
            get { return _applicationCommands; }
            set { _applicationCommands = value; }
        }
        private RelayCommand? _projectSearchCommand;
    
        public ICommand ProjectSearchCommand => _projectSearchCommand ??= new RelayCommand(OnProjectSearch);

        private PspNode<Shape> Projects = new PspNode<Shape> { Node = new ("Projekte")};
        public ICollectionView? PSP_NodeCollectionView { get; private set; }

        private string _projectSearchText = string.Empty;
        public string ProjectSearchText
        {
            get { return _projectSearchText; }
            set
            {
                if (_projectSearchText != value)
                {
                    _projectSearchText = value;
                    NotifyPropertyChanged(() => ProjectSearchText);
                    NotifyPropertyChanged(() => IsExpanded);
                }
            }
        }

        public bool IsExpanded
        {
            get { return _projectSearchText.Length >= 5; }
        }
        private Dictionary<string, Shape> EditResult = [];
        private static readonly object _lock = new();

        public event Action<IDialogResult> RequestClose;

        public NotifyTaskCompletion<ICollectionView>? PspTask { get; private set; }

        public ProjectEditViewModel(IContainerProvider container, IApplicationCommands applicationCommands)
        {
            _container = container;
            _applicationCommands = applicationCommands;

            PspTask = new NotifyTaskCompletion<ICollectionView>(LoadPspDataAsync());             
        }


        private void OnProjectSearch(object obj)
        {
            if (obj != null)
            {
                ProjectSearchText = ConvertPsp((string)obj);
                if (_projectSearchText.Length >= 2 || _projectSearchText.Length == 0)
                    PSP_NodeCollectionView?.Refresh();
            }
        }
  


        private async Task<ICollectionView> LoadPspDataAsync()
        {
            using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            var proj = await db.Projects
                .Include(x => x.OrderRbs)
                .ThenInclude(x => x.MaterialNavigation)
                .Where(x => x.OrderRbs.Any(x => x.Abgeschlossen == false))
                .ToListAsync();

            var uiContext = TaskScheduler.FromCurrentSynchronizationContext();

            await Task.Factory.StartNew(() =>
            {

                foreach (var item in proj.OrderBy(x => x.ProjectPsp))
                {
                    var p = item.ProjectPsp.Trim();
                    Regex regex = new Regex("");
                    foreach(var scheme in RuleInfo.ProjectSchemes)
                    {
                        if(p.StartsWith(scheme.Key, StringComparison.OrdinalIgnoreCase))
                            regex = new Regex(scheme.Value.Regex);
                    }
                    Match match = regex.Match(p);
                    if (match.Success && match.Groups.Count > 1)
                    {
                        string psp = string.Empty;
                        PspNode<Shape> stepNode = new();
                        foreach(var m in match.Groups.Values.Skip(1))
                        {
                            if (m.Value == "") break;
                            psp += m.Value;
                            var node = Projects.Children.FirstOrDefault(x => psp.StartsWith(x.Node.ToString()));
                            if (node == null)
                            {                                
                                stepNode = Projects.Add(new Shape(psp), "Psp-Type");                              
                            }
                            else if(node.Node.ToString() == psp)
                            {
                                stepNode = node;
                            }
                            else
                            {
                                stepNode = node.Add(new Shape(psp), "Psp-Type");                             
                            }
                        }
 
                        foreach (var o in item.OrderRbs)
                        {
                            var sh = new Shape(o.Aid);
                            sh.Description = string.Format("{0} {1}", o.Material, o.MaterialNavigation?.Bezeichng);
                            stepNode.Add(sh, "Order-Type");
                            
                        }
                        if(stepNode.HasOrder)
                        {
                            stepNode.Node.Description = item.ProjectInfo;
                            stepNode.Node.ProjectType = (ProjectTypes.ProjectType)item.ProjectType;
                            stepNode.Node.PropertyChanged += Node_PropertyChanged;
                            
                        }
                    }
                }

            }, CancellationToken.None, TaskCreationOptions.None, uiContext);
            
            PSP_NodeCollectionView = CollectionViewSource.GetDefaultView(Projects.Children);
            PSP_NodeCollectionView.Filter += FilterPredicatePsp;
            return PSP_NodeCollectionView;
        }

        private void Node_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            var tn = sender as Shape;

            if (tn != null)
            {
                Shape result;
                if (EditResult.TryGetValue(tn.ToString(), out result))
                {
                    result = tn;
                }
                else
                {
                    EditResult.Add(tn.ToString(), tn);
                }
                
            }
        }


        private bool FilterPredicatePsp(object obj)
        {
            var psp = (PspNode<Shape>)obj;
            var search = ClearPsp(_projectSearchText);

            bool accepted = ClearPsp(psp.Node.ToString()).Contains(search, StringComparison.CurrentCultureIgnoreCase);
            if (!accepted)
            {
                if (psp.Children != null && search != string.Empty)
                {
                    accepted = psp.Children.Any(x => x.Node.ToString().Contains(search, StringComparison.CurrentCultureIgnoreCase));
                }
            }
            return accepted;

        }


        private string ClearPsp(string pspIn)
        {
            Regex reg = new Regex("\\s+|[-]+|[.]+");
            string psp = reg.Replace(pspIn, "");
            return psp;
        }
 
        private string ConvertPsp(string psp)
        {
            psp = ClearPsp(psp.ToUpper().Trim());

            Regex regex = psp.StartsWith("ds", StringComparison.InvariantCultureIgnoreCase) ?
                new Regex("(DS)([0-9]{6})([0-9]{2})*") : new Regex("(SC-PR)([0-9]{9})([0-9]{2})*");
            var match = regex.Match(psp);
            if (match.Success)
            {
                string retVal;
                retVal = match.Groups[1] + "-" + match.Groups[2];
                foreach (var m in match.Groups[3].Captures.Cast<Capture>())
                {
                    retVal += "-" + m.Value;
                }
                return retVal;
            }

            return psp;
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            var pro = db.Projects;
            foreach (var edit in EditResult)
            {
                var current = pro.First(x => x.ProjectPsp == edit.Key);
                if(current.ProjectInfo != edit.Value.Description) current.ProjectInfo = edit.Value.Description;
                if (current.ProjectType != (int)edit.Value.ProjectType) current.ProjectType = (int)edit.Value.ProjectType;
            }
            db.SaveChanges();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            
        }
    }
}
