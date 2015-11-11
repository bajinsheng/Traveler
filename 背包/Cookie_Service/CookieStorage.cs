using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;
using 背包.Model;

namespace 背包.Cookie_Service
{
    public static class CookieStorage//本地存储相关操作函数
    {
        public static string ReadId()//读取本地存储用户Id
        {
            ApplicationDataContainer localSetting = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSetting.Values.ContainsKey("UserId"))
            {
                string value = localSetting.Values["UserId"].ToString();
                return value;
            }
            else
                return null;           
        }
        public static string ReadName()//读取本地存储用户名
        {
            ApplicationDataContainer localSetting = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSetting.Values.ContainsKey("UserName"))
            {
                string value = localSetting.Values["UserName"].ToString();
                return value;
            }
            else
                return null;
        }
        public static bool LocalId(string id)//本地缓存用户Id
        {
            try
            {
                ApplicationDataContainer localSetting = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSetting.Values["UserId"] = id;
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public static bool LocalName(string name)//本地缓存用户名
        {
            try
            {
                ApplicationDataContainer localSetting = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSetting.Values["UserName"] = name;
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public async static void DeleteCookie()//删除本地缓存
        {
            ApplicationDataContainer localSetting = Windows.Storage.ApplicationData.Current.LocalSettings;
            IStorageFolder applicationFolder = ApplicationData.Current.LocalFolder;
            IStorageFile saveFile = await applicationFolder.GetFileAsync("UserIcon.png");
            
            if (localSetting.Values.ContainsKey("UserId") && localSetting.Values.ContainsKey("UserName"))
            {
                localSetting.Values.Remove("UserId");
                localSetting.Values.Remove("UserName");              
            }
            //try
            //{
            //    await saveFile.DeleteAsync();
            //}
            //catch (Exception e)
            //{
            //    new MessageDialog(e.Message).ShowAsync();
            //}
             
        }
        public async static void StorageCollection(List<Travel> item)//本地存储收藏
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(item.GetType());
            string result = String.Empty;
            using(MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, item);
                ms.Position = 0;
                StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("MyCollection.txt", CreationCollisionOption.ReplaceExisting);
                using (Stream fileStream = await file.OpenStreamForWriteAsync())
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    await ms.CopyToAsync(fileStream);
                    await fileStream.FlushAsync();
                }
            }
        }
        public async static Task<List<Travel>> ReadCollection()//读取本地收藏
        {
            try
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync("MyCollection.txt");
                string fileConten = await FileIO.ReadTextAsync(file);
                DataContractJsonSerializer ds = new DataContractJsonSerializer(typeof(List<Travel>));
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(fileConten));
                List<Travel> obj = (List<Travel>)ds.ReadObject(ms);
                ms.Dispose();
                return obj;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static bool ReadWallSet()//读取壁纸设置
        {
            ApplicationDataContainer localSetting = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSetting.Values.ContainsKey("WallSet"))
            {
                return (bool)localSetting.Values["WallSet"];
            }
            else
                return false;   
        }
        public static bool ReadLocateSet()//读取定位设置
        {
            ApplicationDataContainer localSetting = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSetting.Values.ContainsKey("LocSet"))
            {
                return (bool)localSetting.Values["LocSet"];
            }
            else
                return false;
        }
        public static void LocalWallSet(bool set)//存储动态壁纸设置
        {
            try
            {
                ApplicationDataContainer localSetting = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSetting.Values["WallSet"] = set;
            }
            catch (Exception)
            {
                new MessageDialog("动态壁纸设置时出错！").ShowAsync();
            }
        }
        public static void LocalLocateSet(bool set)//存储定位设置
        {
            try
            {
                ApplicationDataContainer localSetting = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSetting.Values["LocSet"] = set;
            }
            catch (Exception)
            {
                new MessageDialog("定位设置时出错！").ShowAsync();
            }
        }
    }
}
