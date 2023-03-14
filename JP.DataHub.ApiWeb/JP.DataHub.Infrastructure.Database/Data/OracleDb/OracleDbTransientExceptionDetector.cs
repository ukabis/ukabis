// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// NOTE ：https://github.com/dotnet/efcore/blob/main/src/EFCore.SqlServer/Storage/Internal/SqlServerTransientExceptionDetector.cs
// NOTE ：Retry Transient Failures Using SqlClient / ADO.NET With Polly https://rimdev.io/retry-transient-failures-using-sqlclient-adonet-with-polly/
// NOTE ：How is SqlException Number assigned https://stackoverflow.com/questions/18894345/how-is-sqlexception-number-assigned
// NOTE ：Oracle : https://docs.oracle.com/cd/E82638_01/errmg/ORA-00000.html
using System;
using System.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;

namespace JP.DataHub.Infrastructure.Database.Data.OracleDb
{
    // .NET6
    /// <summary>
    ///     Detects the exceptions caused by SQL Server transient failures.
    /// </summary>
    public static class OracleDbTransientExceptionDetector
    {
        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public static bool ShouldRetryOn(Exception ex)
        {
            if (ex is OracleException oracleException)
            {
                foreach (OracleError err in oracleException.Errors)
                {
                    switch (err.Number)
                    {
                        // リトライ対象となるOracleのエラーコードについては
                        // 「ORA」から始まるエラーコードを対象に公式サイトの処置に以下の文言があるコードを対象に設定。
                        // ・『しばらく待ってから操作を再試行してください。』
                        // ・『～必要に応じて再試行』
                        // SQL Error Code: ORA-04020
                        // オブジェクトstringstringstringstringstringをロックしようとしてデッドロックを検出しました。
                        case 4020:
                        // SQL Error Code: ORA-04021
                        // オブジェクトstringstringstringstringstringをロック待ちしていてタイムアウトが発生しました。
                        case 4021:
                        // SQL Error Code: ORA-04022
                        // 待機なしが要求されている間、ライブラリ・オブジェクトのロックを待つ必要があります。
                        case 4022:
                        // SQL Error Code: ORA-04024
                        // ピン・カーソルstringを相互排除しようとして自己デッドロックを検出しました
                        case 4024:
                        // SQL Error Code: ORA-24763
                        // トランザクション操作を完了できません。
                        case 24763:
                        // SQL Error Code: ORA-30006
                        // リソース・ビジー; WAITタイムアウトの期限に達しました。
                        case 30006:
                        // SQL Error Code: ORA-30690
                        // データ通信量検出のためのTCP/IP接続の登録中にタイムアウトが発生しました。
                        case 30690:
                        // SQL Error Code: ORA-30691
                        // データ通信量検出のためのTCP/IP接続の登録中にシステム・リソース割当てに失敗しました。
                        case 30691:
                        // SQL Error Code: ORA-31442
                        // stringのロックを取得中に操作がタイムアウトになりました。
                        case 31442:
                        // SQL Error Code: ORA-31705
                        // ライブラリ・オブジェクトの取得に失敗しました。
                        case 31705:
                        // SQL Error Code: ORA-48345
                        // 操作中にタイムアウトが発生しました
                        case 48345:
                        // SQL Error Code: ORA-55330
                        // ルールベースまたはルール索引stringはビジーです
                        case 55330:
                        // SQL Error Code: ORA-55635
                        // フラッシュバック・データ・アーカイブが有効化されている表"string"."string"の関連付けが解除されています
                        case 55635:
                        // SQL Error Code: ORA-55710
                        // この時点ではシステム・パラメータGLOBAL_TXN_PROCESSESを変更できません
                        case 55710:
                        // SQL Error Code: ORA-63995
                        // 破損ブロックが制御ファイル内で検出されました: (ブロックstring、#ブロックstring)
                        case 63995:
                        // SQL Error Code: ORA-65083
                        // プラガブル・データベースstringの停止が進行中です
                        case 65083:
                        // SQL Error Code: ORA-65182
                        // プラガブル・データベースstringの状態を変更できません
                        case 65182:
                        // SQL Error Code: ORA-65376
                        // PDBをリフレッシュできません
                        case 65376:
                            return true;
                    }
                }
                return false;
            }
            else if (ex is SqlException sqlException)
            {
                foreach (SqlError err in sqlException.Errors)
                {
                    switch (err.Number)
                    {
                        // SQL Error Code: 49920
                        // Cannot process request. Too many operations in progress for subscription "%ld".
                        // The service is busy processing multiple requests for this subscription.
                        // Requests are currently blocked for resource optimization. Query sys.dm_operation_status for operation status.
                        // Wait until pending requests are complete or delete one of your pending requests and retry your request later.
                        case 49920:
                        // SQL Error Code: 49919
                        // Cannot process create or update request. Too many create or update operations in progress for subscription "%ld".
                        // The service is busy processing multiple create or update requests for your subscription or server.
                        // Requests are currently blocked for resource optimization. Query sys.dm_operation_status for pending operations.
                        // Wait till pending create or update requests are complete or delete one of your pending requests and
                        // retry your request later.
                        case 49919:
                        // SQL Error Code: 49918
                        // Cannot process request. Not enough resources to process request.
                        // The service is currently busy.Please retry the request later.
                        case 49918:
                        // SQL Error Code: 41839
                        // Transaction exceeded the maximum number of commit dependencies.
                        case 41839:
                        // SQL Error Code: 41325
                        // The current transaction failed to commit due to a serializable validation failure.
                        case 41325:
                        // SQL Error Code: 41305
                        // The current transaction failed to commit due to a repeatable read validation failure.
                        case 41305:
                        // SQL Error Code: 41302
                        // The current transaction attempted to update a record that has been updated since the transaction started.
                        case 41302:
                        // SQL Error Code: 41301
                        // Dependency failure: a dependency was taken on another transaction that later failed to commit.
                        case 41301:
                        // SQL Error Code: 40613
                        // Database XXXX on server YYYY is not currently available. Please retry the connection later.
                        // If the problem persists, contact customer support, and provide them the session tracing ID of ZZZZZ.
                        case 40613:
                        // SQL Error Code: 40501
                        // The service is currently busy. Retry the request after 10 seconds. Code: (reason code to be decoded).
                        case 40501:
                        // SQL Error Code: 40197
                        // The service has encountered an error processing your request. Please try again.
                        case 40197:
                        // SQL Error Code: 10936
                        // Resource ID : %d. The request limit for the elastic pool is %d and has been reached.
                        // See 'http://go.microsoft.com/fwlink/?LinkId=267637' for assistance.
                        case 10936:
                        // SQL Error Code: 10929
                        // Resource ID: %d. The %s minimum guarantee is %d, maximum limit is %d and the current usage for the database is %d.
                        // However, the server is currently too busy to support requests greater than %d for this database.
                        // For more information, see http://go.microsoft.com/fwlink/?LinkId=267637. Otherwise, please try again.
                        case 10929:
                        // SQL Error Code: 10928
                        // Resource ID: %d. The %s limit for the database is %d and has been reached. For more information,
                        // see http://go.microsoft.com/fwlink/?LinkId=267637.
                        case 10928:
                        // SQL Error Code: 10060
                        // A network-related or instance-specific error occurred while establishing a connection to SQL Server.
                        // The server was not found or was not accessible. Verify that the instance name is correct and that SQL Server
                        // is configured to allow remote connections. (provider: TCP Provider, error: 0 - A connection attempt failed
                        // because the connected party did not properly respond after a period of time, or established connection failed
                        // because connected host has failed to respond.)"}
                        case 10060:
                        // SQL Error Code: 10054
                        // A transport-level error has occurred when sending the request to the server.
                        // (provider: TCP Provider, error: 0 - An existing connection was forcibly closed by the remote host.)
                        case 10054:
                        // SQL Error Code: 10053
                        // A transport-level error has occurred when receiving results from the server.
                        // An established connection was aborted by the software in your host machine.
                        case 10053:
                        // SQL Error Code: 1205
                        // Deadlock
                        case 1205:
                        // SQL Error Code: 233
                        // The client was unable to establish a connection because of an error during connection initialization process before login.
                        // Possible causes include the following: the client tried to connect to an unsupported version of SQL Server;
                        // the server was too busy to accept new connections; or there was a resource limitation (insufficient memory or maximum
                        // allowed connections) on the server. (provider: TCP Provider, error: 0 - An existing connection was forcibly closed by
                        // the remote host.)
                        case 233:
                        // SQL Error Code: 121
                        // The semaphore timeout period has expired
                        case 121:
                        // SQL Error Code: 64
                        // A connection was successfully established with the server, but then an error occurred during the login process.
                        // (provider: TCP Provider, error: 0 - The specified network name is no longer available.)
                        case 64:
                        // DBNETLIB Error Code: 20
                        // The instance of SQL Server you attempted to connect to does not support encryption.
                        case 20:
                        // This exception can be thrown even if the operation completed successfully, so it's safer to let the application fail.
                        // DBNETLIB Error Code: -2
                        // Timeout expired. The timeout period elapsed prior to completion of the operation or the server is not responding. The statement has been terminated.
                        case -2:
                            return true;
                    }
                }

                return false;
            }

            return ex is TimeoutException;
        }
    }
}
