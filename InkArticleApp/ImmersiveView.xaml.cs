using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InkArticleApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ImmersiveView : Page
    {
        public RichTextBoxLogic TextBoxLogic { get; }
        RichEditTextDocument document => (RichEditTextDocument)editor.Document;
        InkEngineDriver driver { get; }
        InkPresenter inkPresenter;

        Symbol CalligraphicPenIcon = (Symbol)0xEDFB;
        Symbol LassoSelect = (Symbol)0xEF20;
        Symbol TouchWriting = (Symbol)0xED5F;

        public ImmersiveView()
        {
            this.InitializeComponent();

            inkPresenter = inkCanvas.InkPresenter;

            inkPresenter.InputDeviceTypes =
                CoreInputDeviceTypes.Pen |
                CoreInputDeviceTypes.Mouse |
                CoreInputDeviceTypes.Touch;

            TextBoxLogic = new RichTextBoxLogic(document);

            Type callerType = typeof(ImmersiveView);

            driver = new InkEngineDriver(callerType, ref processingLabel, inkPresenter, selectionCanvas);
        }

        private void SetFocusToText(object sender, RoutedEventArgs e)
        {
            editor.Focus(FocusState.Keyboard);
        }

        private void editor_LostFocus(object sender, RoutedEventArgs e)
        {
            processingLabel.Focus(FocusState.Programmatic);
        }

        private void CurrentToolChanged(InkToolbar sender, object args)
        {
            bool enabled = sender.ActiveTool.Equals(toolButtonLasso);

            CutButton.IsEnabled = enabled;
            CopyButton.IsEnabled = enabled;
            PasteButton.IsEnabled = enabled;
        }
    }
}
