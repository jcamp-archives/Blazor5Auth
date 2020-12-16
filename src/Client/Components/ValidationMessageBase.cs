using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

// https://chrissainty.com/creating-a-custom-validation-message-component-for-blazor-forms/

namespace Blazor5Auth.Client.Components {
    public class ValidationMessageBase<TValue> : ComponentBase, IDisposable
    {
        private FieldIdentifier _fieldIdentifier;

        [CascadingParameter] private EditContext EditContext { get; set; }
        [Parameter] public Expression<Func<TValue>> For { get; set; }
        [Parameter] public string Class { get; set; }

        protected IEnumerable<string> ValidationMessages => EditContext.GetValidationMessages(_fieldIdentifier);

        protected override void OnInitialized()
        {
            if (For != null)
            {
                _fieldIdentifier = FieldIdentifier.Create(For);
            }
            EditContext.OnValidationStateChanged += HandleValidationStateChanged;
        }

        private void HandleValidationStateChanged(object o, ValidationStateChangedEventArgs args) => StateHasChanged();

        public void Dispose()
        {
            EditContext.OnValidationStateChanged -= HandleValidationStateChanged;
        }
    }
}
