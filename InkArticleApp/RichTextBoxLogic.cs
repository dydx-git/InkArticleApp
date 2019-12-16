using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Popups;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace InkArticleApp
{
    public class RichTextBoxLogic : ObservableObject
    {
        private string _plainText;
        private RichEditTextDocument _document;

        public string PlainText
        {
            get { return _plainText; }
            set { Set(ref _plainText, value); }
        }

        public RichTextBoxLogic(RichEditTextDocument Document)
        {
            _document = Document;
            PlainText = "Heya! Yo yo";
        }

        public void DebugText()
        {
            Debug.Write(PlainText);
        }

        public async void SaveToTextFile() 
        {
            
            if (IsTextBoxEmpty())
            {
                return;
            }

            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.Desktop;

            savePicker.FileTypeChoices.Add("Rich Text", new List<string>() { ".rtf" });

            savePicker.SuggestedFileName = "New Document";

            StorageFile file = await savePicker.PickSaveFileAsync();

            if (file != null)
            {
                CachedFileManager.DeferUpdates(file);

                using (Windows.Storage.Streams.IRandomAccessStream randAccStream =
                    await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    _document.SaveToStream(TextGetOptions.FormatRtf, randAccStream);

                }

                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status != FileUpdateStatus.Complete)
                {
                    MessageDialog errorBox = new MessageDialog($"File {file.Name} couldn't be saved");
                    await errorBox.ShowAsync();
                }
            }
        }

        private bool IsTextBoxEmpty()
        {
            ITextRange documentRange = _document.GetRange(0, TextConstants.MaxUnitCount);
            return documentRange.EndPosition > 1 ? false : true;
        }
    }
}
