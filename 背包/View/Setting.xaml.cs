using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using 背包.Cookie_Service;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace 背包.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Setting : Page
    {
        public Setting()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            WallSet.IsOn = CookieStorage.ReadWallSet();
            LocateSet.IsOn = CookieStorage.ReadLocateSet();
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
        private void WallSet_Toggled(object sender, RoutedEventArgs e)
        {
            CookieStorage.LocalWallSet(WallSet.IsOn);
        }

        private void LocateSet_Toggled(object sender, RoutedEventArgs e)
        {
            CookieStorage.LocalLocateSet(LocateSet.IsOn);
        }
    }
}
