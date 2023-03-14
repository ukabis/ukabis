using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace JP.DataHub.Com.DataAnnotations
{
    /// <summary>
    /// ドメイン引数基底クラス
    /// </summary>
    [DataContract]
    abstract public class AbstractArgumentModel
    {
        /// <summary>
        /// 引数オブジェクト内の検証処理
        /// </summary>
        /// <exception cref="JP.DataHub.Com.DataAnnotations.AbstractArgumentValidationException">
        /// バリデーション処理にて検証失敗した際に発生
        /// </exception>
        public void Validation()
        {
            var context = new ValidationContext(this);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(this, context, validationResults, validateAllProperties: true);
            var errors = validationResults.Where(r => r != ValidationResult.Success);
            if (errors.Any())
            {
                var errorMsgs = errors.Select(x => x.ErrorMessage).ToArray();
                throw new AbstractArgumentValidationException(errorMsgs);
            }
        }
    }
}
