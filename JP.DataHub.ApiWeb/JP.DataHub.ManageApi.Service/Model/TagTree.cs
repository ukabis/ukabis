using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    internal class TagTree
    {
        public IList<TagQueryModel> TreeList { get; }

        public TagTree(IEnumerable<TagQueryModel> sorceList)
        {
            TreeList = new List<TagQueryModel>();
            foreach (var source in sorceList)
            {
                // 親を探す、親がTreeListに存在しないの場合は、先に親をTreeListに追加し、子要素として追加する
                var parent = FindTagData(sorceList, source, TreeList);

                // 親がいない かつ TreeListに存在しないの場合、TreeListに追加する
                if (!parent && !TreeList.Any(x => x.TagId == source.TagId))
                {
                    TreeList.Add(source);
                }
            }

        }

        static private bool FindTagData(IEnumerable<TagQueryModel> sourceList, TagQueryModel tag, IList<TagQueryModel> result = null)
        {
            // 検索結果から対象を検索
            foreach (var source in sourceList)
            {
                // 対象と一致する
                if (source.TagId == tag.ParentTagId)
                {
                    if (!FindTagData(sourceList, source, result))
                    {
                        // TreeListに追加済み？
                        if (!result.Any(x => x.TagId == source.TagId))
                        {
                            // 追加していないのものに親がいないまたは親を追加後、TreeListに追加
                            result.Add(source);
                        }
                    }

                    // 親の子要素に追加していない場合、追加
                    if (!source.Children.Any(x => x.TagId == tag.TagId))
                    {
                        source.Children.Add(tag);
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
