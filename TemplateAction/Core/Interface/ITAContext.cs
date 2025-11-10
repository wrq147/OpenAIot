using System;
using System.Collections;
using System.Text;

namespace TemplateAction.Core
{
    public interface ITAContext
    {
        
        ITARequest Request { get; }
        ITAResponse Response { get; }
        /// <summary>
        /// 获取当前的应用程序，包含模板和插件
        /// </summary>
        /// <returns></returns>
        TASiteApplication Application { get; }
        /// <summary>
        ///  HTTP 请求过程中数据共享
        /// </summary>
        IDictionary Items { get; }
        string Version { get; }
        ITASession Session { get; }
        /// <summary>
        /// url解码
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        string UrlDecode(string str, Encoding encoding);

        /// <summary>
        /// 相对路径转绝对路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string MapPath(string path);

        /// <summary>
        /// 存在则直接返回，不存在再创建
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ITACookie CreateCookie(string name);
        ITACookie CreateCookie(string name, string encodekey);
        bool ExistCookie(string name);
        void SaveCookie(ITACookie cookie);
    }
}
