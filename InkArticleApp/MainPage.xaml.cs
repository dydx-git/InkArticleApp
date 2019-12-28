namespace InkArticleApp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Windows.Devices.Input;
    using Windows.Foundation;
    using Windows.UI.Input.Inking;
    using Windows.UI.Input.Inking.Core;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;
    using Microsoft.Toolkit.Uwp;
    using Windows.Storage.Pickers;
    using Windows.Storage;
    using Windows.Storage.Provider;
    using Windows.UI.Popups;
    using Windows.UI.Text;
    using Windows.UI.Xaml.Media;
    using System.Diagnostics;
    using Windows.UI.Core;
    using Windows.UI.Input.Inking.Analysis;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Navigation;

    public sealed partial class MainPage : Page
    {
        public RichTextBoxLogic TextBoxLogic { get; }
        public FindBoxLogic FinderLogic { get; }
        ProcessAnimator randomTextAnimation { get; }
        RichEditTextDocument document => (RichEditTextDocument)editor.Document;

        InkPresenter inkPresenter;
        InkAnalyzer inkAnalyzer;
        DispatcherTimer dispatcherTimer;
        InkAnalysisResult inkAnalysisResults;
        

        string RecognizedTextDisplay;

        //string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        //public string animatedWord { get; set; }

        //DispatcherTimer AnimatorTimer;

        public MainPage()
        {
            this.InitializeComponent();

            inkPresenter = inkCanvas.InkPresenter;
            inkPresenter.StrokesCollected += InkPresenter_StrokesCollected;
            inkPresenter.StrokesErased += InkPresenter_StrokesErased;
            inkPresenter.StrokeInput.StrokeStarted += StrokeInput_StrokeStarted;
            inkPresenter.StrokeInput.StrokeEnded += StrokeInput_StrokeEnded;

            inkPresenter.InputDeviceTypes =
                CoreInputDeviceTypes.Pen |
                CoreInputDeviceTypes.Mouse |
                CoreInputDeviceTypes.Touch;

            inkAnalyzer = new InkAnalyzer();
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            randomTextAnimation = new ProcessAnimator();

            //AnimatorTimer = new DispatcherTimer();
            //AnimatorTimer.Tick += AnimatorTimer_Tick;

            // We perform analysis when there has been a change to the
            // ink presenter and the user has been idle for 500ms.
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(500);

            TextBoxLogic = new RichTextBoxLogic(document);
            FinderLogic = new FindBoxLogic(document);

        }

        private void StrokeInput_StrokeEnded(InkStrokeInput sender, PointerEventArgs args)
        {
            findBoxLabel.Text = "Find";
        }

        //private void AnimatorTimer_Tick(object sender, object e)
        //{
        //    AnimatorTimer.Stop();
        //    AnimatorTimer.Interval = TimeSpan.FromMilliseconds(random.Next(100));

        //    char[] alphabets = chars.ToCharArray();

        //    FisherYatesShuffler.Shuffle(alphabets);

        //    animatedWord = new String(alphabets.Take(random.Next(6)).ToArray());

        //    FinderLogic.ProcessingLabel = animatedWord;

        //    AnimatorTimer.Start();
        //}

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            dispatcherTimer.Stop();
        }

        private async void DispatcherTimer_Tick(object sender, object e)
        {
            dispatcherTimer.Stop();
            if (!inkAnalyzer.IsAnalyzing)
            {
                inkAnalysisResults = await inkAnalyzer.AnalyzeAsync();
                RecognizeText();
            }
            else
            {
                // Ink analyzer is busy. Wait a while and try again.
                dispatcherTimer.Start();
            }
        }

        private void StrokeInput_StrokeStarted(InkStrokeInput sender, Windows.UI.Core.PointerEventArgs args)
        {
            // We don't want to process ink while a stroke is being drawn
            dispatcherTimer.Stop();
            randomTextAnimation.StartTimer();
        }

        private void InkPresenter_StrokesErased(InkPresenter sender, InkStrokesErasedEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void InkPresenter_StrokesCollected(InkPresenter sender, InkStrokesCollectedEventArgs args)
        {
            dispatcherTimer.Stop();
            randomTextAnimation.StopTimer();
            //AnimatorTimer.Stop();
            inkAnalyzer.AddDataForStrokes(args.Strokes);
            //RecognizeText();
            dispatcherTimer.Start();
            
        }

        private void RecognizeText()
        {
            // Have ink strokes on the canvas changed?
            //if (inkAnalysisResults.Status == InkAnalysisStatus.Updated)
            //{
            // Find all strokes that are recognized as handwriting and 
            // create a corresponding ink analysis InkWord node.
            var inkwordNodes =
                    inkAnalyzer.AnalysisRoot.FindNodes(
                        InkAnalysisNodeKind.Line);

            // Iterate through each InkWord node.
            // Draw primary recognized text on recognitionCanvas 
            // (for this example, we ignore alternatives), and delete 
            // ink analysis data and recognized strokes.
                if (inkwordNodes.Count == 0)
                {
                    return;
                }
                foreach (InkAnalysisLine node in inkwordNodes)
                {
                    // Draw a TextBlock object on the recognitionCanvas.
                    DrawText(node.RecognizedText, node.BoundingRect);

                    foreach (var strokeId in node.GetStrokeIds())
                    {
                        var stroke =
                            inkCanvas.InkPresenter.StrokeContainer.GetStrokeById(strokeId);
                        stroke.Selected = true;
                    }
                    inkAnalyzer.RemoveDataForStrokes(node.GetStrokeIds());
                }
            //}
        }

        private void textImageButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            RecognizeText();
        }

        private void DrawText(string recognizedText, Rect boundingRect)
        {
            RecognizedTextDisplay = recognizedText;
            RecognizedTextDisplay += " ";
            TextBoxLogic.PlainText += RecognizedTextDisplay;
        }
    }
}