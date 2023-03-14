using JP.DataHub.Com.Unity.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.CharacterLimit
{
    /// <summary>
    /// Interface of Character Limit.
    /// </summary>
	[Log]
    public interface ICharacterLimit
    {
        /// <summary>
        /// Get MaxLength.
        /// </summary>
        /// <param name="key1">Key1</param>
        /// <param name="key2">Key2</param>
        /// <param name="key3">Key3</param>
        /// <returns>MaxLength</returns>
		int? GetMaxLength(string key1, string key2, string key3);
    }
}
