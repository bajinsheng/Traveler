using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Email;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;


namespace 背包.View
{
    public sealed partial class Postcard : Page
    {
        private Point currentPoint;
        private Point oldPoint;
        public Postcard()
        {
            this.InitializeComponent();
        }


        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            StatusBar statusbar = StatusBar.GetForCurrentView();
            await statusbar.HideAsync();
        }
        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            StatusBar statusbar = StatusBar.GetForCurrentView();
            await statusbar.ShowAsync(); 
        }
        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            if (frame == null)
            {
                return;
            }

            if (frame.CanGoBack)
            {
                frame.GoBack();
                e.Handled = true;
            }
        }
        #region 手写
        private void Signature_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            currentPoint = e.GetCurrentPoint(Signature).Position;

            Line line = new Line() { X1 = currentPoint.X, Y1 = currentPoint.Y, X2 = oldPoint.X, Y2 = oldPoint.Y };

            line.Stroke = new SolidColorBrush(Colors.Black);
            line.StrokeThickness = 3;
            line.StrokeLineJoin = PenLineJoin.Round;
            line.StrokeStartLineCap = PenLineCap.Round;
            line.StrokeEndLineCap = PenLineCap.Round;
            this.Signature.Children.Add(line);
            oldPoint = currentPoint;
        }

        private void Signature_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            currentPoint = e.GetCurrentPoint(Signature).Position;
            oldPoint = currentPoint;
        }
        #endregion

        private async void SendMail_Click(object sender, RoutedEventArgs e)
        {
            await SavePhoto();
            StorageFile file = await getAttachment();
            SendMail(file);
        }


        private async void SendMail(StorageFile file)
        {
            EmailAttachment emailAttachment = new EmailAttachment(file.Name, file);
            EmailMessage mail = new EmailMessage();
            mail.Attachments.Add(emailAttachment);
            mail.Subject = "明信片";
            mail.Body = "-------发自背包的明信片";
            await EmailManager.ShowComposeNewEmailAsync(mail);
        }
        private async Task SavePhoto()
        {
            RenderTargetBitmap PostCard = new RenderTargetBitmap();
            await PostCard.RenderAsync(LetterPanel);
            var pixelBuffer = await PostCard.GetPixelsAsync();
            IStorageFolder applicationFolder = ApplicationData.Current.LocalFolder;
            IStorageFile saveFile = await applicationFolder.CreateFileAsync("postcard.png", CreationCollisionOption.OpenIfExists);
            using (var fileStream = await saveFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);
                encoder.SetPixelData(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Ignore,
                    (uint)PostCard.PixelWidth,
                    (uint)PostCard.PixelHeight,
                    DisplayInformation.GetForCurrentView().LogicalDpi,
                    DisplayInformation.GetForCurrentView().LogicalDpi,
                    pixelBuffer.ToArray());
                await encoder.FlushAsync();
            }
        }
        private async Task<StorageFile> getAttachment()
        {
            StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.GetFileAsync("postcard.png");
            return file;
        }

    }
}
