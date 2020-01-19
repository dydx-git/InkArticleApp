using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
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
        InkEngineDriver driver { get; }
        InkPresenter inkPresenter;
        IReadOnlyList<InkStroke> inkStrokes;
        public ImmersiveView()
        {
            this.InitializeComponent();

            inkPresenter = inkCanvas.InkPresenter;

            inkPresenter.InputDeviceTypes =
                CoreInputDeviceTypes.Pen |
                CoreInputDeviceTypes.Mouse |
                CoreInputDeviceTypes.Touch;

            Type callerType = typeof(ImmersiveView);

            driver = new InkEngineDriver(callerType, ref processingLabel, inkPresenter);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            editor.Focus(FocusState.Keyboard);
            inkCanvas.Visibility = Visibility.Collapsed;
        }
    }
}
