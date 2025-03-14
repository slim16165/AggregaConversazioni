using System;
using System.Windows;
using ICSharpCode.AvalonEdit;

namespace AggregaConversazioni.Helpers;

public static class AvalonEditBinding
{
    public static readonly DependencyProperty BoundTextProperty =
        DependencyProperty.RegisterAttached(
            "BoundText",
            typeof(string),
            typeof(AvalonEditBinding),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnBoundTextChanged));

    public static string GetBoundText(DependencyObject obj)
    {
        return (string)obj.GetValue(BoundTextProperty);
    }

    public static void SetBoundText(DependencyObject obj, string value)
    {
        obj.SetValue(BoundTextProperty, value);
    }

    private static void OnBoundTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TextEditor editor)
        {
            // Evita loop: aggiorna solo se il testo è diverso
            if (editor.Text != (string)e.NewValue)
                editor.Text = (string)e.NewValue;

            // Sottoscrivi l'evento per aggiornare il binding in fase di modifica
            editor.TextChanged -= Editor_TextChanged;
            editor.TextChanged += Editor_TextChanged;
        }
    }

    private static void Editor_TextChanged(object sender, EventArgs e)
    {
        if (sender is TextEditor editor)
        {
            // Aggiorna la proprietà attached quando il testo cambia
            SetBoundText(editor, editor.Text);
        }
    }
}