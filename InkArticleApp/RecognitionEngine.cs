using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Input.Inking.Analysis;

namespace InkArticleApp
{
    class RecognitionEngine
    {
        InkAnalyzer _inkAnalyzer;
        public RecognitionEngine(InkAnalyzer inkAnalyzer)
        {
            _inkAnalyzer = inkAnalyzer;
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
                return node;
            }

            //the program should never reach this line. Only adding this for compiler satisfaction
            return null;
        }
    }
}
