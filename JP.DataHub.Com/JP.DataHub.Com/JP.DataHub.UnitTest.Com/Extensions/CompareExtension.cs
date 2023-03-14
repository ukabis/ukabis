using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    public static class CompareExtension
    {
        public static string WILDCARD = "{{*}}";

        private class EqualInfo
        {
            public object Left;
            public object Right;
            public bool IsEquals;
            public IEnumerable<string> Names;
        }

        public static void Is(this JProperty actual, JProperty expected, string message = "")
        {
            message = (string.IsNullOrEmpty(message) ? "" : ", " + message);
            if (object.ReferenceEquals(actual, expected)) return;

            if (actual == null) throw new AssertFailedException("actual is null" + message);
            if (expected == null) throw new AssertFailedException("actual is not null" + message);
            if (actual.Type != expected.Type)
            {
                var msg = string.Format("expected type is {0} but actual type is {1}{2}",
                    expected.Type, actual.Type, message);
                throw new AssertFailedException(msg);
            }
            if (actual.Value is JArray acturalarray && expected.Value is JArray exptectedarray)
            {
                var r = Equal(acturalarray, exptectedarray, new[] { actual.GetType().Name });
                if (!r.IsEquals)
                {
                    var msg = string.Format("is not structural equal, failed at {0}, actual = {1} expected = {2}{3}",
                        string.Join(".", r.Names), r.Left, r.Right, message);
                    throw new AssertFailedException(msg);
                }
            }
            else
            {
                EqualJValue(actual.Value as JValue, expected.Value as JValue, null);
            }
        }

        public static void Is(this JToken actual, JToken expected, string message = "")
        {
            message = (string.IsNullOrEmpty(message) ? "" : ", " + message);
            if (object.ReferenceEquals(actual, expected)) return;

            if (actual == null) throw new AssertFailedException("actual is null" + message);
            if (expected == null) throw new AssertFailedException("actual is not null" + message);
            if (actual.GetType() != expected.GetType())
            {
                var msg = string.Format("expected type is {0} but actual type is {1}{2}",
                    expected.GetType().Name, actual.GetType().Name, message);
                throw new AssertFailedException(msg);
            }

            var r = Equal(actual, expected, new[] { actual.GetType().Name }); // root type
            if (!r.IsEquals)
            {
                var msg = string.Format("is not structural equal, failed at {0}, actual = {1} expected = {2}{3}",
                    string.Join(".", r.Names), r.Left, r.Right, message);
                throw new AssertFailedException(msg);
            }
        }


        private static EqualInfo EqualJValue(JValue left, JValue right, IEnumerable<string> names)
        {
            var lefto = IfDateTimeToString(left.ToObject<object>());
            var righto = IfDateTimeToString(right.ToObject<object>());
            if (lefto != null && righto != null)
            {
                if (righto.GetType() == typeof(string))
                {
                    var result = lefto.ToString().StringDiffOnly(righto.ToString());
                    int cnt = result.Where(x => x.Value == WILDCARD).Count();
                    if (result.Count == cnt)
                    {
                        return new EqualInfo { IsEquals = true, Left = left, Right = right, Names = names };
                    }
                    return new EqualInfo { IsEquals = false, Left = left, Right = right, Names = names };
                }
                if (!CompareValue(lefto, righto))
                {
                    if (left.Children().Any())
                    {
                        return new EqualInfo { IsEquals = false, Left = left, Right = right, Names = names.Concat(new[] { "[" + left.Children().ToArray()[0].Path + "]" }) };
                    }
                    else
                    {
                        return new EqualInfo { IsEquals = false, Left = left, Right = right, Names = names.Concat(new[] { "" }) };
                    }
                }
            }
            else if ((lefto == null && righto != null) || (lefto != null && righto == null))
            {
                return new EqualInfo { IsEquals = false, Left = left, Right = right, Names = names.Concat(new[] { "" }) };
            }
            return new EqualInfo { IsEquals = true, Left = left, Right = right, Names = names };
        }

        private static Type[] NumericType = new Type[] { typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) };
        /// <summary>
        /// left と right が同じかどうかチェックする
        /// 片方がnull でないか、型が同一か、値が一緒か
        /// 数値項目については、Json は 1 と 1.0 を比較することがあるので一旦decimalにしてからチェックする
        /// </summary>
        /// <param name="lefto"></param>
        /// <param name="righto"></param>
        /// <returns></returns>
        private static bool CompareValue(object lefto, object righto)
        {
            if ((lefto == null && righto != null) || (lefto != null && righto == null))
            {
                return false;
            }
            if (IsValidType(lefto.GetType(), righto.GetType()) == false)
            {
                return false;
            }

            //数値項目は1.0 と1 を比較する可能性があるため、一旦decimal に合わせて比較
            if (NumericType.Contains(lefto.GetType()) && NumericType.Contains(righto.GetType()))
            {
                if (decimal.TryParse(lefto.ToString(), out decimal leftdeci) == false)
                {
                    return false;
                }
                if (decimal.TryParse(righto.ToString(), out decimal rightdeci) == false)
                {
                    return false;
                }

                if (leftdeci == rightdeci)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (lefto.ToString() == righto.ToString())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private static bool IsValidType(Type left, Type right)
        {
            if (IsIntegerOrRealType(left) && IsIntegerOrRealType(right))
            {
                return true;
            }
            return left == right;
        }

        private static Type[] IntergerTypes = new Type[] { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) };

        private static bool IsIntegerOrRealType(Type left)
        {
            return IntergerTypes.Contains(left);
        }

        private static object IfDateTimeToString(object target)
        {
            if (target?.GetType() == typeof(DateTime))
            {
                return ((DateTime)target).ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss.fff");
            }
            return target;

        }

        private static EqualInfo Equal(JToken left, JToken right, IEnumerable<string> names)
        {
            if (left != null && right != null && left.GetType() == typeof(JArray) && right.GetType() == typeof(JArray))
            {
                if (left.Count() != right.Count())
                {
                    return new EqualInfo { IsEquals = false, Left = left, Right = right, Names = names };
                }
                for (int i = 0; i < left.Count(); i++)
                {
                    var result = Equal(left[i], right[i], names.Concat(new[] { "[" + i + "]" }));
                    if (!result.IsEquals)
                    {
                        return result;
                    }
                }
            }
            else
            {
                if (left.Type >= JTokenType.Integer)
                {
                    var result = EqualJValue(left as JValue, right as JValue, names);
                    if (!result.IsEquals)
                    {
                        return result;
                    }
                }
                else
                {
                    if (left.Count() != right.Count())
                    {
                        return new EqualInfo { IsEquals = false, Left = left, Right = right, Names = names };
                    }
                    foreach (var children in left.Children())
                    {
                        string path = children.Path.Substring(left.Path.Length == 0 ? 0 : children.Path.Length == left.Path.Length ? 0 : left.Path.Length + 1);
                        var left_child = left[path];
                        var right_child = right[path];
                        if (left_child == null && right_child == null)
                        {
                            path = (children as JProperty).Name;
                            left_child = left[path];
                            right_child = right[path];
                        }
                        if (left_child.GetType() == typeof(JValue) && right_child.GetType() == typeof(JValue))
                        {
                            var result = EqualJValue((JValue)left_child, (JValue)right_child, names.Concat(new[] { "[" + path + "]" }));
                            if (!result.IsEquals)
                            {
                                return result;
                            }
                        }
                        else
                        {
                            var result = Equal(left_child, right_child, names.Concat(new[] { "[" + path + "]" }));
                            if (!result.IsEquals)
                            {
                                return result;
                            }
                        }
                    }
                }
            }
            return new EqualInfo { IsEquals = true, Left = left, Right = right, Names = names };
        }
    }
}
