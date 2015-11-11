using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;



namespace 背包.View
{

    public sealed partial class Capture : Page
    {
        BitmapImage backBmp = new BitmapImage();
        BitmapImage frontBmp = new BitmapImage();
        MediaCapture FrontCapture = null;
        MediaCapture BackCapture = null;
        bool IsBackWorking;
        public Capture()
        {
            this.InitializeComponent();
        }
        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.CleanbackCapture();
            this.CleanfrontCapture();
            Windows.UI.ViewManagement.StatusBar statusbar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
            await statusbar.ShowAsync();
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            HardwareButtons.CameraPressed -= HardwareButtons_CameraPressed;
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            Windows.UI.ViewManagement.StatusBar statusbar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
            await statusbar.HideAsync();
            await this.InitializeBackCapture();
            back.Source = BackCapture;
            await BackCapture.StartPreviewAsync();
            IsBackWorking = true;
            HardwareButtons.CameraPressed += HardwareButtons_CameraPressed;
        }


        async void HardwareButtons_CameraPressed(object sender, CameraEventArgs e)
        {
            if (IsBackWorking)
            {
                await backTakePhoto();
                await BackCapture.StopPreviewAsync();
                backResult.Visibility = Visibility.Visible;
                back.Visibility = Visibility.Collapsed;
                this.CleanbackCapture();
                await this.InitializeFrontCapture();
                front.Source = FrontCapture;
                await FrontCapture.StartPreviewAsync();
                IsBackWorking = false;
            }
            else
            {
                await frontTakePhoto();
                frontResult.Visibility = Visibility.Visible;
                front.Visibility = Visibility.Collapsed;
                await FrontCapture.StopPreviewAsync();
                this.CleanfrontCapture();
                take.Visibility = Visibility.Visible;

            }
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

        private async Task InitializeBackCapture()//初始化后置摄像头
        {
            BackCapture = new MediaCapture();
            var deviceCollection = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            var devicebackCamera = deviceCollection.FirstOrDefault(d => d.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Back);
            if (devicebackCamera != null)
            {
                MediaCaptureInitializationSettings setting = new MediaCaptureInitializationSettings();
                setting.AudioDeviceId = "";
                setting.VideoDeviceId = devicebackCamera.Id;
                await BackCapture.InitializeAsync(setting);
            }
            else
            {
                await BackCapture.InitializeAsync();
            }
        }
        private async Task InitializeFrontCapture()//初始化前置摄像头
        {
            FrontCapture = new MediaCapture();
            var deviceCollection = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            var devicefrontCamera = deviceCollection.FirstOrDefault(d => d.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Front);
            if (devicefrontCamera != null)
            {
                MediaCaptureInitializationSettings setting = new MediaCaptureInitializationSettings();
                setting.AudioDeviceId = "";
                setting.VideoDeviceId = devicefrontCamera.Id;
                await FrontCapture.InitializeAsync(setting);
            }
            else
            {
                await BackCapture.InitializeAsync();
            }
        }

        private async void OnTapped(object sender, TappedRoutedEventArgs e)
        {
            RenderTargetBitmap finalimg = new RenderTargetBitmap();
            await finalimg.RenderAsync(PhotoGrid);
            var pixelBuffer = await finalimg.GetPixelsAsync();
            IStorageFolder applicationFolder = ApplicationData.Current.LocalFolder; 
            IStorageFile saveFile = await applicationFolder.CreateFileAsync("photo.png", CreationCollisionOption.OpenIfExists);
            using (var fileStream = await saveFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);
                encoder.SetPixelData(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Ignore,
                    (uint)finalimg.PixelWidth,
                    (uint)finalimg.PixelHeight,
                    DisplayInformation.GetForCurrentView().LogicalDpi,
                    DisplayInformation.GetForCurrentView().LogicalDpi,
                    pixelBuffer.ToArray());
                await encoder.FlushAsync();
            }
            Frame.Navigate(typeof(New), finalimg);
        }
        private void CleanfrontCapture()//清理前置摄像头资源
        {
            if (FrontCapture != null)
            {
                FrontCapture.Dispose();
                FrontCapture = null;
            }
        }
        private void CleanbackCapture()//清理后置摄像头资源
        {
            if (BackCapture != null)
            {
                BackCapture.Dispose();
                BackCapture = null;
            }
        }
        private async Task frontTakePhoto()
        {
            InMemoryRandomAccessStream mmfrontstream = new InMemoryRandomAccessStream();
            InMemoryRandomAccessStream outfrontstream = new InMemoryRandomAccessStream();
            await FrontCapture.CapturePhotoToStreamAsync(Windows.Media.MediaProperties.ImageEncodingProperties.CreateJpeg(), mmfrontstream);
            mmfrontstream.Seek(0); //将指针移到首位置
            await EncodedPhoto(mmfrontstream, outfrontstream, false);
            await frontBmp.SetSourceAsync(outfrontstream);
            frontResult.Source = frontBmp;
            mmfrontstream.Dispose();
            outfrontstream.Dispose();
        }
        private async Task backTakePhoto()
        {
            InMemoryRandomAccessStream mmbackstream = new InMemoryRandomAccessStream();
            await BackCapture.CapturePhotoToStreamAsync(Windows.Media.MediaProperties.ImageEncodingProperties.CreateJpeg(), mmbackstream);
            mmbackstream.Seek(0); //将指针移到首位置
            await backBmp.SetSourceAsync(mmbackstream);
            backResult.Source = backBmp;
            mmbackstream.Dispose();
        }
        private async Task EncodedPhoto(IRandomAccessStream inStream, IRandomAccessStream outStream, bool back)
        {
            // 解码器ID
            Guid JpegDecodderID = BitmapDecoder.JpegDecoderId;
            // 编码器ID
            Guid JpegEncoderID = BitmapEncoder.JpegEncoderId;
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(JpegDecodderID, inStream);
            // 取得像素数据
            byte[] data = (await decoder.GetPixelDataAsync()).DetachPixelData();
            // 创建编码器实例
            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(JpegEncoderID, outStream);
            // 判断屏幕方向
            if (back)
            {
                encoder.BitmapTransform.Rotation = BitmapRotation.Clockwise90Degrees;
            }

            else
            {
                encoder.BitmapTransform.Rotation = BitmapRotation.Clockwise270Degrees;
            }

            // 设置像素数据
            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, decoder.PixelWidth, decoder.PixelHeight, decoder.DpiX, decoder.DpiY, data);
            await encoder.FlushAsync(); //将数据写入到流中
        }
    }
}
