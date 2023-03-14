using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Service;
using JP.DataHub.ManageApi.Service.CharacterLimit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JP.DataHub.ManageApi.Attributes
{
    public class JpDataHubMaxLengthAttribute : ValidationAttribute
    {
        private const string ErrorMessageFormat = "文字数制限を超えています。最大{0}文字で入力して下さい。";
        private int? _maxLength = 0;

        private Lazy<ICharacterLimit> _lazyCharacterLimit = new(() => UnityCore.Resolve<ICharacterLimit>());

        private ICharacterLimit CharacterLimit { get => _lazyCharacterLimit.Value; }

        public JpDataHubMaxLengthAttribute(int maxLength)
        {
            this._maxLength = maxLength;
        }

        public JpDataHubMaxLengthAttribute(Domains type, string key1, string key2)
        {
            _maxLength = null;
            int? ml = CharacterLimit.GetMaxLength(type.ToString(), key1, key2);
            if (ml != null)
            {
                _maxLength = ml;
            }
        }

        public override bool IsValid(object value)
        {
            if (_maxLength != null && value != null)
            {
                if (value is string)
                {
                    string str = value as string;
                    if (str.Length > (int)_maxLength)
                    {
                        return false;
                    }
                }
                else if (value is List<string>)
                {
                    List<string> str = value as List<string>;
                    foreach (string s in str)
                    {
                        if (s.Length > (int)_maxLength)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageFormat, this._maxLength);
        }
    }
}
