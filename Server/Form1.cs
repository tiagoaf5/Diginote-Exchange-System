using System.Runtime.Remoting;
using System.Windows.Forms;

namespace Server
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            RemotingConfiguration.Configure("Server.exe.config", false);
            InitializeComponent();
        }
    }
}
