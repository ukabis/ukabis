# ukabis

![logo](https://github.com/ukabis/ukabis/blob/main/logo.png)

## ukabisとは
近年、高まる消費者の食品の鮮度や品質に対するニーズへの対応、本来食べられるのに廃棄される食品「フードロス」の削減を実現するために、フードチェーン（食の生産、加工・流通、販売・消費、資源循環、育種/品種改良）の変革が求められています。

また、日本の農業は、後継者不足、貿易自由化の中での国際競争力の強化などの課題に直面しており、イノベーションによる高付加価値化・生産性の向上や、2030年に5兆円を目指す「農林水産物・食品の輸出拡大実行戦略」実現のためにも、フードチェーンの再構築が喫緊の課題といえます。

これらの課題に対応するため、第2期戦略的イノベーション創造プログラムでは、ＩＣＴを活用し、国内外の多様化するニーズなどの情報を従来の産業の枠を越えて連携することで、生産者の持つ可能性と潜在力を引き出し、ビジネス力の強化やサービスの質の向上を図るとともに、需給マッチングや精密農業を通じてフードロスの削減にも資する、「スマートフードシステム」の実現をめざす研究開発に2018年度から取り組んでいます。

スマートフードチェーンプラットフォーム「ukabis」は、このスマートフードシステムを支える生産、加工・流通、販売・消費、資源循環、育種/品種改良におけるデータ共有を可能とする情報連携基盤です。

## 本プロジェクトについて
本プロジェクトは、ukabisのソースコードを一部改変したうえで、誰でも利用可能となるよう、オープンソースとして公開したものです。

### ライセンス
本プロジェクトのソースコードは、MITライセンスで提供いたします。
* MITライセンスについて(原文): https://opensource.org/license/mit/
* 上記の参考和訳: https://licenses.opensource.jp/MIT/MIT.html

### ご利用にあたっての注意事項
"ukabis"という用語は、日本において商標登録申請中となります。<br/>
ソースコードのご利用者にあたっては、第三者の権利を侵害することのないようご注意ください。

また、本プロジェクトのソースコードは無保証であり、ビルドや実行方法、あるいはプログラムの動作に関するサポートも行っていません。あらかじめご了承ください。

### フォルダの構成
各フォルダの概要は以下のとおりです。

* JP.DataHub.ApiWeb : ukabis本体
* JP.DataHub.Com : ukabisの共通基盤処理
* JP.DataHub.OData : ManageApiのODataに関わる処理
* UnitTest : UnitTest(単体テスト)用の処理
* UnityInterceptor : UnityContainer (https://github.com/unitycontainer/interception) をukabis用に改変したもの
* Dashboard : ダッシュボード(ukabisを利用するアプリケーション)
* FoodDonation : こども食堂 提供食材登録システム(ukabisを利用するアプリケーション)
* JAS : JAS統合パッケージ(ukabisを利用するアプリケーション)
* SDGs : 自治体SDGs認証システム(ukabisを利用するアプリケーション)

ukabis本体(JP.DataHub.ApiWeb)の構成は以下のとおりです。

* IT.JP.DataHub.ApiWeb  : IT.～以降の同名のプロジェクトのIT(自動テスト)
* IT.JP.DataHub.ManageApi  : IT.～以降の同名のプロジェクトのIT
* IT.JP.DataHub.ODataOverPartition  : IT.～以降の同名のプロジェクトのIT
* IT.JP.DataHub.SmartFoodChainAOP  : IT.～以降の同名のプロジェクトのIT
* JP.DataHub.AdminWeb  : 管理画面ホスト
* JP.DataHub.AdminWeb.Core  : 管理画面基本機能
* JP.DataHub.AdminWeb.Core.Component  : 管理画面UIコンポーネント
* JP.DataHub.AdminWeb.Service  : 管理画面サービスクラス群
* JP.DataHub.AdminWeb.Service.Interface  : 管理画面共通I/F
* JP.DataHub.AdminWeb.WebAPI  : 管理画面APIモデル
* JP.DataHub.Aop  : AOP基本機能
* JP.DataHub.Aop.Build  : Aopに関連するdllを生成
* JP.DataHub.Api.Core  : API基本機能
* JP.DataHub.ApiFilterIntegratedTest  : ApiFilterのIT
* JP.DataHub.ApiFilterSample  : ApiFilterサンプル実装
* JP.DataHub.ApiWeb  : DynamicApi(APIサーバー)ホスト
* JP.DataHub.ApiWeb.Core  : DynamicAPI基本機能
* JP.DataHub.ApiWeb.Domain  : DynamicAPIのドメイン
* JP.DataHub.ApiWeb.Infrastructure  : DynamicApiの根幹処理
* JP.DataHub.ApiWeb.Interface  : DynamicAPIで利用するI/F
* JP.DataHub.Batch.AsyncDynamicApi  : DynamicAPI自動実行バッチ
* JP.DataHub.Batch.DomainDataSync  : スキーマ間のデータ同期バッチ
* JP.DataHub.Batch.LoggingEventProcess  : ログ収集バッチ
* JP.DataHub.Batch.LoggingSummary  : ログサマリーバッチ
* JP.DataHub.Batch.Revoke  : 取り消し処理のバッチ
* JP.DataHub.Batch.TermasRevoke  : 同意取り消し
* JP.DataHub.Batch.TrailEventProcess  : イベント証跡のバッチ
* JP.DataHub.Infrastructure.Database  : 基盤共通のDB処理
* JP.DataHub.ManageApi  : 管理用API(APIサーバー)ホスト
* JP.DataHub.ManageApi.Core  : 管理用APIの基盤部分
* JP.DataHub.ManageApi.Infrastructure  : 管理用APIの根幹処理
* JP.DataHub.ManageApi.Service  : 管理用APIのサービスクラス群
* JP.DataHub.ODataOverPartition  : 領域越えのOData処理
* JP.DataHub.SystemAdminWeb  : システム管理者画面ホスト
* JP.DataHub.SmartFoodChainAOP  : AOP機能
* JP.DataHub.Web.Core  : 基盤共通のweb

### ダッシュボードについて
ukabisを利用するアプリケーションです。<br/>
ukabisのデータベースへ蓄積した農作物のトレーサビリティに関するデータの閲覧や、<br/>
ukabisを利用する事業社、および所属するスタッフや事業所の情報登録・編集、グループの登録・編集等を行うアプリケーションです。
Oracle APEX (Application Express)で実装されています。<br/>
ご利用の場合は、本プロジェクトに含まれているSQLファイルをOracle APEX上でインポートしてください。

### こども食堂 提供食材登録システムについて
ukabisを利用するアプリケーションです。<br/>
事業者が提供する食材と、その食材を希望するこども食堂とのマッチングを行うことを目的とし、<br/>
提供する食材情報の登録・編集、閲覧等を行うことができます。<br/>
PHPで実装されており、動作させるにはApache等のWebサーバが必要です。

### JAS統合パッケージについて
ukabisを利用するアプリケーションです。<br/>
センサーの紐付け、個体識別番号の発行、入出荷、センサーデータのアップロード、JAS格付、<br/>
JASラベル印刷など、JAS認定にかかわる各種機能を統合するアプリケーションです。<br/>
TypeScriptとVue.jsで実装されています。

### 自治体SDGs認証システムについて
ukabisを利用するアプリケーションです。<br/>
生産者や飲食店向けのSDGs認証制度に関する申請や認証などを行うアプリケーションです。<br/>
PHPで実装されており、動作させるにはApache等のWebサーバが必要です。

### Visual Studio用ソリューションファイルについて
本プロジェクトで使用しているソリューションファイル(sln)は以下のとおりです。

* JP.DataHub.ApiWeb\IT.JP.DataHub.NET.sln : IT(自動テスト)用のソリューション
* JP.DataHub.ApiWeb\JP.DataHub.NET.sln : ukabis本体のソリューション

## 本プロジェクトのビルドにあたっての留意事項
本プロジェクトのソースコードは、OSS化にあたり一部を修正、あるいは削除しています。<br/>
そのため、本プロジェクトのソースコードをビルドして実行するには、以下の対応を利用者がご自身で行う必要があります。

### Jsonのスキーマ処理の実装
OSS化にあたり、有償ライセンス製品であるNewtonsoftの『Json.NET Schema』に依存する処理を、オリジナルのソースコードから削除しています。<br/>
代わりに、以下のパスにインターフェースや中身が空のメソッドを定義していますので、これらを利用してスキーマ処理を実装してください。

* JP.DataHub.Com\JP.DataHub.Com\JP.DataHub.Com\Json\Schema\ 配下のソース全て
* JP.DataHub.ApiWeb\JP.DataHub.ApiWeb.Domain\Context\DynamicApi\JsonValidator\JsonFormatCustomValidator.cs GetSchemaFormatList()
* JP.DataHub.ApiWeb\JP.DataHub.ManageApi.Service\DymamicApi\JsonFormatCustomValidator.cs GetSchemaFormatList()

### 各種サービスへの接続設定
以下のファイルを修正し、DB接続設定、管理者IDの設定、OpenIDのIDやパスワードなどを設定してください。<br/>
なお、ファイル名に"{Environment}"と記載されている箇所は、DevelopmentやStaging等の、環境を表す文字列に置き換えてください。

#### appsetting.json
アプリケーション設定ファイルです。<br/>
これらのファイルでは、DB接続設定や管理者ID、OpenIDのIDやパスワードの設定を行います。

* JP.DataHub.ApiWeb\JP.DataHub.AdminWeb\appsettings.{Environment}.json
* JP.DataHub.ApiWeb\JP.DataHub.SystemAdminWeb\appsettings.{Environment}.json
* JP.DataHub.ApiWeb\JP.DataHub.ApiWeb\appsettings.{Environment}.json
* JP.DataHub.ApiWeb\JP.DataHub.ManageApi\appsettings.{Environment}.json
* JP.DataHub.Com\JP.DataHub.Com\JP.DataHub.Infrastructure.Core\appsettings.json
* JP.DataHub.ApiWeb\JP.DataHub.Batch.AsyncDynamicApi\appsettings.{Environment}.json
* JP.DataHub.ApiWeb\JP.DataHub.Batch.DomainDataSync\appsettings.{Environment}.json
* JP.DataHub.ApiWeb\JP.DataHub.Batch.LoggingEventProcess\appsettings.{Environment}.json
* JP.DataHub.ApiWeb\JP.DataHub.Batch.LoggingSummary\appsettings.{Environment}.json
* JP.DataHub.Net6\JP.DataHub.ApiWeb\JP.DataHub.Batch.Revoke\appsettings.{Environment}.json
* JP.DataHub.ApiWeb\JP.DataHub.Batch.TrailEventProcess\appsettings.Development.json
* UnitTest\UnitTest.JP.DataHub.ApiWeb.Domain\appsettings.json
* UnitTest\UnitTest.JP.DataHub.ApiWeb.Infrastructure\appsettings.json
* JP.DataHub.Com\JP.DataHub.Com\UnitTest.JP.DataHub.Com\appsettings.json
* JP.DataHub.Com\JP.DataHub.Com\UnitTest.JP.DataHub.Infrastructore.* Core\appsettings.json
* JP.DataHub.ApiWeb\IT.JP.DataHub.ApiWeb\appsettings.{Environment}.json
* JP.DataHub.ApiWeb\IT.JP.DataHub.ManageApi\appsettings.{Environment}.json
* JP.DataHub.ApiWeb\IT.JP.DataHub.ODataOverPartition\appsettings.{Environment}.json
* JP.DataHub.ApiWeb\IT.JP.DataHub.SmartFoodChainAOP\appsettings.{Environment}.json
* JP.DataHub.Com\JP.DataHub.Com\UnitTest.JP.DataHub.Com\appsettings.json

#### connectionstring.json
DBの接続文字列に関する設定ファイルです。<br/>
これらのファイルでは、DB接続文字列の設定を行います。

* JP.DataHub.ApiWeb\JP.DataHub.ApiWeb\connectionstring.{Environment}.json
* JP.DataHub.ApiWeb\JP.DataHub.ManageApi\connectionstring.{Environment}.json
* JP.DataHub.ApiWeb\JP.DataHub.Batch.AsyncDynamicApi\connectionstring.{Environment}.json
* JP.DataHub.ApiWeb\JP.DataHub.Batch.LoggingEventProcess\connectionstring.{Environment}.json
* JP.DataHub.ApiWeb\JP.DataHub.Batch.LoggingSummary\connectionstring.{Environment}.json
* JP.DataHub.ApiWeb\JP.DataHub.Batch.TrailEventProcess\connectionstring.{Environment}.json
* UnitTest\UnitTest.JP.DataHub.ApiWeb.Infrastructure\connectionstring.json

#### server.json
サーバーの設定ファイルです。<br/>
これらのファイルでは、OpenIDのIDやパスワード、APIサーバー(ManageApi、DynamicWeb)の接続先URL等の設定を行います。

* JP.DataHub.ApiWeb\JP.DataHub.AdminWeb\server.{Environment}.json
* JP.DataHub.ApiWeb\JP.DataHub.SystemAdminWeb\server.{Environment}.json
* JP.DataHub.ApiWeb\JP.DataHub.Batch.DomainDataSync\server.json
* JP.DataHub.ApiWeb\JP.DataHub.Batch.Revoke\server.json
* JP.DataHub.ApiWeb\IT.JP.DataHub.ApiWeb\server.json
* JP.DataHub.ApiWeb\IT.JP.DataHub.ManageApi\server.json
* JP.DataHub.ApiWeb\IT.JP.DataHub.ODataOverPartition\server.json
* JP.DataHub.ApiWeb\IT.JP.DataHub.SmartFoodChainAOP\server.json
* JP.DataHub.Com\JP.DataHub.Com\UnitTest.JP.DataHub.Com\server.json

#### account.json
アカウントの設定ファイルです。<br/>
これらのファイルでは、OpenIDのIDやパスワードの設定を行います。

* JP.DataHub.Com\JP.DataHub.Com\UnitTest.JP.DataHub.Com\account.json
* JP.DataHub.ApiWeb\JP.DataHub.AdminWeb\account.DevelopAzure.json
* JP.DataHub.ApiWeb\JP.DataHub.SystemAdminWeb\account.DevelopAzure.json
* JP.DataHub.ApiWeb\JP.DataHub.Batch.DomainDataSync\account.json
* JP.DataHub.ApiWeb\JP.DataHub.Batch.Revoke\account.json
* JP.DataHub.ApiWeb\IT.JP.DataHub.ApiWeb\account.json
* JP.DataHub.ApiWeb\IT.JP.DataHub.ManageApi\account.json
* JP.DataHub.ApiWeb\IT.JP.DataHub.ODataOverPartition\account.json
* JP.DataHub.ApiWeb\IT.JP.DataHub.SmartFoodChainAOP\account.json
* JP.DataHub.Com\JP.DataHub.Com\UnitTest.JP.DataHub.Com\account.json

#### .oci
OCI(Oracle Cloud Infrastructure)へ接続する際に用いられる、pemファイル及びconfigファイルを格納する場所です。

* JP.DataHub.ApiWeb\JP.DataHub.ApiWeb\.oci\
* JP.DataHub.ApiWeb\JP.DataHub.ManageApi\.oci\

格納するファイル名については、以下の形式にしてください。

* config_{Environment}
* smartfoodchain_api_key_{Environment}.pem

#### wallet
Oracle Databaseへ接続する際に使用するwalletファイルを格納する場所です。<br/>
OCI上で入手したwalletファイルを解凍し、中に含まれるファイルを格納してください。

* JP.DataHub.ApiWeb\JP.DataHub.ApiWeb\wallet\{Environment}\
* JP.DataHub.ApiWeb\JP.DataHub.ManageApi\wallet\{Environment}\
