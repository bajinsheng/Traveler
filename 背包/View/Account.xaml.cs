using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Phone.UI.Input;
using Windows.Security.Authentication.OnlineId;
using Windows.Storage;
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

    public sealed partial class Account : Page
    {
        #region 字段
        OnlineIdAuthenticator _authenticator = new OnlineIdAuthenticator();     
        Uri _unknownUserUri = new Uri("ms-appdata:///Assets/user.png");
        Uri knownUserUri = new Uri("ms-appdata:///local/UserIcon.png");
        string AccessToken;
        bool isChangUser;
        static string URI_API_LIVE = "https://apis.live.net/v5.0/";
        static string URI_PICTURE = URI_API_LIVE + "me/picture?access_token=";
        static string URI_USER_INFO = URI_API_LIVE + "me?access_token=";
        #endregion
        public Account()
        {
            this.InitializeComponent();
        }
        #region 交互函数
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            if(CookieStorage.ReadId() != null)
            {
                UserName.Text = CookieStorage.ReadName();
                UserPicture.ImageSource = new BitmapImage(knownUserUri);
                LogOut.Visibility = Visibility.Visible;
                LogIn.Visibility = Visibility.Collapsed;
                IconBorder.Visibility = Visibility.Visible;
            }
            else
            {
                LogOut.Visibility = Visibility.Collapsed;
                LogIn.Visibility = Visibility.Visible;
                IconBorder.Visibility = Visibility.Collapsed;
                UserName.Text = "";
                UserPicture.ImageSource = new BitmapImage(_unknownUserUri);
            }
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {          
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }
        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)//重写后退键
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
        private async void Log_Click(object sender, RoutedEventArgs e)
        {
            LoginProgressBar.Visibility = Visibility.Visible;
            filter.Visibility = Visibility.Visible;
            var targetArray = new List<OnlineIdServiceTicketRequest>();
            targetArray.Add(new OnlineIdServiceTicketRequest("wl.basic wl.offline_access", "DELEGATION"));
            AccessToken = null;
            try
            {
                var result = await _authenticator.AuthenticateUserAsync(targetArray, CredentialPromptType.PromptIfNeeded);
                if (result.Tickets[0].Value != string.Empty)
                {
                    AccessToken = result.Tickets[0].Value;
                    var json =await GetUserInfo(AccessToken);
                    isChangUser = true;
                    await StorageIcon(AccessToken);
                    StorageInfo(AccessToken,json);
                    LogOut.Visibility = Visibility.Visible;
                    LogIn.Visibility = Visibility.Collapsed;
                    IconBorder.Visibility = Visibility.Visible;
                    LoginProgressBar.Visibility = Visibility.Collapsed;
                    filter.Visibility = Visibility.Collapsed;
                }
                else
                {
                    // errors are to be handled here.
                    await new MessageDialog("Unable to get the ticket. Error: " + result.Tickets[0].ErrorCode.ToString()).ShowAsync();
                }
            }
            catch (System.OperationCanceledException)
            {
                // errors are to be handled here.
                new MessageDialog("Operation canceled.").ShowAsync();
            }
            catch (System.Exception ex)
            {
                // errors are to be handled here.
                new MessageDialog("Unable to get the ticket. Exception: " + ex.Message).ShowAsync();
            }
        }
        private async void LogOut_Click(object sender, RoutedEventArgs e)
        {            
            AccessToken = null;
            CookieStorage.DeleteCookie();
            UserName.Text = "";
            UserPicture.ImageSource = null;
            //UserPicture.Source = new BitmapImage(_unknownUserUri);
            await new MessageDialog("Signed Out!").ShowAsync();           
            LogOut.Visibility = Visibility.Collapsed;
            LogIn.Visibility = Visibility.Visible;
            IconBorder.Visibility = Visibility.Collapsed;
        }
        #endregion
        
        #region 账户相关操作
        public async Task<JsonObject> GetUserInfo(string token)//得到用户信息并进行显示
        {
            var uri = new Uri(URI_USER_INFO + token);
            var client = new HttpClient();
            var result = await client.GetAsync(uri);

            string jsonUserInfo = await result.Content.ReadAsStringAsync();
            if (jsonUserInfo != null)
            {
                var json = JsonObject.Parse(jsonUserInfo);
                UserName.Text = json["name"].GetString();
                UserPicture.ImageSource = new BitmapImage(new Uri(URI_PICTURE + token));
                return json;               
            }
            else
                return null;
        }
        private async void StorageInfo(string token,JsonObject userinfo)//将用户信息进行本地存储并验证上传
        {
            CookieStorage.LocalId(userinfo["id"].GetString());//本地缓存用户id
            CookieStorage.LocalName(userinfo["name"].GetString());//本地存储用户名
            User ne = await User_service.Query(userinfo["id"].GetString());
            if (ne != null)//如果账号已存在 则删除掉重新插入
            {
                await User_service.Delete(ne); 
            }
            User_service.Insert(userinfo["id"].GetString(), userinfo["name"].GetString());
        }
        private async Task StorageIcon(string token)//存储用户图片头像
        {
            var uri = new Uri(URI_PICTURE + token);
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

                    var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("UserIcon.png", CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteBytesAsync(file, allBytes.ToArray());
                }
            }
        }
        
        
        #endregion


    }
}
