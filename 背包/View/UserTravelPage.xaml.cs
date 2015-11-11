using System.Collections.Generic;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using 背包.Cookie_Service;
using 背包.MobileService;
using 背包.Model;


namespace 背包.View
{

    public sealed partial class UserTravelPage : Page
    {
        public UserTravelPage()
        {
            this.InitializeComponent();
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)//导航至本页面绑定相关数据
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            string author = CookieStorage.ReadName();
            List<Travel> userall = await Travel_service.QueryUser(author);
            if(userall.Count == 0)
            {
                EmptyIcon.Visibility = Visibility.Visible;
            }
            else
            {
                UserDetailList.ItemsSource = userall;
                UserDetailTitle.Text = author;
            }           
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
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

        private void UserTravel_Holding(object sender, HoldingRoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if(element != null)
            {
                FlyoutBase.ShowAttachedFlyout(element);
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            UploadProgressBar.Visibility = Visibility.Visible;
            filter.Visibility = Visibility.Visible;
            MenuFlyoutItem item = sender as MenuFlyoutItem;
            Travel d = (Travel)item.DataContext;
            await Travel_service.DeleteTravel(d.Title);
            UploadProgressBar.Visibility = Visibility.Collapsed;
            filter.Visibility = Visibility.Collapsed;
            string author = CookieStorage.ReadName();
            List<Travel> userall = await Travel_service.QueryUser(author);
            UserDetailList.ItemsSource = userall;
        }

        private async void icon_Tapped(object sender, TappedRoutedEventArgs e)
        {
            iconboard.Begin();
            string author = CookieStorage.ReadName();
            List<Travel> userall = await Travel_service.QueryUser(author);
            UserDetailList.ItemsSource = userall;
            UserDetailTitle.Text = author;
        }

        private void UserDetailList_ItemClick(object sender, ItemClickEventArgs e)
        {
            Travel present = e.ClickedItem as Travel;
            Frame.Navigate(typeof(UserTravelDetail), present);
        }     
      
      

    }
}
