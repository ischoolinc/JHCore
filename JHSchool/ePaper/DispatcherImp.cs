using System;
using System.Collections.Generic;
using System.Text;
using SmartSchool.ePaper;
using FISCA.DSAUtil;

namespace JHSchool
{
    public class DispatcherImp : IPaperDispatcher
    {
        #region IPaperDispatcher 成員

        public void Dispatch(ElectronicPaper ePaper)
        {
            string epaperId = EditElectronicPaper.Insert(
                ePaper.Name,
                ePaper.SchoolYear,
                ePaper.Semester,
                ePaper.ViewerType.ToString(),
                ePaper.Metadata);

            List<DSXmlHelper> requests = new List<DSXmlHelper>();
            DSXmlHelper papers = null;
            int count = 0;

            IProgressReceiver receiver = ePaper.ProgressReceiver;
            if (receiver == null)
                receiver = new ElectronicPaperProgress();

            foreach (PaperItem eachPaper in ePaper)
            {
                if (eachPaper.Viewers.Count <= 0) throw new ArgumentException("每一張電子報表至少要有一個 Viewer。");

                if (count % 10 == 0)
                {
                    papers = new DSXmlHelper("Request");
                    requests.Add(papers);
                }

                DSXmlHelper paper = new DSXmlHelper(papers.AddElement("Paper"));
                foreach (string eachViewer in eachPaper.Viewers)
                {
                    paper.AddElement(".", "RefElectronicPaperID", epaperId);
                    paper.AddElement(".", "Format", eachPaper.Format);
                    paper.AddElement(".", "Content", eachPaper.Content);
                    //paper.AddElement(".", "ViewerType", ePaper.ViewerType.ToString());
                    paper.AddElement(".", "ViewerID", eachViewer);

                    count++;
                }
            }

            if (count > 0)
            {
                if (receiver != null)
                    receiver.ProcessStart();

                int current = 1;
                foreach (DSXmlHelper each in requests)
                {
                    EditElectronicPaper.InsertPaperItem(each);

                    if (receiver != null)
                        receiver.ProcessProgress((int)(((float)current / (float)requests.Count) * 100f));

                    current++;
                }

                if (receiver != null)
                    receiver.ProcessEnd();
            }
        }

        #endregion

        private class ElectronicPaperProgress : IProgressReceiver
        {
            ProgressForm form;

            public ElectronicPaperProgress()
            {
                form = new ProgressForm("傳送電子報表…");
            }

            #region IProgressReceiver 成員

            public void ProcessStart()
            {
                form.Minimum = 1;
                form.Maximum = 100;
                form.Value = 1;
                form.Show();
            }

            public void ProcessEnd()
            {
                form.Value = 100;
                form.Close();
            }

            public void ProcessProgress(int progress)
            {
                form.Value = progress;
            }
            #endregion
        }

    }
}
