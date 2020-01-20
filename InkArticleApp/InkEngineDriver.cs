using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Input.Inking.Analysis;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace InkArticleApp
{
    class InkEngineDriver : ObservableObject
    {
        private bool isBoundRect;
        private Polyline lasso;
        private Rect boundingRect;
        Canvas _selectionCanvas;
        InkPresenter _inkPresenter;
        InkAnalyzer inkAnalyzer;
        private TextBlock _animationLabel;
        private bool IsLiveRecognitionOn;
        private Type _callerType;
        ProcessAnimatorTimer randomTextAnimation { get; }
        RecognitionProcessTimer recognitionTimer { get; }

        private InkAnalysisInkDrawing _recognizedShape;

        public InkAnalysisInkDrawing RecognizedShape
        {
            get { return _recognizedShape; }
            set { Set(ref _recognizedShape, value); }
        }


        private string _recognizedText;

        public string recognizedText
        {
            get { return _recognizedText; }
            set { Set(ref _recognizedText, value); }
        }


        public InkEngineDriver(Type callerType, ref TextBlock animationLabel, InkPresenter inkPresenter, Canvas selectionCanvas = null)
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
            _selectionCanvas = selectionCanvas;
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
            if (!IsLiveRecognitionOn)
            {
                ClearSelection();
                _inkPresenter.UnprocessedInput.PointerPressed -= UnprocessedInput_PointerPressed;
                _inkPresenter.UnprocessedInput.PointerMoved -= UnprocessedInput_PointerMoved;
                _inkPresenter.UnprocessedInput.PointerReleased -= UnprocessedInput_PointerReleased;
            }
        }

        private void InkPresenter_StrokesErased(InkPresenter sender, InkStrokesErasedEventArgs args)
        {
            if (!IsLiveRecognitionOn)
            {
                ClearSelection();
                _inkPresenter.UnprocessedInput.PointerPressed -= UnprocessedInput_PointerPressed;
                _inkPresenter.UnprocessedInput.PointerMoved -= UnprocessedInput_PointerMoved;
                _inkPresenter.UnprocessedInput.PointerReleased -= UnprocessedInput_PointerReleased;
            }
            
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

        public async void PassiveRecognizeDrawing()
        {
            inkAnalyzer.AddDataForStrokes(_inkPresenter.StrokeContainer.GetStrokes());
            recognitionTimer.IsDrawingOn = true;
            ActiveRecognition();
        }

        public InkAnalysisInkDrawing GetRecognizedShape()
        {
            if (RecognizedShape != null)
            {
                return RecognizedShape;
            }
            return null;
        }

        private void UnprocessedInput_PointerPressed(InkUnprocessedInput sender, PointerEventArgs args)
        {
            lasso = new Polyline()
            {
                Stroke = new SolidColorBrush(Windows.UI.Colors.Blue),
                StrokeThickness = 1,
                StrokeDashArray = new DoubleCollection() { 5, 2 },
            };

            lasso.Points.Add(args.CurrentPoint.RawPosition);
            _selectionCanvas.Children.Add(lasso);
            isBoundRect = true;
        }

        private void UnprocessedInput_PointerMoved(InkUnprocessedInput sender, PointerEventArgs args)
        {
            if (isBoundRect)
            {
                lasso.Points.Add(args.CurrentPoint.RawPosition);
            }
        }

        private void UnprocessedInput_PointerReleased(InkUnprocessedInput sender, PointerEventArgs args)
        {
            lasso.Points.Add(args.CurrentPoint.RawPosition);

            boundingRect = _inkPresenter.StrokeContainer.SelectWithPolyLine(lasso.Points);
            isBoundRect = false;
            DrawBoundingRect();
        }

        private async void ActiveRecognition()
        {
            recognitionTimer.StartTimer();
            await Task.Delay(800);
            WriteText();
            _inkPresenter.StrokeContainer.DeleteSelected();
            if (recognitionTimer.recognizedEntity != null)
            {
                RecognizedShape = recognitionTimer.recognizedEntity.DrawingNode;
            }
        }

        public async void PassiveRecognition()
        {
            inkAnalyzer.AddDataForStrokes(_inkPresenter.StrokeContainer.GetStrokes());
            ActiveRecognition();
        }

        private void WriteText()
        {
            if (recognitionTimer.recognizedEntity == null)
            {
                return;
            }
            if (recognitionTimer.recognizedEntity.LineNode == null)
            {
                return;
            }
            string NodeText = (recognitionTimer.recognizedEntity != null) ? recognitionTimer.recognizedEntity.LineNode.RecognizedText : null;
            if (NodeText != null && NodeText.Length > 0)
            {
                recognizedText += " " + NodeText;
                recognizedText = recognizedText.Trim();
                NodeText = null;
                Debug.WriteLine(recognizedText);
            }
            else
            {
                Debug.WriteLine("Not Recognized");
            }
        }

        private void ClearDrawnBoundingRect()
        {
            if (_selectionCanvas.Children.Count > 0)
            {
                _selectionCanvas.Children.Clear();
                boundingRect = Rect.Empty;
            }
        }
        private void DrawBoundingRect()
        {
            _selectionCanvas.Children.Clear();

            if (boundingRect.Width <= 0 || boundingRect.Height <= 0)
            {
                return;
            }

            var rectangle = new Rectangle()
            {
                Stroke = new SolidColorBrush(Windows.UI.Colors.Blue),
                StrokeThickness = 1,
                StrokeDashArray = new DoubleCollection() { 5, 2 },
                Width = boundingRect.Width,
                Height = boundingRect.Height
            };

            Canvas.SetLeft(rectangle, boundingRect.X);
            Canvas.SetTop(rectangle, boundingRect.Y);

            _selectionCanvas.Children.Add(rectangle);
        }

        private void ClearSelection()
        {
            var strokes = _inkPresenter.StrokeContainer.GetStrokes();
            foreach (var stroke in strokes)
            {
                stroke.Selected = false;
            }
            ClearDrawnBoundingRect();
        }

        public void ToolButton_Lasso()
        {
            // By default, pen barrel button or right mouse button is processed for inking
            // Set the configuration to instead allow processing these input on the UI thread
            _inkPresenter.InputProcessingConfiguration.RightDragAction = InkInputRightDragAction.LeaveUnprocessed;

            _inkPresenter.UnprocessedInput.PointerPressed += UnprocessedInput_PointerPressed;
            _inkPresenter.UnprocessedInput.PointerMoved += UnprocessedInput_PointerMoved;
            _inkPresenter.UnprocessedInput.PointerReleased += UnprocessedInput_PointerReleased;
        }

        public void OnCopy()
        {
            _inkPresenter.StrokeContainer.CopySelectedToClipboard();
            Debug.WriteLine("OnCopy: " + _inkPresenter.StrokeContainer);
        }

        public void OnCut()
        {
            _inkPresenter.StrokeContainer.CopySelectedToClipboard();
            Debug.WriteLine("OnCut: " + _inkPresenter.StrokeContainer);
            _inkPresenter.StrokeContainer.DeleteSelected();
            ClearDrawnBoundingRect();
        }

        public void OnPaste()
        {
            if (_inkPresenter.StrokeContainer.CanPasteFromClipboard())
            {
                _inkPresenter.StrokeContainer.PasteFromClipboard(new Point(100, 100));
            }
            else
            {
                Debug.WriteLine("Cannot paste from clipboard.");
            }
        }
    }
}
