using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using System.Collections.Generic;

namespace JP.DataHub.AdminWeb.Core.Component.Component
{
    public class FunctionValidator : ValidatorBase
    {
        /// <summary>
        /// Gets or sets the message displayed when the component is invalid. Set to <c>"Required"</c> by default.
        /// </summary>
        [Parameter]
        public override string Text { get; set; } = "入力項目が不適切です。";

        /// <summary>
        /// エラーメッセージを返す関数
        /// </summary>
        [Parameter]
        public Func<bool> Func { get; set; }

        /// <inheritdoc />
        protected override bool Validate(IRadzenFormComponent component) => Func();
    }
}
