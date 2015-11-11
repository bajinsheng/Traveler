using System;
using System.Collections.Generic;
using Windows.Phone.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using 背包.Cookie_Service;
using 背包.MobileService;
using 背包.Model;


namespace 背包.View
{
    public sealed partial class DetailPage : Page
    {
        string num;
        public DetailPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)//导航至本页面绑定相关数据
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            Travel r = e.Parameter as Travel;
            num = r.Id;
            DetailTitle.Text = r.Title;
            List<Travel> detailal = await Travel_service.Query(num);
            if(detailal.Count == 0)
            {
                EmptyIcon.Visibility = Visibility.Visible;
            }
            else
            {
                DetailList.ItemsSource = detailal;
                DependencyObject t = VisualTreeHelper.GetChild(DetailList, 0);
                t = VisualTreeHelper.GetChild(t, 0);
                t = VisualTreeHelper.GetChild(t, 0);
                t = VisualTreeHelper.GetChild(t, 0);
                t = VisualTreeHelper.GetChild(t, 0);
                DependencyObject pinglun = t;
                t = VisualTreeHelper.GetChild(t, 2);
                ListView n = t as ListView;
                List<Comment> all = await Comment_service.Read(num);
                n.ItemsSource = all;
                StackPanel CommentPanel = pinglun as StackPanel;
                CommentPanel.Visibility = Visibility.Visible;
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

        private async void CommentSend_Click(object sender, RoutedEventArgs e)
        {
            if (CookieStorage.ReadId() != null)
            {
                DependencyObject ParentPanel = VisualTreeHelper.GetParent(sender as DependencyObject);
                DependencyObject CommentInput = VisualTreeHelper.GetChild(ParentPanel,0);
                TextBox Input = CommentInput as TextBox;
                await Comment_service.Insert(num, Input.Text);
                DependencyObject GrandparentPanel = VisualTreeHelper.GetParent(ParentPanel);
                DependencyObject CommentListView = VisualTreeHelper.GetChild(GrandparentPanel, 2);
                ListView CommentList = CommentListView as ListView;
                List<Comment> all = await Comment_service.Read(num);
                CommentList.ItemsSource = all;
            }
            else
            {
                await new MessageDialog("请登陆").ShowAsync();
                Frame.Navigate(typeof(Account));
            }
           
        }

        private async void icon_Tapped(object sender, TappedRoutedEventArgs e)
        {
            iconboard.Begin();
            List<Travel> detailal = await Travel_service.Query(num);
            DetailList.ItemsSource = detailal;
            DependencyObject t = VisualTreeHelper.GetChild(DetailList, 0);
            t = VisualTreeHelper.GetChild(t, 0);
            t = VisualTreeHelper.GetChild(t, 0);
            t = VisualTreeHelper.GetChild(t, 0);
            t = VisualTreeHelper.GetChild(t, 0);
            t = VisualTreeHelper.GetChild(t, 2);
            ListView n = t as ListView;
            List<Comment> all = await Comment_service.Read(num);
            n.ItemsSource = all;
        }


    }
}
