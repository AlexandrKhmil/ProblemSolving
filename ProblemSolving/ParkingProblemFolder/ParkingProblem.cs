using System; 
using System.Threading;  
namespace ProblemSolving.ParkingProblemFolder
{
    class ParkingProblem
    {
        /* 12 Задача об автостоянке 
         * Реулизовать модель работы автостоянки, используя многопоточность (машина двигается по автостоянке, пока
         * не обрануживает первое пустое место, становится на него, стоит произвольное время и выезжает со стоянки и 
         * возвращается на нее через некоторое время; если мест на стоянке нет, машина выезжает).
         */ 
        Mutex[] ParkingPlaces { get; set; } = new Mutex[5] 
            { new Mutex(), new Mutex(), new Mutex(), new Mutex(), new Mutex()}; 
        void Activity()
        {
            while (true)
            { 
                Console.WriteLine($"{Thread.CurrentThread.Name}\t- заезжает на стоянку");
                Thread.Sleep(200);
                for (int position = 0; position < ParkingPlaces.Length; position++)
                {
                    if (ParkingPlaces[position].WaitOne(0))
                    {
                        Console.WriteLine($"{Thread.CurrentThread.Name}\t- видит что место {position + 1} свободно и занимает его");
                        Thread.Sleep(new Random().Next(1000, 5001));
                        Console.WriteLine($"{Thread.CurrentThread.Name}\t- уезжает со стоянки освобождая место {position + 1}");
                        ParkingPlaces[position].ReleaseMutex();
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"{Thread.CurrentThread.Name}\t- видит что место {position + 1} занято" +
                            ((position != ParkingPlaces.Length - 1) ? " и едет дальше" : $" и огорченный уезжает в город"));
                        Thread.Sleep(200);
                    }
                } 
                Thread.Sleep(new Random().Next(3000, 6001));
            } 
        } 
        public void Run()
        {  
            for (int i = 1; i < 11; i++)
            {
                new Thread(Activity) { Name = $"Автомобиль #{i}" }.Start();
                Thread.Sleep(new Random().Next(400, 601));
            }
        }
    }
}
