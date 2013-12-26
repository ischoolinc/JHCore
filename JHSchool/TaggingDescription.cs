using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using Framework;

namespace JHSchool
{
    /// <summary>
    /// 終極泛型方法應用實例。
    /// 要研究或修改這段程式時請保持清醒的頭腦。
    /// 帶有「Tag」資訊列的 DescriptionPane。
    /// </summary>
    public partial class TaggingDescription :FISCA.Presentation.DescriptionPane 
    {
        /// <summary>
        /// 指示是否已經訂了 Tag 的 ItemUpdate 事件。
        /// </summary>
        private bool event_subscribe = false;

        public TaggingDescription()
        {
            InitializeComponent();

            JHSchool.Tag.Instance.ItemUpdated += delegate(object sender, ItemUpdatedEventArgs e)
            {
                OnPrimaryKeyChanged(EventArgs.Empty);
            };
        }

        /// <summary>
        /// 終極泛型方法。
        /// </summary>
        /// <typeparam name="Cache">Tag 的 Cache 型別。</typeparam>
        /// <typeparam name="List">Tag 的 List 型別。</typeparam>
        /// <typeparam name="Record">Tag 的 Record 型別。</typeparam>
        protected void DisplayInformation<Cache, List, Record>(Cache cacher)
            where Cache : GeneralTag<Record>
            where List : List<Record>, new()
            where Record : GeneralTagRecord, new()
        {
            if (string.IsNullOrEmpty(PrimaryKey))
            {
                DisplayTags<List, Record>(new List(), "");
            }

            if (cacher.Items.ContainsKey(PrimaryKey))
                DisplayTags<List, Record>(cacher[PrimaryKey] as List, "<無類別資訊>");
            else
            {
                if (!event_subscribe)
                {
                    cacher.ItemUpdated += new EventHandler<ItemUpdatedEventArgs>(TagUpdatedDelegate<Cache, List, Record>);
                    event_subscribe = true;
                }

                cacher.SyncDataBackground(PrimaryKey);
            }
        }

        /// <summary>
        /// 處理 Tag 的 ItemUpdated 事件。
        /// 當有使用者修改 Tag 與  Entity 之間的關係時，此事件會做相對應的處理。
        /// </summary>
        private void TagUpdatedDelegate<Cache, List, Record>(object sender, ItemUpdatedEventArgs e)
            where Cache : GeneralTag<Record>
            where List : List<Record>, new()
            where Record : GeneralTagRecord, new()
        {
            Cache cacher = sender as Cache;

            if (e.PrimaryKeys.Contains(PrimaryKey))
                DisplayTags<List, Record>(cacher[PrimaryKey] as List, "<無類別資訊>");
        }

        /// <summary>
        /// 如果有需要 Override 此方法，請務必呼叫「base.OnParentChanged」。
        /// </summary>
        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            if (Parent != null)
                Parent.SizeChanged += delegate //當 Parent 的大小改變時，要重新計算控制項大小。
                {
                    OnPrimaryKeyChanged(e);
                };
        }

        /// <summary>
        /// 在 Tag Panel 上顯示 Tag。
        /// </summary>
        /// <param name="notTagMessage">當 Tag 數量是 0 時要顯示的訊息。</param>
        private void DisplayTags<T, F>(T tags, string notTagMessage)
            where T : List<F>
            where F : GeneralTagRecord, new()
        {
            TagPanel.SuspendLayout();

            List<PanelEx> tagpanels = new List<PanelEx>();

            foreach (F each in tags)
                tagpanels.Add(CreateTagItem(each.FullName, each.Color));

            //if (tags.Count <= 0)
            //    tagpanels.Add(CreateTagItem(notTagMessage, Color.White));

            CalculateTagSzie(tagpanels);

            TagPanel.Controls.Clear();
            foreach (PanelEx each in tagpanels)
                TagPanel.Controls.Add(each);

            TagPanel.ResumeLayout();
        }

        private PanelEx CreateTagItem(string title, Color color)
        {
            PanelEx objPanel = new PanelEx();

            objPanel.Margin = new System.Windows.Forms.Padding(2);
            objPanel.CanvasColor = System.Drawing.SystemColors.Control;
            objPanel.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            objPanel.Size = new System.Drawing.Size(91, 20);
            objPanel.Style.Alignment = System.Drawing.StringAlignment.Center;
            objPanel.Style.BorderColor.ColorSchemePart = eColorSchemePart.Custom;
            objPanel.Style.BorderColor.Color = Color.LightBlue;
            objPanel.Style.BackColor2.ColorSchemePart = eColorSchemePart.Custom;
            objPanel.Style.BackColor2.Color = color;
            objPanel.Style.BackColor1.ColorSchemePart = eColorSchemePart.Custom;
            objPanel.Style.BackColor1.Color = Color.White;
            objPanel.Style.ForeColor.ColorSchemePart = eColorSchemePart.Custom;
            objPanel.Style.ForeColor.Color = Color.Black;
            objPanel.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
            objPanel.Style.CornerDiameter = 3;
            objPanel.Style.CornerType = DevComponents.DotNetBar.eCornerType.Diagonal;
            objPanel.Style.GradientAngle = 90;
            objPanel.Style.TextTrimming = System.Drawing.StringTrimming.None;
            objPanel.Text = title;
            return objPanel;
        }

        /// <summary>
        /// 計算 Tag 的大小，使其剛好填滿 Tag Panel。
        /// </summary>
        /// <param name="tags"></param>
        private void CalculateTagSzie(List<PanelEx> tags)
        {
            if (Parent == null || tags.Count <= 0) return;

            TableLayout.Width = Parent.Width;
            Width = Parent.Width;

            int partsize = Width / tags.Count;

            foreach (PanelEx each in tags)
                each.Size = new Size(partsize - 10, each.Size.Height);
        }
    }
}
