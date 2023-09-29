using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Lieferliste_WPF.View
{
    class ExampleViewModel : IDropTarget
    {
        public ObservableCollection<SchoolViewModel> Schools { get; private set; }

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            PupilViewModel sourceItem = dropInfo.Data as PupilViewModel;
            SchoolViewModel targetItem = dropInfo.TargetItem as SchoolViewModel;

            if (sourceItem != null && targetItem != null && targetItem.CanAcceptPupils)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Copy;
            }
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            PupilViewModel sourceItem = (PupilViewModel)dropInfo.Data;
            SchoolViewModel targetItem = (SchoolViewModel)dropInfo.TargetItem;
            targetItem.Pupils.Add(sourceItem);
        }
    }

    class SchoolViewModel
    {
        public bool CanAcceptPupils { get; set; }
        public ObservableCollection<PupilViewModel> Pupils { get; private set; }
    }

    class PupilViewModel
    {
        public string Name { get; set; }
    }
}
