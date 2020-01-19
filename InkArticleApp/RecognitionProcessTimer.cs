using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Input.Inking.Analysis;
using Windows.UI.Xaml;

namespace InkArticleApp
{
    class RecognitionProcessTimer : ObservableObject
    {
        DispatcherTimer analysisProcessTimer;
        InkAnalyzer _inkAnalyzer;
        InkAnalysisResult inkAnalysisResults;
        RecognitionEngine Engine;

        private string _recognizedText;

        public string RecognizedText
        {
            get { return _recognizedText; }
            set { Set(ref _recognizedText, value); }
        }

        public int Duration { get; set; }

        private InkAnalysisLine _node;

        public InkAnalysisLine Node
        {
            get { return _node; }
            set { Set(ref _node, value); }
        }

        public RecognitionProcessTimer(InkAnalyzer inkAnalyzer)
        {
            this.Duration = 500;
            _inkAnalyzer = inkAnalyzer;
            Engine = new RecognitionEngine(_inkAnalyzer);
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
                inkAnalysisResults = await _inkAnalyzer.AnalyzeAsync();
                if (inkAnalysisResults.Status == InkAnalysisStatus.Updated)
                {
                    Node = Engine.Recognize();
                    if (Node != null)
                    {
                        RecognizedText = Node.RecognizedText;
                    }
                }
            }
            else
            {
                // Ink analyzer is busy. Wait a while and try again.
                analysisProcessTimer.Start();
            }
        }
    }
}
