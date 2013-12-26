using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.Presentation;
using Framework;
using Framework.Security;

namespace JHSchool
{
    /// <summary>
    /// 整合快取資料功能的NavContentPresentation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LegacyPresentBase<T> : CacheManager<T>
    {
        protected NLDPanel Present { get; set; }

        public LegacyPresentBase(NLDPanel present)
        {
            Present = present;
            UseFilter = false;

            ItemUpdated += delegate(object sender, ItemUpdatedEventArgs e)
            {
                Present.RefillListPane();
                SetSource();
            };

            ItemLoaded += delegate(object sender, EventArgs e)
            {
                Present.ShowLoading = false;
                SetSource();
            };

            Present.CompareSource += delegate(object sender, CompareEventArgs e)
            {
                e.Result = QuickCompare(e.Value1, e.Value2);
            };

            Present.SelectedSourceChanged += delegate
            {
                if (SelectedListChanged != null)
                    SelectedListChanged(this, EventArgs.Empty);
            };

            Present.TempSourceChanged += delegate
            {
                if (TemporaListChanged != null)
                    TemporaListChanged(this, EventArgs.Empty);
            };
        }

        /// <summary>
        /// 一次取得所有資料項目
        /// </summary>
        /// <returns>傳回索引鍵跟快取資料的查詢</returns>
        protected override Dictionary<string, T> GetAllData()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 一次取得部份指定鍵值的資料。
        /// </summary>
        /// <param name="primaryKeys">要取得的鍵值</param>
        /// <returns>傳回索引鍵跟快取資料的查詢</returns>
        protected override Dictionary<string, T> GetData(IEnumerable<string> primaryKeys)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 新增學生到待處理。
        /// </summary>
        /// <param name="primaryKeys">學生編號清單。</param>
        public void AddToTemporal(List<string> primaryKeys)
        {
            AddToTemp(primaryKeys);
        }

        /// <summary>
        /// 將學生移出待處理。
        /// </summary>
        /// <param name="primaryKeys">學生編號清單。</param>
        public void RemoveFromTemporal(List<string> primaryKeys)
        {
            RemoveFromTemp(primaryKeys);
        }

        public List<string> SelectedKeys
        {
            get
            {
                return AsKeyList(SelectedList);
            }
        }

        public List<string> TemporalKeys
        {
            get
            {
                return AsKeyList(TemporaList);
            }
        }

        protected virtual List<string> AsKeyList(List<T> list)
        {
            throw new NotImplementedException();
        }

        public void AddDetailBulider(IDetailBulider item)
        {
            DetailContent content = item.GetContent();
            if (content == null) return;

            if (Attribute.IsDefined(content.GetType(), typeof(FeatureCodeAttribute)))
            {
                FeatureCodeAttribute fca = Attribute.GetCustomAttribute(content.GetType(), typeof(FeatureCodeAttribute)) as FeatureCodeAttribute;
                if (fca != null)
                {
                    if (Framework.Legacy.GlobalOld.Acl[content.GetType()].Viewable)
                        Present.AddDetailBulider(item);
                }
            }
            else
                Present.AddDetailBulider(item);
        }

        public void PopupDetailPane(string id)
        {
            Present.PopupDetailPane(id);
        }

        public void AddListPaneField(ListPaneField field)
        {
            Present.AddListPaneField(field);
        }

        public void AddView(INavView view)
        {
            Present.AddView(view);
        }

        public void AddToTemp(List<string> primaryKeys)
        {
            Present.AddToTemp(primaryKeys);
        }

        public void RemoveFromTemp(List<string> primaryKeys)
        {
            Present.RemoveFromTemp(primaryKeys);
        }

        public DivisionBarManager RibbonBarItems { get { return Present.RibbonBarItems; } }

        public MenuButtonControl FilterMenu { get { return Present.FilterMenu; } }

        public MenuButtonControl SearchConditionMenu { get { return Present.SearchConditionMenu; } }

        public MenuButton ListPaneContexMenu { get { return Present.ListPaneContexMenu; } }

        protected void RaiseItemUpdated(IEnumerable<string> primaryKeys)
        {
            OnItemUpdated(new ItemUpdatedEventArgs(primaryKeys));
        }

        public event EventHandler SelectedListChanged;

        public event EventHandler TemporaListChanged;

        #region Type Convert
        public List<T> TemporaList
        {
            get
            {
                List<T> list = new List<T>();
                foreach (string each in Present.TempSource)
                    list.Add(Items[each]);

                return list;
            }
        }

        public List<T> SelectedList
        {
            get
            {
                List<T> list = new List<T>();
                foreach (string each in Present.SelectedSource)
                    list.Add(Items[each]);

                return list;
            }
        }
        #endregion

        /// <summary>
        /// 重新整理Filter的內容，於資料讀取完成或資料變更時呼叫
        /// </summary>
        protected virtual void FillFilter() { }
        /// <summary>
        /// 取得或設定，使用Filter機制
        /// </summary>
        private bool _UseFilter = false;
        protected bool UseFilter { get { return _UseFilter; } set { _UseFilter = value; FilterMenu.Visible = value; } }
        private void SetSource()
        {
            if (_UseFilter)
                FillFilter();
            else
                Present.SetFilteredSource(new List<string>(Items.Keys));
        }
        /// <summary>
        /// 取得指定索引的項目，若指定的鍵值不存在則會先嚐試進行查尋
        /// </summary>
        /// <param name="primaryKey">取得項目的鍵值</param>
        /// <returns>該鍵值的項目，若傳入鍵值沒有對應項目則傳回default(T)</returns>
        public T this[string primaryKey]
        {
            get { return Items[primaryKey]; }
        }

        /// <summary>
        /// 重新排序快取資料，快取的資料型別若為IComparable則將自動進行排序
        /// 不需呼叫此方法也會維持順序，唯有當IComparable.CompareTo實作變更時使用此方法重新排序
        /// </summary>
        public override void SortItems()
        {
            base.SortItems();
            Present.RefillListPane();
        }

        /// <summary>
        /// 取得所有資料，此方法將於背景執行續進行，並於完成後引發ItemLoaded事件
        /// </summary>
        public override void SyncAllBackground()
        {
            base.SyncAllBackground();
            Present.ShowLoading = true;
        }
    }

    public static class JHPrevious_Extensions
    {
        public static StudentRecord AsOldStudent(this JHSchool.Data.JHStudentRecord student)
        {
            return new StudentRecord(student);
        }

        public static List<StudentRecord> AsOldStudent(this IEnumerable<JHSchool.Data.JHStudentRecord> students)
        {
            List<StudentRecord> oldstuds = new List<StudentRecord>();
            foreach (JHSchool.Data.JHStudentRecord each in students)
                oldstuds.Add(new StudentRecord(each));
            return oldstuds;
        }
    }
}
