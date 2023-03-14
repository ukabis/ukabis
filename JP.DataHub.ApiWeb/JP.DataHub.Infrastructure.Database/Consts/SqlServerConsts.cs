using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Infrastructure.Database.Consts
{
    // .NET6
    public static class SqlServerConsts
    {
        /// <summary>
        /// テーブル名に置換されるAPIクエリ用変数
        /// </summary>
        public const string TableNameVariable = "TABLE_NAME";

        /// <summary>
        /// 添付ファイルメタ情報用テーブル名
        /// </summary>
        public const string AttachFileMetaTableName = "AttachFileMeta";

        /// <summary>
        /// 添付ファイルメタ情報検索用テーブル名
        /// </summary>
        public const string AttachFileMetaSearchTableName = "AttachFileMetaSearch";

        /// <summary>
        /// マルチバイト文字列(PK)の最大文字数
        /// </summary>
        /// <remarks>
        /// 本来の最大文字数は4000文字or無制限だが、PKは450文字までしかインデックスが貼れないため450文字とする。
        /// </remarks>
        public const int PrimaryKeyTableColumnMaxLength = 450;

        /// <summary>
        /// マルチバイト文字列の最大文字数
        /// </summary>
        /// <remarks>
        /// 本来の最大文字数は4000文字or無制限だが、850文字までしかインデックスが貼れないため850文字とする。
        /// </remarks>
        public const int NCharTableColumnMaxLength = 850;

        /// <summary>
        /// 数値型全体の有効桁数
        /// </summary>
        /// <remarks>
        /// SQLServerのnumeric型の最大値38とする。
        /// 整数部の有効桁数はこの値からNumericScaleの桁数を引いたものとなる。
        /// </remarks>
        public const int NumericPrecision = 38;

        /// <summary>
        /// 数値型の小数点以下の有効数字
        /// </summary>
        /// <remarks>
        /// SQLServerのnumeric型の最大値38とする。
        /// </remarks>
        public const int NumericScale = 18;

        /// <summary>
        /// 列名の最大文字数
        /// </summary>
        /// <remarks>
        /// SQLServerの仕様上は最大128文字だがインデックス名も128文字以下である必要があるため、
        /// インデックス名のプレフィックス(40文字)を考慮して88文字とする。
        /// </remarks>
        public const int TableColumnNameMaxLength = 88;

        /// <summary>
        /// 最大列数
        /// </summary>
        /// <remarks>
        /// SQLServerの仕様上は最大1024列だが管理項目+バッファ用に124列確保して900とする。
        /// </remarks>
        public const int TableMaxColumns = 900;

        /// <summary>
        /// 列名パターン
        /// </summary>
        /// <remarks>
        /// パターンの内容
        /// ・列名として許容されている文字のみである(ダブルクォート、ヌル文字、"@"が含まれていない)こと。
        ///   ("@"は使用できないわわけではないがバインド変数のプレフィックスと被るため使用禁止とする)
        /// </remarks>
        public const string TableColumnNamePattern = "^([^\"\0@]+)$";
    }
}
