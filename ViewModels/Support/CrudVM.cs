using Lieferliste_WPF.Entities;
using Lieferliste_WPF.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lieferliste_WPF.Messages;

namespace Lieferliste_WPF.ViewModels.Support
{
    public class CrudVM :ViewModelBase
    {
        protected EntitiesLL db = new EntitiesLL();
        protected EntitiesAlloc dbAlloc = new EntitiesAlloc();

        protected void HandleCommand(CommandMessage action)
        {
            if (isCurrentView)
            {
                switch (action.Command)
                {
                    case CommandType.Insert:
                        break;
                    case CommandType.Edit:
                        break;
                    case CommandType.Delete:
                        DeleteCurrent();
                        break;
                    case CommandType.Commit:
                        CommitUpdates();
                        break;
                    case CommandType.Refresh:
                        RefreshData();
                        break;
                    default:
                        break;
                }
            }
        }
        //private Visibility throbberVisible = Visibility.Visible;
        //public Visibility ThrobberVisible
        //{
        //    get { return throbberVisible; }
        //    set
        //    {
        //        throbberVisible = value;
        //        RaisePropertyChanged();
        //    }
        //}
        private string errorMessage;

        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                errorMessage = value;
                RaisePropertyChanged();
            }
        }

        protected virtual void CommitUpdates()
        {
        }
        protected virtual void DeleteCurrent()
        {
        }
        protected virtual void RefreshData()
        {
           // GetData();

        }
        protected virtual void GetData()
        {
        }
 
        protected CrudVM()
        {
            GetData();

        }
        protected bool isCurrentView = false;
        private void CurrentUserControl(NavigateMessage nm)
        {
            if (this.GetType() == nm.ViewModelType)
            {
                isCurrentView = true;
            }
            else
            {
                isCurrentView = false;
            }
        }

    }
}
