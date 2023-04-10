using RobotEditor.Interfaces;
using RobotEditor.Languages;
using RobotEditor.Robots;
using System.IO;
using System.Windows;

namespace RobotEditor.ViewModel;

public sealed class DocumentViewModel : DocumentBase, IEditorDocument
{
    public DocumentViewModel()
    {
    }


    public DocumentViewModel(string filepath)
        : base(filepath)
    {
    }

    public DocumentViewModel(string filepath, AbstractLanguageClass lang)
        : base(filepath, lang)
    {
    }

    public static DocumentViewModel Instance { get; set; }


    public override void Close()
    {
        if (IsDirty)
        {
            MessageBoxResult messageBoxResult =
                MessageBox.Show(string.Format("Save changes for file '{0}'?", FileName), "RobotEditor",
                    MessageBoxButton.YesNoCancel);
            if (messageBoxResult == MessageBoxResult.Cancel)
            {
                return;
            }
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                Save(TextBox);
            }
        }
    }

    // internal void Save(Editor txtBox)
    // {
    //     if (txtBox.Filename == null)
    //     {
    //         txtBox.SaveAs();
    //     }
    //     IsDirty = false;
    // }
    // protected void TextChanged(object sender)
    // {
    //     TextBox = (sender as Editor);
    //     if (TextBox != null)
    //     {
    //         FileLanguage.RawText = TextBox.Text;
    //     }
    //     OnPropertyChanged(nameof(Title));
    // }
    public override void Load(string filepath)
    {
        FilePath = filepath;
        Instance = this;
        TextBox.FileLanguage = FileLanguage;
        TextBox.Filename = filepath;
        TextBox.SetHighlighting();
        if (File.Exists(filepath))
        {
            TextBox.Text = File.ReadAllText(filepath);
        }
        OnPropertyChanged(nameof(Title));
    }

    //  public void SelectText(IVariable var)
    //  {
    //      if (var.Name == null)
    //      {
    //          throw new ArgumentNullException("var");
    //      }
    //      var flag = TextBox.Text.Length >= var.Offset;
    //      if (flag)
    //      {
    //          TextBox.SelectText(var);
    //      }
    //  }
}