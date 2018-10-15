using System.Collections.Generic;
using System.Threading;
using UnityEngine.Events;
using UnityUtilities;

namespace Minecraft.Scripts.Utility.Multithreading {
    public abstract class JobSystem<W, T> where W : AbstractWorker<T> where T : Job {
        private W FindWorker() {
            var w = workers[lastWorker++];
            if (lastWorker >= TotalWorkers) {
                lastWorker %= TotalWorkers;
            }

            return w;
        }

        private byte lastWorker;
        private W[] workers;
        public byte TotalWorkers = 2;

        public void Enqueue(T job) {
            FindWorker().Enqueue(job);
        }

        protected void InitializeWorkers() {
            workers = new W[TotalWorkers];
            for (byte i = 0; i < TotalWorkers; i++) {
                workers[i] = InstantiateWorker(i);
            }
        }

        protected abstract W InstantiateWorker(byte b);

        public void TerminateWorkers() {
            foreach (var worker in workers) {
                worker.Stop();
            }
        }
    }


    public abstract class Job {
        private readonly UnityAction startedCallback;
        private readonly UnityAction finishedCallback;

        public Job(UnityAction startedCallback, UnityAction finishedCallback) {
            this.startedCallback = startedCallback;
            this.finishedCallback = finishedCallback;
        }

        public void StartedCallback() {
            startedCallback?.Invoke();
        }


        public void FinishedCallback() {
            finishedCallback?.Invoke();
        }
    }


    public abstract class AbstractWorker<T> where T : Job {
        private readonly AutoResetEvent handle = new AutoResetEvent(false);
        private readonly Queue<T> jobQueue = new Queue<T>();
        private readonly byte id;

        public AbstractWorker(byte workerId) {
            id = workerId;
            var thread = new Thread(Process) {
                Name = ToString()
            };
            thread.Start();
        }

        public void Enqueue(T job) {
            jobQueue.Enqueue(job);
            handle.Set();
        }

        public sealed override string ToString() => $"Worker #{id}";

        private void Process() {
            while (!shouldStop) {
                handle.WaitOne();
                while (!jobQueue.IsEmpty()) {
                    var job = jobQueue.Dequeue();
                    job.StartedCallback();
                    Execute(job);
                    job.FinishedCallback();
                }
            }
        }

        protected abstract void Execute(T job);

        private bool shouldStop;

        public void Stop() {
            shouldStop = true;
        }
    }
}