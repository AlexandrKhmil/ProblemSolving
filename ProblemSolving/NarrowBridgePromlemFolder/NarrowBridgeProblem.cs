using System;
using System.Collections.Generic; 
using System.Threading; 
namespace ProblemSolving.NarrowBridgePromlemFolder
{
    class NarrowBridgeProblem
    {
        /* 10 Задача об узком мосте
         * К узкому мосту приезжают машины с севера и с юга. Машины движущиеся в одном направлении
         * могут переезжать мост одновременно, противоположном - нет. Разработйте решения для системы
         * переезда моста. Решение может быть несправедливым. 
         */ 
        int CarCount = 0;
        object CarCountLocker = new object();
        Random RandomArrive = new Random();
        IDictionary<string, ManualResetEvent> Roads = new Dictionary<string, ManualResetEvent>(2)
            { { "North", new ManualResetEvent(true) }, { "South", new ManualResetEvent(false) } }; 
        void Pass(object direction)
        { 
            if(Roads[(string)direction].WaitOne()) 
                Console.WriteLine($"{Thread.CurrentThread.Name} - свободно проезжает"); 
            else
            { 
                Console.WriteLine($"{Thread.CurrentThread.Name} - остаеться ждать");
                Roads[(string)direction].WaitOne(-1);
                Console.WriteLine($"{Thread.CurrentThread.Name} - после ожидания проезжает");
            }
        }
        void TrafficLightFunctionality()
        { 
            Timer timer = new Timer(new TimerCallback((object o) => {
                foreach (string key in Roads.Keys)
                    if (Roads[key].WaitOne(0)) Roads[key].Reset();
                    else Roads[key].Set(); 
                Console.WriteLine("Светофор - мост меняет направление");
            }), 0, 0, 2000); 
        }
        public void Run()
        {
            for (int i = 0; i < 2; i++) 
                new Thread((object way) => { 
                    while (true)
                    {
                        Monitor.TryEnter(CarCountLocker, -1);
                        Console.WriteLine($"С {(((int)way == 0) ? "Севера" : "Юга")} приехала Машина #{++CarCount}");
                        new Thread(Pass) { Name = $"Машина #{CarCount}" }.Start((((int)way == 0) ? "North" : "South"));
                        Monitor.Exit(CarCountLocker); 
                        Monitor.TryEnter(RandomArrive, -1);
                        int sleepTime = RandomArrive.Next(200, 1001);
                        Monitor.Exit(RandomArrive);
                        Thread.Sleep(sleepTime);
                    } 
                }).Start(i); 
            new Thread(TrafficLightFunctionality).Start();
         }
    }
}
