using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Framework.Security;

namespace JHSchool
{
    public partial class DetailContentBase : FISCA.Presentation.DetailContent 
    {
        public DetailContentBase()
        {
            InitializeComponent();
        }

        public new bool SaveButtonVisible
        {
            get { return base.SaveButtonVisible; }
            set
            {
                //判斷權限
                if (Attribute.IsDefined(GetType(), typeof(FeatureCodeAttribute)))
                {
                    FeatureCodeAttribute fca = Attribute.GetCustomAttribute(GetType(), typeof(FeatureCodeAttribute)) as FeatureCodeAttribute;
                    if (fca != null)
                    {
                        if (Framework.Legacy.GlobalOld.Acl[GetType()].Editable)
                            base.SaveButtonVisible = value;
                    }
                }
                else //沒有定議權限就按照平常方法處理。
                    base.SaveButtonVisible = value;
            }
        }

        public new bool CancelButtonVisible
        {
            get { return base.CancelButtonVisible; }
            set
            {
                //判斷權限
                if (Attribute.IsDefined(GetType(), typeof(FeatureCodeAttribute)))
                {
                    FeatureCodeAttribute fca = Attribute.GetCustomAttribute(GetType(), typeof(FeatureCodeAttribute)) as FeatureCodeAttribute;
                    if (fca != null)
                    {
                        if (Framework.Legacy.GlobalOld.Acl[GetType()].Editable)
                            base.CancelButtonVisible = value;
                    }
                }
                else //沒有定議權限就按照平常方法處理。
                    base.CancelButtonVisible = value;
            }
        }

    }
}
