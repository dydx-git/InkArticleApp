using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace InkArticleApp
{
    public class FindBoxLogic : ObservableObject
    {
        Windows.UI.Color highlightBackgroundColor = (Windows.UI.Color)App.Current.Resources["SystemColorHighlightColor"];
        Windows.UI.Color highlightForegroundColor = (Windows.UI.Color)App.Current.Resources["SystemColorHighlightTextColor"];

        SolidColorBrush defaultBackground = (SolidColorBrush)App.Current.Resources["TextControlBackgroundFocused"];
        SolidColorBrush defaultForeground = (SolidColorBrush)App.Current.Resources["TextControlForeground"];

        private string _text;
        private RichEditTextDocument _document;
        private string _processingLabel;

        public string ProcessingLabel
        {
            get { return _processingLabel; }
            set { Set(ref _processingLabel, value); }
        }


        public string Text
        {
            get { return _text; }
            set { Set(ref _text, value); }
        }

        public FindBoxLogic(RichEditTextDocument Document)
        {
            _document = Document;
        }

        public void FindBoxHighlightMatches()
        {
            FindBoxRemoveHighlights();

            string textToFind = Text;
            if (textToFind != null)
            {
                ITextRange searchRange = _document.GetRange(0, 0);
                while (searchRange.FindText(textToFind, TextConstants.MaxUnitCount, FindOptions.None
                    ) > 0)
                {
                    searchRange.CharacterFormat.BackgroundColor = highlightBackgroundColor;
                    searchRange.CharacterFormat.ForegroundColor = highlightForegroundColor;
                }
            }
        }

        public void FindBoxRemoveHighlights()
        {
            ITextRange documentRange = _document.GetRange(0, TextConstants.MaxUnitCount);
            documentRange.CharacterFormat.BackgroundColor = defaultBackground.Color;
            documentRange.CharacterFormat.ForegroundColor = defaultForeground.Color;
        }
    }
}
