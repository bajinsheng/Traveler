using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;
using 背包.Model;

namespace 背包.MobileService
{
    public static class User_service
    {
	//Please instead the storage connecting string with your azure account
        //static string StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=bagblob;AccountKey=Your Key";
        private static async Task<bool> UploadToAzureStorage(string id)//上传图像
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("users");
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(id + ".png");
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync("UserIcon.png");
                MemoryStream ms = new MemoryStream();
                await blockBlob.UploadFromFileAsync(file);
                return true;
            }
            catch (Exception e)
            {
                new MessageDialog(e.Message).ShowAsync();
                return false;
            }
        }
        private static async void DeleteAzureStorage(string id)//删除图像
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("users");
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(id + ".png");
                await blockBlob.DeleteAsync();
            }
            catch (Exception e)
            {
                new MessageDialog("删除图片出错：" + e.Message).ShowAsync();
            }
        }
        public static async Task Delete(User user)//删除用户
        {
            IMobileServiceTable<User> userTable = App.MobileService.GetTable<User>();
            await userTable.DeleteAsync(user);
            DeleteAzureStorage(user.UserName);
            
        }
        public static async void Insert(string userid,string username)
        {
            try
            {
                User todoItem = new User() { UserId = userid, UserName = username};
                IMobileServiceTable<User> userTable = App.MobileService.GetTable<User>();
                await userTable.InsertAsync(todoItem);
            }
            catch(Exception e)
            {
                new MessageDialog("用户信息存储失败！" + e.Message).ShowAsync();
            }
            bool x = await UploadToAzureStorage(username);
            if(x)
            {
                await new MessageDialog("登陆成功！").ShowAsync();
            }
            
        }
        public static async Task<User> Query(string userid)
        {
            try
            {
                IMobileServiceTable<User> userTable = App.MobileService.GetTable<User>();
                List<User> result = await userTable.Where((r) => r.UserId == userid).ToListAsync();
                if (result.Count == 0)
                    return null;
                else
                {
                    User a = result.FirstOrDefault();
                    return a;
                }
            }
            catch(Exception e)
            {
                new MessageDialog(e.Message).ShowAsync();
                return null;
            }
        }
        public static async Task<string> QueryConvert(string username)
        {
            IMobileServiceTable<User> userTable = App.MobileService.GetTable<User>();
            List<User> result = await userTable.Where((r) => r.UserName == username).ToListAsync();
            User a = result.FirstOrDefault();
            return a.UserId;
        }
       
    }
}
