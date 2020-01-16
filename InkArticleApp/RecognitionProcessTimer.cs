using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Input.Inking.Analysis;
using Windows.UI.Xaml;

namespace InkArticleApp
{
    class RecognitionProcessTimer
    {
        DispatcherTimer analysisProcessTimer;
        InkAnalyzer _inkAnalyzer;
        InkAnalysisResult inkAnalysisResults;
        public int Duration { get; set; }
        public RecognitionProcessTimer(InkAnalyzer inkAnalyzer)
        {
            this.Duration = 500;
            _inkAnalyzer = inkAnalyzer;
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
                analysisProcessTimer = null;
            }
        }

        private async void AnalysisProcessTimer_Tick(object sender, object e)
        {
            analysisProcessTimer.Stop();
            if (!_inkAnalyzer.IsAnalyzing)
            {
                inkAnalysisResults = await _inkAnalyzer.AnalyzeAsync();
                RecognizeText();
            }
            else
            {
                // Ink analyzer is busy. Wait a while and try again.
                analysisProcessTimer.Start();
            }
        }
    }
}
