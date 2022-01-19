using System.ServiceProcess;
using NLog;

namespace HoneySpotService
{
    public partial class HoneySpotService : ServiceBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly HoneySpotListenerSRV s;
        public HoneySpotService()
        {
            InitializeComponent();
            s = new HoneySpotListenerSRV();
        }

        protected override void OnStart(string[] args)
        {
            Logger.Info("Start event");
            s.Start();
        }

        protected override void OnStop()
        {
            Logger.Info("Stop event");
            s.Stop();
        }

        protected override void OnShutdown()
        {
            Logger.Info("Windows is going shutdown");
            Stop();
        }


        public void Start()
        {
            OnStart(null);
        }
    }
}
