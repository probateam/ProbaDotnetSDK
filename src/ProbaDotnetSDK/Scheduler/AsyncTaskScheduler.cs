using ProbaDotnetSDK.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProbaDotnetSDK.Scheduler
{
    internal class AsyncTaskScheduler
    {
        private ConcurrentQueue<TaskOrder> TaskList { get; }
        private int ConcurrencyLevel { get; }
        private SemaphoreSlim Semaphore { get; }
        private CancellationTokenSource CancellationTokenSource { get; }
        private ProbaHttpClient ProbaClient { get; }
        public List<Exception> Exceptions { get; private set; }
        public bool Initialized { get; }
        private List<(int count, bool success, HttpStatusCode statusCode)> Responses { get; }

        public AsyncTaskScheduler(int concurrencyLevel, CancellationTokenSource cancellationTokenSource, ProbaHttpClient probaClient)
        {
            TaskList = new ConcurrentQueue<TaskOrder>();
            ConcurrencyLevel = concurrencyLevel;
            Semaphore = new SemaphoreSlim(0, concurrencyLevel);
            CancellationTokenSource = cancellationTokenSource;
            ProbaClient = probaClient;
            Exceptions = new List<Exception>();
            Initialized = true;
            Responses = new List<(int count, bool success, HttpStatusCode statusCode)>();
        }

        public void Schedule(TaskOrder order)
        {
            if (!Initialized) throw new InvalidOperationException("You need to initilize the object first.");
            TaskList.Enqueue(order);
            Semaphore.Release();
        }

        private async Task<TaskOrder> DequeueAsync()
        {
            await Semaphore.WaitAsync();

            TaskList.TryDequeue(out var job);

            return job;

        }

        public async void StartAsync()
        {
            if (!Initialized) throw new InvalidOperationException("You need to initilize the object first.");

            while (!CancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    var job = await DequeueAsync();
                    bool sucess;
                    HttpStatusCode statusCode;
                    switch (job.Type)
                    {
                        case TaskType.SendAchievementEvent:
                            (sucess, statusCode) = await ProbaClient.SendAchievementEventAsync(job.Achievement);
                            break;
                        case TaskType.SendAdvertisementEvent:
                            (sucess, statusCode) = await ProbaClient.SendAdvertisementEventAsync(job.Advertisement);
                            break;
                        case TaskType.SendBusinessEvent:
                            (sucess, statusCode) = await ProbaClient.SendBusinessEventAsync(job.Businesse);
                            break;
                        case TaskType.SendContentViewEvent:
                            (sucess, statusCode) = await ProbaClient.SendContentViewEventAsync(job.ContentView);
                            break;
                        case TaskType.SendDesignEvent:
                            (sucess, statusCode) = await ProbaClient.SendDesignEventAsync(job.DesignEvent);
                            break;
                        case TaskType.SendProgressionEvent:
                            (sucess, statusCode) = await ProbaClient.SendProgressionEventAsync(job.Progression);
                            break;
                        case TaskType.SendSocialEvent:
                            (sucess, statusCode) = await ProbaClient.SendSocialEventAsync(job.Social);
                            break;
                        case TaskType.SendTapEvent:
                            (sucess, statusCode) = await ProbaClient.SendTapEventAsync(job.Tap);
                            break;
                        default:
                            (sucess, statusCode) = (true, HttpStatusCode.NotFound);
                            break;
                    }
                    Responses.Add((Responses.Count, sucess, statusCode));
                }
                catch (Exception ex)
                {
                    Exceptions.Add(ex);
                }

            }
        }

    }
}
