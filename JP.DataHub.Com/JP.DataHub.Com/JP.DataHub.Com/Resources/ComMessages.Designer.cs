﻿//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

namespace JP.DataHub.Com.Resources {
    using System;
    
    
    /// <summary>
    ///   ローカライズされた文字列などを検索するための、厳密に型指定されたリソース クラスです。
    /// </summary>
    // このクラスは StronglyTypedResourceBuilder クラスが ResGen
    // または Visual Studio のようなツールを使用して自動生成されました。
    // メンバーを追加または削除するには、.ResX ファイルを編集して、/str オプションと共に
    // ResGen を実行し直すか、または VS プロジェクトをビルドし直します。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ComMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ComMessages() {
        }
        
        /// <summary>
        ///   このクラスで使用されているキャッシュされた ResourceManager インスタンスを返します。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("JP.DataHub.Com.Resources.ComMessages", typeof(ComMessages).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   すべてについて、現在のスレッドの CurrentUICulture プロパティをオーバーライドします
        ///   現在のスレッドの CurrentUICulture プロパティをオーバーライドします。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   ClientIdが見つからないか、ベンダーまたはシステムが使用できません。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ClientIdNotFoundOrVendorSystemUnusable {
            get {
                return ResourceManager.GetString("ClientIdNotFoundOrVendorSystemUnusable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   ベンダーシステム認証用のアクセストークンを取得し直してください。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ClientIdNotFoundOrVendorSystemUnusable_Detail {
            get {
                return ResourceManager.GetString("ClientIdNotFoundOrVendorSystemUnusable_Detail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   HTTPヘッダー：AuthorizationにOpenID認証用のアクセストークンを指定してください。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string OpenIdAuthRequired_Detail {
            get {
                return ResourceManager.GetString("OpenIdAuthRequired_Detail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   ベンダーやシステムが未指定です に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string VendorOrSystemUnspecified {
            get {
                return ResourceManager.GetString("VendorOrSystemUnspecified", resourceCulture);
            }
        }
        
        /// <summary>
        ///   AccessTokenIdがありません。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string VendorSystemAccessTokenIdMissing {
            get {
                return ResourceManager.GetString("VendorSystemAccessTokenIdMissing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   VendorSystem AccessTokenが見つかりません に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string VendorSystemAccessTokenNotFound {
            get {
                return ResourceManager.GetString("VendorSystemAccessTokenNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   ベンダーシステム認証用のアクセストークンを取得し直してください に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string VendorSystemAccessTokenNotFound_Detail {
            get {
                return ResourceManager.GetString("VendorSystemAccessTokenNotFound_Detail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   VendorSystem認証が必要です に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string VendorSystemAuthRequired {
            get {
                return ResourceManager.GetString("VendorSystemAuthRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   HTTPヘッダー：X-Authorizationにベンダーシステム認証用のアクセストークンを指定してください。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string VendorSystemAuthRequired_Detail {
            get {
                return ResourceManager.GetString("VendorSystemAuthRequired_Detail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   VendorSystem AccessTokenの有効期限が切れています に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string VendorSystemTokenExpired {
            get {
                return ResourceManager.GetString("VendorSystemTokenExpired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   VendorSystem AccessTokenが無効です に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string VendorSystemTokenInvalid {
            get {
                return ResourceManager.GetString("VendorSystemTokenInvalid", resourceCulture);
            }
        }
    }
}
