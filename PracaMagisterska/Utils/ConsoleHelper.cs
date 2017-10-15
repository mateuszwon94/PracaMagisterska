using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PracaMagisterska.WPF.Utils {
    /// <summary>
    /// Helper class for handle console.
    /// </summary>
    public static class ConsoleHelper {
        #region Public

        /// <summary>
        /// Static constructor.
        /// Gets consol handle and disable exit button to prevent app killing.
        /// </summary>
        static ConsoleHelper() {
            console_ = GetConsoleWindow();
            IntPtr exitButton = GetSystemMenu(console_, false);
            if ( exitButton != IntPtr.Zero )
                DeleteMenu(exitButton, SC_CLOSE, MF_BYCOMMAND);
        }

        /// <summary>
        /// Method, which allows user to show Console and define its state.
        /// </summary>
        /// <param name="state">Initial state of console</param>
        public static void Show(ConsoleState state = ConsoleState.Show) {
            Console.Clear();
            State = state;
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Hide console.
        /// </summary>
        public static void Hide() {
            Console.ForegroundColor = ConsoleColor.White;
            State = ConsoleState.Hide;
        }

        #endregion Public

        #region Static fields

        /// <summary>
        /// Current console state.
        /// </summary>
        public static ConsoleState State {
            get => state_;
            set {
                ShowWindow(console_, (int)value);
                state_ = value;
            }
        }

        /// <summary>
        /// Returns <value>true</value> if console is visible, elsewhere return <value>false</value>.
        /// </summary>
        public static bool IsVisible => State != ConsoleState.Hide;

        #endregion Static fields
        
        #region Private members

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        private static extern IntPtr DeleteMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        private const uint SC_CLOSE = 0xF060;
        private const uint MF_BYCOMMAND = (uint)0x00000000L;

        private static readonly IntPtr console_;
        private static ConsoleState state_;

        #endregion Private members

        public enum ConsoleState {
            Hide = 0,             //Hides the window and activates another window.
            ShowNormal = 1,       //Activates and displays a window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when displaying the window for the first time.
            ShowMinimized = 2,    //Activates the window and displays it as a minimized window.
            Maximize = 3,         //Maximizes the specified window.
            ShowMaximized = 3,    //Activates the window and displays it as a maximized window.
            ShowNoActivate = 4,   //Displays a window in its most recent size and position. This value is similar to SW_SHOWNORMAL, except that the window is not activated.
            Show = 5,             //Activates the window and displays it in its current size and position.
            Minimize = 6,         //Minimizes the specified window and activates the next top-level window in the Z order.
            ShowMinNoActive = 7,  //Displays the window as a minimized window. This value is similar to SW_SHOWMINIMIZED, except the window is not activated.
            ShowNa = 8,           //Displays the window in its current size and position. This value is similar to SW_SHOW, except that the window is not activated.
            Restore = 9,          //Activates and displays the window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when restoring a minimized window.
            ShowDefault = 10,     //Sets the show state based on the SW_ value specified in the STARTUPINFO structure passed to the CreateProcess function by the program that started the application.
            ForceMinimize = 11,   //Minimizes a window, even if the thread that owns the window is not responding. This flag should only be used when minimizing windows from a different thread.
        }
    }
}
