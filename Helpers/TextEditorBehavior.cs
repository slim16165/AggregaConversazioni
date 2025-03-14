using System;
using System.Windows;
using Microsoft.Xaml.Behaviors;  // Cambiato da System.Windows.Interactivity
using ICSharpCode.AvalonEdit;

public class TextEditorBehavior : Behavior<TextEditor>
{
    public static readonly DependencyProperty BoundTextProperty =
        DependencyProperty.Register(nameof(BoundText), typeof(string), typeof(TextEditorBehavior),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnBoundTextChanged));

    public string BoundText
    {
        get => (string)GetValue(BoundTextProperty);
        set => SetValue(BoundTextProperty, value);
    }

    private static void OnBoundTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TextEditorBehavior behavior && behavior.AssociatedObject != null)
        {
            // Evita aggiornamenti inutili
            if (behavior.AssociatedObject.Text == (string)e.NewValue)
                return;

            var document = behavior.AssociatedObject.Document;
            if (document != null)
            {
                document.BeginUpdate();
                try
                {
                    behavior.AssociatedObject.Text = (string)e.NewValue;
                }
                finally
                {
                    document.EndUpdate();
                }
            }
            else
            {
                behavior.AssociatedObject.Text = (string)e.NewValue;
            }
        }
    }


    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.TextChanged += Editor_TextChanged;
    }

    private void Editor_TextChanged(object sender, EventArgs e)
    {
        BoundText = AssociatedObject.Text;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.TextChanged -= Editor_TextChanged;
    }
}