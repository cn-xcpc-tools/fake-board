using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Board.Services
{
    public class DataService : BackgroundService
    {
        public static DataService Instance { get; private set; }

        private ILogger<DataService> Logger { get; }

        internal readonly ILoggerFactory _loggerFactory;

        private readonly Dictionary<string, DataHolder> _dict;
        public BoardOptions Options { get; }

        public DataHolder this[string name] => _dict.GetValueOrDefault(name) ?? new DataHolder(name, "Not Found", _loggerFactory);

        public DataService(ILoggerFactory loggerFactory, IOptions<BoardOptions> options)
        {
            Logger = loggerFactory.CreateLogger<DataService>();
            _loggerFactory = loggerFactory;
            _dict = new Dictionary<string, DataHolder>();
            Instance = this;
            Fetchers = new List<HdojFetcher>();
            Options = options.Value;
        }

        public bool Have(string s)
        {
            return _dict.ContainsKey(s);
        }

        public void Create(string name, string title)
        {
            _dict.TryAdd(name, new DataHolder(name, title, _loggerFactory));
        }

        public Tenant[] GetTenants()
        {
            return Options.Tenants ?? Array.Empty<Tenant>();
        }

        public readonly List<HdojFetcher> Fetchers;

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            if (Options.Tenants != null)
            {
                var tenants = Options.Tenants;

                foreach (var tenant in tenants)
                {
                    var dataHolder = new DataHolder(tenant.name, tenant.title, _loggerFactory);
                    await dataHolder.StartAsync();
                    _dict.TryAdd(tenant.name, dataHolder);

                    if (!string.IsNullOrEmpty(tenant.hdoj))
                    {
                        dataHolder._hdoj = tenant.hdoj;
                        var st = tenant.hdoj.Split(";");
                        var cid = int.Parse(st[0]);
                        var qot = int.Parse(st[3]);
                        Fetchers.Add(new HdojFetcher(cid, dataHolder, st[1], st[2], qot, _loggerFactory.CreateLogger("HdojFetcher." + tenant.name)));
                    }
                }
            }

            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);

            var tenants = _dict.Values.ToList();
            foreach (var tenant in tenants)
            {
                await tenant.StopAsync();
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    foreach (var item in Fetchers)
                    {
                        await item.Work();                        
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Unknown error.");
                }

                await Task.Delay(15 * 1000, stoppingToken);
            }
        }
    }
}
