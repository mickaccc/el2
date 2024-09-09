using BrendanGrant.Helpers.FileAssociation;
using El2Core.Constants;
using El2Core.Models;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Mail;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace El2Core.Utils
{
    public abstract class AttachmentFactory
    {
        public abstract IDisplayAttachment CreateDisplayAttachment(string link, bool isLink);
        public abstract IDbAttachment CreateDbAttachment(string? link, bool isLink);
        private ImageSource? GetIcon(ProgramIcon programIcon)
        {
            try
            {
                Icon? icon = Icon.ExtractIcon(programIcon.Path, programIcon.Index, 32);
                if (icon != null)
                {
                    return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                                icon.Handle,
                                new Int32Rect(0, 0, icon.Width, icon.Height),
                                BitmapSizeOptions.FromEmptyOptions());
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{0}\n{1}", e.Message, e.InnerException), "GetIcon", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return null;
        }
        public IDisplayAttachment FloatAttachment(IDisplayAttachment attachment, string? file, bool isLink)
        {   
            
            FileInfo fi = new FileInfo(file ?? string.Empty);
            var fileass = new FileAssociationInfo(fi.Extension);
            if (fileass.Exists)
            {
                var prog = new ProgramAssociationInfo(fileass.ProgID);
                ImageSource? icon;

                if (prog.Exists)
                {
                    icon = GetIcon(prog.DefaultIcon);
                }
                else icon = new BitmapImage(new Uri("\\Images\\unknown-file.png", UriKind.Relative));

                attachment.Content = icon;
                attachment.Name = (isLink) ? fi.FullName : fi.Name;
            }
            return attachment;
        }
        public IDbAttachment FloatAttachment(IDbAttachment dbAttachment, string fileString)
        {
            FileInfo fi = new FileInfo(fileString);
            if (fi.Exists)
            {
                
                if (fi.Length < 0x500000)    //Filesize of 5 MiB
                {

                    MemoryStream ms = new MemoryStream();
                    using (FileStream file = new FileStream(fileString, FileMode.Open, FileAccess.Read))
                        file.CopyTo(ms);
                    dbAttachment.Link = fi.Name;
                    dbAttachment.BinaryData = ms.ToArray();
                    dbAttachment.TimeStamp = DateTime.Now;
                }
                else if (MessageBox.Show("Die Datei ist größer als 5 MiB, soll es als Link gespeichert werden?", "",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    dbAttachment.Link = fi.FullName;
                    dbAttachment.IsLink = true;
                    dbAttachment.TimeStamp = DateTime.Now;
                }
            }
            else MessageBox.Show("Datei wurde nicht gefunden", "Datei anfügen", MessageBoxButton.OK, MessageBoxImage.Error);
            return dbAttachment;
        }
        private void OnOpenFileExecuted(object obj)
        {
            try
            {
                Attachment att = (Attachment)obj;
                FileInfo fi = new FileInfo(att.Name);
                string filepath;
                if (att.IsLink)
                {
                    filepath = att.Name;
                }
                else
                {
                    var pa = Project.ProjectAttachments.First(x => x.AttachId == att.Ident);
                    using MemoryStream memoryStream = new(pa.AttachmentBin);

                    filepath = Path.Combine(Path.GetTempPath(), fi.Name);
                    using FileStream fs = new(filepath, FileMode.Create);
                    memoryStream.CopyTo(fs);
                    fs.Flush();
                    fs.Close();
                }
                var asso = MiniFileAssociation.Association.GetAssociatedExePath(fi.Extension);

                if (asso != null) new Process() { StartInfo = new ProcessStartInfo(filepath) { UseShellExecute = true } }.Start();
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{0}\n{1}", e.Message, e.InnerException), "OpenStream", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool OnRemoveFileCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.DelProjAttachment);
        }

        private void OnRemoveFileExecuted(object obj)
        {
            var att = (Attachment)obj;
            Attachments.Remove(att);
            var dbAtt = _dbctx.ProjectAttachments.FirstOrDefault(x => x.AttachId == att.Ident);
            if (dbAtt != null)
            {
                _dbctx.ProjectAttachments.Remove(dbAtt);
                _dbctx.SaveChanges();
            }
        }
        private bool OnPrintCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.PrintProj);
        }
    }
    public interface IDisplayAttachment
    {
        string Name { get; set; }
        string? Description { get; set; }
        object? Content { get; set; }
    }
    public interface IDbAttachment
    {
        string? Link { get; set; }
        bool IsLink { get; set; }
        DateTime TimeStamp { get; set; }
        byte[]? BinaryData { get; set; }
    }

}
