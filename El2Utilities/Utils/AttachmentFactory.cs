using BrendanGrant.Helpers.FileAssociation;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT;

namespace El2Core.Utils
{
    public abstract class AttachmentFactory
    {
        public abstract IDisplayAttachment CreateDisplayAttachment(string link, bool isLink);
        public abstract IDbAttachment CreateDbAttachment(string link, bool isLink);
        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto, PreserveSig = true, SetLastError = false)]
        public static extern IntPtr GetActiveWindow();
        private static BitmapSource? GetIcon(ProgramIcon programIcon)
        {
            try
            {
                Icon? icon = Icon.ExtractAssociatedIcon(programIcon.Path);
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
        public static IDisplayAttachment FloatAttachment(IDisplayAttachment attachment, string? file, bool isLink)
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
                attachment.IsLink = isLink;
            }
            return attachment;
        }
        public static IDbAttachment FloatAttachment(IDbAttachment dbAttachment, string fileString, bool isLink)
        {
            FileInfo fi = new FileInfo(fileString);
            if (fi.Exists)
            {
                if (isLink)
                {
                    dbAttachment.Link = fi.FullName;
                    dbAttachment.IsLink = true;
                    dbAttachment.TimeStamp = DateTime.Now;
                }
                else
                {
                    if (fi.Length < 0x500000)    //Filesize of 5 MiB
                    {

                        MemoryStream ms = new MemoryStream();
                        using (FileStream file = new FileStream(fileString, FileMode.Open, FileAccess.Read))
                            file.CopyTo(ms);
                        dbAttachment.Link = fi.Name;
                        dbAttachment.IsLink= false;
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
            }
            else MessageBox.Show("Datei wurde nicht gefunden", "Datei anfügen", MessageBoxButton.OK, MessageBoxImage.Error);
            return dbAttachment;
        }
        public static void OpenFile(string file, MemoryStream? memoryStream)
        {
            try
            {
                FileInfo fi = new FileInfo(file);
                string filepath;
                if (memoryStream == null)  
                {
                    filepath = fi.FullName;
                }
                else
                {

                    filepath = Path.Combine(Path.GetTempPath(), fi.Name);
                    using FileStream fs = new(filepath, FileMode.Create);
                    memoryStream.CopyTo(fs);
                    fs.Flush();
                    fs.Close();
                }
         
                 new Process() { StartInfo = new ProcessStartInfo(filepath) { UseShellExecute = true } }.Start();
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{0}\n{1}", e.Message, e.InnerException), "OpenStream", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public static async Task<string> GetFilePickerPath()
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            var initializeWithWindowWrapper = openPicker.As<IInitializeWithWindow>();
            initializeWithWindowWrapper.Initialize(GetActiveWindow());
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add("*");
            StorageFile op = await openPicker.PickSingleFileAsync();
            if (op != null) { return op.Path; }
            return string.Empty;
        }


    }
    public interface IDisplayAttachment
    {
        int Id { get; set; }
        string Name { get; set; }
        string? Description { get; set; }
        object? Content { get; set; }
        bool IsLink { get; set; }
    }
    public interface IDbAttachment
    {
        string Link { get; set; }
        bool IsLink { get; set; }
        DateTime TimeStamp { get; set; }
        byte[]? BinaryData { get; set; }
    }

}
