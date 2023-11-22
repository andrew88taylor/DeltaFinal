using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;

namespace Delta
{
    public partial class ExerciseInputModalPage : ContentPage
    {
        public event Action<Exercise> OnExerciseSaved;

        public ExerciseInputModalPage()
        {
            InitializeComponent();
        }

        // Event handler for the 'Add Field' button click.
        private void OnAddFieldClicked(object sender, EventArgs e)
        {
            var fieldStack = new StackLayout { Orientation = StackOrientation.Horizontal };

            var fieldNameEntry = new Entry { Placeholder = "Field name", WidthRequest = 100 };
            var fieldValueEntry = new Entry { Placeholder = "Value", WidthRequest = 200, Keyboard = Keyboard.Text };

            var deleteButton = new Button { Text = "Delete"};
            deleteButton.Clicked += OnDeleteFieldClicked;

            fieldStack.Children.Add(fieldNameEntry);
            fieldStack.Children.Add(fieldValueEntry);
            fieldStack.Children.Add(deleteButton);

            CustomFieldsContainer.Children.Add(fieldStack);
        }

        private void OnDeleteFieldClicked(object sender, EventArgs e)
        {
            if (sender is Button deleteButton)
            {
                if (deleteButton.Parent is StackLayout parentStack)
                {
                    CustomFieldsContainer.Children.Remove(parentStack);
                }
            }
        }


        // Event handler for the 'Save' button click.
        private void OnSaveClicked(object sender, EventArgs e)
        {
            var exercise = new Exercise
            {
                Type = TypeEntry.Text,
                CustomFields = new List<CustomField>()
            };

            foreach (StackLayout fieldStack in CustomFieldsContainer.Children)
            {
                var fieldNameEntry = (Entry)fieldStack.Children[0];
                var fieldValueEntry = (Entry)fieldStack.Children[1];
                if (!string.IsNullOrWhiteSpace(fieldNameEntry.Text) && !string.IsNullOrWhiteSpace(fieldValueEntry.Text))
                {
                    exercise.CustomFields.Add(new CustomField
                    {
                        FieldName = fieldNameEntry.Text,
                        FieldValue = fieldValueEntry.Text
                    });
                }
            }

            OnExerciseSaved?.Invoke(exercise);
            Navigation.PopModalAsync();
        }

        // Event handler for the 'Back' button click.
        private void OnBackClicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}
