using System;
using System.Runtime.InteropServices;

namespace PracaMagisterska.WPF.Utils {
    /// <summary>
    /// Helper class for handle console.
    /// </summary>
    public class ConsoleHelper {
        #region Public

        /// <summary>
        /// Static constructor.
        /// Gets consol handle and disable exit button to prevent app killing.
        /// </summary>
        static ConsoleHelper() {
            console_ = GetConsoleWindow();
            IntPtr systemMenu = GetSystemMenu(console_, false);
            if ( systemMenu != IntPtr.Zero )
                DeleteMenu(systemMenu, SC_CLOSE, MF_BYCOMMAND);
        }

        /// <summary>
        /// Writes colored text onto Standard Output
        /// </summary>
        /// <param name="text">Text to write</param>
        /// <param name="color">Color of text</param>
        public static void WriteColor(string text, ConsoleColor color = ConsoleColor.White) {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Writes colored text onto Standard Output and add new line
        /// </summary>
        /// <param name="text">Text to write</param>
        /// <param name="color">Color of text</param>
        public static void WriteLineColor(string text, ConsoleColor color = ConsoleColor.White) {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Method, which allows user to show Console and define its state.
        /// </summary>
        /// <param name="state">Initial state of console</param>
        public static void ShowConsole(ConsoleState state = ConsoleState.Show, bool clear = true) {
            if ( clear ) Console.Clear();

            State                   = state;
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Hide console.
        /// </summary>
        public static void HideConsole() {
            Console.ForegroundColor = ConsoleColor.White;
            State                   = ConsoleState.Hide;
        }

        #endregion Public

        #region Static fields

        /// <summary>
        /// Returns <value>true</value> if console is visible, elsewhere return <value>false</value>.
        /// </summary>
        public static bool IsVisible => State != ConsoleState.Hide;

        #endregion Static fields

        #region Private members

        /// <summary>
        /// Windows build in function.
        /// Retrieves the window handle used by the console associated with the calling process.
        /// </summary>
        /// <returns>The return value is a handle to the window used by the console associated with the calling process or <see cref="IntPtr.Zero"/> if there is no such associated console.</returns>
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        /// <summary>
        /// Windows build in function.
        /// Sets the specified window's show state.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <param name="nCmdShow">Controls how the window is to be shown.</param>
        /// <returns>If the window was previously visible, the return value is nonzero.
        /// If the window was previously hidden, the return value is zero. </returns>
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        /// <summary>
        /// Windows build in function.
        /// Enables the application to access the window menu (also known as the system menu or the control menu) for copying and modifying. 
        /// </summary>
        /// <param name="hWnd">A handle to the window that will own a copy of the window menu</param>
        /// <param name="bRevert">The action to be taken. 
        /// If this parameter is <value>false</value>, GetSystemMenu returns a handle to the copy of the window menu currently in use.
        /// The copy is initially identical to the window menu, but it can be modified. 
        /// If this parameter is <value>true</value>, GetSystemMenu resets the window menu back to the default state.
        /// The previous window menu, if any, is destroyed</param>
        /// <returns>If the bRevert parameter is <value>false</value>, the return value is a handle to a copy of the window menu.
        /// If the bRevert parameter is <value>true</value>, the return value is <see cref="IntPtr.Zero"/></returns>
        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        /// <summary>
        /// Windows build in function.
        /// Deletes an item from the specified menu.
        /// If the menu item opens a menu or submenu, this function destroys the handle to the menu or submenu and frees the memory used by the menu or submenu.
        /// </summary>
        /// <param name="hMenu">A handle to the menu to be changed.</param>
        /// <param name="uPosition">The menu item to be deleted, as determined by the uFlags parameter.</param>
        /// <param name="uFlags">ndicates how the uPosition parameter is interpreted.
        /// This parameter must be one of the following values.</param>
        /// <returns>If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero. </returns>
        [DllImport("user32.dll")]
        private static extern IntPtr DeleteMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        /// <summary>
        /// Current console state.
        /// </summary>
        private static ConsoleState State {
            get => state_;
            set {
                ShowWindow(console_, (int)value);
                state_ = value;
            }
        }

        /// <summary>
        /// Position of close button on system menu.
        /// </summary>
        private const uint SC_CLOSE = 0xF060;

        /// <summary>
        /// Default flag used by DeleteMenu function.
        /// </summary>
        private const uint MF_BYCOMMAND = (uint)0x00000000L;

        /// <summary>
        /// A handle to console window.
        /// </summary>
        private static readonly IntPtr console_;

        /// <summary>
        /// Current state of a console window.
        /// </summary>
        private static ConsoleState state_;

        #endregion Private members

        /// <summary>
        /// Possible states of a console window
        /// </summary>
        public enum ConsoleState {
            Hide            = 0,  // Hides the window and activates another window.
            ShowNormal      = 1,  // Activates and displays a window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when displaying the window for the first time.
            ShowMinimized   = 2,  // Activates the window and displays it as a minimized window.
            Maximize        = 3,  // Maximizes the specified window.
            ShowMaximized   = 3,  // Activates the window and displays it as a maximized window.
            ShowNoActivate  = 4,  // Displays a window in its most recent size and position. This value is similar to SW_SHOWNORMAL, except that the window is not activated.
            Show            = 5,  // Activates the window and displays it in its current size and position.
            Minimize        = 6,  // Minimizes the specified window and activates the next top-level window in the Z order.
            ShowMinNoActive = 7,  // Displays the window as a minimized window. This value is similar to SW_SHOWMINIMIZED, except the window is not activated.
            ShowNa          = 8,  // Displays the window in its current size and position. This value is similar to SW_SHOW, except that the window is not activated.
            Restore         = 9,  // Activates and displays the window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when restoring a minimized window.
            ShowDefault     = 10, // Sets the show state based on the SW_ value specified in the STARTUPINFO structure passed to the CreateProcess function by the program that started the application.
            ForceMinimize   = 11, // Minimizes a window, even if the thread that owns the window is not responding. This flag should only be used when minimizing windows from a different thread.
        }
    }
}