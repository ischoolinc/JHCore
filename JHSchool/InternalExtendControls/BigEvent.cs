using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace JHSchool.InternalExtendControls
{
    /// <summary>
    /// 負責大型事件的管理。
    /// 引發事件過程中，所有被引發的方法都會被例外保護，不會因為其中一個方法產生錯誤，而使得後面的事件接收者無法接收事件。
    /// </summary>
    public class BigEvent
    {
        /// <summary>
        /// 建立事件管理器實體。
        /// </summary>
        /// <param name="eventHandler">要管理的事件實體。</param>
        /// <param name="sender">事件傳送者。</param>
        /// <param name="args">事件參數。</param>
        public BigEvent(Delegate eventHandler, object sender, params object[] args)
            : this(string.Empty, eventHandler, sender, args)
        {
        }

        /// <summary>
        /// 建立事件管理器實體。
        /// </summary>
        /// <param name="eventName">事件名稱，可利於除錯。</param>
        /// <param name="eventHandler">要管理的事件實體。</param>
        /// <param name="sender">事件傳送者。</param>
        /// <param name="args">事件參數。</param>
        public BigEvent(string eventName, Delegate eventHandler, object sender, params object[] args)
        {
            EventHandler = eventHandler;
            Sender = sender;
            Arguments = args;
            EventName = eventName;
            Exceptions = new List<Exception>();
        }
        /// <summary>
        /// 當非同步引發 UI 事件完成時發生。這個事件會在背景執行緒引發。
        /// </summary>
        public event EventHandler UIRaiseAsyncComplete;
        /// <summary>
        /// 取得事件實體。
        /// </summary>
        public Delegate EventHandler { get; private set; }
        /// <summary>
        /// 取得傳送事件的物件 
        /// </summary>
        public object Sender { get; private set; }
        /// <summary>
        /// 取得事件參數。
        /// </summary>
        public object[] Arguments { get; private set; }
        /// <summary>
        /// 取得或設定事件名稱，用於除錯，如果有提供將會對除錯很有幫助。
        /// </summary>
        public string EventName { get; set; }
        /// <summary>
        /// 取得引發事件中所發生的錯誤。
        /// </summary>
        public List<Exception> Exceptions { get; private set; }
        /// <summary>
        /// 指示事件引發途中是否有錯誤。
        /// </summary>
        public bool HasException { get { return Exceptions.Count > 0; } }
        /// <summary>
        /// 引發事件，所有被引發的方法都會被例外保護，不會因為其中一個方法產生錯誤，而使得後面的事件接收者無法接收事件。
        /// 不管呼叫端是在哪一執行緒，引發的事件會在 UI 執行緒執行，適合用於引發 UI 相關事件。
        /// 使用此方法會誇執行緒引發 UI 事件請注意死結問題。
        /// </summary>
        public void UIRaise()
        {
            if (Framework.Legacy.GlobalOld.MainThreadControl.InvokeRequired)
                Framework.Legacy.GlobalOld.MainThreadControl.Invoke(new MethodInvoker(Raise));
            else
                Raise();
        }
        /// <summary>
        /// 引發事件，所有被引發的方法都會被例外保護，不會因為其中一個方法產生錯誤，而使得後面的事件接收者無法接收事件。
        /// 不管呼叫端是在哪一執行緒，引發的事件會在 UI 執行緒執行，適合用於引發 UI 相關事件。
        /// 此方法是 UIRaise 的非同步版本。
        /// </summary>
        public void UIRaiseAsync()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(RaiseUIEvent));
        }
        private void RaiseUIEvent(object state)
        {
            UIRaise();

            if (UIRaiseAsyncComplete != null)
                UIRaiseAsyncComplete(this, EventArgs.Empty);
        }
        /// <summary>
        /// 引發事件，所有被引發的方法都會被例外保護，不會因為其中一個方法產生錯誤，而使得後面的事件接收者無法接收事件。
        /// </summary>
        public void Raise()
        {
            if (EventHandler != null)
            {
                List<object> eargs = new List<object>();
                eargs.Add(Sender);
                eargs.AddRange(Arguments);

                Exceptions.Clear(); //先清除之前的狀態。

                //if (Diagnostic.Options.OutputDiagnosticMessage)
                //    Trace.WriteLine(string.Format("引發者：「{0}」,事件名稱：「{1}」", Sender.ToString(), string.IsNullOrEmpty(EventName) ? "<沒有事件名稱>" : EventName));
                foreach (Delegate each in EventHandler.GetInvocationList())
                {
                    //if (Diagnostic.Options.OutputDiagnosticMessage)
                    //    Trace.WriteLine(string.Format("接收者：{0},成員：{1}", each.Method.ReflectedType.FullName, each.Method.Name));
                    try
                    {
                        each.DynamicInvoke(eargs.ToArray());
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null)
                        {
                            //if (Diagnostic.Options.OutputDiagnosticMessage)
                            //    Trace.WriteLine(string.Format("接收者錯誤：{0}", ex.InnerException.Message));
                            Exceptions.Add(new RaiseEventException(ex.InnerException));
                        }
                        else
                        {
                            //if (Diagnostic.Options.OutputDiagnosticMessage)
                            //    Trace.WriteLine(string.Format("接收者錯誤，沒有訊息！"));
                            Exceptions.Add(new RaiseEventException(ex));
                        }
                    }
                }
            }
        }
    }

    public class RaiseEventException : Exception
    {
        public RaiseEventException(Exception originException)
            : base("事件傳播過程發生錯誤，請查看 InnerException 檢視詳細資訊。", originException)
        {
        }
    }
}
