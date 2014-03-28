using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GroundControl.Gui
{
    public partial class BlogMessageDialog : Form
    {
        public BlogMessageDialog()
        {
            InitializeComponent();
        }

        public String Message { get { return textBox1.Text; } }
    }
}
