using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Extensions
{
    public static class DelegateExtensions
    {
        public static Func<Task> ToAsync(this Action action, Action before = null, Action after = null)
        {
            return () => Task.Run(() => { if (before != null) before(); action(); if (after != null) after(); });
        }

        public static Func<T1, Task> ToAsync<T1>(this Action<T1> action, Action before = null, Action after = null)
        {
            return (arg1) => Task.Run(() => { if (before != null) before(); action(arg1); if (after != null) after(); });
        }

        public static Func<T1, T2, Task> ToAsync<T1, T2>(this Action<T1, T2> action, Action before = null, Action after = null)
        {
            return (arg1, arg2) => Task.Run(() => { if (before != null) before(); action(arg1, arg2); if (after != null) after(); });
        }

        public static Func<T1, T2, T3, Task> ToAsync<T1, T2, T3>(this Action<T1, T2, T3> action, Action before = null, Action after = null)
        {
            return (arg1, arg2, arg3) => Task.Run(() => { if (before != null) before(); action(arg1, arg2, arg3); if (after != null) after(); });
        }

        public static Func<T1, T2, T3, T4, Task> ToAsync<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, Action before = null, Action after = null)
        {
            return (arg1, arg2, arg3, arg4) => Task.Run(() => { if (before != null) before(); action(arg1, arg2, arg3, arg4); if (after != null) after(); });
        }

        public static Func<T1, T2, T3, T4, T5, Task> ToAsync<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, Action before = null, Action after = null)
        {
            return (arg1, arg2, arg3, arg4, arg5) => Task.Run(() => { if (before != null) before(); action(arg1, arg2, arg3, arg4, arg5); if (after != null) after(); });
        }

        public static Func<T1, T2, T3, T4, T5, T6, Task> ToAsync<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, Action before = null, Action after = null)
        {
            return (arg1, arg2, arg3, arg4, arg5, arg6) => Task.Run(() => { if (before != null) before(); action(arg1, arg2, arg3, arg4, arg5, arg6); if (after != null) after(); });
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, Task> ToAsync<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, Action before = null, Action after = null)
        {
            return (arg1, arg2, arg3, arg4, arg5, arg6, arg7) => Task.Run(() => { if (before != null) before(); action(arg1, arg2, arg3, arg4, arg5, arg6, arg7); if (after != null) after(); });
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, Task> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, Action before = null, Action after = null)
        {
            return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) => Task.Run(() => { if (before != null) before(); action(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8); if (after != null) after(); });
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Task> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, Action before = null, Action after = null)
        {
            return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9) => Task.Run(() => { if (before != null) before(); action(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9); if (after != null) after(); });
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action, Action before = null, Action after = null)
        {
            return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10) => Task.Run(() => { if (before != null) before(); action(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10); if (after != null) after(); });
        }

        public static Func<Task<TResult>> ToAsync<TResult>(this Func<TResult> func, Action before = null, Action after = null)
        {
            return () => Task.Run(() => { if (before != null) before(); var result = func(); if (after != null) after(); return result; });
        }

        public static Func<T1, Task<TResult>> ToAsync<T1, TResult>(this Func<T1, TResult> func, Action before = null, Action after = null)
        {
            return (arg1) => Task.Run(() => { if (before != null) before(); var result = func(arg1); if (after != null) after(); return result; });
        }

        public static Func<T1, T2, Task<TResult>> ToAsync<T1, T2, TResult>(this Func<T1, T2, TResult> func, Action before = null, Action after = null)
        {
            return (arg1, arg2) => Task.Run(() => { if (before != null) before(); var result = func(arg1, arg2); if (after != null) after(); return result; });
        }

        public static Func<T1, T2, T3, Task<TResult>> ToAsync<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func, Action before = null, Action after = null)
        {
            return (arg1, arg2, arg3) => Task.Run(() => { if (before != null) before(); var result = func(arg1, arg2, arg3); if (after != null) after(); return result; });
        }

        public static Func<T1, T2, T3, T4, Task<TResult>> ToAsync<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> func, Action before = null, Action after = null)
        {
            return (arg1, arg2, arg3, arg4) => Task.Run(() => { if (before != null) before(); var result = func(arg1, arg2, arg3, arg4); if (after != null) after(); return result; });
        }

        public static Func<T1, T2, T3, T4, T5, Task<TResult>> ToAsync<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, TResult> func, Action before = null, Action after = null)
        {
            return (arg1, arg2, arg3, arg4, arg5) => Task.Run(() => { if (before != null) before(); var result = func(arg1, arg2, arg3, arg4, arg5); if (after != null) after(); return result; });
        }

        public static Func<T1, T2, T3, T4, T5, T6, Task<TResult>> ToAsync<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, TResult> func, Action before = null, Action after = null)
        {
            return (arg1, arg2, arg3, arg4, arg5, arg6) => Task.Run(() => { if (before != null) before(); var result = func(arg1, arg2, arg3, arg4, arg5, arg6); if (after != null) after(); return result; });
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, Task<TResult>> ToAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, Action before = null, Action after = null)
        {
            return (arg1, arg2, arg3, arg4, arg5, arg6, arg7) => Task.Run(() => { if (before != null) before(); var result = func(arg1, arg2, arg3, arg4, arg5, arg6, arg7); if (after != null) after(); return result; });
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, Task<TResult>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, Action before = null, Action after = null)
        {
            return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) => Task.Run(() => { if (before != null) before(); var result = func(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8); if (after != null) after(); return result; });
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Task<TResult>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, Action before = null, Action after = null)
        {
            return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9) => Task.Run(() => { if (before != null) before(); var result = func(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9); if (after != null) after(); return result; });
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task<TResult>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, Action before = null, Action after = null)
        {
            return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10) => Task.Run(() => { if (before != null) before(); var result = func(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10); if (after != null) after(); return result; });
        }
    }
}
