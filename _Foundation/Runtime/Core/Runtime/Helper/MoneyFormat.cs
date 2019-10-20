
namespace SeanLib.Core
{
    /// <summary>
    /// 钱格式转换工具
    /// </summary>
    public static class MoneyFormat
    {
        /// <summary>
        /// 如果钱大于一万，则用万做单位，否则直接表示
        /// </summary>
        /// <param name="money"></param>
        /// <returns></returns>
        public static string ToMoneyW(int money)
        {
            if (money >= 10000)
            {
                float g = (float)money / 10000;
                return g + "万";
            }
            else
            {
                return money.ToString("N0");
            }
        }
    }
}
