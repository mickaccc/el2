using Lieferliste_WPF.Entities;
using Lieferliste_WPF.Working;
using Lieferliste_WPF.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;


namespace Lieferliste_WPF
{
    public class DbManager
    {
        private const int MAX_ERRORS_TO_LOG = 100;
        private static List<int[]> pause = new List<int[]>();
        private static DataSetPermission _permissionDataSet = new DataSetPermission();
        private static DataSetEL2 _tablesDataSet = new DataSetEL2();

        private static DataSetEL2TableAdapters.RessourceOfBIDTableAdapter _ressOfBID = new DataSetEL2TableAdapters.RessourceOfBIDTableAdapter();
        private static DataSetEL2TableAdapters.RessZuteilTableAdapter _ressZuteil = new DataSetEL2TableAdapters.RessZuteilTableAdapter();
        private static DataSetEL2TableAdapters.tblPauseTableAdapter _pauseAdapt = new DataSetEL2TableAdapters.tblPauseTableAdapter();
        private static DataSetEL2TableAdapters.tblRessKappaTableAdapter _kappaAdapt = new DataSetEL2TableAdapters.tblRessKappaTableAdapter();
        private static DataSetEL2TableAdapters.tblProjektTableAdapter _projAktAdapt = new DataSetEL2TableAdapters.tblProjektTableAdapter();
        private static DataSetEL2TableAdapters.RessZutTableAdapter _resZut = new DataSetEL2TableAdapters.RessZutTableAdapter();
        private static DataSetEL2TableAdapters.tblRessourceVorgangTableAdapter resVorg = new DataSetEL2TableAdapters.tblRessourceVorgangTableAdapter();
        private static DataSetEL2TableAdapters.OrderHeaderByVIDTableAdapter Header = new DataSetEL2TableAdapters.OrderHeaderByVIDTableAdapter();
        private static DataSetEL2TableAdapters.tblRessourceTableAdapter res = new DataSetEL2TableAdapters.tblRessourceTableAdapter();
        private static DataSetEL2TableAdapters.tblAuftragTableAdapter ord = new DataSetEL2TableAdapters.tblAuftragTableAdapter();
        private static DataSetEL2TableAdapters.tblVorgangTableAdapter lie = new DataSetEL2TableAdapters.tblVorgangTableAdapter();
        private static DataSetPermissionTableAdapters.tblUserListeTableAdapter user = new DataSetPermissionTableAdapters.tblUserListeTableAdapter();
        private static DataSetEL2TableAdapters.OrderListTableAdapter _orderList = new DataSetEL2TableAdapters.OrderListTableAdapter();

        private static DataSetEL2TableAdapters.lieferlisteTableAdapter _lieferListeAdapter = new DataSetEL2TableAdapters.lieferlisteTableAdapter();
        private static int _bid;

        private static DbManager _instance;

