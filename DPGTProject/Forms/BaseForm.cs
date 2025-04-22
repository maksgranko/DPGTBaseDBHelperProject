using System.Windows.Forms;
using DPGTProject.Configs;

namespace DPGTProject.Forms
{
    public class BaseForm : Form
    {
        public BaseForm()
        {
            DesignConfig.ApplyTheme(SystemConfig.applicationTheme, this);
            if (SystemConfig.Icon != null)
            {
                this.Icon = SystemConfig.Icon;
            }
        }

        protected void SetControlVisibility(Control control, bool isVisible)
        {
            if (control != null)
            {
                control.Visible = isVisible;
            }
        }
    }
}
