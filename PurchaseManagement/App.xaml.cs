﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace PurchaseManagement
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static bool isRestart;
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += onCurrentDomainUnhandledException;
            DispatcherUnhandledException += onDispatcherUnhandledException;
            PurchaseManagement.Properties.Resources.Culture = new System.Globalization.CultureInfo(PurchaseManagement.Properties.Settings.Default.NewestLang);
            PurchaseManagement.Properties.Settings.Default.UsingLang = PurchaseManagement.Properties.Settings.Default.NewestLang;
            if (PurchaseManagement.Properties.Settings.Default.UsingLang == "zh-CN")
            {
                var fontFamily = new System.Windows.Media.FontFamily(PurchaseManagement.Properties.Settings.Default.FontSetting);
                System.Windows.Documents.TextElement.FontFamilyProperty.OverrideMetadata(typeof(System.Windows.Documents.TextElement), new FrameworkPropertyMetadata(fontFamily));
                System.Windows.Controls.TextBlock.FontFamilyProperty.OverrideMetadata(typeof(System.Windows.Controls.TextBlock), new FrameworkPropertyMetadata(fontFamily));
            }
        }
        
        public static void Restart()
        {
            isRestart = true;
            Current.Shutdown();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            if (isRestart && !string.IsNullOrEmpty(App.ResourceAssembly.Location))
            {
                Process.Start(App.ResourceAssembly.Location);
            }
        }

        #region Dispose on exception

        private void disposeOnUnhandledException()
        {
        }

        #endregion

        #region Template application config

        #region Unexpected exception handler

        private void onCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = (Exception)e.ExceptionObject;
#if debug
            Debug.WriteLine(sender.ToString() + "\n" + e.ExceptionObject);
#endif
            logExceptionInfo(exception, "AppDomain.CurrentDomain.UnhandledException");
        }

        private void onDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
#if debug
            Debug.WriteLine(e.Exception.TargetSite);
