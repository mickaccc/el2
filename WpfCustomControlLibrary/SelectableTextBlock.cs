using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace WpfCustomControlLibrary
{
    public class SelectableTextBlock : TextBlock
    {
        private static readonly Type TextEditorType
= Type.GetType("System.Windows.Documents.TextEditor, PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
        private static readonly PropertyInfo IsReadOnlyProp
            = TextEditorType.GetProperty("IsReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly PropertyInfo TextViewProp
            = TextEditorType.GetProperty("TextView", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly MethodInfo RegisterMethod
            = TextEditorType.GetMethod("RegisterCommandHandlers",
            BindingFlags.Static | BindingFlags.NonPublic, null, new[] { typeof(Type), typeof(bool), typeof(bool), typeof(bool) }, null);
        private static readonly Type TextContainerType
            = Type.GetType("System.Windows.Documents.ITextContainer, PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
        private static readonly PropertyInfo TextContainerTextViewProp
            = TextContainerType.GetProperty("TextView");
        private static readonly PropertyInfo TextContainerTextSelectionProp
            = TextContainerType.GetProperty("TextSelection");
        private static readonly PropertyInfo TextContainerProp = typeof(TextBlock).GetProperty("TextContainer", BindingFlags.Instance | BindingFlags.NonPublic);

        private static void RegisterCommandHandlers(Type controlType, bool acceptsRichContent, bool readOnly, bool registerEventListeners)
        {
            RegisterMethod.Invoke(null, new object[] { controlType, acceptsRichContent, readOnly, registerEventListeners });
        }

        static SelectableTextBlock()
        {
            FocusableProperty.OverrideMetadata(typeof(SelectableTextBlock), new FrameworkPropertyMetadata(true));
            RegisterCommandHandlers(typeof(SelectableTextBlock), true, true, true);

            // remove the focus rectangle around the control
            FocusVisualStyleProperty.OverrideMetadata(typeof(SelectableTextBlock), new FrameworkPropertyMetadata((object)null));
        }

        //private readonly TextEditorWrapper _editor;
        private object? textContainer;
        private object? editor;
        public TextSelection TextSelection { get; private set; }

        public SelectableTextBlock()
        {
            textContainer = TextContainerProp.GetValue(this);

            editor = Activator.CreateInstance(TextEditorType, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance,
                null, new[] { textContainer, this, false }, null);


            IsReadOnlyProp.SetValue(editor, true);
            TextViewProp.SetValue(editor, TextContainerTextViewProp.GetValue(textContainer));

            TextSelection = (TextSelection)TextContainerTextSelectionProp.GetValue(textContainer);
            TextSelection.Changed += (s, e) => OnSelectionChanged?.Invoke(this, e);
        }

        public event EventHandler OnSelectionChanged;
    }
}
