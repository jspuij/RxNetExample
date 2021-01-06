using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace RxNetExample.Pages
{
    public sealed partial class Index : IDisposable
    {
        /// <summary>
        /// Random number generator.
        /// </summary>
        private static readonly Random rng = new Random();

        /// <summary>
        /// The subject that will emit event. It's static, it's preserved between navigation.
        /// </summary>
        private static readonly ReplaySubject<(int, int)> subject = new ReplaySubject<(int, int)>(100);

        /// <summary>
        /// Event counter.
        /// </summary>
        private static int eventCounter = 0;

        /// <summary>
        /// The timer that randomly emits integers.
        /// </summary>
        private readonly Timer randomEmitTimer;

        /// <summary>
        /// Subscription 
        /// </summary>
        private IDisposable subscription;


        /// <summary>
        /// Screen buffer.
        /// </summary>
        private IList<(int, int)> screenbuffer;



        public Index()
        {
            randomEmitTimer = new Timer(this.Tick, null, rng.Next(0, 200), rng.Next(10, 200));
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                subscription = subject
                    // Per 10 items or per second updating
                    .Buffer(TimeSpan.FromSeconds(1), 10)
                    .Subscribe(x => this.InvokeAsync(() =>
                    {
                        screenbuffer = x;
                        this.StateHasChanged();
                    }));
            }
        }

        /// <summary>
        /// Tick method. Emits a random value and sets a new period.
        /// </summary>
        /// <param name="o">Some state.</param>
        private void Tick (object o)
        {
            // send the next values to the replay subject.
            subject.OnNext((++eventCounter, rng.Next(0, 100)));
            randomEmitTimer.Change(rng.Next(0, 200), rng.Next(10, 200));
        }

        /// <summary>
        /// Clean up.
        /// </summary>
        public void Dispose()
        {
            randomEmitTimer.Dispose();
            subscription?.Dispose();
        }
    }
}
