using System;
using System.Windows.Input;

namespace Lieferliste_WPF.Commands
{
    public static class eLCommands
    {
        private static RoutedUICommand showOrderView;
        private static RoutedUICommand toArchive;
        private static RoutedUICommand newImage;
        private static RoutedUICommand removeImage;
        private static RoutedUICommand rotateFriendImage90;
        private static RoutedUICommand rotateFriendImage180;
        private static RoutedUICommand rotateFriendImage270;
        private static RoutedUICommand gotoNextFriend;
        private static RoutedUICommand gotoPreviousFriend;
        private static RoutedUICommand listSortAscending;
        private static RoutedUICommand listSortDescending;
        private static RoutedUICommand listExportXPS;

        static eLCommands()
        {
            showOrderView = new RoutedUICommand("Auftrag anzeigen", "ShowOrderView", typeof(eLCommands));
            showOrderView.InputGestures.Add(new KeyGesture(Key.A, ModifierKeys.Alt));
            toArchive = new RoutedUICommand("Auftrag ablegen", "ToAchive", typeof(eLCommands));
            //newFriend.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Alt | ModifierKeys.Control));
            //newFriend.InputGestures.Add(new MouseGesture(MouseAction.LeftDoubleClick, ModifierKeys.Alt | ModifierKeys.Control));
            newImage = new RoutedUICommand("Neues Bild", "NewImage", typeof(eLCommands));
            newImage.InputGestures.Add(new KeyGesture(Key.I, ModifierKeys.Alt | ModifierKeys.Control));
            removeImage = new RoutedUICommand("Bild entfernen", "RemoveImage", typeof(eLCommands));
            removeImage.InputGestures.Add(new KeyGesture(Key.X, ModifierKeys.Alt | ModifierKeys.Control));
            rotateFriendImage90 = new RoutedUICommand("Bild um 90 Grad drehen", "RotateImage90", typeof(eLCommands));
            rotateFriendImage90.InputGestures.Add(new KeyGesture(Key.R, ModifierKeys.Alt | ModifierKeys.Control));
            rotateFriendImage180 = new RoutedUICommand("Bild um 180 Grad drehen", "RotateImage180", typeof(eLCommands));
            rotateFriendImage270 = new RoutedUICommand("Bild um 270 Grad drehen", "RotateImage270", typeof(eLCommands));
            gotoNextFriend = new RoutedUICommand("Vorwärts", "GotoNextFriend", typeof(eLCommands));
            gotoPreviousFriend = new RoutedUICommand("Rückwärts", "GotoPreviousFriend", typeof(eLCommands));
            listSortAscending = new RoutedUICommand("Aufsteigend sortieren", "ListSortAscending", typeof(eLCommands));
            listSortDescending = new RoutedUICommand("Absteigend sortieren", "ListSortDescending", typeof(eLCommands));
            listExportXPS = new RoutedUICommand("Liste als XPS exportieren", "ListExportXPS", typeof(eLCommands));
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
        public static RoutedUICommand NewImage
        {
            get { return newImage; }
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
