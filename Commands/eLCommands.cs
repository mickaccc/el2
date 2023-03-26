using System.Windows.Input;

namespace Lieferliste_WPF.Commands
{
    public class ELCommands
    {
        private static RoutedUICommand showOrderView;
        private static RoutedUICommand toArchive;
        private static RoutedUICommand showArchive;
        private static RoutedUICommand removeImage;
        private static RoutedUICommand rotateFriendImage90;
        private static RoutedUICommand rotateFriendImage180;
        private static RoutedUICommand rotateFriendImage270;
        private static RoutedUICommand gotoNextFriend;
        private static RoutedUICommand gotoPreviousFriend;
        private static RoutedUICommand listSortAscending;
        private static RoutedUICommand listSortDescending;
        private static RoutedUICommand listExportXPS;

        static ELCommands()
        {
            showOrderView = new RoutedUICommand("Auftrag anzeigen", "ShowOrderView", typeof(ELCommands));
            showOrderView.InputGestures.Add(new KeyGesture(Key.A, ModifierKeys.Alt));
            toArchive = new RoutedUICommand("Auftrag ablegen", "ToAchive", typeof(ELCommands));
            //newFriend.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Alt | ModifierKeys.Control));
            //newFriend.InputGestures.Add(new MouseGesture(MouseAction.LeftDoubleClick, ModifierKeys.Alt | ModifierKeys.Control));
            showArchive = new RoutedUICommand("Archiv anzeigen", "NewImage", typeof(ELCommands));
            showArchive.InputGestures.Add(new KeyGesture(Key.I, ModifierKeys.Alt | ModifierKeys.Control));
            removeImage = new RoutedUICommand("Bild entfernen", "RemoveImage", typeof(ELCommands));
            removeImage.InputGestures.Add(new KeyGesture(Key.X, ModifierKeys.Alt | ModifierKeys.Control));
            rotateFriendImage90 = new RoutedUICommand("Bild um 90 Grad drehen", "RotateImage90", typeof(ELCommands));
            rotateFriendImage90.InputGestures.Add(new KeyGesture(Key.R, ModifierKeys.Alt | ModifierKeys.Control));
            rotateFriendImage180 = new RoutedUICommand("Bild um 180 Grad drehen", "RotateImage180", typeof(ELCommands));
            rotateFriendImage270 = new RoutedUICommand("Bild um 270 Grad drehen", "RotateImage270", typeof(ELCommands));
            gotoNextFriend = new RoutedUICommand("Vorwärts", "GotoNextFriend", typeof(ELCommands));
            gotoPreviousFriend = new RoutedUICommand("Rückwärts", "GotoPreviousFriend", typeof(ELCommands));
            listSortAscending = new RoutedUICommand("Aufsteigend sortieren", "ListSortAscending", typeof(ELCommands));
            listSortDescending = new RoutedUICommand("Absteigend sortieren", "ListSortDescending", typeof(ELCommands));
            listExportXPS = new RoutedUICommand("Liste als XPS exportieren", "ListExportXPS", typeof(ELCommands));
            listExportXPS.InputGestures.Add(new KeyGesture(Key.E, ModifierKeys.Control));
        }
        public static RoutedUICommand ShowOrderView
        {
            get { return showOrderView; }
        }
        public static RoutedUICommand ToArchive
        {
            get { return toArchive; }
        }
        public static RoutedUICommand ShowArchive
        {
            get { return showArchive; }
        }
        public static RoutedUICommand RemoveImage
        {
            get { return removeImage; }
        }
        public static RoutedUICommand RotateImage90
        {
            get { return rotateFriendImage90; }
        }
        public static RoutedUICommand RotateImage180
        {
            get { return rotateFriendImage180; }
        }
        public static RoutedUICommand RotateImage270
        {
            get { return rotateFriendImage270; }
        }
        public static RoutedUICommand GotoNextFriend
        {
            get { return gotoNextFriend; }
        }
        public static RoutedUICommand GotoPreviousFriend
        {
            get { return gotoPreviousFriend; }
        }
        public static RoutedUICommand ListSortAscending
        {
            get { return listSortAscending; }
        }
        public static RoutedUICommand ListSortDescending
        {
            get { return listSortDescending; }
        }
        public static RoutedUICommand ListExportXPS
        {
            get { return listExportXPS; }
        }
    }
}
