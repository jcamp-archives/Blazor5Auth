using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

// https://docs.microsoft.com/en-us/aspnet/core/blazor/forms-validation?view=aspnetcore-5.0

namespace Blazor5Auth.Client.Components
{
    public class ServerValidator : ComponentBase
    {
        private ValidationMessageStore messageStore;

        [CascadingParameter]
        private EditContext CurrentEditContext { get; set; }

        protected override void OnInitialized()
        {
            if (CurrentEditContext == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(ServerValidator)} requires a cascading " +
                    $"parameter of type {nameof(EditContext)}. " +
                    $"For example, you can use {nameof(ServerValidator)} " +
                    $"inside an {nameof(EditForm)}.");
            }

            messageStore = new ValidationMessageStore(CurrentEditContext);

            CurrentEditContext.OnValidationRequested += (s, e) =>
                messageStore.Clear();
            CurrentEditContext.OnFieldChanged += (s, e) =>
                messageStore.Clear(e.FieldIdentifier);
        }

        public void DisplayErrors(IDictionary<string, IEnumerable<string>> errors)
        {
            foreach (var err in errors)
            {
                var fieldIdentifier = ToFieldIdentifier(CurrentEditContext, err.Key);
                messageStore.Add(fieldIdentifier, err.Value);
            }

            CurrentEditContext.NotifyValidationStateChanged();
        }

        public void DisplayErrors(IDictionary<string, string[]> errors)
        {
            foreach (var err in errors)
            {
                var fieldIdentifier = ToFieldIdentifier(CurrentEditContext, err.Key);
                messageStore.Add(fieldIdentifier, err.Value);
            }

            CurrentEditContext.NotifyValidationStateChanged();
        }

        public void ClearErrors()
        {
            messageStore.Clear();
            CurrentEditContext.NotifyValidationStateChanged();
        }

        private static readonly char[] separators = new[] { '.', '[' };
        private static FieldIdentifier ToFieldIdentifier(EditContext editContext, string propertyPath)
        {
            // This code is taken from an article by Steve Sanderson (https://blog.stevensanderson.com/2019/09/04/blazor-fluentvalidation/)
            // all credit goes to him for this code.

            // This method parses property paths like 'SomeProp.MyCollection[123].ChildProp'
            // and returns a FieldIdentifier which is an (instance, propName) pair. For example,
            // it would return the pair (SomeProp.MyCollection[123], "ChildProp"). It traverses
            // as far into the propertyPath as it can go until it finds any null instance.

            var obj = editContext.Model;

            while (true)
            {
                var nextTokenEnd = propertyPath.IndexOfAny(separators);
                if (nextTokenEnd < 0)
                {
                    return new FieldIdentifier(obj, propertyPath);
                }

                var nextToken = propertyPath.Substring(0, nextTokenEnd);
                propertyPath = propertyPath.Substring(nextTokenEnd + 1);

                object newObj;
                if (nextToken.EndsWith("]"))
                {
                    // It's an indexer
                    // This code assumes C# conventions (one indexer named Item with one param)
                    nextToken = nextToken.Substring(0, nextToken.Length - 1);
                    var prop = obj.GetType().GetProperty("Item");

                    if (prop is object)
                    {
                        // we've got an Item property
                        var indexerType = prop.GetIndexParameters()[0].ParameterType;
                        var indexerValue = Convert.ChangeType(nextToken, indexerType);
                        newObj = prop.GetValue(obj, new object[] { indexerValue });
                    }
                    else
                    {
                        // If there is no Item property
                        // Try to cast the object to array
                        if (obj is object[] array)
                        {
                            var indexerValue = Convert.ToInt32(nextToken);
                            newObj = array[indexerValue];
                        }
                        else
                        {
                            throw new InvalidOperationException($"Could not find indexer on object of type {obj.GetType().FullName}.");
                        }
                    }
                }
                else
                {
                    // It's a regular property
                    var prop = obj.GetType().GetProperty(nextToken);
                    if (prop == null)
                    {
                        throw new InvalidOperationException($"Could not find property named {nextToken} on object of type {obj.GetType().FullName}.");
                    }
                    newObj = prop.GetValue(obj);
                }

                if (newObj == null)
                {
                    // This is as far as we can go
                    return new FieldIdentifier(obj, nextToken);
                }

                obj = newObj;
            }
        }
    }
}
