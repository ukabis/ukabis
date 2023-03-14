using JP.DataHub.Com.SQL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace UnitTest.JP.DataHub.Com.SQL
{
    [TestClass]
    public class UnitTest_TwoWaySql
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
        }

        [TestInitialize]
        public void TestInitialize()
        {
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        [TestMethod]
        public void Core_TwoWaySql_Normal()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("min", 5);
            dic.Add("max", 100);
            string sql = "SELECT * FROM Entity WHERE age>=/*ds min*/10 AND age<=/*ds max*/20";
            var two = new TwowaySqlParser(sql, dic);
            two.Sql.Is("SELECT * FROM Entity WHERE age>=@min AND age<=@max");
            two.Parameters.Count().Is(2);
            two.Parameters["@min"].Is(5);
            two.Parameters["@max"].Is(100);
        }

        [TestMethod]
        public void Core_TwoWaySql_duplicate()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("min", 5);
            string sql = "SELECT * FROM Entity WHERE age==/*ds min*/10 OR age==/*ds min*/20";
            var two = new TwowaySqlParser(sql, dic);
            two.Sql.Is("SELECT * FROM Entity WHERE age==@min OR age==@min");
            two.Parameters.Count().Is(1);
            two.Parameters["@min"].Is(5);
        }

        [TestMethod]
        public void Core_TwoWaySql_In()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("test", new object[] { 100, 200, 300, 400 });
            string sql = "SELECT * FROM Entity WHERE hoge IN /*ds test*/(1,2,3)";
            var two = new TwowaySqlParser(sql, dic);
            two.Sql.Is("SELECT * FROM Entity WHERE hoge IN (@test_0, @test_1, @test_2, @test_3)");
            two.Parameters.Count().Is(4);
            two.Parameters["@test_0"].Is(100);
            two.Parameters["@test_1"].Is(200);
            two.Parameters["@test_2"].Is(300);
            two.Parameters["@test_3"].Is(400);
        }

        private string if_sql = @"
SELECT * FROM Entity
/*ds where*/
WHERE
/*ds if name!=null*/
    name LIKE /*ds name*/'A%'
/*ds end if*/
/*ds if age!=null*/
    AND age=/*ds age*/100
/*ds end if*/
/*ds end where*/
";

        private string if_sql2 = @"
UPDATE
    hoge
SET
    column1=/*ds column1*/'null' 
/*ds if column2!=null*/
    ,column2=/*ds column2*/'null' 
/*ds end if*/
WHERE
    key=/*ds key*/'123'
";

        private string if_sql3 = @"
SELECT * FROM Entity
WHERE
/*ds if name!=null*/
    name LIKE /*ds name*/'A%' AND 
/*ds end if*/
/*ds if age!=null*/
    age=/*ds age*/100 AND
/*ds end if*/
    is_active = 1
";

        /// <summary>
        /// ds-whereなし、二重ネスト
        /// </summary>
        private string if_sql_nest = @"
UPDATE
    hoge
SET
    key = 'fuga'
/*ds if name!=null*/
    ,name = /*ds name*/'A%'
/*ds if age!=null*/
    ,age = /*ds age*/100
/*ds end if*/
/*ds end if*/
";

        /// <summary>
        /// ds-whereあり、二重ネスト
        /// </summary>
        private string if_sql_nest_where = @"
SELECT * FROM Entity
/*ds where*/
WHERE
/*ds if name!=null*/
    name LIKE /*ds name*/'A%'
/*ds if age!=null*/
    AND age=/*ds age*/100
/*ds end if*/
/*ds end if*/
/*ds end where*/
";

        /// <summary>
        /// ds-whereあり、三重ネスト、複数の子要素、ds-ifの間に文字列あり
        /// </summary>
        private string if_sql_nest_where_complex = @"
SELECT * FROM Entity
/*ds where*/
WHERE
/*ds if name!=null*/
    name LIKE /*ds name*/'A%'
/*ds if age!=null*/
    AND age=/*ds age*/100
/*ds if ex1!=null*/
    AND ex1=/*ds ex1*/'hoge'
/*ds end if*/
    AND ex2=/*ds ex2*/'hoge'
/*ds if ex3!=null*/
    AND ex3=/*ds ex3*/'hoge'
/*ds end if*/
    AND ex4=/*ds ex4*/'hoge'
