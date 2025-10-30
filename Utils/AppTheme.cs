using System;
using System.Drawing;
using System.Windows.Forms;

namespace CuahangNongduoc.Utils
{
    public static class AppTheme
    {
        // 🎨 Bảng màu phân lớp
        private static readonly Color PrimaryColor = Color.FromArgb(34, 139, 34);        // Màu xanh lá chính (ForestGreen)
        private static readonly Color PrimaryTextColor = Color.White;                   // Màu chữ trên nền đậm
        private static readonly Color BodyTextColor = Color.FromArgb(30, 50, 30);       // Màu chữ nội dung
        private static readonly Color AccentColor = Color.FromArgb(220, 245, 220);      // Nền phụ (header bảng, groupbox)
        private static readonly Color BackgroundColor = Color.FromArgb(250, 255, 250);  // Nền tổng thể
        private static readonly Color BorderColor = Color.FromArgb(200, 220, 200);      // Viền control
        private static readonly Color HoverColor = Color.FromArgb(235, 255, 235);       // Hover nhạt

        private static readonly Font DefaultFont = new Font("Segoe UI", 10.5F, FontStyle.Regular);

        // ⚙️ Gọi ở form_Load
        public static void ApplyTheme(Form form)
        {
            form.BackColor = BackgroundColor;
            form.Font = DefaultFont;

            foreach (Control ctrl in form.Controls)
                ApplyControlTheme(ctrl);
        }

        // 🎨 Xử lý từng loại control
        private static void ApplyControlTheme(Control ctrl)
        {
            switch (ctrl)
            {
                case Button btn:
                    StyleButton(btn);
                    break;
                case Label lbl:
                    StyleLabel(lbl);
                    break;
                case GroupBox gb:
                    StyleGroupBox(gb);
                    break;
                case TextBox txt:
                    StyleTextBox(txt);
                    break;
                case ComboBox cb:
                    StyleComboBox(cb);
                    break;
                case DataGridView dgv:
                    StyleDataGridView(dgv);
                    break;
                case StatusStrip ss:
                    StyleStatusStrip(ss);
                    break;
                case ToolStrip ts:
                    StyleToolStrip(ts);
                    break;
                case Panel pnl:
                    pnl.BackColor = BackgroundColor;
                    break;
            }

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
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.None;
            dgv.EnableHeadersVisualStyles = false;

            // Header
            dgv.ColumnHeadersDefaultCellStyle.BackColor = PrimaryColor;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = PrimaryTextColor;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            // Dòng xen kẽ
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 255, 245);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(200, 235, 200);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.GridColor = BorderColor;

            dgv.ColumnHeadersHeight = 30;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
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
            // 💚 Nền thanh nhạt, dễ nhìn
            ts.BackColor = Color.FromArgb(225, 250, 225); // xanh pastel nhạt
            ts.ForeColor = Color.Black;
            ts.GripStyle = ToolStripGripStyle.Hidden;
            ts.RenderMode = ToolStripRenderMode.System;

            // 📦 Style cho từng nút trong ToolStrip
            foreach (ToolStripItem item in ts.Items)
            {
                item.ForeColor = Color.Black;

                if (item is ToolStripButton btn)
                {
                    // 🧩 Chỉ hiện icon, ẩn chữ
                    btn.DisplayStyle = ToolStripItemDisplayStyle.Image;
                    btn.ImageScaling = ToolStripItemImageScaling.SizeToFit; // auto fit icon

                    // 🌿 Nền trong suốt + hiệu ứng hover nhẹ
                    btn.BackColor = Color.Transparent;
                    btn.Margin = new Padding(2, 1, 2, 1); // tạo khoảng cách đều icon

                    btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(235, 255, 235);
                    btn.MouseLeave += (s, e) => btn.BackColor = Color.Transparent;
                }
                else if (item is ToolStripLabel lbl)
                {
                    // Nếu có label (vd: "of 2") thì để chữ đen cho dễ đọc
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
