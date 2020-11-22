using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Examen
{
    class Program
    {
        string path = "ganadores.txt";
        int pasosDadosLiebre = 0;
        int pasosDadosTortuga = 0;
        const int meta = 25;
        private static object l = new object();
        public bool ganador = false;
        static Random rand = new Random();
        string ganoHoy = "";
        static void Main(string[] args)
        {
            Program p = new Program();
            p.LecturaArchivo();

            Console.WriteLine("Y alla van!");

            Thread tortu = new Thread(p.correTortuga);
            Thread liebre = new Thread(p.correLiebre);

            liebre.Start();
            tortu.Start();

            liebre.Join();

            p.ganoHoy = (p.pasosDadosLiebre > p.pasosDadosTortuga) ? "liebre" : "tortuga";

            Console.WriteLine("El ganador es la "+p.ganoHoy);

            p.escribeArchivo();
            p.muestraArchivos();
            Console.ReadKey();
        }

        public void correLiebre()
        {
            
            while (!ganador)
            {
                lock (l)
                {
                    pasosDadosLiebre += 6;
                    if(meta - pasosDadosLiebre <= 0)
                    {
                        ganador = true;
                    }
                    if (!ganador)
                    {
                        
                        Console.WriteLine("Liebre avanzo "+pasosDadosLiebre+" pasos");
                        if (rand.Next(10) < 6)
                        {
                            Thread duerme = new Thread(dormir);
                            duerme.Start();
                            Console.WriteLine("El cuonejo se durmio!");
                            Monitor.Wait(l);
                        }

                    }
                }
                Thread.Sleep(200);
            }
        }

        public void correTortuga()
        {
            while (!ganador)
            {
                lock (l)
                {
                    pasosDadosTortuga += 1;
                    if(meta-pasosDadosTortuga <= 0)
                    {
                        ganador = true;
                    }
                    if (!ganador)
                    {
                        Console.WriteLine("Tortuga a avanzado " + pasosDadosTortuga + " pasos");
                        if(pasosDadosTortuga == pasosDadosLiebre)
                        {
                            if (rand.Next(10) < 5)
                            {
                                Console.WriteLine("La tortuga hizo ruido");
                                Monitor.Pulse(l);
                            }
                        }
                    }
                }Thread.Sleep(300);
            }
        }
        public void dormir()
        {
            Thread.Sleep(2500);
            lock (l)
            {
                Monitor.Pulse(l);
            }
        }
        public void LecturaArchivo()
        {
            
            
               if (File.Exists(path))
               {
                Process proc = Process.Start("notepad", path);
                proc.WaitForExit();
                Console.WriteLine("PID: " + proc.Id);
            }
            else
            {
                using(StreamWriter sw = new StreamWriter(path))
                {
                    Console.WriteLine("Archivo creado");
                }
            }
            
        }

        public void escribeArchivo()
        {
            using (StreamWriter sw = new StreamWriter(path, true))
            {
                sw.WriteLine(ganoHoy);
            }
        }

        public void muestraArchivos()
        {
            FileInfo f = new FileInfo(path);
            Console.WriteLine("Directorio: " + f.DirectoryName);
            FileInfo[] arch = f.Directory.GetFiles();

            foreach(FileInfo fi in arch)
            {
                Console.WriteLine("\tName: " + fi.Name + "\n\t" + fi.Length);
            }
        }
    }
}