/*ds end if*/
/*ds end if*/
/*ds end where*/
";

        [TestMethod]
        public void Core_TwoWaySql_If1()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            var two = new TwowaySqlParser(if_sql, dic);
            two.Sql.Is(@"
SELECT * FROM Entity");
            two.Parameters.Count().Is(0);
        }

        [TestMethod]
        public void Core_TwoWaySql_If2()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("name", "hoge");
            var two = new TwowaySqlParser(if_sql, dic);
            two.Sql.Is(@"
SELECT * FROM Entity
WHERE
    name LIKE @name");
            two.Parameters.Count().Is(1);
            two.Parameters["@name"].Is("hoge");
        }

        [TestMethod]
        public void Core_TwoWaySql_If3()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("age", 123);
            var two = new TwowaySqlParser(if_sql, dic);
            two.Sql.Is(@"
SELECT * FROM Entity
WHERE
    age=@age");
            two.Parameters.Count().Is(1);
            two.Parameters["@age"].Is(123);
        }

        [TestMethod]
        public void Core_TwoWaySql_If4()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("name", "hoge");
            dic.Add("age", 123);
            var two = new TwowaySqlParser(if_sql, dic);
            two.Sql.Is(@"
SELECT * FROM Entity
WHERE
    name LIKE @name

    AND age=@age");
            two.Parameters.Count().Is(2);
            two.Parameters["@name"].Is("hoge");
            two.Parameters["@age"].Is(123);
        }

        [TestMethod]
        public void Core_TwoWaySql_If2_1()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("column1", "test");
            dic.Add("key", "1");
            var two = new TwowaySqlParser(if_sql2, dic);
            two.Sql.Is(@"
UPDATE
    hoge
SET
    column1=@column1 

WHERE
    key=@key");
            two.Parameters.Count().Is(2);
            two.Parameters["@column1"].Is("test");
            two.Parameters["@key"].Is("1");
        }

        [TestMethod]
        public void BlobFileSharding_TwoWaySql_If2_2()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("column1", "test");
            dic.Add("column2", "xyz");
            dic.Add("key", "1");
            var two = new TwowaySqlParser(if_sql2, dic);
            two.Sql.Is(@"
UPDATE
    hoge
SET
    column1=@column1 
    ,column2=@column2 

WHERE
    key=@key");
            two.Parameters.Count().Is(3);
            two.Parameters["@column1"].Is("test");
            two.Parameters["@column2"].Is("xyz");
            two.Parameters["@key"].Is("1");
        }

        [TestMethod]
        public void Core_TwoWaySql_If3_1()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("age", 123);
            var two = new TwowaySqlParser(if_sql3, dic);
            two.Sql.Is(@"
SELECT * FROM Entity
WHERE

    age=@age AND

    is_active = 1");
            two.Parameters.Count().Is(1);
            two.Parameters["@age"].Is(123);
        }

        [TestMethod]
        public void Core_TwoWaySql_If3_2()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            var two = new TwowaySqlParser(if_sql3, dic);
            two.Sql.Is(@"
SELECT * FROM Entity
WHERE

    is_active = 1");
            two.Parameters.Count().Is(0);
        }

        [TestMethod]
        public void Core_TwoWaySql_6937_不具合再現1()
        {
            var query = @"
SELECT * FROM Entity
/*ds where*/
WHERE
/*ds if name != null*/
AND name = ANY (
        SELECT name FROM Entity2 WHERE name = /*ds name*/'AA'
    )
/*ds end if*/
/*ds end where*/
";
            // 不具合修正前の結果(ANY以降が欠落)
            // SELECT * FROM Entity
            // WHERE
            // name = ANY(

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("name", "test");
            var two = new TwowaySqlParser(query, dic);
            two.Sql.Is(@"
SELECT * FROM Entity
WHERE
name = ANY (
        SELECT name FROM Entity2 WHERE name = @name
    )");
            two.Parameters.Count().Is(1);
            two.Parameters["@name"].Is("test");
        }

        [TestMethod]
        public void Core_TwoWaySql_6937_不具合再現2()
        {
            // ANYの閉じカッコの前の空白がタブ
            var query = @"
SELECT * FROM Entity
/*ds where*/
WHERE
/*ds if name1 != null*/
    name1 = /*ds name1*/'AA'
/*ds end if*/
/*ds if name2 != null*/
AND name2 = ANY (
        SELECT name FROM Entity2 WHERE name = /*ds name2*/'AA'
	)
/*ds end if*/
/*ds end where*/
";
            // 不具合修正前の結果(ANYの閉じカッコが欠落)
            // SELECT * FROM Entity
            // WHERE
            //     name1 = @name1
            // AND name2 = ANY (
            //         SELECT name FROM Entity2 WHERE name = @name2

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("name1", "test1");
            dic.Add("name2", "test2");
            var two = new TwowaySqlParser(query, dic);
            two.Sql.Is(@"
SELECT * FROM Entity
WHERE
    name1 = @name1

AND name2 = ANY (
        SELECT name FROM Entity2 WHERE name = @name2
	)");
            two.Parameters.Count().Is(2);
            two.Parameters["@name1"].Is("test1");
            two.Parameters["@name2"].Is("test2");
        }

        [TestMethod]
        public void Core_TwoWaySql_6937_不具合再現3()
        {
            var query = @"
SELECT * FROM Entity
/*ds where*/
WHERE
/*ds if name != null*/
    name = /*ds name*/'AA'
/*ds end if*/
/*ds end where*/
GROUP BY name
";
            // 不具合修正前の結果(WHEREの後ろが欠落)
            // SELECT * FROM Entity
            // WHERE
            //     name = @name

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("name", "test");
            var two = new TwowaySqlParser(query, dic);
            two.Sql.Is(@"
SELECT * FROM Entity
WHERE
    name = @name 
GROUP BY name");
            two.Parameters.Count().Is(1);
            two.Parameters["@name"].Is("test");
        }

        [TestMethod]
        public void Core_TwoWaySql_6937_不具合再現4()
        {
            var query = @"
SELECT * FROM Entity
/*ds where*/
WHERE
/*ds if name1 != null*/
    name1 = /*ds name1*/'AA'
/*ds end if*/
AND name2 = 'hoge'
/*ds if name3 != null*/
AND name3 = /*ds name3*/'AA'
/*ds end if*/
/*ds end where*/
";
            // 不具合修正前の結果(ifとifの間が欠落)
            // SELECT * FROM Entity
            // WHERE
            //     name1 = @name

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("name1", "test1");
            var two = new TwowaySqlParser(query, dic);
            two.Sql.Is(@"
SELECT * FROM Entity
WHERE
    name1 = @name1

AND name2 = 'hoge'");
            two.Parameters.Count().Is(1);
            two.Parameters["@name1"].Is("test1");
        }

        [TestMethod]
        public void Core_TwoWaySql_Nest1()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            var two = new TwowaySqlParser(if_sql_nest, dic);
            two.Sql.Is(@"
UPDATE
    hoge
SET
    key = 'fuga'");
            two.Parameters.Count().Is(0);
        }

        [TestMethod]
        public void Core_TwoWaySql_Nest2()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("name", "hoge");
            var two = new TwowaySqlParser(if_sql_nest, dic);
            two.Sql.Is(@"
UPDATE
    hoge
SET
    key = 'fuga'
    ,name = @name");
            two.Parameters.Count().Is(1);
            two.Parameters["@name"].Is("hoge");
        }

        [TestMethod]
        public void Core_TwoWaySql_Nest3()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("age", 123);
            var two = new TwowaySqlParser(if_sql_nest, dic);
            two.Sql.Is(@"
UPDATE
    hoge
SET
    key = 'fuga'");
            two.Parameters.Count().Is(0);
        }

        [TestMethod]
        public void Core_TwoWaySql_Nest4()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("name", "hoge");
            dic.Add("age", 123);
            var two = new TwowaySqlParser(if_sql_nest, dic);
            two.Sql.Is(@"
UPDATE
    hoge
SET
    key = 'fuga'
    ,name = @name
    ,age = @age");
            two.Parameters.Count().Is(2);
            two.Parameters["@name"].Is("hoge");
            two.Parameters["@age"].Is(123);
        }

        [TestMethod]
        public void Core_TwoWaySql_Where_Nest1()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            var two = new TwowaySqlParser(if_sql_nest_where, dic);
            two.Sql.Is(@"
SELECT * FROM Entity");
            two.Parameters.Count().Is(0);
        }

        [TestMethod]
        public void Core_TwoWaySql_Where_Nest2()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("name", "hoge");
            var two = new TwowaySqlParser(if_sql_nest_where, dic);
            two.Sql.Is(@"
SELECT * FROM Entity
WHERE
    name LIKE @name");
            two.Parameters.Count().Is(1);
            two.Parameters["@name"].Is("hoge");
        }

        [TestMethod]
        public void Core_TwoWaySql_Where_Nest3()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("age", 123);
            var two = new TwowaySqlParser(if_sql_nest_where, dic);
            two.Sql.Is(@"
SELECT * FROM Entity");
            two.Parameters.Count().Is(0);
        }

        [TestMethod]
        public void Core_TwoWaySql_Where_Nest4()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("name", "hoge");
            dic.Add("age", 123);
            var two = new TwowaySqlParser(if_sql_nest_where, dic);
            two.Sql.Is(@"
SELECT * FROM Entity
WHERE
    name LIKE @name
    AND age=@age");
            two.Parameters.Count().Is(2);
            two.Parameters["@name"].Is("hoge");
            two.Parameters["@age"].Is(123);
        }

        [TestMethod]
        public void Core_TwoWaySql_Where_Nest_Complex1()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            var two = new TwowaySqlParser(if_sql_nest_where_complex, dic);
            two.Sql.Is(@"
SELECT * FROM Entity");
            two.Parameters.Count().Is(0);
        }

        [TestMethod]
        public void Core_TwoWaySql_Where_Nest_Complex2()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("age", 123);
            dic.Add("ex1", "piyo1");
            dic.Add("ex2", "piyo2");
            dic.Add("ex3", "piyo3");
            dic.Add("ex4", "piyo4");
            var two = new TwowaySqlParser(if_sql_nest_where_complex, dic);
            two.Sql.Is(@"
SELECT * FROM Entity");
            two.Parameters.Count().Is(0);
        }

        [TestMethod]
        public void Core_TwoWaySql_Where_Nest_Complex3()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("name", "hoge");
            dic.Add("age", 123);
            dic.Add("ex2", "piyo2");
            dic.Add("ex4", "piyo4");
            var two = new TwowaySqlParser(if_sql_nest_where_complex, dic);
            two.Sql.Is(@"
SELECT * FROM Entity
WHERE
    name LIKE @name
    AND age=@age

    AND ex2=@ex2

    AND ex4=@ex4");
            two.Parameters.Count().Is(4);
            two.Parameters["@name"].Is("hoge");
            two.Parameters["@age"].Is(123);
            two.Parameters["@ex2"].Is("piyo2");
            two.Parameters["@ex4"].Is("piyo4");
        }

        [TestMethod]
        public void Core_TwoWaySql_Where_Nest_Complex4()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("name", "hoge");
            dic.Add("age", 123);
            dic.Add("ex2", "piyo2");
            dic.Add("ex3", "piyo3");
            dic.Add("ex4", "piyo4");
            var two = new TwowaySqlParser(if_sql_nest_where_complex, dic);
            two.Sql.Is(@"
SELECT * FROM Entity
WHERE
    name LIKE @name
    AND age=@age

    AND ex2=@ex2
    AND ex3=@ex3

    AND ex4=@ex4");
            two.Parameters.Count().Is(5);
            two.Parameters["@name"].Is("hoge");
            two.Parameters["@age"].Is(123);
            two.Parameters["@ex2"].Is("piyo2");
            two.Parameters["@ex3"].Is("piyo3");
            two.Parameters["@ex4"].Is("piyo4");
        }

        [TestMethod]
        public void Core_TwoWaySql_Where_Nest_Complex5()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("name", "hoge");
            dic.Add("age", 123);
            dic.Add("ex1", "piyo1");
            dic.Add("ex2", "piyo2");
            dic.Add("ex3", "piyo3");
            dic.Add("ex4", "piyo4");
            var two = new TwowaySqlParser(if_sql_nest_where_complex, dic);
            two.Sql.Is(@"
SELECT * FROM Entity
WHERE
    name LIKE @name
    AND age=@age
    AND ex1=@ex1

    AND ex2=@ex2
    AND ex3=@ex3

    AND ex4=@ex4");
            two.Parameters.Count().Is(6);
            two.Parameters["@name"].Is("hoge");
            two.Parameters["@age"].Is(123);
            two.Parameters["@ex1"].Is("piyo1");
            two.Parameters["@ex2"].Is("piyo2");
            two.Parameters["@ex3"].Is("piyo3");
            two.Parameters["@ex4"].Is("piyo4");
        }
    }
}
