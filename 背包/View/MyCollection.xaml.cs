using System;
using System.Collections.Generic;
using Windows.Phone.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using 背包.Cookie_Service;
using 背包.Model;

namespace 背包.View
{
    public sealed partial class MyCollection : Page
    {
        public MyCollection()
        {
            this.InitializeComponent();
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            List<Travel> col = await CookieStorage.ReadCollection();
            if(col == null)
            {
                EmptyIcon.Visibility = Visibility.Visible;
            }
            else
            {
                CollectionList.ItemsSource = col;
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

        private void MyCollection_Holding(object sender, HoldingRoutedEventArgs e)
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
            List<Travel> col = await CookieStorage.ReadCollection();
            foreach (Travel a in col)
            {
                if (a.Id == d.Id)
                {
                    d = a;
                }
            }
            if (col == null)
            {
                await new MessageDialog("删除失败！").ShowAsync();
                return;
            }
            col.Remove(d);
            CookieStorage.StorageCollection(col);
            await new MessageDialog("删除成功！").ShowAsync();
            CollectionList.ItemsSource = col;
            return;
        }

        private void CollectionList_ItemClick(object sender, ItemClickEventArgs e)
        {
            Travel present = e.ClickedItem as Travel;
            Frame.Navigate(typeof(DetailPage), present);
        }

        private async void icon_Tapped(object sender, TappedRoutedEventArgs e)
        {
            iconboard.Begin();
            List<Travel> col = await CookieStorage.ReadCollection();
            CollectionList.ItemsSource = col;
        }     
    }
}
