using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using System.Xml;
using System.Xml.Serialization;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using RobotEditor.Interfaces;
using RobotEditor.Languages;

namespace RobotEditor.Controls.TextEditor;

[Localizable(false)]
[Serializable]
public sealed class EditorOptions : TextEditorOptions, IOptions
{
    private static EditorOptions _instance;
    [NonSerialized] private bool _allowScrollingBelowDocument;
    [NonSerialized] private Color _backgroundColor = Colors.White;
    private Color _borderColor = Colors.Transparent;

    private double _borderThickness;
    private bool _enableAnimations = true;
    private bool _enableFolding = true;
    private Color _foldToolTipBackgroundBorderColor = Colors.WhiteSmoke;
    [NonSerialized] private Color _foldToolTipBackgroundColor = Colors.Red;
    private double _foldToolTipBorderThickness = 1.0;
    [NonSerialized] private Color _fontColor = Colors.Black;
    private bool _highlightCurrentLine = true;
    [NonSerialized] private Color _lineNumbersFontColor = Colors.Gray;
    private Color _lineNumbersForeground = Colors.Gray;
    private bool _mouseWheelZoom = true;
    [NonSerialized] private Color _selectedBorderColor = Colors.Orange;
    private double _selectedBorderThickness = 1.0;
    [NonSerialized] private Color _selectedFontColor = Colors.White;
    [NonSerialized] private Color _selectedTextBackground = Colors.SteelBlue;
    [NonSerialized] private Color _selectedTextBorderColor = Colors.Orange;
    [NonSerialized] private Color _selectedLineColor = Colors.Yellow;
    private bool _showLineNumbers = true;
    private string _timestampFormat = "ddd MMM d hh:mm:ss yyyy";
    private bool _wrapWords = true;

    public EditorOptions()
    {
        RegisterSyntaxHighlighting();
    }

    private static string OptionsPath => Path.Combine(Global.StartupPath, "Options.xml");

    public static EditorOptions Instance
    {
        get => _instance ??= ReadXml();
        set => _instance = value;
    }

    public override bool ShowSpaces
    {
        get => base.ShowSpaces;
        set
        {
            base.ShowSpaces = value;
            OnPropertyChanged(nameof(ShowSpaces));
        }
    }

    public Color SelectedTextBackground
    {
        get => _selectedTextBackground;
        set
        {
            _selectedTextBackground = value;
            OnPropertyChanged(nameof(SelectedTextBackground));
        }
    }

    public Color BackgroundColor
    {
        get => _backgroundColor;
        set
        {
            _backgroundColor = value;
            OnPropertyChanged(nameof(BackgroundColor));
        }
    }

    public Color FontColor
    {
        get => _fontColor;
        set
        {
            _fontColor = value;
            OnPropertyChanged(nameof(FontColor));
        }
    }

    public Color SelectedFontColor
    {
        get => _selectedFontColor;
        set
        {
            _selectedFontColor = value;
            OnPropertyChanged(nameof(SelectedFontColor));
        }
    }

    public Color SelectedBorderColor
    {
        get => _selectedBorderColor;
        set
        {
            _selectedBorderColor = value;
            OnPropertyChanged(nameof(SelectedBorderColor));
        }
    }

    public bool AllowScrollingBelowDocument
    {
        get => _allowScrollingBelowDocument;
        set
        {
            _allowScrollingBelowDocument = value;
            OnPropertyChanged(nameof(AllowScrollingBelowDocument));
        }
    }

    public Color LineNumbersFontColor
    {
        get => _lineNumbersFontColor;
        set
        {
            _lineNumbersFontColor = value;
            OnPropertyChanged(nameof(LineNumbersFontColor));
        }
    }

    public Color BorderColor
    {
        get => _borderColor;
        set
        {
            _borderColor = value;
            OnPropertyChanged(nameof(BorderColor));
        }
    }

    public Color LineNumbersForeground
    {
        get => _lineNumbersForeground;
        set
        {
            _lineNumbersForeground = value;
            OnPropertyChanged(nameof(LineNumbersForeground));
        }
    }

    public Color SelectedTextBorderColor
    {
        get => _selectedTextBorderColor;
        set
        {
            _selectedTextBorderColor = value;
            OnPropertyChanged(nameof(SelectedTextBorderColor));
        }
    }

    public double SelectedBorderThickness
    {
        get => _selectedBorderThickness;
        set
        {
            _selectedBorderThickness = value;
            OnPropertyChanged(nameof(SelectedBorderThickness));
        }
    }

    public double BorderThickness
    {
        get => _borderThickness;
        set
        {
            _borderThickness = value;
            OnPropertyChanged(nameof(BorderThickness));
        }
    }

