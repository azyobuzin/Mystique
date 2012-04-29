﻿using System;
using System.IO;
using System.Text;
using System.Windows;
using Inscribe;
using Inscribe.Common;
using Inscribe.Core;
using Livet;
using System.Linq;


namespace Mystique
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Application.Current.Exit += new ExitEventHandler(Exitting);
            DispatcherHelper.UIDispatcher = Dispatcher;

            //改造が原因でレポート送られちゃったら申し訳ない
            //AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            Initializer.Init();
            UpdateReceiver.StartSchedule();
        }

        void Exitting(object sender, ExitEventArgs e)
        {
            ThreadHelper.HaltThreads();
        }

        //集約エラーハンドラ
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ERROR THROWN:");
            System.Diagnostics.Debug.WriteLine(e.ExceptionObject);
            StringBuilder body = new StringBuilder();
            body.AppendLine("********************************************************************************");
            body.AppendLine(" ERROR TRACE: " + DateTime.Now.ToString());
            body.AppendLine("********************************************************************************");
            body.AppendLine(e.ExceptionObject.ToString());
            body.AppendLine();
            body.AppendLine("MEMORY USAGE:");
            var cp = System.Diagnostics.Process.GetCurrentProcess();
            body.AppendLine("paged:" + cp.PagedMemorySize64 + " / peak-virtual:" + cp.PeakVirtualMemorySize64);
            body.AppendLine(Environment.OSVersion.VersionString + " (is64?" + (IntPtr.Size == 8).ToString() + ")");
            body.AppendLine(Define.GetFormattedVersion() + " @" + Define.ExeFilePath);

            #region Freezable Bug 対策

            var argexcp = e.ExceptionObject as ArgumentException;
            if (argexcp != null && argexcp.Message.Contains("Freezable") && argexcp.ParamName == "context")
            {
                try
                {
                    body.AppendLine("FREEZABLE***************************************************");
                    body.AppendLine("Source:" + argexcp.Source);
                    body.AppendLine("FREEZABLE END************************************************");
                }
                catch { }
            }

            #endregion

            if (Define.IsOperatingSystemSupported)
            {
                var tpath = Path.GetTempFileName();
                using (var sw = new StreamWriter(tpath))
                {
                    sw.WriteLine(body.ToString());
                }
                var apppath = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
                System.Diagnostics.Process.Start(Path.Combine(apppath, Define.FeedbackAppName), tpath);
                Environment.Exit(1);
            }
            else
            {
                // WinXPでβ以上の場合は何も吐かずに落ちる
                if (Define.IsNightlyVersion)
                {
                    var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "krile_trace_" + Path.GetRandomFileName() + ".txt");
                    using (var sw = new StreamWriter(path))
                    {
                        sw.WriteLine(body.ToString());
                    }
                    MessageBox.Show("エラーが発生し、Krileの動作を継続できなくなりました。" + Environment.NewLine +
                            "ご利用のオペレーティングシステムでは自動フィードバックシステムを利用できません。" + Environment.NewLine +
                            "お手数ですが、デスクトップに生成されるエラートレースを@karnoまでお知らせください。",
                            "サポート対象外のOS", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
