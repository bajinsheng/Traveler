using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Windows.UI.Popups;
using 背包.Cookie_Service;
using 背包.Model;

namespace 背包.MobileService
{
    public static class Geolocator_service
    {
        public static async Task Insert(double latitude, double longitude)//上传地理位置
        {
            try
            {
                Geolocation todoItem = new Geolocation() { Name = CookieStorage.ReadName(), Latitude = latitude, Longitude = longitude };
                IMobileServiceTable<Geolocation> geoTable = App.MobileService.GetTable<Geolocation>();
                await geoTable.InsertAsync(todoItem);
            }
            catch(Exception e)
            {
                new MessageDialog("地理位置上传失败：" + e.ToString());
            }
            
        }
        public static async Task<List<Geolocation>> Read()//读取所有地理位置
        {
            try
            {
                IMobileServiceTable<Geolocation> geoTable = App.MobileService.GetTable<Geolocation>();
                List<Geolocation> r = await geoTable.ToListAsync();
                return r;
            }
            catch(Exception e)
            {
                new MessageDialog("地理位置读取失败：" + e.ToString());
                return null;
            }

        }
    }
}
