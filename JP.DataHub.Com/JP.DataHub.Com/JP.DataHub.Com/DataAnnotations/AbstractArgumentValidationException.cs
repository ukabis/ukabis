using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace JP.DataHub.Com.DataAnnotations
{
    /// <summary>
    /// ドメイン引数基底クラスのバリデーション処理にて検証失敗した際に発生する例外
    /// </summary>
    /// <seealso cref="System.ComponentModel.DataAnnotations.ValidationException" />
    [Serializable]
    public class AbstractArgumentValidationException : ValidationException
    {
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="mssages">エラーメッセージ</param>
        public AbstractArgumentValidationException(params string[] mssages) : base()
        {
            this.Message = mssages;
        }
        #endregion

        #region プロパティ
        /// <summary>
        /// 現在の例外を説明するメッセージを取得します。
        /// </summary>
        public new string[] Message { get; }
        #endregion

        /// <summary>
        /// 派生クラスでオーバーライドされた場合は、その例外に関する情報を使用して <see cref="T:System.Runtime.Serialization.SerializationInfo" /> を設定します。
        /// </summary>
        /// <param name="info">スローされた例外に関する、シリアル化されたオブジェクト データを保持する <see cref="T:System.Runtime.Serialization.SerializationInfo" /> です。</param>
        /// <param name="context">転送元または転送先についてのコンテキスト情報を含む <see cref="T:System.Runtime.Serialization.StreamingContext" /> です。</param>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter" />
        /// </PermissionSet>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// 派生クラスでオーバーライドされた場合、それ以後に発生する 1 つ以上の例外の主要な原因である <see cref="T:System.Exception" /> を返します。
        /// </summary>
        /// <returns>
        /// 例外のチェインでスローされた最初の例外。現在の例外の <see cref="P:System.Exception.InnerException" /> プロパティが null 参照 (Visual Basic の場合は Nothing) である場合、このプロパティは現在の例外を返します。
        /// </returns>
        public override Exception GetBaseException()
        {
            return base.GetBaseException();
        }
    }
}
