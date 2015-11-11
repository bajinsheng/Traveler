using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using 背包.Cookie_Service;
using 背包.Model;

namespace 背包.MobileService
{
    public static class Comment_service
    {
        public static async Task Insert(string id, string text)//添加评论
        {
            Comment todoItem = new Comment() { IdArticle = id, Content = text, Author = CookieStorage.ReadName(), Time = DateTime.Now };
            IMobileServiceTable<Comment> userTable = App.MobileService.GetTable<Comment>();
            await userTable.InsertAsync(todoItem);
            await new MessageDialog("评论成功！").ShowAsync();
        }
        public static async Task<List<Comment>> Read(string id)//读取当前文章所有评论
        {
            IMobileServiceTable<Comment> userTable = App.MobileService.GetTable<Comment>();
            List<Comment> r = await userTable.Where((item) => item.IdArticle == id).OrderByDescending((item) => item.Time).ToListAsync();
            if (r == null)
                return null;
            return r;
        }
    }
}
