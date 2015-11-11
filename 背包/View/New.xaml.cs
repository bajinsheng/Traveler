using System;
using System.Collections.Generic;
using System.Net.Http;
using Windows.Data.Json;
using Windows.Devices.Geolocation;
using Windows.Media.SpeechRecognition;
using Windows.Phone.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using 背包.Cookie_Service;
using 背包.MobileService;
using 背包.Model;


namespace 背包.View
{
    public sealed partial class New : Page
    {
        string idparent = "0";
        public New()
        {
            this.InitializeComponent();

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
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            string preauthor = CookieStorage.ReadName();
            List<Travel> mytravel = await Travel_service.QueryUser(preauthor);
            MyTravel.ItemsSource = mytravel;
            RenderTargetBitmap dis = e.Parameter as RenderTargetBitmap;
            Display.Source = dis;
            if (CookieStorage.ReadLocateSet())
            {
                Geocoding();
            }
            else
            {
                Location.Text = "暂时无法获取位置信息";
            }
        }
        private void MyTravel_DropDownClosed(object sender, object e)
        {
            Travel NowTravel = MyTravel.SelectedItem as Travel;
            if(NowTravel != null)
                idparent = NowTravel.Id;
        }
        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            UploadProgressBar.Visibility = Visibility.Visible;
            filter.Visibility = Visibility.Visible;
            if (idparent == "0")
            {
                await new MessageDialog("未选择旅程！").ShowAsync();
                return;
            }
            else
            {
                if (Location.Text != "定位中...." && Location.Text != "暂时无法获取位置信息")
                {
                    await Travel_service.Insert(idparent,Location.Text, InputText.Text);
                }
                else
                {
                    await Travel_service.Insert(idparent, "", InputText.Text);
                }

                Frame.Navigate(typeof(MainPage));
            }

        }
        private async void VoiceInput_Click(object sender, RoutedEventArgs e)
        {
            string message = "";
            try
            {
                SpeechRecognizer speechRecognizer = new SpeechRecognizer();
                SpeechRecognitionCompilationResult compilationResult = await speechRecognizer.CompileConstraintsAsync();

                if (compilationResult.Status == SpeechRecognitionResultStatus.Success)
                {
                    var result = await speechRecognizer.RecognizeWithUIAsync();
                    if (result.Confidence == SpeechRecognitionConfidence.Rejected)
                    {
                        message = "语音识别不到";
                    }
                    else
                    {
                        InputText.Text = result.Text;
                    }
                }

            }
            catch (Exception err)
            {
                message = "异常信息：" + err.Message + err.HResult;
            }
            if (message != "")
                await new MessageDialog(message).ShowAsync();
        }


        #region 定位函数
        private async void Geocoding()
        {
            Geolocator geolocator = new Geolocator();
            Geoposition pos = await geolocator.GetGeopositionAsync();
            await Geolocator_service.Insert(pos.Coordinate.Point.Position.Latitude, pos.Coordinate.Point.Position.Longitude);
            var uri = new Uri(
                "http://api.map.baidu.com/geocoder/v2/?coordtype=wgs84ll&ak=GgLEGGgX8Q1ZIOGTPduNDq24&location="
                + pos.Coordinate.Point.Position.Latitude
                + ","
                + pos.Coordinate.Point.Position.Longitude
                + "&output=json&pois=0");
            var client = new HttpClient();
            var result = await client.GetAsync(uri);
            string jsonUserInfo = await result.Content.ReadAsStringAsync();
            var json = JsonObject.Parse(jsonUserInfo);
            if (json["status"].GetNumber() == 0)
            {
                var tt = json["result"].GetObject();
                Location.Text = tt["formatted_address"].GetString();               
            }
            else
            {
                Location.Text = "暂时无法获取位置信息";
            }
        }
        #endregion


    }
}
