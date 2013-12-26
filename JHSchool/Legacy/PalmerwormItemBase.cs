using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FISCA.DSAUtil;
using Framework.Security;
using Framework;

namespace JHSchool.Legacy
{
    public partial class PalmerwormItemBase : UserControl, SmartSchool.Customization.PlugIn.ExtendedContent.IContentItem
    {
        private bool _SaveButtonVisible;
        private bool _CancelButtonVisible;

        private string _Title;

        public PalmerwormItemBase()
        {
            InitializeComponent();
            int x = (this.Width - picWaiting.Width) / 2;
            int y = (this.Height - picWaiting.Height) / 2;
            this.picWaiting.Location = new Point(x, y);
        }

        #region IContentItem 成員

        public bool SaveButtonVisible
        {
            get { return _SaveButtonVisible; }
            set 
            {
                if (_SaveButtonVisible != value)
                {
                    _SaveButtonVisible = value;
                    if (SaveButtonVisibleChanged != null)
                        SaveButtonVisibleChanged.Invoke(this, new EventArgs());
                }
            }
        }

        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }

        public Control DisplayControl
        {
            get { return this; }
        }

        public virtual void LoadContent(string id)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual void Save()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual void Undo()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public event EventHandler SaveButtonVisibleChanged;

        public bool CancelButtonVisible
        {
            get { return _CancelButtonVisible; }
            set
            {
                if ( _CancelButtonVisible != value )
                {
                    _CancelButtonVisible = value;
                    if ( CancelButtonVisibleChanged != null )
                        CancelButtonVisibleChanged.Invoke(this, new EventArgs());
                }
            }
        }

        public event EventHandler CancelButtonVisibleChanged;

        #endregion

        private void PalmerwormItemBase_SizeChanged(object sender, EventArgs e)
        {
            int x = (this.Width - picWaiting.Width) / 2;
            int y = (this.Height - picWaiting.Height) / 2;
            this.picWaiting.Location = new Point(x, y);
        }

        protected bool WaitingPicVisible
        {
            get { return picWaiting.Visible; }
            set
            {
                if ( value )
                {
                    if ( Controls.Contains(picWaiting) )
                    {
                        Controls.SetChildIndex(picWaiting, 0);
                        picWaiting.Show();
                    }
                }
                else
                {
                    picWaiting.Hide();
                }
            }
        }

        #region ICloneable 成員

        public virtual object Clone()
        {
            throw new Exception("必須實做Clone");
        }

        #endregion
    }
}
