using System;
using System.Windows.Forms;

namespace CuahangNongduoc.Utils.Binding
{
    /// <summary>
    /// Fluent helper to declaratively register bindings for WinForms controls.
    /// </summary>
    public sealed class ControlBindingBuilder
    {
        private readonly BindingSource _source;

        public ControlBindingBuilder(BindingSource source)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public ControlBindingBuilder BindText(Control control, string dataMember)
        {
            return Bind(control, "Text", dataMember);
        }

        public ControlBindingBuilder BindValue(Control control, string dataMember)
        {
            return Bind(control, "Value", dataMember);
        }

        public ControlBindingBuilder BindSelectedValue(ListControl control, string dataMember)
        {
            return Bind(control, "SelectedValue", dataMember);
        }

        private ControlBindingBuilder Bind(Control control, string propertyName, string dataMember)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            control.DataBindings.Clear();
            control.DataBindings.Add(propertyName, _source, dataMember, true, DataSourceUpdateMode.OnPropertyChanged);
            return this;
        }
    }
}
