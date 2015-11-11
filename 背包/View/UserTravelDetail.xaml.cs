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

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace 背包.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class UserTravelDetail : Page
    {
        string num;
        public UserTravelDetail()
        {
            this.InitializeComponent();
        }


        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            Travel r = e.Parameter as Travel;
            num = r.Id;
            UserTravelDetailTitle.Text = r.Title;
            List<Travel> detailal = await Travel_service.Query(num);
            if(detailal.Count == 0)
            {
                EmptyIcon.Visibility = Visibility.Visible;
            }
            else
            {
                UserTravelDetailList.ItemsSource = detailal;
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
        private void icon_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void UserTravelDetailItem_Holding(object sender, HoldingRoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element != null)
            {
                FlyoutBase.ShowAttachedFlyout(element);
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem item = sender as MenuFlyoutItem;
            Travel d = (Travel)item.DataContext;
            await Travel_service.Delete(d.Id);
            string author = CookieStorage.ReadName();
            List<Travel> detailal = await Travel_service.Query(num);
            UserTravelDetailList.ItemsSource = detailal;
        }
    }
}
