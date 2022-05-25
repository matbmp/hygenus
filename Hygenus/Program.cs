using System;

namespace Hygenus
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Hygenus())
                game.Run();
        }
    }
}
