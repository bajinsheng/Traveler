using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;
using 背包.Cookie_Service;
using 背包.Model;

namespace 背包.MobileService
{
    public static class Travel_service
    {
	//Please instead the storage connecting string with your azure account
        //static string StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=bagblob;AccountKey=Your Key";
        private static async Task<bool> UploadToAzureStorage(string id)//上传图像
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("travels");
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(id + ".png");
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync("photo.png");
                MemoryStream ms = new MemoryStream();               
                await blockBlob.UploadFromFileAsync(file);
                return true;
            }
            catch(Exception e)
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
                CloudBlobContainer container = blobClient.GetContainerReference("travels");
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(id + ".png");
                await blockBlob.DeleteAsync(); 
            }
            catch (Exception e)
            {
                new MessageDialog("删除图片出错："+e.Message).ShowAsync();
            }
        }
        public static async Task<bool> Insert(string title)//插入新旅程
        {
            IMobileServiceTable<Travel> userTable = App.MobileService.GetTable<Travel>();
            List<Travel> query = await userTable.Where((item) => item.Title == title).ToListAsync();
            if (query.Count > 0)
            {
                await new MessageDialog("标题已存在！").ShowAsync();
                return false;
            }
            else
            {
                Travel todoItem = new Travel() { Title = title, LocationOrAuthor = CookieStorage.ReadName(), Time = DateTime.Now, IdParent = "head" };
                await userTable.InsertAsync(todoItem);
                return true;
            }
        }
        public static async Task<bool> Insert(string idparent, string location, string title)//插入新节点
        {
            IMobileServiceTable<Travel> travelTable = App.MobileService.GetTable<Travel>();
            Travel todoItem = new Travel() { Title = title, IdParent = idparent, LocationOrAuthor = location, Time = DateTime.Now };
            await travelTable.InsertAsync(todoItem);
            List<Travel> head = await travelTable.Where((item) => item.Title == title && item.IdParent == idparent).ToListAsync();
            Travel a = head.FirstOrDefault();
            bool x = await UploadToAzureStorage(a.Id);
            if (x)
            {
                await new MessageDialog("新消息保存成功！").ShowAsync();
                return true;
            }
            else
                return false;         
        }
        public static async Task<List<Travel>> Query()//查询所有旅程标题
        {
            try
            {
                IMobileServiceTable<Travel> userTable = App.MobileService.GetTable<Travel>();
                List<Travel> r = await userTable.Where((item) => item.IdParent == "head").OrderByDescending((item) => item.Time).ToListAsync();
                return r;
            }
            catch(Exception)
            {
                new MessageDialog("连接服务器失败！").ShowAsync();
            }
            return null;
        }
        public static async Task<List<Travel>> Query(string idparent)//查询当前旅程所有记录
        {
            IMobileServiceTable<Travel> userTable = App.MobileService.GetTable<Travel>();
            List<Travel> r = await userTable.Where((item) => item.IdParent == idparent).OrderByDescending((item) => item.Time).ToListAsync();
            return r;
        }
        public static async Task<Travel> QueryId(string id)//查询一条记录
        {
            IMobileServiceTable<Travel> userTable = App.MobileService.GetTable<Travel>();
            List<Travel> r = await userTable.Where((item) => item.Id == id).ToListAsync();
            Travel a = r.FirstOrDefault();
            return a;
        }
        public static async Task<List<Travel>> QueryUser(string username)//查询当前使用者所有旅程
        {
            IMobileServiceTable<Travel> userTable = App.MobileService.GetTable<Travel>();
            List<Travel> r = await userTable.Where((item) => item.LocationOrAuthor == username && item.IdParent == "head").OrderByDescending((item) => item.Time).ToListAsync();
            return r;
        }
        public static async Task<bool> DeleteTravel(string title)//删除旅程
        {
            IMobileServiceTable<Travel> userTable = App.MobileService.GetTable<Travel>();
            List<Travel> head = await userTable.Where((item) => item.Title == title && item.IdParent == "head").ToListAsync();
            if (head.Count != 1)
            {
                await new MessageDialog("ERROR! 找不到标题或标题不止一个！").ShowAsync();
                return false;
            }
            Travel f = head.FirstOrDefault();
            string id = f.Id;
            await userTable.DeleteAsync(f);
            List<Travel> r = await userTable.Where((item) => item.IdParent == id).ToListAsync();
            foreach (Travel item in r)
            {
                DeleteAzureStorage(item.Id);
                await userTable.DeleteAsync(item);

            }
            await new MessageDialog("删除成功").ShowAsync();
            return true;

        }
        public static async Task Delete(string id)//删除节点
        {
            IMobileServiceTable<Travel> userTable = App.MobileService.GetTable<Travel>();
            List<Travel> head = await userTable.Where((item) => item.Id == id).ToListAsync();
            Travel a = head.FirstOrDefault();
            await userTable.DeleteAsync(a);
            DeleteAzureStorage(id);
            await new MessageDialog("删除成功！").ShowAsync();
        }
    }
    

}
