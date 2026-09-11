// COM 5113 Pathfinding Assessment

using System;
using System.Windows.Forms;

namespace PathFinderAssessment
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Configure the Windows Forms application.
            ApplicationConfiguration.Initialize();

            // Start the main graphical interface.
            Application.Run(
                new MainForm());
        }
    }
}