        protected DbManager()
        {
            try
            {
                if (getPermissions(ViewModels.MainWindowViewModel.currentUser).Count == 0)
                {
                    MessageBox.Show("Sie haben keine Berechtigungen um diese Anwendung zu verwenden!", "ERROR", MessageBoxButton.OK);
                    Application.Current.Shutdown();
                }
                _pauseAdapt.Fill(_tablesDataSet.tblPause);
                _kappaAdapt.Fill(_tablesDataSet.tblRessKappa);
                _projAktAdapt.Fill(_tablesDataSet.tblProjekt);


                resVorg.Fill(_tablesDataSet.tblRessourceVorgang);
                LogErrors(_tablesDataSet);
            }
            catch (Exception e)
            {

                LogErrors(_permissionDataSet);
                LogErrors(_tablesDataSet);
                MessageBox.Show("Datenbankproblem 'DbManager'\n" + e.Message + "\n" + e.InnerException, "ERROR",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

            }

        }
        public static DbManager Instance()
        {
            if (_instance == null)
            {
                _instance = new DbManager();
            }
            return _instance;
        }
        private static void LogErrors(DataSet dataSet)
        {
            foreach (DataTable dataTable in dataSet.Tables)
            {
                LogErrors(dataTable);
            }
        }

        private static void LogErrors(DataTable dataTable)
        {
            if (!dataTable.HasErrors) return;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(
                CultureInfo.CurrentCulture,
                "ConstraintException while  filling {0}",
                dataTable.TableName);
            DataRow[] errorRows = dataTable.GetErrors();
            for (int i = 0; (i < MAX_ERRORS_TO_LOG) && (i < errorRows.Length); i++)
            {
                sb.AppendLine();
                sb.Append(errorRows[i].RowError);
            }
            System.Windows.MessageBox.Show("'dbManager'\n" + sb.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        internal void boundingProcess(Process pro, int RID, short pos)
        {

            try
            {
                resVorg.Insert(RID, String.Format("{0:D12}{1:D5}", Convert.ToInt32(pro.OrderNumber), Convert.ToInt32(pro.ExecutionNumber)),
                    DateTime.Now, "", pos, 1, null, DateTime.Now);
            }
            catch (Exception)
            {
                LogErrors(_tablesDataSet);
            }
        }

        internal void changeBoudedProcess(Process pro, int? newRID, short? newPOS, float? korrect)
        {
            try
            {
                String vid = String.Format("{0:D12}{1:D5}", Convert.ToInt32(pro.OrderNumber), Convert.ToInt32(pro.ExecutionNumber));
                DataSetEL2.tblRessourceVorgangRow Row = _tablesDataSet.tblRessourceVorgang.Single(x => x.VID == vid);

                if (newRID != null)
                {
                    Row.RID = (int)newRID;
                }
                if (newPOS != null)
                {
                    Row.SPOS = (short)newPOS;

                }
                if (korrect != null)
                {
                    Row.korrect = (int)korrect;
                }

                resVorg.Update(_tablesDataSet.tblRessourceVorgang);
            }
            catch (Exception)
            {

                LogErrors(_tablesDataSet);
            }
        }

        internal void removeBoundedProcess(Process pro, int RID)
        {
            //TODO: Check its coorect
            try
            {
                String Original_VID = String.Format("{0:D12}{1:D5}", Convert.ToInt32(pro.OrderNumber), Convert.ToInt32(pro.ExecutionNumber));
                resVorg.DeleteByRID(RID);
            }
            catch (Exception)
            {

                LogErrors(_tablesDataSet);
            }
        }


        internal DataSetPerspectives.PerspectiveDataTable getPerspectives(string User)
        {
            DataSetPerspectivesTableAdapters.QueriesTableAdapter ta = new DataSetPerspectivesTableAdapters.QueriesTableAdapter();
            return (DataSetPerspectives.PerspectiveDataTable)ta.PerspectiveUser(User);
        }
        internal DataSetEL2.RessZuteilViewDataTable getRessZuteilView()
        {
            return _tablesDataSet.RessZuteilView;
        }

        internal DataTable getProjects()
        {
            return _tablesDataSet.tblProjekt;
        }
        internal DataSetPermission.RolesDataTable getRoles()
        {
            return _permissionDataSet.Roles;
        }
        internal List<Stripe> getPauseList()
        {
            List<Stripe> ret = new List<Stripe>();
            foreach (DataRow r in _pauseAdapt.GetData())
            {
                StripePause st = new StripePause((int)r["Anfang"], (int)r["Ende"]);
                st.ToolTip = (String)r["Bemerkung"];
                ret.Add(st);
            }
            return ret;
        }

        internal DataSetEL2.OrderListDataTable getOrderList(String AID)
        {
            _orderList.Fill(_tablesDataSet.OrderList, AID);
            return _orderList.GetData(AID);
        }
        internal DataTable changeRessOfBID(int BID)
        {
            _bid = BID;
            _ressOfBID.Fill(_tablesDataSet.RessourceOfBID, BID);
            return _tablesDataSet.RessourceOfBID;
        }
        internal DataSetEL2.RessZuteilDataTable getRessZuteil(int Bid)
        {
            DataTable dt = _tablesDataSet.tblRessourceVorgang.GetChanges();
            return _ressZuteil.GetData(Bid);
        }
        internal DataTable getResZut(int RID)
        {
            _resZut.Fill(_tablesDataSet.RessZut, RID);

            return _resZut.GetData(RID);
        }
        internal DataRow[] changeRessZuteil(int BID, bool isNew)
        {
            DataRow[] ret;
            _bid = BID;
            _ressZuteil.Fill(_tablesDataSet.RessZuteil, BID);
            if (isNew)
            {
                ret = _tablesDataSet.RessZuteil.Select();
            }
            else
            {
                ret = _tablesDataSet.RessZuteil.Select("VGRID IS NULL");
            }
            return ret;
        }
        internal List<Stripe> getRessKapaList(DateTime Dt, int RID)
        {
            List<Stripe> ret = new List<Stripe>();
            DataRow dr = _kappaAdapt.GetData().Where(x => x.RID == RID && x.Datum == Dt).SingleOrDefault();

            if (dr != null)
            {
                int? start = dr["start1"] as int?;
                int? end = dr["end1"] as int?;
                String comment = dr["comment1"] as String;
                if (start != null && end != null) ret.Add(new ShiftOne((int)start, (int)end, comment));

                start = dr["start2"] as int?;
                end = dr["end2"] as int?;
                comment = dr["comment2"] as String;
                if (start != null && end != null) ret.Add(new ShiftTwo((int)start, (int)end, comment));

                start = dr["start3"] as int?;
                end = dr["end3"] as int?;
                comment = dr["comment3"] as String;
                if (start != null && end != null) ret.Add(new ShiftThree((int)start, (int)end, comment));

            }
            return ret;

        }
        internal List<DateTime> getRessKapaDates(int rid)
        {
            DateUtils.CalendarWeek week = DateUtils.GetGermanCalendarWeek(DateTime.Now);
            DateTime monday = DateUtils.GetMonday(week.Year, week.Week);
            return _kappaAdapt.GetData().Where(x => x.RID == rid && x.Datum >= monday).Select(y => y.Datum).ToList<DateTime>();

        }

        internal DataSetEL2TableAdapters.tblRessKappaTableAdapter getRessKappa(int Rid)
        {
            return _kappaAdapt;
        }

        internal void SyncronizeRessKapa(int RID, SortedDictionary<DateTime, DayLine> dl)
        {
            DataSetEL2.tblRessKappaRow dt = null;
            try
            {
                foreach (DayLine d in dl.Values)
                {
                    dt = _tablesDataSet.tblRessKappa.AsParallel().Where(x => x.Datum == d.Day && x.RID == RID && x.updated != d.Updated).Single();
                    if (dt != null) UpdateRessKappa(RID, d);

                }

                DateTime[] f = getRessKapaDates(RID).Except(dl.Keys).ToArray();
                foreach (DateTime da in f)
                {
                    _kappaAdapt.DeleteByRID_Datum(RID, da.ToString());
                }
            }
            catch (Exception)
            {

                LogErrors(_tablesDataSet);
            }
        }


        internal void UpdateRessKappa(int RID, DayLine dl)
        {
            try
            {
                DataSetEL2.tblRessKappaRow row = _tablesDataSet.tblRessKappa.Where(x => x.Datum == dl.Day
            && x.RID == RID).SingleOrDefault();
                if (row == null) return;
                DateTime? r = null;

                r = (row.IsupdatedNull()) ? (DateTime?)null : row.updated;

                if (r != dl.Updated)
                {

                    if (dl.Where(x => x.Type == 1).Count() > 0)
                    {
                        row.start1 = dl.Where(x => x.Type == 1).First().Start;
                        row.end1 = dl.Where(x => x.Type == 1).First().End;
                        row.comment1 = dl.Where(x => x.Type == 1).First().Comment;
                    }
                    else { row.Setstart1Null(); row.Setend1Null(); row.Setcomment1Null(); }

                    if (dl.Where(x => x.Type == 2).Count() > 0)
                    {
                        row.start2 = dl.Where(x => x.Type == 2).First().Start;
                        row.end2 = dl.Where(x => x.Type == 2).First().End;
                        row.comment2 = dl.Where(x => x.Type == 2).First().Comment;
                    }
                    else { row.Setstart2Null(); row.Setend2Null(); row.Setcomment2Null(); }

                    if (dl.Where(x => x.Type == 3).Count() > 0)
                    {
                        row.start3 = dl.Where(x => x.Type == 3).First().Start;
                        row.end3 = dl.Where(x => x.Type == 3).First().End;
                        row.comment3 = dl.Where(x => x.Type == 3).First().Comment;
                    }
                    else { row.Setstart3Null(); row.Setend3Null(); row.Setcomment3Null(); }
                    row.updated = dl.Updated;
                    _kappaAdapt.Update(row);
                }
            }
            catch (Exception)
            {

                LogErrors(_tablesDataSet);
            }
        }

        internal void InsertRessKappa(int rid, DayLine dl)
        {
            try
            {
                Stripe s1 = dl.Where(x => x.Type == 1).SingleOrDefault();
                Stripe s2 = dl.Where(x => x.Type == 2).SingleOrDefault();
                Stripe s3 = dl.Where(x => x.Type == 3).SingleOrDefault();

                _kappaAdapt.Insert(rid, dl.Day,
                    (s1 == null) ? (int?)null : s1.Start, (s1 == null) ? (int?)null : s1.End, (s1 == null) ? null : s1.Comment,
                    (s2 == null) ? (int?)null : s2.Start, (s2 == null) ? (int?)null : s2.End, (s2 == null) ? null : s2.Comment,
                    (s3 == null) ? (int?)null : s3.Start, (s3 == null) ? (int?)null : s3.End, (s3 == null) ? null : s3.Comment,
                    DateTime.Now, null);
            }
            catch (Exception)
            {

                LogErrors(_tablesDataSet);
            }
        }
        // TODO implemnt Delete DatabaseRow
        internal void Delete(DayLine dl)
        {
            DataSetEL2.tblRessKappaRow row = _tablesDataSet.tblRessKappa.Where(x => x.Datum == dl.Day).Single();
            //_kappaAdapt.DeleteByID(row.ID);

        }

        internal short getMaxSPOS(int rid)
        {
            List<DataSetEL2.tblRessourceVorgangRow> dt = resVorg.GetData().Select("RID=" + rid).Cast<DataSetEL2.tblRessourceVorgangRow>().ToList();
            short result = 0;
            if (dt.Count > 0) result = dt.Max(x => x.SPOS);
            return result;
        }

        internal DataSetEL2.RessZuteilDataTable UpdateZuteil(int VGRID, int RID, short newPos)
        {
            try
            {
                DataSetEL2.tblRessourceVorgangRow drow = resVorg.GetData().Where(x => x.VGRID == VGRID).SingleOrDefault();
                if (drow != null)
                {
                    drow.SPOS = newPos;
                    drow.RID = RID;

                    resVorg.Update(drow);
                }
                return _ressZuteil.GetData(_bid);
            }
            catch (SqlException e)
            {
                System.Windows.MessageBox.Show("Fehler bei SQLUpdate: " + e.ErrorCode);
                return _ressZuteil.GetData(_bid);
            }

        }
        internal DataSetEL2.RessZuteilDataTable DeleteZuteil(int VGRID)
        {

            //resVorg.DeleteQuery(VGRID);
            return _ressZuteil.GetData(_bid);
        }

        internal EnumerableRowCollection getUsers()
        {
            return user.GetData().OrderBy(x => x.Name);
        }
        internal DataSetEL2.tblRessourceDataTable getResources()
        {

            res.Fill(_tablesDataSet.tblRessource);
            return _tablesDataSet.tblRessource;
        }

        internal DataRow getVorgSelect(String VID)
        {
            return lie.GetData().Where(x => x.VID == VID).SingleOrDefault();
        }

        internal void UpdateVorgang(string vid, string field, object value)
        {

            DataSetEL2.tblVorgangRow dr = lie.GetData().Select("VID='" + vid + "'").SingleOrDefault() as DataSetEL2.tblVorgangRow;
            if (dr != null)
            {

                switch (field.ToUpper())
                {
                    case "BEM_M":
                        dr.Bem_M = Convert.ToString(value);
                        break;
                    case "BEM_MA":
                        dr.Bem_MA = Convert.ToString(value);
                        break;
                    case "BEM_T":
                        dr.Bem_T = Convert.ToString(value);
                        break;
                    case "TERMIN":
                        dr.Termin = Convert.ToDateTime(value);
                        break;
                    case "AUSGEBL":
                        dr.ausgebl = Convert.ToBoolean(value);
                        break;
                    case "MARKER":
                        dr.marker = Convert.ToString(value);
                        break;
                }

                lie.Update(dr);
            }
        }
        internal bool UpdateOrder(String AID, String field, Object value)
        {
            SqlConnection con = new SqlConnection(Properties.Settings.Default.DB_COS_LIEFERLISTE_SQLConnectionString);
            con.Open();
            int res = 0;
            switch (field.ToUpper())
            {
                case "ABGESCHLOSSEN":

                    SqlCommand cmd = new SqlCommand("dbo.Archivate", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@resultVal", res);
                    cmd.Parameters.AddWithValue("@aid", AID);
                    cmd.Parameters.AddWithValue("@boolValue", Convert.ToBoolean(value));
                    cmd.Parameters.AddWithValue("@boolForce", false);

                    cmd.ExecuteNonQuery();
                    if (res > 0)
                    {
                        if (MessageBox.Show(string.Format("Achtung!\nEs gibt noch {d} Vorgänge mit offener Menge.\n\n"
                         + "Soll trotzdem abgelegt werden?", res), "Warnung", MessageBoxButton.YesNo, MessageBoxImage.Warning)
                            == MessageBoxResult.Yes)
                        {
                            cmd.Parameters.RemoveAt(2);
                            cmd.Parameters.AddWithValue("@boolforce", true);
                            cmd.ExecuteNonQuery();
                            if (res != 0)
                            {
                                MessageBox.Show("Es ist ein Fehler beim ablegen aufgetreten!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    break;

                case "MAPPE":
                    SqlCommand Mcmd = new SqlCommand("UPDATE dbo.tblAuftrag SET Mappe=" + Convert.ToByte(Convert.ToBoolean(value))
                        + " WHERE AID='" + AID + "'", con);
                    Mcmd.ExecuteNonQuery();
                    break;
                case "LIEFERTERMIN":
                    SqlCommand Lcmd = new SqlCommand("UPDATE dbo.tblAuftrag SET LieferTermin='" + Convert.ToString(value)
                        + "' WHERE AID='" + AID + "'", con);
                    Lcmd.ExecuteNonQuery();
                    break;
                case "PRIO":
                    SqlCommand Pcmd = new SqlCommand("UPDATE dbo.tblAuftrag SET Dringend=" + Convert.ToByte(Convert.ToBoolean(value))
                        + " WHERE AID='" + AID + "'", con);
                    Pcmd.ExecuteNonQuery();
                    break;
                case "PRIOTXT":
                    SqlCommand Ptcmd = new SqlCommand("UPDATE dbo.tblAuftrag SET Bemerkung='" + Convert.ToString(value)
                        + "' WHERE AID='" + AID + "'", con);
                    Ptcmd.ExecuteNonQuery();
                    break;
            }
            con.Close();
            return res == 0;
        }



        internal object getHeaderInfo(string VID)
        {

            Header.Fill(_tablesDataSet.OrderHeaderByVID, VID);
            return _tablesDataSet.OrderHeaderByVID;
        }

        internal DataSetEL2.lieferlisteDataTable getLieferliste()
        {


            return _tablesDataSet.lieferliste;
        }

        internal DataSetPermission.PermissionUSERDataTable getPermissions(string USERID)
        {
            _permissionDataSet.PermissionUSER.Where(r => ((string)r["UserIdent"]) == USERID);
            return _permissionDataSet.PermissionUSER;
        }
    }
}
