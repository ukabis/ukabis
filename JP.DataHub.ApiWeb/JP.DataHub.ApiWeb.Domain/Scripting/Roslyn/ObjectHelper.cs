using System.ComponentModel;
using JP.DataHub.ApiWeb.Domain.Scripting.Attributes;

namespace JP.DataHub.ApiWeb.Domain.Scripting.Roslyn
{
    [RoslynScriptHelp]
    public static class ObjectHelper
    {
        /// <summary>
        /// T型に合わせて変換するヘルパークラス。変換できない場合はnullを返す
        /// 構文チェック用のダミークラスが[JP.DataHub.ManageApi.Service.DymamicApi.Scripting.Roslyn]に存在します。
        /// パブリックメソッドを追加・削除・変更する場合はダミークラスも同様にしてください。
        /// </returns>
        public static T To<T>(this object obj)
        {
            var result = _convert(obj, typeof(T));
            if (result == null)
            {
                return default(T);
            }
            else
            {
                return (T)result;
            }
        }

        /// <summary>
        /// <ja>T型に合わせて変換する。変換できない場合はnullを返す</ja>
        /// <en>Convert according to T type. Returns null if conversion is not possible</en>
        /// </summary>
        /// <typeparam name="T">
        /// <ja>型T</ja>
        /// <en>Type T</en>
        /// </typeparam>
        /// <param name="obj">
        /// <ja>オブジェクト</ja>
        /// <en>object</en>
        /// </param>
        /// <returns>
        /// <ja>変換した型Tの値</ja>
        /// <en>Converted type T value</en>
        /// </returns>
        public static T Convert<T>(this object obj) => (T)_convert(obj, typeof(T));

        /// <summary>
        /// <ja>Typeに合わせて変換する。変換できない場合はnullを返す</ja>
        /// <en>Convert according to type. Returns null if conversion is not possible</en>
        /// </summary>
        /// <param name="obj">
        /// <ja>オブジェクト</ja>
        /// <en>object</en>
        /// </param>
        /// <param name="type">
        /// <ja>変換する型</ja>
        /// <en>Type to convert</en>
        /// </param>
        /// <returns>
        /// <ja>変換した型Tの値</ja>
        /// <en>Converted type T value</en>
        /// </returns>
        public static object Convert(this object obj, Type type) => _convert(obj, type);

        private static object _convert(object obj, Type type)
        {
            try
            {
                var conv = TypeDescriptor.GetConverter(type);
                if (conv == null)
                {
                    return null;
                }
                return conv.ConvertFromString(obj?.ToString());
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// <ja>型Tの変換出来るか？</ja>
        /// <en>Is it possible to convert type T?</en>
        /// </summary>
        /// <typeparam name="T">
        /// <ja>型T</ja>
        /// <en>Type T</en>
        /// </typeparam>
        /// <param name="obj">
        /// <ja>オブジェクト</ja>
        /// <en>object</en>
        /// </param>
        /// <returns>
        /// <ja>変換できたかを返す。true変換できる。false変換できない</ja>
        /// <en>Returns whether the conversion was successful. can be converted to true. false Cannot be converted</en>
        /// </returns>
        public static bool IsValid<T>(this object obj)
        {
            // string型には何でも変換ができる
            if (typeof(T) == typeof(string))
            {
                return true;
            }
            // Nullableか？Nullableなら型変換が出来ない場合はnullとするのでtrueを返す
            var type = typeof(T);
            if (type.IsGenericType)
            {
                var def = type.GetGenericTypeDefinition();
                if (def.Name.StartsWith("Nullable"))
                {
                    return true;
                }
            }
            // それ以外
            var conv = TypeDescriptor.GetConverter(typeof(T));
            return conv.IsValid(obj);
        }
    }
}