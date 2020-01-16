using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Input.Inking.Analysis;
using Windows.UI.Xaml.Controls;

namespace InkArticleApp
{
    class InkEngineDriver
    {
        InkPresenter inkPresenter;
        InkAnalysisResult inkAnalysisResults;
        private TextBlock _findBoxLabel;
        ProcessAnimatorTimer randomTextAnimation { get; }

        RecognitionProcessTimer recognitionTimer;

        public InkEngineDriver(ref TextBlock findBoxLabel)
        {
            _findBoxLabel = findBoxLabel;
            inkPresenter.StrokesCollected += InkPresenter_StrokesCollected;
            inkPresenter.StrokesErased += InkPresenter_StrokesErased;
            inkPresenter.StrokeInput.StrokeStarted += StrokeInput_StrokeStarted;
            inkPresenter.StrokeInput.StrokeEnded += StrokeInput_StrokeEnded;
            recognitionTimer = new RecognitionProcessTimer(inkAnalyzer)
        }

        private void StrokeInput_StrokeEnded(InkStrokeInput sender, PointerEventArgs args)
        {
            _findBoxLabel.Text = "Find";
        }

        private void StrokeInput_StrokeStarted(InkStrokeInput sender, PointerEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void InkPresenter_StrokesErased(InkPresenter sender, InkStrokesErasedEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void InkPresenter_StrokesCollected(InkPresenter sender, InkStrokesCollectedEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
