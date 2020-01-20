using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Input.Inking.Analysis;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

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
        BingImageSearch imageSearch;

        Symbol CalligraphicPenIcon = (Symbol)0xEDFB;
        Symbol LassoSelect = (Symbol)0xEF20;
        Symbol TouchWriting = (Symbol)0xED5F;

        public ImmersiveView()
        {
            this.InitializeComponent();

            inkPresenter = inkCanvas.InkPresenter;

            inkCanvas.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Mouse | CoreInputDeviceTypes.Pen;

            TextBoxLogic = new RichTextBoxLogic(document);

            Type callerType = typeof(ImmersiveView);

            driver = new InkEngineDriver(callerType, ref processingLabel, inkPresenter, selectionCanvas);

            imageSearch = new BingImageSearch();

            // Customize the ruler
            var ruler = new InkPresenterRuler(inkCanvas.InkPresenter);
            ruler.BackgroundColor = Windows.UI.Colors.PaleTurquoise;
            ruler.ForegroundColor = Windows.UI.Colors.MidnightBlue;
            ruler.Length = 800;
            ruler.AreTickMarksVisible = false;
            ruler.IsCompassVisible = false;

            // Customize the protractor
            var protractor = new InkPresenterProtractor(inkCanvas.InkPresenter);
            protractor.BackgroundColor = Windows.UI.Colors.Bisque;
            protractor.ForegroundColor = Windows.UI.Colors.DarkGreen;
            protractor.AccentColor = Windows.UI.Colors.Firebrick;
            protractor.AreRaysVisible = false;
            protractor.AreTickMarksVisible = false;
            protractor.IsAngleReadoutVisible = false;
            protractor.IsCenterMarkerVisible = false;
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

        private void DrawEllipse(InkAnalysisInkDrawing shape)
        {
            var points = shape.Points;
            Ellipse ellipse = new Ellipse();

            ellipse.Width = shape.BoundingRect.Width;
            ellipse.Height = shape.BoundingRect.Height;

            Canvas.SetTop(ellipse, shape.BoundingRect.Top);
            Canvas.SetLeft(ellipse, shape.BoundingRect.Left);

            var brush = new SolidColorBrush(Windows.UI.ColorHelper.FromArgb(255, 0, 0, 255));
            ellipse.Stroke = brush;
            ellipse.StrokeThickness = 2;
            recognitionCanvas.Children.Add(ellipse);
        }

        // Draw a polygon on the recognitionCanvas.
        private void DrawPolygon(InkAnalysisInkDrawing shape)
        {
            List<Point> points = new List<Point>(shape.Points);
            Polygon polygon = new Polygon();

            foreach (Point point in points)
            {
                polygon.Points.Add(point);
            }

            var brush = new SolidColorBrush(Windows.UI.ColorHelper.FromArgb(255, 0, 0, 255));
            polygon.Stroke = brush;
            polygon.StrokeThickness = 2;
            recognitionCanvas.Children.Add(polygon);
        }

        public string DrawShape(InkAnalysisInkDrawing RShape)
        {
            if (RShape == null)
            {
                return "";
            }
            if (RShape.DrawingKind == InkAnalysisDrawingKind.Circle
                    || RShape.DrawingKind == InkAnalysisDrawingKind.Ellipse)
            {
                DrawEllipse(RShape);
            }
            else
            {
                DrawPolygon(RShape);
            }
            return "Hey";
        }

        private void GetShape_Click(object sender, RoutedEventArgs e)
        {
            InkAnalysisInkDrawing shape = driver.GetRecognizedShape();
            DrawShape(shape);
        }

        private void NavigatePage_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void textImageButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedText = editor.Document.Selection.Text;
            string imageUrl = imageSearch.FindUrlOfImage(selectedText);
            mainSplitView.IsPaneOpen = true;
            foundObjectImage.Source = new BitmapImage(new Uri(imageUrl, UriKind.Absolute));
        }

        private void ToggleTheme_Click(object sender, RoutedEventArgs e)
        {
            string theme = this.RequestedTheme.ToString();
            if (theme == "Light")
            {
                this.RequestedTheme = ElementTheme.Dark;
                
            }
            else
            {
                this.RequestedTheme = ElementTheme.Light;
            }
        }

        private void toggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (toggleButton.IsChecked == true)
            {
                inkCanvas.InkPresenter.InputDeviceTypes |= CoreInputDeviceTypes.Touch;
            }
            else
            {
                inkCanvas.InkPresenter.InputDeviceTypes &= ~CoreInputDeviceTypes.Touch;
            }
        }
    }
}
