using El2Core.Models;
using El2Core.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lieferliste_WPF.ViewModels
{
    internal class EmployNoteViewModel
    {
        public EmployNoteViewModel(IContainerProvider containerProvider)
        {
            container = containerProvider;
            LoadingData();
        }


        public string Title { get; } = "Arbeitszeiten";
        IContainerProvider container;
        public List<Vorgang> VorgangRef { get; private set; } = [];
        public ObservableCollection<EmployeeNote> EmployeeNotes { get; private set; } = [];
        private void LoadingData()
        {
            using var db = container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            VorgangRef.AddRange(db.Vorgangs
                .Include(x => x.AidNavigation)
                .Include(x => x.AidNavigation.MaterialNavigation)
                .Include(x => x.AidNavigation.DummyMatNavigation)
                .Where(x => x.AidNavigation.Abgeschlossen == false));

            EmployeeNotes.AddRange(db.EmployeeNotes.Where(x => x.AccId.Equals(UserInfo.User.UserId)).OrderBy(x => x.Date));
        }
        public string[] SelectedRef { get; } =
        [
            "Reinigen", "Cip", "Anlernen"
        ];
    }
}