    public Color HighlightedLineColor
    {
        get => _selectedLineColor;
        set
        {
            _selectedLineColor = value;
            OnPropertyChanged(nameof(HighlightedLineColor));
        }
    }

    public Color FoldToolTipBackgroundColor
    {
        get => _foldToolTipBackgroundColor;
        set
        {
            _foldToolTipBackgroundColor = value;
            OnPropertyChanged(nameof(FoldToolTipBackgroundColor));
        }
    }

    public Color FoldToolTipBackgroundBorderColor
    {
        get => _foldToolTipBackgroundBorderColor;
        set
        {
            _foldToolTipBackgroundBorderColor = value;
            OnPropertyChanged(nameof(FoldToolTipBackgroundBorderColor));
        }
    }

    public double FoldToolTipBorderThickness
    {
        get => _foldToolTipBorderThickness;
        set
        {
            _foldToolTipBorderThickness = value;
            OnPropertyChanged(nameof(FoldToolTipBorderThickness));
        }
    }

    public bool WrapWords
    {
        get => _wrapWords;
        set
        {
            _wrapWords = value;
            OnPropertyChanged(nameof(WrapWords));
        }
    }

    public string TimestampFormat
    {
        get => _timestampFormat;
        set
        {
            _timestampFormat = value;
            OnPropertyChanged(nameof(TimestampFormat));
            OnPropertyChanged(nameof(TimestampSample));
        }
    }

    public string TimestampSample => DateTime.Now.ToString(_timestampFormat);

    public new bool HighlightCurrentLine
    {
        get => _highlightCurrentLine;
        set => _highlightCurrentLine = value;
    }

    [DefaultValue(true)]
    public bool EnableFolding
    {
        get => _enableFolding;
        set
        {
            _enableFolding = value;
            OnPropertyChanged(nameof(EnableFolding));
        }
    }

    [DefaultValue(true)]
    public bool MouseWheelZoom
    {
        get => _mouseWheelZoom;
        set
        {
            if (_mouseWheelZoom != value)
            {
                _mouseWheelZoom = value;
                OnPropertyChanged(nameof(MouseWheelZoom));
            }
        }
    }

    public bool EnableAnimations
    {
        get => _enableAnimations;
        set
        {
            _enableAnimations = value;
            OnPropertyChanged(nameof(EnableAnimations));
        }
    }

    public bool ShowLineNumbers
    {
        get => _showLineNumbers;
        set
        {
            _showLineNumbers = value;
            OnPropertyChanged(nameof(ShowLineNumbers));
        }
    }

    public string Title => "Text Editor Options";

    ~EditorOptions()
    {
        WriteXml();
    }

    private void WriteXml()
    {
        var xmlSerializer = new XmlSerializer(typeof (EditorOptions));
        TextWriter textWriter = new StreamWriter(OptionsPath);
        xmlSerializer.Serialize(textWriter, this);
        textWriter.Close();
    }

    private static EditorOptions ReadXml()
    {
        var editorOptions = new EditorOptions();
        EditorOptions result;
        if (!File.Exists(OptionsPath))
        {
            result = editorOptions;
        }
        else
        {
            var xmlSerializer = new XmlSerializer(typeof (EditorOptions));
            var fileStream = new FileStream(OptionsPath, FileMode.Open);
            try
            {
                editorOptions = (EditorOptions) xmlSerializer.Deserialize(fileStream);
            }
            catch
            {
            }
            finally
            {
                fileStream.Close();
            }
            result = editorOptions;
        }
        return result;
    }

    [Localizable(false)]
    private static void Register(string name, string[] ext)
    {
        /// Lets get the current namespace of EditorOptions
        var type = typeof(EditorOptions);
        var ns=type.Namespace;
        

        var filename = $"{ns}.SyntaxHighlighting.{name}Highlight.xshd";

        var asm = Assembly.GetExecutingAssembly();
        var names = asm.GetManifestResourceNames();

        // Does the name exist?
        var validName = names.FirstOrDefault(o => o.Equals(filename, StringComparison.InvariantCultureIgnoreCase));
        if (validName == null)
        {

        }

        using var manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(validName);
        if (manifestResourceStream == null)
        {
            throw new InvalidOperationException("Could not find embedded resource");
        }
        IHighlightingDefinition highlighting;
        using (var xmlTextReader = new XmlTextReader(manifestResourceStream))
        {
            highlighting = HighlightingLoader.Load(xmlTextReader, HighlightingManager.Instance);
        }
        HighlightingManager.Instance.RegisterHighlighting(name, ext, highlighting);
    }

    private static void RegisterSyntaxHighlighting()
    {
        Register("KUKA", KUKA.Ext.ToArray());
        Register("KAWASAKI", Kawasaki.EXT.ToArray());
        Register("Fanuc", Fanuc.EXT.ToArray());
        Register("ABB", ABB.EXT.ToArray());
    }
}