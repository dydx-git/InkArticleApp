namespace InkArticleApp
{
    using System;
    using Windows.UI.Input.Inking;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Text;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media.Imaging;

    public sealed partial class MainPage : Page
    {
        const string SubscriptionKey = "c9a04275991c49c09b11be302036aa0c";
        const string UriBase = "https://itemimageretriever.cognitiveservices.azure.com/bing/v7.0/images/search";
        public RichTextBoxLogic TextBoxLogic { get; }
        public FindBoxLogic FinderLogic { get; }
        ProcessAnimatorTimer randomTextAnimation { get; }
        InkEngineDriver driver { get; }
        RichEditTextDocument document => (RichEditTextDocument)editor.Document;
        BingImageSearch imageSearch;

        InkPresenter inkPresenter;

        // Icon for calligraphic pen custom button.
        Symbol CalligraphicPenIcon = (Symbol)0xEDFB;

        public MainPage()
        {
            this.InitializeComponent();

            inkPresenter = inkCanvas.InkPresenter;

            inkPresenter.InputDeviceTypes =
                CoreInputDeviceTypes.Pen |
                CoreInputDeviceTypes.Mouse |
                CoreInputDeviceTypes.Touch;

            TextBoxLogic = new RichTextBoxLogic(document);
            FinderLogic = new FindBoxLogic(document);

            Type callerType = typeof(MainPage);

            driver = new InkEngineDriver(callerType, ref findBoxLabel, inkPresenter);

            imageSearch = new BingImageSearch();
        }

        private void textImageButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            string selectedText = editor.Document.Selection.Text;
            string imageUrl = imageSearch.FindUrlOfImage(selectedText);
            mainSplitView.IsPaneOpen = true;
            // Display the first image found.
            foundObjectImage.Source = new BitmapImage(new Uri(imageUrl, UriKind.Absolute));
        }

        private void NavigatePage_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ImmersiveView));
        }

        private void ToggleTheme_Click(object sender, RoutedEventArgs e)
        {
            string theme = this.RequestedTheme.ToString();
            if (theme == "Light")
            {
                this.RequestedTheme = ElementTheme.Dark;
            }
            else
            {
                this.RequestedTheme = ElementTheme.Light;
            }
        }
    }
}