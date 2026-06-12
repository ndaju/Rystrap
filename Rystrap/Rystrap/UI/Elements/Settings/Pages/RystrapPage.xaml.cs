using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rystrap.UI.ViewModels.Settings;

namespace Rystrap.UI.Elements.Settings.Pages
{
    /// <summary>
    /// Interaction logic for RystrapPage.xaml
    /// </summary>
    public partial class RystrapPage
    {
        public RystrapPage()
        {
            DataContext = new RystrapViewModel();
            InitializeComponent();
        }
    }
}
