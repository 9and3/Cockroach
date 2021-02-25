//using Susing Microsoft.VisualBasic.CompilerServices;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;


namespace CockroachGH {
    [DesignerGenerated]
    public class Control_Display : UserControl {
        private IContainer components;

        internal Color DispButBack {
            get {
                return this.DynamicBut.BackColor;
            }
            set {
                this.DynamicBut.BackColor = value;
            }
        }

        internal Color DispButFrame {
            get {
                return this.DynamicBut.FlatAppearance.BorderColor;
            }
            set {
                this.DynamicBut.FlatAppearance.BorderColor = value;
            }
        }

        internal virtual Button DynamicBut {
            get {
                return this._DynamicBut;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set {
                EventHandler eventHandler = new EventHandler(this.DynamicBut_Click);
                Button button = this._DynamicBut;
                if (button != null) {
                    button.Click -= eventHandler;
                }
                this._DynamicBut = value;
                button = this._DynamicBut;
                if (button != null) {
                    button.Click += eventHandler;
                }
            }
        }

        internal virtual Button MinusBut {
            get {
                return this._MinusBut;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set {
                EventHandler eventHandler = new EventHandler(this.MinusBut_Click);
                Button button = this._MinusBut;
                if (button != null) {
                    button.Click -= eventHandler;
                }
                this._MinusBut = value;
                button = this._MinusBut;
                if (button != null) {
                    button.Click += eventHandler;
                }
            }
        }

        internal virtual Button PlusBut {
            get {
                return this._PlusBut;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set {
                EventHandler eventHandler = new EventHandler(this.PlusBut_Click);
                Button button = this._PlusBut;
                if (button != null) {
                    button.Click -= eventHandler;
                }
                this._PlusBut = value;
                button = this._PlusBut;
                if (button != null) {
                    button.Click += eventHandler;
                }
            }
        }

        public Control_Display() {
            this.InitializeComponent();
        }

        [DebuggerNonUserCode]
        protected override void Dispose(bool disposing) {
            try {
                if (disposing && this.components != null) {
                    this.components.Dispose();
                }
            } finally {
                base.Dispose(disposing);
            }
        }

        private void DynamicBut_Click(object sender, EventArgs e) {
            Control_Display.DynamicClickedEventHandler dynamicClickedEventHandler = this.DynamicClicked;
            if (dynamicClickedEventHandler != null) {
                dynamicClickedEventHandler();
            }
        }

        [DebuggerStepThrough]
        private void InitializeComponent() {
            this.DynamicBut = new Button();
            this.MinusBut = new Button();
            this.PlusBut = new Button();
            base.SuspendLayout();
            this.DynamicBut.BackColor = Color.White;
            this.DynamicBut.FlatAppearance.BorderColor = Color.FromArgb(189, 189, 189);
            this.DynamicBut.FlatAppearance.CheckedBackColor = Color.White;
            this.DynamicBut.FlatAppearance.MouseDownBackColor = Color.White;
            this.DynamicBut.FlatStyle = FlatStyle.Flat;
            this.DynamicBut.Location = new Point(0, 1);
            this.DynamicBut.Name = "DynamicBut";
            this.DynamicBut.Size = new Size(120, 28);
            this.DynamicBut.TabIndex = 0;
            this.DynamicBut.Text = "Dynamic display";
            this.DynamicBut.UseVisualStyleBackColor = false;
            this.MinusBut.BackColor = Color.White;
            this.MinusBut.FlatAppearance.BorderColor = Color.FromArgb(189, 189, 189);
            this.MinusBut.FlatAppearance.CheckedBackColor = Color.White;
            this.MinusBut.FlatAppearance.MouseDownBackColor = Color.White;
            this.MinusBut.FlatStyle = FlatStyle.Flat;
            this.MinusBut.Font = new Font("Microsoft Sans Serif", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.MinusBut.Location = new Point(123, 1);
            this.MinusBut.Name = "MinusBut";
            this.MinusBut.Size = new Size(28, 28);
            this.MinusBut.TabIndex = 1;
            this.MinusBut.Text = "-";
            this.MinusBut.UseVisualStyleBackColor = false;
            this.PlusBut.BackColor = Color.White;
            this.PlusBut.FlatAppearance.BorderColor = Color.FromArgb(189, 189, 189);
            this.PlusBut.FlatAppearance.CheckedBackColor = Color.White;
            this.PlusBut.FlatAppearance.MouseDownBackColor = Color.White;
            this.PlusBut.FlatStyle = FlatStyle.Flat;
            this.PlusBut.Font = new Font("Microsoft Sans Serif", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.PlusBut.Location = new Point(150, 1);
            this.PlusBut.Name = "PlusBut";
            this.PlusBut.Size = new Size(28, 28);
            this.PlusBut.TabIndex = 2;
            this.PlusBut.Text = "+";
            this.PlusBut.UseVisualStyleBackColor = false;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            base.Controls.Add(this.PlusBut);
            base.Controls.Add(this.MinusBut);
            base.Controls.Add(this.DynamicBut);
            base.Name = "DisplayControl";
            base.Size = new Size(180, 30);
            base.ResumeLayout(false);
        }

        private void MinusBut_Click(object sender, EventArgs e) {
            Control_Display.MinusClickedEventHandler minusClickedEventHandler = this.MinusClicked;
            if (minusClickedEventHandler != null) {
                minusClickedEventHandler();
            }
        }

        private void PlusBut_Click(object sender, EventArgs e) {
            Control_Display.PlusClickedEventHandler plusClickedEventHandler = this.PlusClicked;
            if (plusClickedEventHandler != null) {
                plusClickedEventHandler();
            }
        }

        public event Control_Display.DynamicClickedEventHandler DynamicClicked;

        public event Control_Display.MinusClickedEventHandler MinusClicked;

        public event Control_Display.PlusClickedEventHandler PlusClicked;

        public delegate void DynamicClickedEventHandler();

        public delegate void MinusClickedEventHandler();

        public delegate void PlusClickedEventHandler();
    }
}
