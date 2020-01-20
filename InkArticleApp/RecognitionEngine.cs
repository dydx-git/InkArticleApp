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
        public RecognizedEntity recognizedEntity;

        public RecognitionEngine(InkPresenter inkPresenter, InkAnalyzer inkAnalyzer)
        {
            _inkPresenter = inkPresenter;
            _inkAnalyzer = inkAnalyzer;
            recognizedEntity = new RecognizedEntity();
        }

        public async Task<RecognizedEntity> StartRecognitionAsync(bool IsDrawingOn)
        {
            inkAnalysisResults = await _inkAnalyzer.AnalyzeAsync();
            if (inkAnalysisResults.Status == InkAnalysisStatus.Updated)
            {
                if (IsDrawingOn)
                {
                    recognizedEntity.DrawingNode = RecognizeDrawing();
                    if (recognizedEntity.DrawingNode != null)
                    {
                        string recognizedShape = recognizedEntity.DrawingNode.DrawingKind.ToString();
                        _inkAnalyzer.ClearDataForAllStrokes();
                        IsDrawingOn = false;
                        return recognizedEntity;
                    }
                }
                else
                {
                    recognizedEntity.LineNode = RecognizeText();
                    if (recognizedEntity.LineNode != null)
                    {
                        string recognizedText = recognizedEntity.LineNode.RecognizedText;
                        _inkAnalyzer.ClearDataForAllStrokes();
                        return recognizedEntity;
                    }
                }
            }
            return null;
        }

        public InkAnalysisLine RecognizeText()
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

        public InkAnalysisInkDrawing RecognizeDrawing()
        {
            var inkDrawingNodes =
                _inkAnalyzer.AnalysisRoot.FindNodes(
                    InkAnalysisNodeKind.InkDrawing);

            if (inkDrawingNodes.Count > 0)
            {
                foreach (InkAnalysisInkDrawing node in inkDrawingNodes)
                {
                    if (node.DrawingKind == InkAnalysisDrawingKind.Drawing)
                    {
                        Debug.Write("Shape not recognized");
                    }
                    else
                    {
                        InkAnalysisInkDrawing NodeCopy = node;
                        RemoveAnalyzedStrokes(node);
                        return NodeCopy;
                    }
                }
            }
            return null;
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

        public void RemoveAnalyzedStrokes(InkAnalysisInkDrawing node)
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
