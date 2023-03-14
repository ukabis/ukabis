using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.ApiWeb.Domain.JsonPropertyFormatExtensions
{
    internal class UpdateValueCollection : List<UpdateValue>
    {
        private const string DUMMY = "__dummy__";
        private List<UpdateJson> merge = null;

        public List<UpdateJson> Merge()
        {
            if (merge == null)
            {
                merge = _marge().ToList();
            }
            return merge;
        }

        private static Type[] NumericType = new Type[] { typeof(float), typeof(double), typeof(decimal) };
        private IEnumerable<UpdateJson> _marge()
        {
            if (this.Any() == true)
            {
                var grpUpdate = this.GroupBy(x => new { x.Url, x.RollbackUrl, x.TargetHttpMethod, x.RollbackHttpMethod, x.ControllerUrl }).Where(xx => xx.Key.TargetHttpMethod == new HttpMethod("Patch")).ToList();
                var grpRegDel = this.GroupBy(x => new { x.Url, x.RollbackUrl, x.TargetHttpMethod, x.RollbackHttpMethod, x.ControllerUrl }).Where(xx => xx.Key.TargetHttpMethod != new HttpMethod("Patch")).ToList();
                foreach (var x in grpUpdate)
                {
                    var json = $"{{ '{DUMMY}' : null }}".ToJson();
                    var before = $"{{ '{DUMMY}' : null }}".ToJson();
                    foreach (var item in x)
                    {
                        if (item.Property == null)
                        {
                            json = item.Value.ToJson();
                        }
                        else
                        {
                            json.RemoveField(item.Property);
                            var pval = new JProperty(item.Property, item.Value);
                            //リクエストが小数点有りタイプの場合で、値に小数点が無い場合は、AddAfterSelfをすると.0 が付いてしまうため
                            //明示的にlongにして値を設定する
                            if (NumericType.Contains(item.Value?.GetType()))
                            {
                                //小数点が無い
                                if (!item.Value.ToString().Contains("."))
                                {
                                    if (long.TryParse(item.Value.ToString(), out long longval))
                                    {
                                        pval = new JProperty(item.Property, longval);
                                    }
                                }
                            }
                            json.FirstOrDefault().AddAfterSelf(pval);

                            before.RemoveField(item.Property);
                            var prollbackval = new JProperty(item.Property, item.RollbackValue);
                            //リクエストが小数点有りタイプの場合で、値に小数点が無い場合は、AddAfterSelfをすると.0 が付いてしまうため
                            //明示的にlongにして値を設定する
                            if (NumericType.Contains(item.RollbackValue?.GetType()))
                            {
                                //小数点が無い
                                if (!item.RollbackValue.ToString().Contains("."))
                                {
                                    if (long.TryParse(item.RollbackValue.ToString(), out long longrollbackval))
                                    {
                                        prollbackval = new JProperty(item.Property, longrollbackval);
                                    }
                                }
                            }
                            before.FirstOrDefault().AddAfterSelf(prollbackval);
                        }
                    }
                    json.RemoveField(DUMMY);
                    before.RemoveField(DUMMY);
                    yield return new UpdateJson() { Url = x.Key.Url, Json = json, BeforeJson = before.Count() == 0 ? null : before.ToString(), targetHttpMethod = x.Key.TargetHttpMethod, RollbackUrl = string.IsNullOrEmpty(x.Key.RollbackUrl) ? x.Key.Url : x.Key.RollbackUrl, rollbackHttpMethod = x.Key.RollbackHttpMethod, ControllerUrl = x.Key.ControllerUrl };
                }
                foreach (var rd1 in grpRegDel)
                {
                    foreach (var item in rd1)
                    {
                        yield return new UpdateJson() { Url = rd1.Key.Url, Json = item.Value.ToJson(), BeforeJson = item.RollbackValue?.ToJson().ToString(), targetHttpMethod = rd1.Key.TargetHttpMethod, RollbackUrl = rd1.Key.RollbackUrl, rollbackHttpMethod = rd1.Key.RollbackHttpMethod, ControllerUrl = rd1.Key.ControllerUrl };
                    }
                }
            }
            yield break;
        }

        public override string ToString()
        {
            string result = null;
            for (int i = 0; i < merge.Count; i++)
            {
                var u = merge[i];
                result += $"要素の中のNotify先{i}番目\r\nURL : {u.Url}\r\njson : {u.Json?.ToString()}\r\nBeforeJson : {u.BeforeJson?.ToString()}\r\nRollbackFailFlag : {u.IsRollbackFail}\r\n\r\n";
            }
            return result;
        }
    }
}
