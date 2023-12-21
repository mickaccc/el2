using CompositeCommands.Core;
using El2Core.Models;
using El2Core.ViewModelBase;
using Microsoft.EntityFrameworkCore;
using Prism.Ioc;
using System;
using El2Core.Converters;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpCompress.Common;
using System.Text.RegularExpressions;

namespace Lieferliste_WPF.ViewModels
{
    public class ProjectEditViewModel : ViewModelBase
    {
        IContainerProvider _container;
        IApplicationCommands _applicationCommands;

        public ProjectEditViewModel(IContainerProvider container, IApplicationCommands applicationCommands)
        {
            _container = container;
            _applicationCommands = applicationCommands;

            LoadData();
        }
        public string Title { get; } = "Projekt Editor";
        private List<OrderRb> _orders = [];
        private List<Project> _projects = [];
        public List<Project> Projects => _projects;
        public List<OrderRb> Orders => _orders;
        private List<PSP_Node> _PSP_Nodes;
   

        private void LoadData()
        {
            using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            var orders = db.OrderRbs.Include(x => x.MaterialNavigation).Where(x => x.Abgeschlossen == false);
            _orders.AddRange(orders);

            var proj = db.Projects;
            _projects.AddRange(proj);

            AddPSP_Node("DS-620051-20-12");
        }
        private void AddPSP_Node(string psp)
        {
            var p = Psp_Formatter(psp);

            var node = new PSP_Node();
            node.Name = psp;
            var result = CreatePspNode(psp, node);
            _PSP_Nodes.Add(result);

        }
        private PSP_Node CreatePspNode(string psp, PSP_Node pspNode)
        {
            var p = new PSP_Node();
            var root = psp[..9];
            while (psp != root)
            {
                var Pspnode = new PSP_Node();
                Pspnode.Name = psp;
                Pspnode.Child.Add(pspNode);
                var parent = psp[..psp.LastIndexOf('-')];
                p = CreatePspNode(parent, Pspnode);
            }
            var result = new PSP_Node();
            result.Name = root;
            result.Child.Add(p);
            return result;
        }
        private string Psp_Formatter(string psp)
        {
            var strVal = (string)psp;
            Regex regex = new Regex("(DS)([0-9]{6})([0-9]{2})*");
            var match = regex.Match(strVal);
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
    }
    class PSP_Node()
    {
        
 
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        private string _description = string.Empty;

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        private List<PSP_Node> _child = [];

        public List<PSP_Node> Child
        {
            get { return _child; }
            set { _child = value; }
        }

    }
}
