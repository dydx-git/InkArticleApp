using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Input.Inking;
using Windows.UI.Input.Inking.Analysis;
using Windows.UI.Xaml;

namespace InkArticleApp
{
    class RecognitionProcessTimer : ObservableObject
    {
        DispatcherTimer analysisProcessTimer;
        InkPresenter _inkPresenter;
        InkAnalyzer _inkAnalyzer;
        RecognitionEngine Engine;

        private string _recognizedText;

        public string RecognizedText
        {
            get { return _recognizedText; }
            set { Set(ref _recognizedText, value); }
        }

        public int Duration { get; set; }

        public RecognitionProcessTimer(InkPresenter inkPresenter, InkAnalyzer inkAnalyzer)
        {
            this.Duration = 500;
            _inkPresenter = inkPresenter;
            _inkAnalyzer = inkAnalyzer;
            Engine = new RecognitionEngine(_inkPresenter, _inkAnalyzer);
        }

        public void StartTimer()
        {
            analysisProcessTimer = new DispatcherTimer();
            analysisProcessTimer.Interval = TimeSpan.FromMilliseconds(this.Duration);
            analysisProcessTimer.Tick += AnalysisProcessTimer_Tick;
            analysisProcessTimer.Start();
        }

        public void StopTimer()
        {
            if (analysisProcessTimer != null)
            {
                analysisProcessTimer.Stop();
            }
        }

        private async void AnalysisProcessTimer_Tick(object sender, object e)
        {
            analysisProcessTimer.Stop();
            if (!_inkAnalyzer.IsAnalyzing)
            {
                RecognizedText = await Engine.StartRecognitionAsync();
            }
            else
            {
                // Ink analyzer is busy. Wait a while and try again.
                analysisProcessTimer.Start();
            }
        }
    }
}
