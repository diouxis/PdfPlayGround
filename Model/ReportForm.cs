using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace PdfPlayGround.Model
{
    public class ReportData
    {
        public JObject Data { get; set; }
    }

    public class ReportForm
    {
        public List<Card> Cards { get; set; }
    }

    public class Card
    {
        public string Title { get; set; }
        public List<Field> Fields { get; set; }
    }

    public class Field
    {
        public string Name { get; set; }
        public string FieldType { get; set; }
        public string Label { get; set; }
        public object Value { get; set; }
        public List<Field> Fields { get; set; }
        public List<FieldOption> Options { get; set; }

        public string ValueString => Value?.ToString();

        public Field() { }
        public Field(Field field)
        {
            Name = field.Name;
            FieldType = field.FieldType;
            Label = field.Label;
            Fields = field.Fields;
            Options = field.Options;
        }

        public void FillData(JObject dataSet)
        {
            if (!string.IsNullOrEmpty(Name) && dataSet.TryGetValue(Name, out var value))
            {
                switch (FieldType)
                {
                    case "GroupField":
                        var fieldArray = new List<List<Field>>();
                        if (value is JArray array)
                        {
                            foreach (var jtoken in array)
                            {
                                if (jtoken is JObject subDataSet)
                                {
                                    var fields = Fields.Select(x => {
                                        var field = new Field(x);
                                        field.FillData(subDataSet);
                                        return field;
                                    }).ToList();
                                    fieldArray.Add(fields);
                                }
                            }
                        }
                        Value = fieldArray;
                        break;
                    case "DateTimeField":
                        Value = value.Value<DateTime>().ToShortDateString();
                        break;
                    case "SelectField":
                        Value = Options.FirstOrDefault(x => x.Value == value.Value<string>())?.Label;
                        break;
                    case "FileField":
                        Value = value.Select(x => new ReportFile { 
                            Name = x.Value<string>("name"),
                            Url = x.Value<string>("url")
                        });
                        break;
                    default:
                        Value = value.Value<string>();
                        break;
                }
            }
        }
    }

    public class FieldOption
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }

    public class ReportFile
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
