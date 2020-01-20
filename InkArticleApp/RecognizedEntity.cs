using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Input.Inking.Analysis;

namespace InkArticleApp
{
    class RecognizedEntity
    {
        public InkAnalysisLine LineNode;
        public InkAnalysisInkDrawing DrawingNode;

        public RecognizedEntity()
        {
            LineNode = null;
            DrawingNode = null;
        }
    }
}
