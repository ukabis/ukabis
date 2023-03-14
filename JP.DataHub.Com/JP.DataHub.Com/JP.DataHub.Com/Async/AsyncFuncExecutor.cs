using System;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Async
{
    public class AsyncFuncExecutor<TResult>
    {
        protected readonly Func<Task<TResult>> m_AsyncAction;

        public AsyncFuncExecutor(Func<Task<TResult>> asyncAction)
        {
            m_AsyncAction = asyncAction;
        }

        public Task<TResult> ExecuteAsync()
        {
            return m_AsyncAction();
        }
    }

    public class AsyncFuncExecutor<T1, TResult>
    {
        protected readonly Func<T1, Task<TResult>> m_AsyncAction;

        public AsyncFuncExecutor(Func<T1, Task<TResult>> asyncAction)
        {
            m_AsyncAction = asyncAction;
        }

        public Task<TResult> ExecuteAsync(T1 arg1)
        {
            return m_AsyncAction(arg1);
        }
    }

    public class AsyncFuncExecutor<T1, T2, TResult>
    {
        protected readonly Func<T1, T2, Task<TResult>> m_AsyncAction;

        public AsyncFuncExecutor(Func<T1, T2, Task<TResult>> asyncAction)
        {
            m_AsyncAction = asyncAction;
        }

        public Task<TResult> ExecuteAsync(T1 arg1, T2 arg2)
        {
            return m_AsyncAction(arg1, arg2);
        }
    }

    public class AsyncFuncExecutor<T1, T2, T3, TResult>
    {
        protected readonly Func<T1, T2, T3, Task<TResult>> m_AsyncAction;

        public AsyncFuncExecutor(Func<T1, T2, T3, Task<TResult>> asyncAction)
        {
            m_AsyncAction = asyncAction;
        }

        public Task<TResult> ExecuteAsync(T1 arg1, T2 arg2, T3 arg3)
        {
            return m_AsyncAction(arg1, arg2, arg3);
        }
    }

    public class AsyncFuncExecutor<T1, T2, T3, T4, TResult>
    {
        protected readonly Func<T1, T2, T3, T4, Task<TResult>> m_AsyncAction;

        public AsyncFuncExecutor(Func<T1, T2, T3, T4, Task<TResult>> asyncAction)
        {
            m_AsyncAction = asyncAction;
        }

        public Task<TResult> ExecuteAsync(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return m_AsyncAction(arg1, arg2, arg3, arg4);
        }
    }

    public class AsyncFuncExecutor<T1, T2, T3, T4, T5, TResult>
    {
        protected readonly Func<T1, T2, T3, T4, T5, Task<TResult>> m_AsyncAction;

        public AsyncFuncExecutor(Func<T1, T2, T3, T4, T5, Task<TResult>> asyncAction)
        {
            m_AsyncAction = asyncAction;
        }

        public Task<TResult> ExecuteAsync(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return m_AsyncAction(arg1, arg2, arg3, arg4, arg5);
        }
    }

    public class AsyncFuncExecutor<T1, T2, T3, T4, T5, T6, TResult>
    {
        protected readonly Func<T1, T2, T3, T4, T5, T6, Task<TResult>> m_AsyncAction;

        public AsyncFuncExecutor(Func<T1, T2, T3, T4, T5, T6, Task<TResult>> asyncAction)
        {
            m_AsyncAction = asyncAction;
        }

        public Task<TResult> ExecuteAsync(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            return m_AsyncAction(arg1, arg2, arg3, arg4, arg5, arg6);
        }
    }

    public class AsyncFuncExecutor<T1, T2, T3, T4, T5, T6, T7, TResult>
    {
        protected readonly Func<T1, T2, T3, T4, T5, T6, T7, Task<TResult>> m_AsyncAction;

        public AsyncFuncExecutor(Func<T1, T2, T3, T4, T5, T6, T7, Task<TResult>> asyncAction)
        {
            m_AsyncAction = asyncAction;
        }

        public Task<TResult> ExecuteAsync(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            return m_AsyncAction(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
    }

    public class AsyncFuncExecutor<T1, T2, T3, T4, T5, T6, T7, T8, TResult>
    {
        protected readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, Task<TResult>> m_AsyncAction;

        public AsyncFuncExecutor(Func<T1, T2, T3, T4, T5, T6, T7, T8, Task<TResult>> asyncAction)
        {
            m_AsyncAction = asyncAction;
        }

        public Task<TResult> ExecuteAsync(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            return m_AsyncAction(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }
    }

    public class AsyncFuncExecutor<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>
    {
        protected readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Task<TResult>> m_AsyncAction;

        public AsyncFuncExecutor(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Task<TResult>> asyncAction)
        {
            m_AsyncAction = asyncAction;
        }

        public Task<TResult> ExecuteAsync(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            return m_AsyncAction(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }
    }

    public class AsyncFuncExecutor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>
    {
        protected readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task<TResult>> m_AsyncAction;

        public AsyncFuncExecutor(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task<TResult>> asyncAction)
        {
            m_AsyncAction = asyncAction;
        }

        public Task<TResult> ExecuteAsync(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            return m_AsyncAction(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }
    }
}
