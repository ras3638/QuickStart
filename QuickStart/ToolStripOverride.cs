using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace QuickStart
{
	public class ToolStripOverride : ToolStripProfessionalRenderer
	{
		public ToolStripOverride() { }

		protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) { }
    }

    public class ToolStripDropDownOverride : ToolStripProfessionalRenderer
    {
        public ToolStripDropDownOverride() { }

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            if (e.Item.GetType() == typeof(ToolStripDropDownButton))
            {
                //Change arrow direction
                Rectangle r = e.ArrowRectangle;
                List<Point> points = new List<Point>();

                points.Add(new Point(r.Left - 3, r.Height - 8));
                points.Add(new Point(r.Right - 2, r.Height - 8));
                points.Add(new Point(r.Left + (r.Width / 2) - 2, (r.Height / 2) - 3));

                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                e.Graphics.FillPolygon(Brushes.Black, points.ToArray());
            }
            else
            {
                base.OnRenderArrow(e);
            }
        }

    }

}