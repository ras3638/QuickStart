using System.Windows.Forms;

namespace QuickStart
{
	public class ToolStripOverride : ToolStripProfessionalRenderer
	{
		public ToolStripOverride() { }

		protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) { }
	}
}