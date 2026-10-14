using System;
using System.Reflection;
using System.Windows.Forms;

namespace JHSchool
{
    /// <summary>
    /// 詳細資料面板的延遲載入：面板所在的群組收合時不載入資料，展開時才載入目前的記錄。
    /// </summary>
    /// <remarks>
    /// FISCA 的 DetailContent 本身沒有收合狀態。收合的是包住面板的 DetailItemContainer
    /// （繼承 DevComponents ExpandablePanel），而 DetailItemContainer 不論是否展開都會設定 PrimaryKey。
    /// 這裡沿著 Parent 往上找出具有 Expanded 屬性與 ExpandedChanged 事件的容器，
    /// 以反射存取，避免綁定特定版本的 DevComponents 組件。
    /// </remarks>
    internal sealed class DetailContentLazyLoader
    {
        private readonly Control _content;
        private readonly Func<string> _getPrimaryKey;
        private readonly Action _load;

        private Control _container;
        private PropertyInfo _expandedProperty;
        private string _loadedKey;

        /// <param name="content">詳細資料面板。</param>
        /// <param name="getPrimaryKey">取得面板目前的 PrimaryKey。</param>
        /// <param name="load">實際載入資料的動作。</param>
        public DetailContentLazyLoader(Control content, Func<string> getPrimaryKey, Action load)
        {
            _content = content;
            _getPrimaryKey = getPrimaryKey;
            _load = load;
            _content.ParentChanged += delegate { AttachContainer(); };
        }

        /// <summary>
        /// 於 PrimaryKey 變更時呼叫：群組展開中就立即載入，收合中則等到展開時再載入。
        /// </summary>
        public void RequestLoad()
        {
            AttachContainer();
            if (IsCollapsed) return;

            _loadedKey = _getPrimaryKey();
            _load();
        }

        private bool IsCollapsed
        {
            get
            {
                if (_container == null) return false;
                return !(bool)_expandedProperty.GetValue(_container, null);
            }
        }

        private void AttachContainer()
        {
            if (_container != null) return;

            for (Control parent = _content.Parent; parent != null; parent = parent.Parent)
            {
                PropertyInfo expanded = parent.GetType().GetProperty("Expanded", typeof(bool));
                EventInfo expandedChanged = parent.GetType().GetEvent("ExpandedChanged");
                if (expanded == null || expandedChanged == null) continue;

                _container = parent;
                _expandedProperty = expanded;
                expandedChanged.AddEventHandler(parent, Delegate.CreateDelegate(
                    expandedChanged.EventHandlerType, this,
                    typeof(DetailContentLazyLoader).GetMethod("Container_ExpandedChanged", BindingFlags.NonPublic | BindingFlags.Instance)));
                return;
            }
        }

        private void Container_ExpandedChanged(object sender, EventArgs e)
        {
            if (IsCollapsed || _content.IsDisposed) return;

            string key = _getPrimaryKey();
            if (!string.IsNullOrEmpty(key) && key != _loadedKey)
                RequestLoad();
        }
    }
}
