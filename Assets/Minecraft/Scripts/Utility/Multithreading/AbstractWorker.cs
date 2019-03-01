using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Random = System.Random;
using UnityEngine.Events;
using UnityUtilities;

namespace Minecraft.Scripts.Utility.Multithreading {
    public abstract class JobSystem<W, T> where W : AbstractWorker<T> where T : Job<T> {
        private W FindWorker() {
#if UNITY_EDITOR
            if (workers == null) {
                InitializeWorkers();
            }
#endif
            var w = workers[lastWorker++];
            if (lastWorker >= TotalWorkers) {
                lastWorker %= TotalWorkers;
            }

            return w;
        }

        private byte lastWorker;
        private W[] workers;
        public byte TotalWorkers = 2;

        public IEnumerable<W> Workers => workers;

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

    public enum JobState {
        /// <summary>
        /// Not queued for processing 
        /// </summary>
        Idle,

        /// <summary>
        /// Queued and waiting to be picked up
        /// </summary>
        Waiting,

        /// <summary>
        /// Currently running
        /// </summary>
        Active,

        /// <summary>
        /// Finished
        /// </summary>
        Finished
    }


    public abstract class Job<T> where T : Job<T> {
        private readonly UnityAction<T, JobState> onJobUpdated;
        private JobState currentState;

        protected Job(UnityAction<T, JobState> onJobUpdated = null) {
            this.onJobUpdated = onJobUpdated;
            currentState = JobState.Idle;
        }

        public void UpdateStatus(JobState newState) {
            currentState = newState;
            onJobUpdated?.Invoke((T) this, newState);
        }
    }


    public abstract class AbstractWorker<T> where T : Job<T> {
        private readonly AutoResetEvent handle = new AutoResetEvent(false);
        protected readonly List<T> jobQueue = new List<T>();
        private readonly byte id;
        private T currentJob;

        public T CurrentJob => currentJob;

        public AbstractWorker(byte workerId) {
            id = workerId;
            new Thread(Process) {
                Name = ToString()
            }.Start();
        }

        private static readonly Random internalRandom = new Random();

        protected virtual T Dequeue() {
            var index = internalRandom.Next(0, jobQueue.Count);
            return jobQueue[index];
        }

        public void Enqueue(T job) {
            jobQueue.Add(job);
            job.UpdateStatus(JobState.Waiting);
            handle.Set();
        }

        public sealed override string ToString() => $"{GetType().Name} #{id}";

        private void Process() {
            while (!shouldStop) {
                handle.WaitOne();
                while (!jobQueue.IsEmpty()) {
                    currentJob = Dequeue();
                    jobQueue.Remove(currentJob);
                    if (currentJob == null) {
                        continue;
                    }

                    Debug.Log("Worker " + this + " running job " + currentJob);
                    currentJob.UpdateStatus(JobState.Active);
                    Execute(currentJob);
                    currentJob.UpdateStatus(JobState.Finished);
                    currentJob = null;
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