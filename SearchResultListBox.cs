using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Ndexer {
    public class SearchResultListBox : ListBox {
        Bitmap _ScratchBuffer = null;
        Graphics _ScratchGraphics = null;

        public SearchResultListBox() 
            : base() {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
        }

        protected override void Dispose (bool disposing) {
            DisposeGraphics();

            base.Dispose(disposing);
        }

        void DisposeGraphics () {
            if (_ScratchGraphics != null) {
                _ScratchGraphics.Dispose();
                _ScratchGraphics = null;
            }

            if (_ScratchBuffer != null) {
                _ScratchBuffer.Dispose();
                _ScratchBuffer = null;
            }
        }

        protected Graphics GetScratchGraphics (Rectangle bounds) {
            if ((_ScratchBuffer == null) || (_ScratchBuffer.Width < bounds.Width) || (_ScratchBuffer.Height < bounds.Height)) {
                DisposeGraphics();

                _ScratchBuffer = new Bitmap(bounds.Width, bounds.Height);
                _ScratchGraphics = Graphics.FromImage(_ScratchBuffer);
            }

            _ScratchGraphics.SetClip(bounds, System.Drawing.Drawing2D.CombineMode.Replace);

            return _ScratchGraphics;
        }

        protected override void OnDrawItem (DrawItemEventArgs e) {
            var bounds = new Rectangle(0, 0, e.Bounds.Width, e.Bounds.Height);
            var g = GetScratchGraphics(bounds);
            g.Clear(e.BackColor);

            var newArgs = new DrawItemEventArgs(g, e.Font, bounds, e.Index, e.State, e.ForeColor, e.BackColor);

            base.OnDrawItem(newArgs);
            e.Graphics.DrawImageUnscaledAndClipped(_ScratchBuffer, e.Bounds);
        }

        protected override void OnPaintBackground (PaintEventArgs pevent) {
        }
    }
}
