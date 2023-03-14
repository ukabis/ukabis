namespace JP.DataHub.ManageApi.Service.DymamicApi
{
    public class ActionType
    {
        public ActionTypes Value { get; }

        public ActionType(ActionTypes value)
        {
            Value = value;

        }
        public static ActionType Parse(string actionTypeCode)
        {
            foreach (ActionTypes value in Enum.GetValues(typeof(ActionTypes)))
            {
                //Script,Asyncは内部的にActionTypeとして扱うためパースできないようにする。
                if (value.GetCode() == actionTypeCode && value != ActionTypes.Script && value != ActionTypes.Async)
                {
                    return new ActionType(value);
                }
            }
            return new ActionType(ActionTypes.Unknown);
        }
    }

    public enum ActionTypes
    {
        [ActionTypeConvert(Code = "xxx")]
        [ActionClassConvert(Version = 1, ActionClass = null)]
        Unknown,
        [ActionTypeConvert(Code = "reg")]
        [ActionClassConvert(Version = 1, ActionClass = typeof(Actions.RegistAction))]
        Regist,
        [ActionTypeConvert(Code = "quy")]
        [ActionClassConvert(Version = 1, ActionClass = typeof(Actions.QueryAction))]
        Query,
        [ActionTypeConvert(Code = "del")]
        [ActionClassConvert(Version = 1, ActionClass = typeof(Actions.DeleteDataAction))]
        DeleteData,

        [ActionTypeConvert(Code = "gtw")]
        [ActionClassConvert(Version = 1, ActionClass = typeof(Actions.GatewayAction))]
        GateWay,
        [ActionTypeConvert(Code = "scr")]
        [ActionClassConvert(Version = 1, ActionClass = typeof(Actions.ScriptAction))]
        Script,

        [ActionTypeConvert(Code = "oda")]
        [ActionClassConvert(Version = 1, ActionClass = typeof(Actions.ODataAction))]
        OData,

        [ActionTypeConvert(Code = "upd")]
        [ActionClassConvert(Version = 1, ActionClass = typeof(Actions.UpdateAction))]
        Update,

        [ActionTypeConvert(Code = "async")]
        [ActionClassConvert(Version = 1, ActionClass = typeof(Actions.AsyncAction))]
        Async,

        [ActionTypeConvert(Code = "equery")]
        [ActionClassConvert(Version = 1, ActionClass = typeof(Actions.EnumerableQueryAction))]
        EnumerableQuery,

        [ActionTypeConvert(Code = "aup")]
        [ActionClassConvert(Version = 1, ActionClass = typeof(Actions.AttachFileUploadAction))]
        AttachFileUpload,

        [ActionTypeConvert(Code = "adl")]
        [ActionClassConvert(Version = 1, ActionClass = typeof(Actions.AttachFileDownloadAction))]
        AttachFileDownload,

        [ActionTypeConvert(Code = "ade")]
        [ActionClassConvert(Version = 1, ActionClass = typeof(Actions.AttachFileDeleteAction))]
        AttachFileDelete,

        [ActionTypeConvert(Code = "odd")]
        [ActionClassConvert(Version = 1, ActionClass = typeof(Actions.ODataDeleteAction))]
        ODataDelete,

        [ActionTypeConvert(Code = "odp")]
        [ActionClassConvert(Version = 1, ActionClass = typeof(Actions.ODataPatchAction))]
        ODataPatch,

        [ActionTypeConvert(Code = "gdv")]
        [ActionClassConvert(Version = 1, ActionClass = typeof(Actions.GetDocumentVersionAction))]
        GetDocumentVersion,

        [ActionTypeConvert(Code = "gdh")]
        [ActionClassConvert(Version = 1, ActionClass = typeof(Actions.GetDocumentHistoryAction))]
        GetDocumentHistory,

        [ActionTypeConvert(Code = "dod")]
        [ActionClassConvert(Version = 1, ActionClass = typeof(Actions.DriveOutDocumentAction))]
        DriveOutDocument,

        [ActionTypeConvert(Code = "rtd")]
        [ActionClassConvert(Version = 1, ActionClass = typeof(Actions.ReturnDocumentAction))]
        ReturnDocument,

        [ActionTypeConvert(Code = "gah")]
        [ActionClassConvert(Version = 1, ActionClass = typeof(Actions.GetAttachFileDocumentHistoryAction))]
        GetAttachFileDocumentHistory,

        [ActionTypeConvert(Code = "dad")]
        [ActionClassConvert(Version = 1, ActionClass = typeof(Actions.DriveOutAttachFileDocumentAction))]
        DriveOutAttachFileDocument,

        [ActionTypeConvert(Code = "rad")]
        [ActionClassConvert(Version = 1, ActionClass = typeof(Actions.ReturnAttachFileDocumentAction))]
        ReturnAttachFileDocument,

        [ActionTypeConvert(Code = "rrd")]
        [ActionClassConvert(Version = 1, ActionClass = typeof(Actions.RegisterRawDataAction))]
        RegisterRawData,

        [ActionTypeConvert(Code = "ord")]
        [ActionClassConvert(Version = 1, ActionClass = typeof(Actions.ODataRawDataAction))]
        ODataRawData,

        [ActionTypeConvert(Code = "hta")]
        [ActionClassConvert(Version = 1, ActionClass = typeof(Actions.HistoryThrowAwayAction))]
        HistoryThrowAway,

        [ActionTypeConvert(Code = "ars")]
        [ActionClassConvert(Version = 1, ActionClass = typeof(Actions.AdaptResourceSchemaAction))]
        AdaptResourceSchema,

        [ActionTypeConvert(Code = "grs")]
        [ActionClassConvert(Version = 1, ActionClass = typeof(Actions.GetResourceSchemaAction))]
        GetResourceSchema,
    }

    public class ActionTypeConvertAttribute : Attribute
    {
        public string Code { get; set; }

    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ActionClassConvertAttribute : Attribute
    {
        public int Version { get; set; }

        public Type ActionClass { get; set; }

    }

    internal static class ActionTypesEx
    {
        private static Dictionary<ActionTypes, string> codeList;
        public static HashSet<ActionTypes> AttachFileActionTypes = new HashSet<ActionTypes>()
        {
            ActionTypes.AttachFileUpload,
            ActionTypes.AttachFileDelete,
            ActionTypes.AttachFileDownload,
            ActionTypes.DriveOutAttachFileDocument,
            ActionTypes.GetAttachFileDocumentHistory,
            ActionTypes.ReturnAttachFileDocument,
        };

        internal static string GetCode(this ActionTypes actionType)
        {
            return codeList[actionType];
        }

        static ActionTypesEx()
        {
            codeList = new Dictionary<ActionTypes, string>();
            foreach (ActionTypes value in Enum.GetValues(typeof(ActionTypes)))
            {
                var attrCode = (ActionTypeConvertAttribute)value.GetType()?.GetMember(value.ToString())?.FirstOrDefault()?.GetCustomAttributes(typeof(ActionTypeConvertAttribute), false).FirstOrDefault();
                if (attrCode != null)
                {
                    codeList.Add(value, attrCode.Code);
                }
            }
        }
    }
}
