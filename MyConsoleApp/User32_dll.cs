using System.Runtime.InteropServices;

namespace MyConsoleApp;

partial class User32_dll
{
    [LibraryImport("user32.dll")]
    internal static partial short GetAsyncKeyState(int vKey);
}
