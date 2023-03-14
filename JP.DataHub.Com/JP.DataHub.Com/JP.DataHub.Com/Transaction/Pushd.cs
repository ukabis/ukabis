using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Unity
{
    public class Pushd<T> : IDisposable
    {
        private T backup;
        private Action<T> Exit;
        private bool iscall_exit = false;
        private readonly object obj = new object();

        public Pushd(Func<T> Before, Action Setup, Action<T> exit)
        {
            backup = Before();
            Setup();
            Exit = exit;
        }

        public void Popd()
        {
            lock (obj)
            {
                if (iscall_exit == false)
                {
                    iscall_exit = true;
                    Exit(backup);
                }
            }
        }

        public void Dispose()
        {
            Popd();
        }
    }

    public class Pushd<T1, T2> : IDisposable
    {
        private T1 backup1;
        private T2 backup2;
        private Action<T1, T2> Exit;
        private bool iscall_exit = false;
        private readonly object obj = new object();

        public Pushd(Func<T1> Before1, Func<T2> Before2, Action Setup, Action<T1, T2> exit)
        {
            backup1 = Before1();
            backup2 = Before2();
            Setup();
            Exit = exit;
        }

        public void Popd()
        {
            lock (obj)
            {
                if (iscall_exit == false)
                {
                    iscall_exit = true;
                    Exit(backup1, backup2);
                }
            }
        }

        public void Dispose()
        {
            Popd();
            GC.SuppressFinalize(obj);
        }
    }
}
