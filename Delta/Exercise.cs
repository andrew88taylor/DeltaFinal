using System;
using System.Collections.Generic;

namespace Delta
{
    public class Workout
    {
        public DateTime Date { get; set; }
        public Dictionary<string, Dictionary<string, string>> Exercises { get; set; }
    }

    public class Exercise
    {
        public string Type { get; set; }
        public List<CustomField> CustomFields { get; set; } = new List<CustomField>();

        public string CustomFieldsDisplay
        {
            get
            {
                return string.Join(", ", CustomFields.Select(cf => $"{cf.FieldName}: {cf.FieldValue}"));
            }
        }
    }

    public class CustomField
    {
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
    }
}




