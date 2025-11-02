using System;
using System.Drawing;
using System.Windows.Forms;

namespace CuahangNongduoc.Utils
{
    public static class AppTheme
    {
        // 🎨 Bảng màu
        private static readonly Color PrimaryColor = Color.FromArgb(34, 139, 34);
        private static readonly Color PrimaryTextColor = Color.White;
        private static readonly Color BodyTextColor = Color.FromArgb(30, 50, 30);
        private static readonly Color AccentColor = Color.FromArgb(220, 245, 220);
        private static readonly Color BackgroundColor = Color.FromArgb(250, 255, 250);
        private static readonly Color BorderColor = Color.FromArgb(200, 220, 200);
        private static readonly Color HoverColor = Color.FromArgb(235, 255, 235);

        private static readonly Font DefaultFont = new Font("Segoe UI", 10.5F, FontStyle.Regular);

        // ⚙️ Gọi trong Form_Load
        public static void ApplyTheme(Form form)
        {
            form.BackColor = BackgroundColor;
            form.Font = DefaultFont;

            // Áp dụng cho mọi control hiện có
            foreach (Control ctrl in form.Controls)
                ApplyControlTheme(ctrl);

            // Lắng nghe sự kiện thêm control mới
            form.ControlAdded += (s, e) => ApplyControlTheme(e.Control);
        }

        // 🎨 Xử lý từng loại control
        private static void ApplyControlTheme(Control ctrl)
        {
            switch (ctrl)
            {
                case Button btn: StyleButton(btn); break;
                case Label lbl: StyleLabel(lbl); break;
                case GroupBox gb: StyleGroupBox(gb); break;
                case TextBox txt: StyleTextBox(txt); break;
                case ComboBox cb: StyleComboBox(cb); break;
                case DataGridView dgv: StyleDataGridView(dgv); break;
                case StatusStrip ss: StyleStatusStrip(ss); break;
                case ToolStrip ts: StyleToolStrip(ts); break;
                case Panel pnl: pnl.BackColor = BackgroundColor; break;
            }

            // Đệ quy cho control con
            foreach (Control child in ctrl.Controls)
                ApplyControlTheme(child);
        }

        // 🔘 Button
        private static void StyleButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = BorderColor;
            btn.BackColor = Color.White;
            btn.ForeColor = BodyTextColor;
            btn.Font = DefaultFont;
            btn.Cursor = Cursors.Hand;

            btn.MouseEnter += (s, e) =>
            {
                btn.BackColor = HoverColor;
                btn.FlatAppearance.BorderColor = PrimaryColor;
            };
            btn.MouseLeave += (s, e) =>
            {
                btn.BackColor = Color.White;
                btn.FlatAppearance.BorderColor = BorderColor;
            };
        }

        // 🏷 Label
        private static void StyleLabel(Label lbl)
        {
            lbl.ForeColor = BodyTextColor;
            lbl.BackColor = Color.Transparent;
            lbl.Font = DefaultFont;
        }

        // 📦 GroupBox
        private static void StyleGroupBox(GroupBox gb)
        {
            gb.Font = new Font(DefaultFont.FontFamily, 10F, FontStyle.Bold);
            gb.ForeColor = PrimaryColor;
            gb.BackColor = AccentColor;
            gb.Padding = new Padding(8, 4, 8, 8);
        }

        // ✏️ TextBox
        private static void StyleTextBox(TextBox txt)
        {
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.BackColor = Color.White;
            txt.ForeColor = BodyTextColor;
            txt.Font = DefaultFont;
        }

        // 🔽 ComboBox
        private static void StyleComboBox(ComboBox cb)
        {
            cb.BackColor = Color.White;
            cb.ForeColor = BodyTextColor;
            cb.DropDownStyle = ComboBoxStyle.DropDownList;
            cb.Font = DefaultFont;
        }

        // 📊 DataGridView
        private static void StyleDataGridView(DataGridView dgv)
        {
            // 🌿 Tông nền tổng thể
            dgv.BackgroundColor = Color.FromArgb(240, 255, 240);
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.FromArgb(220, 240, 220);
            dgv.RowHeadersVisible = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // 🌿 Header
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = PrimaryColor;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = PrimaryTextColor;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersHeight = 30;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // 🌿 Dòng dữ liệu
            dgv.DefaultCellStyle.BackColor = Color.FromArgb(245, 255, 245);
            dgv.DefaultCellStyle.ForeColor = BodyTextColor;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(200, 235, 200);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10.5F, FontStyle.Regular);
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 255, 245);

            // 🌿 Re-apply mỗi khi binding xong
            dgv.DataBindingComplete += (s, e) =>
            {
                dgv.EnableHeadersVisualStyles = false;
                dgv.ColumnHeadersDefaultCellStyle.BackColor = PrimaryColor;
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = PrimaryTextColor;
                dgv.DefaultCellStyle.BackColor = Color.FromArgb(245, 255, 245);
                dgv.DefaultCellStyle.ForeColor = BodyTextColor;
                dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(200, 235, 200);
                dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
                dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 255, 245);
                dgv.Invalidate();
                dgv.Refresh();
            };
        }

        // 📏 StatusStrip
        private static void StyleStatusStrip(StatusStrip ss)
        {
            ss.BackColor = PrimaryColor;
            ss.ForeColor = PrimaryTextColor;
            ss.Font = new Font("Segoe UI", 9.5F);
        }

        // 🧭 ToolStrip / MenuStrip
        private static void StyleToolStrip(ToolStrip ts)
        {
            ts.BackColor = Color.FromArgb(225, 250, 225);
            ts.ForeColor = Color.Black;
            ts.GripStyle = ToolStripGripStyle.Hidden;
            ts.RenderMode = ToolStripRenderMode.System;

            foreach (ToolStripItem item in ts.Items)
            {
                item.ForeColor = Color.Black;

                if (item is ToolStripButton btn)
                {
                    btn.DisplayStyle = ToolStripItemDisplayStyle.Image;
                    btn.ImageScaling = ToolStripItemImageScaling.SizeToFit;
                    btn.BackColor = Color.Transparent;
                    btn.Margin = new Padding(2, 1, 2, 1);
                    btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(235, 255, 235);
                    btn.MouseLeave += (s, e) => btn.BackColor = Color.Transparent;
                }
                else if (item is ToolStripLabel lbl)
                {
                    lbl.ForeColor = Color.Black;
                }
                else if (item is ToolStripTextBox txt)
                {
                    txt.BackColor = Color.White;
                    txt.ForeColor = Color.Black;
                    txt.BorderStyle = BorderStyle.FixedSingle;
                }
            }
        }
    }
}
