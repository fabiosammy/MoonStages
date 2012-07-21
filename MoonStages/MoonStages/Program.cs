using System;
using System.Collections.Generic;
using System.Text;

namespace NeHeLessons
{
        class Program
        {
                static void Main (string[] args)
                {
                        Console.Title = "Computacao Grafica";                        
						using (MoonStages moonStage = new MoonStages() ){
                        	moonStage.Run (30.0);
                        }

                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine ("Bye!!");
                }
        }
}
