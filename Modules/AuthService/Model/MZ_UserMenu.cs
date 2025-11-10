using System;
namespace AuthService
{
    public class MZ_UserMenu : MZ_Menu
    {
        /// <summary>
        /// 0为允许，1为禁止
        /// </summary>
        public int RightType { get; set; }
    }
}
