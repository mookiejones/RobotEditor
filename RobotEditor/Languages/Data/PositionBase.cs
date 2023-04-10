using RobotEditor.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace RobotEditor.Languages.Data;

public sealed class PositionBase : IPosition
{
    private readonly ReadOnlyObservableCollection<PositionValue> _positionalValues = null;
    private ObservableCollection<PositionValue> _values = new();

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
            string[] array = RawValue.Split(new[]
            {
                '='
            });
            string[] source = array[1][1..^1].Split(new[]
            {
                ','
            });
            foreach (string[] current in
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
        string text = string.Empty;
        string result;
        try
        {
            text = PositionalValues.Aggregate(text,
                (current, v) => current + string.Format("{0} {1},", v.Name, v.Value));
            result = text[..^1];
        }
        catch
        {
            result = string.Empty;
        }
        return result;
    }

    public override string ToString() => RawValue;
}