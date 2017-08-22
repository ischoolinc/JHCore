using System;
using FISCA;
using Framework;
using JHSchool.Properties;
using FISCA.Presentation;

namespace JHSchool
{
    static public class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [ApplicationMain()]
        static public void Main()
        {
            //1秒
            Framework.Program.Initial();
            //FISCA.Presentation.MotherForm.PreferenceProvider = new PresentationPreference();

            //學校組態基本上就是儲存在 App.Configuration 之中。
            School.Configuration = new School.SchoolConfigRedirect(Framework.App.Configuration);

            //電子報表
            SmartSchool.ePaper.DispatcherProvider.Register("ischool", new DispatcherImp(), true);

            Class.Instance.SyncAllBackground();
            Class.Instance.WaitLoadingComplete();
            Student.Instance.SyncAllBackground();
            Student.Instance.WaitLoadingComplete();
            Teacher.Instance.SyncAllBackground();
            Teacher.Instance.WaitLoadingComplete();
            Course.Instance.SyncAllBackground();
            Course.Instance.WaitLoadingComplete();

            //K12.Student.Instance.AddView(new ShowAllStudentsView());
            //1.1 秒
            Student.Instance.SetupPresentation();
            Class.Instance.SetupPresentation();
            Teacher.Instance.SetupPresentation();
            Course.Instance.SetupPresentation(); //課程的類別已調整
            //K12.Course.Instance.AddView(new ShowAllStudentsView());

            //設定 ASPOSE 元件的 License。
            System.IO.Stream stream = new System.IO.MemoryStream(Resources.Aspose_Total);

            stream.Seek(0, System.IO.SeekOrigin.Begin);
            new Aspose.Words.License().SetLicense(stream);
            stream.Seek(0, System.IO.SeekOrigin.Begin);

            // 2017/8/22 穎驊依據高雄小組專案 [03-05][04+] EXCEL匯入格式可否修正為xlsx也可匯入？ 更改為新版 Aspose.Cells_201402 寫法，
            //另外詢問耀明後，補充此段程式碼Aspose 已在別的地方做認證，不需要重覆做。

            //new Aspose.Cells.License().SetLicense(stream);


            stream.Seek(0, System.IO.SeekOrigin.Begin);
            new Aspose.BarCode.License().SetLicense(stream);
            stream.Seek(0, System.IO.SeekOrigin.Begin);
            new Aspose.Pdf.License().SetLicense(stream);

            FISCA.LogAgent.ApplicationLog.Log("[特殊歷程]", "登入", string.Format("使用者{0}已登入系統", FISCA.Authentication.DSAServices.UserAccount));

            // 變更使用者密碼
            FISCA.Presentation.MotherForm.StartMenu["安全性"].BeginGroup = true;
            FISCA.Presentation.MotherForm.StartMenu["安全性"].Image = Properties.Resources.foreign_key_lock_64;
            FISCA.Presentation.MotherForm.StartMenu["安全性"]["變更密碼"].Enable = User.Acl["StartButton0004"].Executable;
            FISCA.Presentation.MotherForm.StartMenu["安全性"]["變更密碼"].Click += delegate
            {
                JHSchool.UserInfoManager uim = new UserInfoManager();
                uim.ShowDialog();
            };

            // 管理學校基本資料
            FISCA.Presentation.MotherForm.StartMenu["管理學校基本資料"].Image = Properties.Resources.school_fav_64;
            FISCA.Presentation.MotherForm.StartMenu["管理學校基本資料"].Enable = User.Acl["StartButton0003"].Executable;
            FISCA.Presentation.MotherForm.StartMenu["管理學校基本資料"].Click += delegate
            {
                JHSchool.SchoolInfoMangement sim = new SchoolInfoMangement();
                sim.ShowDialog();
            };

            Framework.Security.RoleAclSource.Instance["系統"].Add(new Framework.Security.RibbonFeature("StartButton0003", "管理學校基本資料"));
            Framework.Security.RoleAclSource.Instance["系統"].Add(new Framework.Security.RibbonFeature("StartButton0004", "變更密碼"));

            FISCA.Presentation.MotherForm.StartMenu["重新登入"].Image = Properties.Resources.world_upload_64;
            FISCA.Presentation.MotherForm.StartMenu["重新登入"].BeginGroup = true;
            FISCA.Presentation.MotherForm.StartMenu["重新登入"].Click += new EventHandler(Restart_Click);

            //設定畫面選取Count
            SelectedListChanged();

            //new K12.General.Feedback.NewsNotice();

            //Student.Instance.RibbonBarItems["測試"]["測Tag"].Click += delegate
            //{
            //    Tag.Instance.SyncAllBackground();
            //    Framework.FISCA.Presentation.Controls.MsgBox.Show(Tag.Instance.Items.Count.ToString());

            //    foreach (TagRecord ech in Tag.Instance.Items.GetStudentTags())
            //        Console.WriteLine(ech.FullName);
            //};

            //StudentTag.Instance.ItemLoaded += delegate
            //{
            //    Framework.FISCA.Presentation.Controls.MsgBox.Show(StudentTag.Instance.Items.Count.ToString());
            //};

            //ClassTag.Instance.ItemLoaded += delegate
            //{
            //    Framework.FISCA.Presentation.Controls.MsgBox.Show(ClassTag.Instance.Items.Count.ToString());
            //};

            //TeacherTag.Instance.ItemLoaded += delegate
            //{
            //    Framework.FISCA.Presentation.Controls.MsgBox.Show(TeacherTag.Instance.Items.Count.ToString());
            //};

            //CourseTag.Instance.ItemLoaded += delegate
            //{
            //    Framework.FISCA.Presentation.Controls.MsgBox.Show(CourseTag.Instance.Items.Count.ToString());
            //};

            //StudentTag.Instance.SyncAllBackground();
            //ClassTag.Instance.SyncAllBackground();
            //TeacherTag.Instance.SyncAllBackground();
            //CourseTag.Instance.SyncAllBackground();

            //Student.Instance.SelectedListChanged += delegate
            //{
            //    if (Student.Instance.SelectedList.Count > 0)
            //    {
            //        Framework.FISCA.Presentation.Controls.MsgBox.Show(Student.Instance.SelectedList[0].GetTags().Count.ToString());
            //    }
            //};
        }

        private static void Restart_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Restart();
        }

        /// <summary>
        /// 設定畫面選取Count
        /// </summary>
        private static void SelectedListChanged()
        {
            K12.Presentation.NLDPanels.Student.SelectedSourceChanged += delegate
            {
                MotherForm.SetStatusBarMessage("已選取" + K12.Presentation.NLDPanels.Student.SelectedSource.Count + "名學生");
            };

            K12.Presentation.NLDPanels.Class.SelectedSourceChanged += delegate
            {
                MotherForm.SetStatusBarMessage("已選取" + K12.Presentation.NLDPanels.Class.SelectedSource.Count + "個班級");
            };

            K12.Presentation.NLDPanels.Teacher.SelectedSourceChanged += delegate
            {
                MotherForm.SetStatusBarMessage("已選取" + K12.Presentation.NLDPanels.Teacher.SelectedSource.Count + "名教師");
            };

            K12.Presentation.NLDPanels.Course.SelectedSourceChanged += delegate
            {
                MotherForm.SetStatusBarMessage("已選取" + K12.Presentation.NLDPanels.Course.SelectedSource.Count + "個課程");
            };

        }
    }
}
