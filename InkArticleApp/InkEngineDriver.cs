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
        private TextBlock _animationLabel;
        private bool IsLiveRecognitionOn;
        private Type _callerType;
        ProcessAnimatorTimer randomTextAnimation { get; }
        RecognitionProcessTimer recognitionTimer { get; }
        private string _recognizedText;

        public string recognizedText
        {
            get { return _recognizedText; }
            set { Set(ref _recognizedText, value); }
        }


        public InkEngineDriver(Type callerType, ref TextBlock animationLabel, InkPresenter inkPresenter)
        {
            _callerType = callerType;
            if (_callerType == typeof(MainPage))
            {
                IsLiveRecognitionOn = true;
            }
            else
            {
                IsLiveRecognitionOn = false;
            }
            _inkPresenter = inkPresenter;
            _animationLabel = animationLabel;
            _inkPresenter.StrokesCollected += InkPresenter_StrokesCollected;
            _inkPresenter.StrokesErased += InkPresenter_StrokesErased;
            _inkPresenter.StrokeInput.StrokeStarted += StrokeInput_StrokeStarted;
            _inkPresenter.StrokeInput.StrokeEnded += StrokeInput_StrokeEnded;
            inkAnalyzer = new InkAnalyzer();
            recognitionTimer = new RecognitionProcessTimer(_inkPresenter, inkAnalyzer);
            randomTextAnimation = new ProcessAnimatorTimer();
        }

        private void StrokeInput_StrokeEnded(InkStrokeInput sender, PointerEventArgs args)
        {
            _animationLabel.Text = "Find";
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
            if (IsLiveRecognitionOn)
            {
                inkAnalyzer.AddDataForStrokes(args.Strokes);
                ActiveRecognition();
            }
        }

        private async void ActiveRecognition()
        {
            recognitionTimer.StartTimer();
            await Task.Delay(800);
            WriteText();
            _inkPresenter.StrokeContainer.DeleteSelected();
        }

        public async void PassiveRecognition()
        {
            inkAnalyzer.AddDataForStrokes(_inkPresenter.StrokeContainer.GetStrokes());
            ActiveRecognition();
        }

        private void WriteText()
        {
            if (recognitionTimer.RecognizedText != null && recognitionTimer.RecognizedText.Length > 0)
            {
                recognizedText += " " + recognitionTimer.RecognizedText;
                recognizedText = recognizedText.Trim();
                recognitionTimer.RecognizedText = null;
                Debug.WriteLine(recognizedText);
            }
            else
            {
                Debug.WriteLine("Not Recognized");
            }
            }
    }
}
