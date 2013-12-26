using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace JHSchool
{
    public partial class AllItemView :FISCA.Presentation.NavView
    {
        public AllItemView()
        {
            InitializeComponent();
            this.Text = "顯示所有項目";
        }

        #region NavView 成員

        public bool Active
        {
            get;
            set;
        }

        public string Description
        {
            get { return "顯示所有項目"; }
        }

        public Control DisplayPane
        {
            get { return this; }
        }

        //public Control ConfigurationPane
        //{
        //    get { return null; }
        //}
        private List<string> _PrimaryKeys = new List<string>();
        public new void Layout(List<string> PrimaryKeys)
        {
            _PrimaryKeys =new List<string>( PrimaryKeys);
            if(ListPaneSourceChanged !=null)
                ListPaneSourceChanged (this,new FISCA.Presentation.ListPaneSourceChangedEventArgs(PrimaryKeys));
        }

        public event EventHandler<FISCA.Presentation.ListPaneSourceChangedEventArgs> ListPaneSourceChanged;

        #endregion

        private void ShowAllStudentsView_Click(object sender, EventArgs e)
        {
            if ( ListPaneSourceChanged != null )
                ListPaneSourceChanged(this, new FISCA.Presentation.ListPaneSourceChangedEventArgs(_PrimaryKeys));
        }
    }
}
