using Axiom.Math;
using Core;
using Core.Serial;
using Core.Wobs;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class Server
    {
        private const int WindowsError_AccessDenied = 5;
        private const int WindowsError_OperationAborted = 995;

        private bool _exiting;
        private Stopwatch _updateStopwatch = new Stopwatch();
        private TimeSpan _lastUpdate;

        public void Start()
        {
            var listener = new HttpListener();
            listener.Prefixes.Add("http://*:8080/moolgoss/");
            if (!TryStart(listener)) return;
            var world = new Atom<World>(World.Empty
                .SetWob(new Planet(Guid.NewGuid(), "Earth"))
                .SetWob(new Planet(Guid.NewGuid(), "Jupiteroid"))
                .SetWob(new Station(Guid.NewGuid(), new Vector3(200, 0, 100))));
            var service = new Service(world);
            var marshalledService = Marshal.Get(service);
            var handleRequests = RunRepeatedly(() => HandleRequest(listener, marshalledService));
            var updateWorld = RunRepeatedly(() => UpdateWorld(world));
            Console.WriteLine("Enter 'exit' to stop the server.");
            while (true) if (Console.ReadLine() == "exit") break;
            Stop(listener, handleRequests, updateWorld);
        }

        public void Stop(HttpListener listener, Task handleRequests, Task updateWorld)
        {
            _exiting = true;
            listener.Stop();
            updateWorld.Wait(TimeSpan.FromSeconds(2));
            handleRequests.Wait(TimeSpan.FromSeconds(2));
        }

        private Task RunRepeatedly(Action action)
        {
            return Task.Factory.StartNew(new Action(() =>
            {
                while (!_exiting)
                    try { action(); }
                    catch (Exception e) { Console.WriteLine("[{0:u}] Exception: {1}", DateTime.Now, e); }
            }), TaskCreationOptions.LongRunning);
        }

        private static bool TryStart(HttpListener listener)
        {
            var prefixes = listener.Prefixes;
            try
            {
                listener.Start();
                return true;
            }
            catch (HttpListenerException ex)
            {
                if (ex.ErrorCode != WindowsError_AccessDenied) throw;
                var username = Environment.GetEnvironmentVariable("USERNAME");
                var userdomain = Environment.GetEnvironmentVariable("USERDOMAIN");
                Console.WriteLine("Access limitation. If you're on Windows, you need to run this as Administrator:");
                foreach (var prefix in prefixes)
                    Console.WriteLine("  netsh http add urlacl url={0} user={1}\\{2} listen=yes",
                        prefix, userdomain, username);
                return false;
            }
        }

        private static void HandleRequest(HttpListener listener, IMarshalled marshalledService)
        {
            try
            {
                var context = listener.GetContext();
                var data = new byte[context.Request.ContentLength64];
                context.Request.InputStream.ReadTo(data);
                var callspec = Serialization.Build<MarshalledCall>(data);
                var result = marshalledService.Invoke(callspec.Name, callspec.Args);
                if (result != null)
                    context.Response.Close(Serialization.Break(result), true);
                else
                    context.Response.Close();
            }
            catch (HttpListenerException ex)
            {
                if (ex.ErrorCode != WindowsError_OperationAborted) throw;
                // Terminate peacefully by request.
            }
        }

        private void UpdateWorld(Atom<World> world)
        {
            EnsureUpdateStopwatchInitialized();
            Thread.Sleep(TimeSpan.FromSeconds(1));
            var elapsed = _updateStopwatch.Elapsed;
            var secondsPassed = (float)(elapsed - _lastUpdate).TotalSeconds;
            _lastUpdate = elapsed;
            var ids = world.Value.Wobs.Keys;
            foreach (var id in ids) world.Set(w => TryUpdateWob(w, id, secondsPassed));
        }

        private void EnsureUpdateStopwatchInitialized()
        {
            if (_updateStopwatch.IsRunning) return;
            _lastUpdate = TimeSpan.Zero;
            _updateStopwatch.Restart();
        }

        private static World TryUpdateWob(World world, Guid id, float secondsPassed)
        {
            var wob = world.GetWob<Wob>(id);
            return wob == null ? world : world.SetWob(wob.Update(secondsPassed));
        }
    }
}
