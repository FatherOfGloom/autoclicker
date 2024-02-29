using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents.DocumentStructures;


namespace Autoclicker
{
    internal class UserAPI
    {
        public UserAPI()
        {
        }
        public Point MousePosition
        {
            get
            {
                var w32MousePosition = new Win32Point();
                GetCursorPos(ref w32MousePosition);

                return new Point(w32MousePosition.X, w32MousePosition.Y);
            }
        }
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(ref Win32Point pt);
        [StructLayout(LayoutKind.Sequential)]
        private struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwdata, int dwextrainfo);
        [DllImport("user32.dll")]
        static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, int vk);
        [DllImport("user32.dll")]
        static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        private enum MouseEventFlags
        {
            LeftDown = 2,
            LeftUp = 4,
        }
        private enum KeyModifier
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            WinKey = 8
        }
        public event EventHandler? KeyPressed;
        public void BeginRegisterHotkeyPressed(char key, CancellationToken token)
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if (IsKeyPressed(key))
                    {
                        KeyPressed?.Invoke(this, EventArgs.Empty);
                    }
                }
            });
        }
        public bool IsKeyPressed(char key)
        {
            throw new NotImplementedException();
        }
        public void ClickLeftMouseButton(Point point)
        {
            mouse_event((int)MouseEventFlags.LeftDown, (int)point.X, (int)point.Y, 0, 0);
            mouse_event((int)MouseEventFlags.LeftUp, (int)point.X, (int)point.Y, 0, 0);
        }
    }
}