#endif
            logExceptionInfo(e.Exception, "AppDomain.DispatcherUnhandledException");
        }

        private void logExceptionInfo(Exception exception, string typeName = "Undefined Exception")
        {
            disposeOnUnhandledException();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("\n---------------------\n{0}\n", typeName);
            sb.AppendFormat("\n\n0.TargetSite\n{0}\n\n1.StackTrace\n{1}\n\n2.Source\n{2}\n\n3.Message\n{3}\n\n4.HResult\n{4}\n",
                exception.TargetSite,
                exception.StackTrace,
                exception.Source,
                exception.Message,
                exception.HResult);
            if (exception.InnerException != null)
            {
                sb.Append("\n---------------------\nInnerException\n");
                sb.AppendFormat("\n\n5.0.TargetSite\n{0}\n\n5.1.StackTrace\n{1}\n\n5.2.Source\n{2}\n\n5.3.Message\n{3}\n\n5.4.HResult\n{4}\n",
                    exception.InnerException.TargetSite,
                    exception.InnerException.StackTrace,
                    exception.InnerException.Source,
                    exception.InnerException.Message,
                    exception.InnerException.HResult);
            }


            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string log = Path.GetDirectoryName(path) + "\\log.txt";
            using (StreamWriter sw = new StreamWriter(log, true, Encoding.UTF8))
            {
                sw.Write(string.Format("---------------------\n\n{0}\n{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"), sb.ToString()));
            }
        }

        #endregion
        
        #region One instance support

        static App()
        {
            // Ensure other instances of this application are not running.
            singleInstanceWatcher = new Semaphore(
                0, // Initial count.
                1, // Maximum count.
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, out createdNew);

            if (createdNew)
            {
                // This thread created the kernel object so no other instance
                // of this application must be running.
            }
            else
            {
                // This thread opened an existing kernel object with the same
                // string name; another instance of this app must be running now.

                // Gets a new System.Diagnostics.Process component and the
                // associates it with currently active process.
                Process current = Process.GetCurrentProcess();

                // Enumerate through all the process resources on the share
                // local computer that the specified process name.
                foreach (Process process in Process.GetProcessesByName(current.ProcessName))
                {
                    if (process.Id != current.Id)
                    {
                        NativeMethods.SetForegroundWindow(process.MainWindowHandle);
                        NativeMethods.ShowWindow(process.MainWindowHandle, WindowShowStyle.Restore);
                        // Terminate this process and gives the underlying operating 
                        // system the specified exit code.
                        Environment.Exit(-2);
                        break;
                    }
                }
                Debug.WriteLine("Unknow error, process less than two, but 'createdNew' is false.");
            }
        }

        private static class NativeMethods
        {
            /// <summary>
            /// Brings the thread that created the specified window into the
            /// foreground and activates the window. Keyboard input is directed
            /// to the window, and various visual cues are changed for the user.
            /// The system assigns a slightly higher priority to the thread that
            /// created the foreground window than it does to other threads.
            /// </summary>
            /// <param name="hWnd">A handle to the window that should be
            /// activated and brought to the foreground.
            /// </param>
            /// <returns>If the window was brought to the foreground, the
            /// return value is nonzero. </returns>
            [DllImport("user32.dll")]
            internal static extern bool SetForegroundWindow(IntPtr hWnd);

            /// <summary>Shows a Window</summary>
            /// <remarks>
            /// <para>To perform certain special effects when showing or hiding a
            /// window, use AnimateWindow.</para>
            /// <para>The first time an application calls ShowWindow, it should use
            /// the WinMain function's nCmdShow parameter as its nCmdShow ..
            /// Subsequent calls to ShowWindow must use one of the values in the
            /// given list, instead of the one specified by the WinMain function's
            /// nCmdShow parameter.</para>
            /// <para>As noted in the discussion of the nCmdShow parameter, the
            /// nCmdShow value is ignored in the first call to ShowWindow if the
            /// program that launched the application specifies startup information
            /// in the structure. In this case, ShowWindow uses the information
            /// specified in the STARTUPINFO structure to show the window. On
            /// subsequent calls, the application must call ShowWindow with ..
            /// set to SW_SHOWDEFAULT to use the startup information provided by ..
            /// program that launched the application. This behavior is designed ..
            /// the following situations: </para>
            /// <list type="">
            ///    <item>Applications create their main window by calling ..
            ///    with the WS_VISIBLE flag set. </item>
            ///    <item>Applications create their main window by calling ..
            ///    with the WS_VISIBLE flag cleared, and later call ShowWindow ..
            ///    SW_SHOW flag set to make it visible.</item>
            /// </list></remarks>
            /// <param name="hWnd">Handle to the window.</param>
            /// <param name="nCmdShow">Specifies how the window is to be shown.
            /// This parameter is ignored the first time an application calls
            /// ShowWindow, if the program that launched the application provides a
            /// STARTUPINFO structure. Otherwise, the first time ShowWindow .. ,
            /// the value should be the value obtained by the WinMain function ..
            /// nCmdShow parameter. In subsequent calls, this parameter ..
            /// the WindowShowStyle members.</param>
            /// <returns>
            /// If the window was previously visible, the return value is nonzero.
            /// If the window was previously hidden, the return value is zero.
            /// </returns>
            [DllImport("user32.dll")]
            internal static extern bool ShowWindow(IntPtr hWnd,
                WindowShowStyle nCmdShow);
        }

        /// <summary>
        /// Enumeration of the different ways of showing a window.</summary>
        internal enum WindowShowStyle : uint
        {
            /// <summary>Hides the window and activates another window.</summary>
            /// <remarks>See SW_HIDE</remarks>
            Hide = 0,
            /// <summary>Activates and displays a window. If the window ..
            /// or maximized, the system restores it to its original size and
            /// position. An application should specify this flag when displaying
            /// the window for the first time.</summary>
            /// <remarks>See SW_SHOWNORMAL</remarks>
            ShowNormal = 1,
            /// <summary>Activates the window and displays it ..</summary>
            /// <remarks>See SW_SHOWMINIMIZED</remarks>
            ShowMinimized = 2,
            /// <summary>Activates the window and displays it ..</summary>
            /// <remarks>See SW_SHOWMAXIMIZED</remarks>
            ShowMaximized = 3,
            /// <summary>Maximizes the specified window.</summary>
            /// <remarks>See SW_MAXIMIZE</remarks>
            Maximize = 3,
            /// <summary>Displays a window in its most recent size and position.
            /// This value is similar to "ShowNormal", except the window is not
            /// actived.</summary>
            /// <remarks>See SW_SHOWNOACTIVATE</remarks>
            ShowNormalNoActivate = 4,
            /// <summary>Activates the window and displays it in its current size
            /// and position.</summary>
            /// <remarks>See SW_SHOW</remarks>
            Show = 5,
            /// <summary>Minimizes the specified window and activates the next
            /// top-level window in the Z order.</summary>
            /// <remarks>See SW_MINIMIZE</remarks>
            Minimize = 6,
            /// <summary>Displays the window as a minimized window. This value is
            /// similar to "ShowMinimized", except the window ..</summary>
            /// <remarks>See SW_SHOWMINNOACTIVE</remarks>
            ShowMinNoActivate = 7,
            /// <summary>Displays the window in its current size and position. This
            /// value is similar to "Show", except the window ..</summary>
            /// <remarks>See SW_SHOWNA</remarks>
            ShowNoActivate = 8,
            /// <summary>Activates and displays the window. If the window is
            /// minimized or maximized, the system restores it to its original size
            /// and position. An application should specify this flag ..
            /// a minimized window.</summary>
            /// <remarks>See SW_RESTORE</remarks>
            Restore = 9,
            /// <summary>Sets the show state based on the SW_ value specified ..
            /// STARTUPINFO structure passed to the CreateProcess function by the
            /// program that started the application.</summary>
            /// <remarks>See SW_SHOWDEFAULT</remarks>
            ShowDefault = 10,
            /// <summary>Windows 2000/XP: Minimizes a window, even if the thread
            /// that owns the window is hung. This flag should only be used when
            /// minimizing windows from a different thread.</summary>
            /// <remarks>See SW_FORCEMINIMIZE</remarks>
            ForceMinimized = 11


        }

        private static readonly Semaphore singleInstanceWatcher;
        private static readonly bool createdNew;
        #endregion

        #endregion
    }
}
