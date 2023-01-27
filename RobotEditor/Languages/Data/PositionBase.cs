using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using RobotEditor.Interfaces;

namespace RobotEditor.Languages.Data
{
    public sealed class PositionBase : IPosition
    {
        private readonly ReadOnlyObservableCollection<PositionValue> _positionalValues = null;
        private ObservableCollection<PositionValue> _values = new ObservableCollection<PositionValue>();

        public PositionBase(string value)
        {
            RawValue = value;
            ParseValues();
        }

        public string Type { get; set; }
        public string RawValue { get; set; }
        public string Scope { get; set; }
        public string Name { get; set; }

        public IEnumerable<PositionValue> PositionalValues => _positionalValues ?? new ReadOnlyObservableCollection<PositionValue>(_values);

        public void ParseValues()
        {
            try
            {
                _values = new ObservableCollection<PositionValue>();
                var array = RawValue.Split(new[]
                {
                    '='
                });
                var source = array[1].Substring(1, array[1].Length - 2).Split(new[]
                {
                    ','
                });
                foreach (var current in
                    from s in source
                    select s.Split(new[]
                    {
                        ' '
                    }))
                {
                    _values.Add(new PositionValue
                    {
                        Name = current[0],
                        Value = current[1]
                    });
                }
            }
            catch
            {
            }
        }

        [Localizable(false)]
        public string ExtractFromMatch()
        {
            var text = string.Empty;
            string result;
            try
            {
                text = PositionalValues.Aggregate(text,
                    (current, v) => current + string.Format("{0} {1},", v.Name, v.Value));
                result = text.Substring(0, text.Length - 1);
            }
            catch
            {
                result = string.Empty;
            }
            return result;
        }

        public override string ToString() => RawValue;

        private string ConvertFromHex(string value)
        {
            var value2 = double.Parse(value.Substring(1, value.Length - 2), NumberStyles.HexNumber);
            return Convert.ToString(value2);
        }

        private bool IsNumeric(string value)
        {
            bool result;
            try
            {
                var num = Convert.ToDouble(value);
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }
    }
}