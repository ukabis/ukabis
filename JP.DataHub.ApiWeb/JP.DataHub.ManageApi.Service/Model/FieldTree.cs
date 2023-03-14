using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    internal class FieldTree
    {
        public IList<FieldQueryModel> TreeList { get; }

        public FieldTree(IEnumerable<FieldQueryModel> sorceList)
        {
            TreeList = new List<FieldQueryModel>();
            foreach (var source in sorceList)
            {
                // 親を探す、親がTreeListに存在しないの場合は、先に親をTreeListに追加し、子要素として追加する
                var parent = FindFieldData(sorceList, source, TreeList);

                // 親がいない かつ TreeListに存在しないの場合、TreeListに追加する
                if (!parent && !TreeList.Any(x => x.FieldId == source.FieldId))
                {
                    TreeList.Add(source);
                }
            }
        }

        static private bool FindFieldData(IEnumerable<FieldQueryModel> sourceList, FieldQueryModel field, IList<FieldQueryModel> result = null)
        {
            // 検索結果から対象を検索
            foreach (var source in sourceList)
            {
                // 対象と一致する
                if (source.FieldId == field.ParentFieldId)
                {
                    if (!FindFieldData(sourceList, source, result))
                    {
                        // TreeListに追加済み？
                        if (!result.Any(x => x.FieldId == source.FieldId))
                        {
                            // 追加していないのものに親がいないまたは親を追加後、TreeListに追加
                            result.Add(source);
                        }
                    }

                    // 親の子要素に追加していない場合、追加
                    if (!source.Children.Any(x => x.FieldId == field.FieldId))
                    {
                        source.Children.Add(field);
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
