using System;

namespace SimpleDesktop
{
    class Application : System.Windows.Application, ClearSight.Core.IApplication
    {
        ClearSight.Core.Window.WPFWindow window;

        public Application()
        {
            window = new ClearSight.Core.Window.WPFWindow(1024, 786);
        }

        public bool IsRunning
        {
            get
            {
                return !((ClearSight.Core.Window.IWindow)window).Closed;
            }
        }

        public void AfterEngineInit()
        {
        }

        public void AfterEngineShutdown()
        {
        }

        public void BeforeEngineShutdown()
        {
        }

        new public void Run()
        {
            Run(window);
        }

        [STAThread]
        static void Main(string[] args)
        {
            ClearSight.Core.Engine.RunApplication<Application>();
        }
    }
}
