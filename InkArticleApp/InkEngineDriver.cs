using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Input.Inking;
using Windows.UI.Input.Inking.Analysis;

namespace InkArticleApp
{
    class InkEngineDriver
    {
        InkPresenter inkPresenter;
        InkAnalyzer inkAnalyzer;
        InkAnalysisResult inkAnalysisResults;

        public InkEngineDriver()
        {
            inkPresenter.StrokesCollected += InkPresenter_StrokesCollected;
            inkPresenter.StrokesErased += InkPresenter_StrokesErased;
            inkPresenter.StrokeInput.StrokeStarted += StrokeInput_StrokeStarted;
            inkPresenter.StrokeInput.StrokeEnded += StrokeInput_StrokeEnded;
        }

        private void StrokeInput_StrokeEnded(InkStrokeInput sender, Windows.UI.Core.PointerEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void StrokeInput_StrokeStarted(InkStrokeInput sender, Windows.UI.Core.PointerEventArgs args)
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
