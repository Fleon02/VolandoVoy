using System.Text.RegularExpressions;
using Microsoft.Maui.Controls;

namespace TFGVolandoVoy.Behaviors
{
    public class EmailValidationBehavior : Behavior<Entry>
    {
        const string emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(bindable);
        }

        void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            var entry = sender as Entry;
            bool isValid = Regex.IsMatch(args.NewTextValue, emailRegex);
            entry.TextColor = isValid ? GetThemeBasedColor() : Colors.Red;
        }

        private Color GetThemeBasedColor()
        {
            return Application.Current.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black;
        }
    }
}
