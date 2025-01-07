using System.Windows.Forms;

namespace planimals.Forms
{
    public partial class Editor : Form
    {
        MainForm mainForm;

        //add text boxes and buttons

        public Editor(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;

        }
    }
}
