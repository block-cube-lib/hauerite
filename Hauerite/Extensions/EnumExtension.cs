using System;
using System.ComponentModel;
using System.Linq;

namespace Hauerite.Extensions
{
    /// <summary>
    /// Enumの拡張関数を定義する
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// EnumのDescriptionAttributeに設定された文字列を取得する
        /// </summary>
        /// <typeparam name="T">Enum</typeparam>
        /// <param name="value">Enum型の変数</param>
        /// <returns>
        /// EnumのDescriptionAttributeに設定された文字列。
        /// 存在しない場合はToStringの結果を返す
        /// </returns>
        public static string GetDiscription<T>(this T value)
            where T : struct, Enum
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            var description = attributes.Select(n => n.Description).FirstOrDefault();

            if (description == null)
            {
                return value.ToString();
            }

            return description;
        }
    }
}
