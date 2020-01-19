using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Input.Inking;
using Windows.UI.Input.Inking.Analysis;

namespace InkArticleApp
{
    class RecognitionEngine
    {
        InkAnalyzer _inkAnalyzer;
        InkPresenter _inkPresenter;
        private InkAnalysisResult inkAnalysisResults;
        public InkAnalysisLine Node { get; set; }

        public RecognitionEngine(InkPresenter inkPresenter, InkAnalyzer inkAnalyzer)
        {
            _inkPresenter = inkPresenter;
            _inkAnalyzer = inkAnalyzer;
        }

        public async Task<string> StartRecognitionAsync()
        {
            inkAnalysisResults = await _inkAnalyzer.AnalyzeAsync();
            if (inkAnalysisResults.Status == InkAnalysisStatus.Updated)
            {
                Node = Recognize();
                if (Node != null)
                {
                    string recognizedText = Node.RecognizedText;
                    _inkAnalyzer.ClearDataForAllStrokes();
                    return recognizedText;
                }
            }
            return null;
        }

        public InkAnalysisLine Recognize()
        {
            var inkwordNodes =
                    _inkAnalyzer.AnalysisRoot.FindNodes(
                        InkAnalysisNodeKind.Line);

            if (inkwordNodes.Count == 0)
            {
                return null;
            }

            foreach (InkAnalysisLine node in inkwordNodes)
            {
                RemoveAnalyzedStrokes(node);
                return node;
            }

            return null; //the program should never reach this line. Only adding this for compiler satisfaction
        }

        public void RemoveAnalyzedStrokes(InkAnalysisLine node)
        {
            foreach (var strokeId in node.GetStrokeIds())
            {
                var stroke =
                    _inkPresenter.StrokeContainer.GetStrokeById(strokeId);
                stroke.Selected = true;
            }
        } 
    }
}
