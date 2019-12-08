using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProblemSolving.CrossroadProblemFolder
{
    class CrossroadProblem
    {
        /* 11 Задача о перекрестке
         * Организовать бесконфликтное движение на перекрестке двух двухстронних дорог шириной в две полосы каждая 
         * (т.е. реализовать светофор, который будет переключаться, оценивая обстановку на дороге.  
         */
        public delegate void ArrivedHandler(int q);
        public static event ArrivedHandler Arrived;
        Random random = new Random();
        string[] RoadsNames = new string[4] { "Cевер", "Запад", "Юг", "Восток" };
        int[] Roads = new int[4] { 0, 0, 0, 0 };
        ManualResetEvent[] CrossroadsPermits = new ManualResetEvent[4]
            { new ManualResetEvent(true), new ManualResetEvent(false), new ManualResetEvent(true), new ManualResetEvent(false) };
        bool[] TrafficLightStatus = new bool[4] { true, false, true, false};
        string GetDirection()
        {
            Monitor.TryEnter(random, -1);
            int randDirection = random.Next(0, 3);
            Monitor.Exit(random);
            switch (randDirection)
            {
                case 0: { return "налево"; }
                case 1: { return "прямо"; } 
                default: { return "направо"; }
            } 
        }
        void ChangeLight(int[] ways)
        {   
            if (TrafficLightStatus[ways[0]] != true && TrafficLightStatus[ways[1]] != true)
            {
                for (int i = 0; i < TrafficLightStatus.Length; i++)
                {
                    if (i == ways[0] || i == ways[1]) CrossroadsPermits[i].Set();
                    else CrossroadsPermits[i].Reset();
                    TrafficLightStatus[i] = !TrafficLightStatus[i];
                }
                Console.WriteLine($"Светофор переключаеться и теперь машины с {RoadsNames[ways[0]]} и {RoadsNames[ways[1]]} трассы могут ехать");
            } 
        }
        public void Run()
        {
            Arrived += (int way) => { 
                Console.WriteLine($"{RoadsNames[way]} - приехала машина и теперь там {Roads[way]}");
                if (Roads[0] + Roads[2] > (Roads[1] + Roads[3]) * 2) ChangeLight(new int[] { 0, 2 });
                else if (Roads[1] + Roads[3] > (Roads[0] + Roads[2]) * 2) ChangeLight(new int[] { 1, 3 });
                else if (TrafficLightStatus[way]) CrossroadsPermits[way].Set();
            };
            for (int i = 0; i < Roads.Length; i++)
            {
                new Thread((object way) => {
                    while (true)
                    { 
                        Roads[(int)way]++;
                        Arrived?.Invoke((int)way);
                        Monitor.TryEnter(random, -1);
                        int sleepTime = random.Next(200, 1001);
                        Monitor.Exit(random);
                        Thread.Sleep(sleepTime);
                    }
                }).Start(i);
                new Thread((object way) => {
                    while (true)
                    { 
                        CrossroadsPermits[(int)way].WaitOne();
                        if (Roads[(int)way] > 0)
                        {
                            Console.WriteLine($"{RoadsNames[(int)way]} - машина едет {GetDirection()}, осталось {--Roads[(int)way]}");
                            Thread.Sleep(300);
                        }
                        else CrossroadsPermits[(int)way].Reset();   
                    }
                }).Start(i);
            }
        }
    }
}
