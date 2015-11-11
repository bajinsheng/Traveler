using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using 背包.Cookie_Service;
using 背包.MobileService;
using 背包.Model;
using 背包.View;


namespace 背包
{
   
    public sealed partial class MainPage : Page
    {
        int toastset = -1;
        public MainPage()
        {            
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            if (CookieStorage.ReadWallSet())
            {
                WallpaperSet();
            }
                        
        }  
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }
        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            pop.IsOpen = true;
            DispatcherTimer dct = new DispatcherTimer();
            dct.Interval = new TimeSpan(0, 0, 2);
            dct.Tick += dct_Tick;
            dct.Start();
            toastset *= -1;
            st1.Begin();
            if (toastset == -1)
            {
                Application.Current.Exit();
            }
            e.Handled = true;
        }
        void dct_Tick(object sender, object e)
        {
            toastset *= -1;
        }
        private async void MainList_Loaded(object sender, RoutedEventArgs e)//坑爹的数据绑定方法
        {
            if(!IsInternetConnect())
            {
                await new MessageDialog("当前无网络连接！").ShowAsync();
                Application.Current.Exit();
            }
            else
            {
                ListView n = sender as ListView;
                List<Travel> all = await Travel_service.Query();
                n.ItemsSource = all;
                MainPageLoad.Visibility = Visibility.Collapsed;
            }
            
        }

        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            if (CookieStorage.ReadId() != null)
            {
                Frame.Navigate(typeof(Capture));
            }
            else
            {
                await new MessageDialog("请登陆").ShowAsync();
                Frame.Navigate(typeof(Account));
            }
        }

        private async void MyTravel_Click(object sender, RoutedEventArgs e)
        {
            if(CookieStorage.ReadId()!=null)
            {
                Frame.Navigate(typeof(UserTravelPage));
            }
            else
            {
                await new MessageDialog("请登陆").ShowAsync();
                Frame.Navigate(typeof(Account));
            }
            
        }

        private async void AddTravel_Click(object sender, RoutedEventArgs e)
        {
            if(CookieStorage.ReadId()!=null)
            {
                NewPanel.Visibility = Visibility.Visible;
            }
            else
            {
                await new MessageDialog("请登陆").ShowAsync();
                Frame.Navigate(typeof(Account));
            }
            
        }

        private void MainList_ItemClick(object sender, ItemClickEventArgs e)
        {
            Travel present = e.ClickedItem as Travel;
            Frame.Navigate(typeof(DetailPage), present);
        }

        private async void NewTravelSave_Click(object sender, RoutedEventArgs e)
        {
            
            if(Title.Text == "")
            {
                await new MessageDialog("标题不能为空！").ShowAsync();
                return;
            }
            bool a = await Travel_service.Insert(Title.Text);
            Title.Text = "";
            NewPanel.Visibility = Visibility.Collapsed;
            if(a)
            {
                await new MessageDialog("保存成功！").ShowAsync();
            }
            
            Frame.Navigate(typeof(Loading));
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Account));
        }

        private async void Collection_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem item = sender as MenuFlyoutItem;
            Travel a = item.DataContext as Travel;
            List<Travel> col = await CookieStorage.ReadCollection();
            if(col == null)
            {
                col = new List<Travel>();
            }
            col.Add(a);
            CookieStorage.StorageCollection(col);
            await new MessageDialog("收藏成功！").ShowAsync();
        }

        private void MainTravel_Holding(object sender, HoldingRoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if(element != null)
            {
                FlyoutBase.ShowAttachedFlyout(element);
            }
        }

        private void Psotcard_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Postcard));
        }
        private void MyCollection_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MyCollection));
        }




        private bool IsInternetConnect()//判断网络连接情况
        {
            ConnectionProfile a = NetworkInformation.GetInternetConnectionProfile();
            if (a == null)
                return false;
            else
                return true;
        }
        private async void WallpaperSet()//壁纸设置方法
        {
            await DownloadWallpaper();
            MainBackground.ImageSource = new BitmapImage(new Uri("ms-appdata:///local/Wallpaper.png"));
        }
        private async Task DownloadWallpaper()//下载Bing壁纸
        {
            var uri = new Uri(
                "http://appserver.m.bing.net/BackgroundImageService/TodayImageService.svc/GetTodayImage?dateOffset=0&urlEncodeHeaders=true&osName=windowsPhone&osVersion=8.10&orientation=480x800&deviceName=WP8Device&mkt=zh-CN");
            List<Byte> allBytes = new List<byte>();
            using (var responce = await HttpWebRequest.Create(uri).GetResponseAsync())
            {
                using (Stream responseStream = responce.GetResponseStream())
                {
                    byte[] buffer = new byte[3000];
                    int bytesRead = 0;
                    while ((bytesRead = await responseStream.ReadAsync(buffer, 0, 3000)) > 0)
                    {
                        allBytes.AddRange(buffer.Take(bytesRead));
                    }

                    var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("Wallpaper.png", CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteBytesAsync(file, allBytes.ToArray());
                }
            }
        }

       
        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Setting));
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NewPanel.Visibility = Visibility.Collapsed;
            Title.Text = "";
        }

        private void Footprint_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(FootPrint));
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(About));
        }

        

        
    }
}
