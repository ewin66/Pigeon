﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Pigeon.Addresses;
using Pigeon.NetMQ;
using Pigeon.NetMQ.Publishers;
using Pigeon.Sandbox.Contracts;
using Pigeon.Unity;
using Unity;

namespace Pigeon.Sandbox.Programs
{
    public class Publisher
    {
        private readonly Router router;

        private Random random = new Random();
        private double price = 100.0;
        private double drift = 0.00001;
        private double vol = 0.05;
        private int sent = 0;

        private bool stopping = false;
        private Thread publisherLoop;


        public Publisher()
        {
            router = UnityBuilder.Named("Publisher")
                                 .WithTransport<NetMQTransport>(t => t.WithPublisher(TcpAddress.Wildcard(5556)))
                                 .Build();
        }

        private void Publish(object sender, ElapsedEventArgs e)
        {
            price = price + drift + (0.5 - random.NextDouble()) * vol;
            var observation = new Observation("AAPL", price);
            router.Publish(observation);
            sent += 1;

            if (sent % 1000 == 0)
                Console.WriteLine($"Publishing: {observation}");
        }

        private void PublishLoop()
        {
            while (!stopping)
            {
                price = price + drift + (0.5 - random.NextDouble()) * vol;
                var observation = new Observation("AAPL", price);
                router.Publish(observation);
                sent += 1;
            }
        }

        public void Start()
        {
            router.Start();
            publisherLoop = new Thread(new ThreadStart(PublishLoop));
            var stopwatch = Stopwatch.StartNew();
            publisherLoop.Start();

            Console.WriteLine("Press enter to stop the publisher");
            Console.ReadLine();

            stopping = true;
            stopwatch.Stop();
            publisherLoop.Join();
            router.Stop();
            
            Console.WriteLine($"{sent:N0} sent in {stopwatch.ElapsedMilliseconds:N0}ms, {((double)sent / (double)stopwatch.ElapsedMilliseconds)*1000:N3} per second");
        }


        public static void Run()
        {
            var publisher = new Publisher();
            publisher.Start();
        }
    }
}
