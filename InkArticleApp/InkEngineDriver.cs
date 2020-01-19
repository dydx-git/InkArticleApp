using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Input.Inking.Analysis;
using Windows.UI.Xaml.Controls;

namespace InkArticleApp
{
    class InkEngineDriver : ObservableObject
    {
        InkPresenter _inkPresenter;
        InkAnalyzer inkAnalyzer;
        private TextBlock _findBoxLabel;
        ProcessAnimatorTimer randomTextAnimation { get; }
        RecognitionProcessTimer recognitionTimer { get; }
        private string _recognizedText;

        public string recognizedText
        {
            get { return _recognizedText; }
            set { Set(ref _recognizedText, value); }
        }


        public InkEngineDriver(ref TextBlock findBoxLabel, InkPresenter inkPresenter)
        {
            _inkPresenter = inkPresenter;
            _findBoxLabel = findBoxLabel;
            _inkPresenter.StrokesCollected += InkPresenter_StrokesCollected;
            _inkPresenter.StrokesErased += InkPresenter_StrokesErased;
            _inkPresenter.StrokeInput.StrokeStarted += StrokeInput_StrokeStarted;
            _inkPresenter.StrokeInput.StrokeEnded += StrokeInput_StrokeEnded;
            inkAnalyzer = new InkAnalyzer();
            recognitionTimer = new RecognitionProcessTimer(inkAnalyzer);
            randomTextAnimation = new ProcessAnimatorTimer();
        }

        private void StrokeInput_StrokeEnded(InkStrokeInput sender, PointerEventArgs args)
        {
            _findBoxLabel.Text = "Find";
            recognitionTimer.StopTimer();
        }

        private void StrokeInput_StrokeStarted(InkStrokeInput sender, PointerEventArgs args)
        {
            recognitionTimer.StopTimer();
            randomTextAnimation.StartTimer();
        }

        private void InkPresenter_StrokesErased(InkPresenter sender, InkStrokesErasedEventArgs args)
        {
            throw new NotImplementedException();
        }

        private async void InkPresenter_StrokesCollected(InkPresenter sender, InkStrokesCollectedEventArgs args)
        {
            recognitionTimer.StopTimer();
            randomTextAnimation.StopTimer();
            inkAnalyzer.AddDataForStrokes(args.Strokes);
            recognitionTimer.StartTimer();
            await Task.Delay(800);
            WriteText();
        }

        private void WriteText()
        {
            if (recognitionTimer.RecognizedText != null && recognitionTimer.RecognizedText.Length > 0)
            {
                recognizedText += " " + recognitionTimer.RecognizedText;
            }
        }
    }
}
