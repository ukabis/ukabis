using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Transaction
{
    public interface IWantTransaction : IDisposable
    {
        /// <summary>
        /// トランザクション管理をしてほしいか？
        /// </summary>
        bool IsTransactionManage { get; }

        /// <summary>
        /// トランザクションの状態
        /// </summary>
        bool IsTransactionStatus { get; }

        void Close();

        void Rollback();
    }
}
