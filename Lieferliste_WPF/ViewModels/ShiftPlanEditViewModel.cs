using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lieferliste_WPF.ViewModels
{
    internal class ShiftPlanEditViewModel
    {
        private IContainerProvider _container;
        public string Title { get; } = "Schichtplan";
        public ImmutableDictionary<int, bool[]> ShiftPlan { get; }
        public ShiftPlanEditViewModel(IContainerProvider container)
        {
            _container = container;
            Dictionary<int, bool[]> dict = [];
            for(int i=0; i<7;i++)
            {
                dict.Add(i, new bool[1440]);
            }
            ShiftPlan = dict.ToImmutableDictionary();
        }
    }
}
