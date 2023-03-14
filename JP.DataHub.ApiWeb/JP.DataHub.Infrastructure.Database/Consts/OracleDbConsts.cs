using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Infrastructure.Database.Consts
{
    // .NET6
    public static class OracleDbConsts
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
        /// DB_BLOCK_SIZEが8192バイトのとき、インデックス可能な最大値は約6400バイトとなる。
        /// NLS_NCHAR_CHARACTERSETが「AL16UTF16」のとき、1文字は最大4バイトとなる。
        /// したがって、文字数は6400の1/4の1200文字となる。
        /// </remarks>
        public const int PrimaryKeyTableColumnMaxLength = 1200;

        /// <summary>
        /// マルチバイト文字列の最大文字数
        /// </summary>
        /// <remarks>
        /// DB_BLOCK_SIZEが8192バイトのとき、インデックス可能な最大値は約6400バイトとなる。
        /// NLS_NCHAR_CHARACTERSETが「AL16UTF16」のとき、1文字は最大4バイトとなる。
        /// したがって、文字数は6400の1/4の1200文字となる。
        /// </remarks>
        public const int NCharTableColumnMaxLength = 1200;

        /// <summary>
        /// 数値型全体の有効桁数
        /// </summary>
        /// <remarks>
        /// Oracleのnumber型の最大値38とする。
        /// 整数部の有効桁数はこの値からNumericScaleの桁数を引いたものとなる。
        /// </remarks>
        public const int NumericPrecision = 38;

        /// <summary>
        /// 数値型の小数点以下の有効数字
        /// </summary>
        /// <remarks>
        /// Oracleのnumber型の最大値127とする。
        /// </remarks>
        public const int NumericScale = 127;

        /// <summary>
        /// 数値型の小数点以下の既定の精度
        /// </summary>
        /// <remarks>
        /// Oracleのnumber型の10とする。
        /// </remarks>
        public const int NumericDefaultScale = 10;

        /// <summary>
        /// 列名の最大文字数
        /// </summary>
        /// <remarks>
        /// Oracle(12.2以降)の列名の最大長128とする。
        /// </remarks>
        public const int TableColumnNameMaxLength = 128;

        /// <summary>
        /// 最大列数
        /// </summary>
        /// <remarks>
        /// Oracleの仕様上は最大1000列だが管理項目+バッファ用に100列確保して900とする。
        /// </remarks>
        public const int TableMaxColumns = 900;

        /// <summary>
        /// 列名パターン
        /// </summary>
        /// <remarks>
        /// パターンの内容
        /// ・列名として許容されている文字のみである(英数字、アンダースコア、ドル記号およびシャープ記号のみ)こと。
        /// </remarks>
        public const string TableColumnNamePattern = "(^[0-9a-zA-Z_$#]+)$";
    }
}
