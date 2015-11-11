using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using 背包.MobileService;
using 背包.Model;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace 背包.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class FootPrint : Page
    {
        Geolocator geolocator = null;
        public FootPrint()
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
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            // Map Token for testing purpose,    
            // otherwise you'll get an alart message in Map Control   
            MyMap.MapServiceToken = "NGj5WksCMeAFIY5vFJQ3-Q";
            geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;

            try
            {
                // Getting Current Location   
                Geoposition geoposition = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10));

                MapIcon mapIcon = new MapIcon();
                // Locate your MapIcon   
                mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/mygeo.png"));
                // Show above the MapIcon   
                mapIcon.Title = "我的位置";
                // Setting up MapIcon location   
                mapIcon.Location = new Geopoint(new BasicGeoposition()
                {
                    //Latitude = geoposition.Coordinate.Latitude, [Don't use]   
                    //Longitude = geoposition.Coordinate.Longitude [Don't use]   
                    Latitude = geoposition.Coordinate.Point.Position.Latitude,
                    Longitude = geoposition.Coordinate.Point.Position.Longitude
                });
                // Positon of the MapIcon   
                mapIcon.NormalizedAnchorPoint = new Point(0.5, 0.5);
                MyMap.MapElements.Add(mapIcon);
                List<Geolocation> allsite = await Geolocator_service.Read();
                foreach(Geolocation item in allsite)
                {
                    MapIcon mapItem = new MapIcon();
                    // Locate your MapIcon   
                    mapItem.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/geo.png"));
                    // Show above the MapIcon   
                    mapItem.Title = item.Name;
                    // Setting up MapIcon location   
                    mapItem.Location = new Geopoint(new BasicGeoposition()
                    {
                        //Latitude = geoposition.Coordinate.Latitude, [Don't use]   
                        //Longitude = geoposition.Coordinate.Longitude [Don't use]   
                        Latitude = item.Latitude,
                        Longitude = item.Longitude
                    });
                    // Positon of the MapIcon   
                    //mapItem.NormalizedAnchorPoint = new Point(0.5, 0.5);
                    MyMap.MapElements.Add(mapItem);
                }
                // Showing in the Map   
                //
            }
            catch (UnauthorizedAccessException)
            {
                new MessageDialog("定位服务被关闭!");
            }   
        }

        private async void LocateMe_Click(object sender, RoutedEventArgs e)
        {
            Geoposition pos = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10));
            await MyMap.TrySetViewAsync(new Geopoint(new BasicGeoposition()
            {
                //Latitude = geoposition.Coordinate.Latitude, [Don't use]   
                //Longitude = geoposition.Coordinate.Longitude [Don't use]   
                Latitude = pos.Coordinate.Point.Position.Latitude,
                Longitude = pos.Coordinate.Point.Position.Longitude
            }), 10D, 0, 0, MapAnimationKind.Bow);
        }
    }
}
