namespace JP.DataHub.ManageApi.Service.DymamicApi.Scripting.Roslyn
{
    /// <summary>
    /// QueryStringからキーを探した結果インターフェース
    /// </summary>
    public interface IKeyValueResult
    {
        /// <summary>
        /// キーが存在したか
        /// </summary>
        bool HasKey { get; set; }
        /// <summary>
        /// キーが存在し、その値を型Tに変換できたか？
        /// </summary>
        bool IsValid { get; set; }
        /// <summary>
        /// 値
        /// </summary>
        string Source { get; set; }
        /// <summary>
        /// 値を型Tに変換したオブジェクト
        /// </summary>
        object Object { get; set; }
        /// <summary>
        /// 型T
        /// </summary>
        Type Type { get; set; }
    }

    /// <summary>
    /// QueryStringからキーを探した結果クラスのgeneric版
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class KeyValueResult<T> : IKeyValueResult
    {
        /// <summary>
        /// キーが存在したか
        /// </summary>
        public bool HasKey { get; set; }
        /// <summary>
        /// キーが存在し、その値を型Tに変換できたか？
        /// </summary>
        public bool IsValid { get; set; }
        /// <summary>
        /// 値
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// 値を型Tに変換したオブジェクト
        /// </summary>
        public object Object { get; set; }
        /// <summary>
        /// 値を型Tに変換したもの
        /// </summary>
        public T Value { get => Object == null ? default(T) : (T)Object; set => Object = value; }
        /// <summary>
        /// 型T
        /// </summary>
        public Type Type { get; set; }
    }
}