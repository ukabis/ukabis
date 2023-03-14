using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Cache.Attributes;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.RFC7807;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Database;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Repository;

namespace JP.DataHub.ManageApi.Infrastructure.Repository
{
    internal class TestRepository : ITestRepository
    {
        /// <summary>
        /// キャッシュの説明
        /// キャッシュ属性を指定することによってGetの戻り値をキャッシュする
        /// その際キーは引数のidの中身になる
        /// Getによって永続化層へアクセスするキーワードをCacheEntityで指定する。
        /// 永続化層＝DBではないので、必ずしも物理的なテーブル名を指定する必要はない
        /// が、しかし、永続化層＝DBの場合は、テーブル名にしたほうが分かりやすいだろう
        /// また、JOINして取得するような場合は、CacheEntityの引数を複数することができる
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // allParam:trueにすると、引数パラメータのすべてを使ってキャッシュキーをつくります
        [CacheArg(allParam: true)]
        // CacheArgの文字列を指定すると、その引数パラメータを使ってキャッシュキーをつくります
        // [CacheArg("id")]
        [CacheEntity("TEST")]
        [Cache]
        public TestModel Get(string id)
        {
            // SELECT * FROM Test  WHERE id=@id
            if (id == "exception")
            {
                throw new Exception("error");
            }
            else if (id == "rfc")
            {
                throw new Rfc7807Exception(ErrorCodeMessage.Code.E99998.GetRFC7807());
            }
            return new TestModel() { Id = Guid.NewGuid().ToString(), Name = "Test", VendorId = "b5c74429-7862-4893-91cf-35a64ff30bbc", SystemId = "e2b171ed-c102-4dfd-b01e-d663a9db6a11" };
        }

        /// <summary>
        /// これも↑の説明と同様にキャッシュする
        /// 影響するテーブル名も同じである
        /// しかし、永続化層（DB）だった場合、TESTテーブルから全件取得する（であろう）からCacheArgなどの指定は不要となる
        /// </summary>
        /// <returns></returns>
        [CacheEntity("TEST")]
        [Cache]
        public IList<TestModel> GetList()
        {
            // SELECT * FROM test
            return new List<TestModel>() {
                new TestModel() { Id = Guid.NewGuid().ToString(), Name = "Test", VendorId = "b5c74429-7862-4893-91cf-35a64ff30bbc", SystemId = "e2b171ed-c102-4dfd-b01e-d663a9db6a11" },
                new TestModel() { Id = Guid.NewGuid().ToString(), Name = "Test", VendorId = "b5c74429-7862-4893-91cf-35a64ff30FFF", SystemId = "e2b171ed-c102-4dfd-b01e-d663a9db6FFF" },
            };
        }

        /// <summary>
        /// 以下（３つ）のメソッドはデータの更新（または削除）である
        /// これが成功するということは、TESTテーブルの状態が変化したことになる
        /// および
        /// model.Idの値をキーとしたデータも変更（追加）されたことになる
        /// CacheEntityFireは、TESTテーブルが変更したことを通知（発火）し、それを利用しているキャッシュを消去することが出来る
        /// このクラスでいうところのGetやGetListでキャッシュされたものがそれにあたる（CacheEntityで指定しているから）
        /// </summary>
        /// <param name="model"></param>
        [CacheFire]
        [CacheIdFire("Id", "model.Id")]
        [CacheEntityFire("TEST")]

        /// DomainDataSyncの同期のために情報を伝えるための仕組み（コンストラクタは２つのバリエーションあり）
        /// コンストラクタの記述①
        /// このメソッドでContainerDynamicSeparationテーブルへの登録だった場合、以下のように記述する
        /// 第1引数はテーブル名
        /// 第2引数はPKの値を見つけるための引数パラメータのObjectPathを指定する
        /// 基本的に１つのメソッドでは１つのテーブルへの更新（または削除）であるから次のように記述すれば
        /// 良いが、２つ以上のテーブルへの更新の場合は、この２つのパラメータ（テーブル名とObjectPath）を1セット
        /// として、複数記述することが出来る
        /// コンストラクタの記述②
        /// 第1引数にParameterTypeの指定が出来るようになった。ここにArgumentを指定すると、メソッドの引数パラメータから値を取得する
        /// Returnを指定すると、メソッドの帰り値から値を取得する
        /// それ以外は記述①と同様である
        /// 以下の例は引数パラメータの場合である
        [DomainDataSync(DynamicApiDatabase.TABLE_CONTAINERDYNAMICSEPARATION, "model.Id", "1", "model.hoge")]
        /// 以下の例はリターン値の場合である
        [DomainDataSync(ParameterType.Result, DynamicApiDatabase.TABLE_CONTAINERDYNAMICSEPARATION, "Id")]
        public TestModel Register(TestModel model)
        {
            if (model?.Id == "exception")
            {
                throw new Exception("error");
            }
            else if (model?.Id == "rfc")
            {
                throw new Rfc7807Exception(ErrorCodeMessage.Code.E99998.GetRFC7807());
            }
            model.Id = Guid.NewGuid().ToString();
            return model;
        }

        /// <summary>
        /// CacheIdFireにより引数idの値をキーとしたデータを消去する
        /// これはDeleteの前にGetでキャッシュされている場合、そのデータが消去される
        /// </summary>
        /// <param name="id"></param>
        [CacheIdFire("Id", "id")]
        [CacheEntityFire("TEST")]
        [DomainDataSync(DynamicApiDatabase.TABLE_CONTAINERDYNAMICSEPARATION, "id")]
        public void Delete(string id)
        {
            // DELETE test WHERE id=@id
        }

        public void Delete(List<string> id)
        {
        }

        [CacheIdFire("Id", "model.Id")]
        [CacheEntityFire("TEST")]
        [DomainDataSync(DynamicApiDatabase.TABLE_CONTAINERDYNAMICSEPARATION, "model.Id")]
        public void Update(TestModel model)
        {
        }

        [DomainDataSync(ParameterType.Result, DynamicApiDatabase.TABLE_CONTAINERDYNAMICSEPARATION, "Id")]
        [DomainDataSync(DynamicApiDatabase.TABLE_CONTAINERDYNAMICSEPARATION, "model.Id")]
        public IList<TestModel> UpdateArray(List<TestModel> model)
        {
            return null;
        }
    }
}
