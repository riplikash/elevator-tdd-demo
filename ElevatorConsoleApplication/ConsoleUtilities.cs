﻿using System;

namespace ElevatorConsoleApplication
{
    public class ConsoleUtilities
    {

        public static int GetIntFromUser()
        {
            string stringValue = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace)
                {
                    double val = 0;
                    bool tempChar = double.TryParse(key.KeyChar.ToString(), out val);
                    if (tempChar)
                    {
                        stringValue += key.KeyChar;
                        Console.Write(key.KeyChar);
                    }
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && stringValue.Length > 0)
                    {
                        stringValue = stringValue.Substring(0, (stringValue.Length - 1));
                        Console.Write("\b \b");
                    }
                }
            }
                // Stops Receving Keys Once Enter is Pressed
            while (key.Key != ConsoleKey.Enter);
            return Int32.Parse(stringValue);

        }

        public static int GetChoiceFromUser(string message, params string[] options)
        {
            int rtChoice = -1;
            Console.WriteLine(message);
            Console.Write($"Select 1-{options.Length}: ");
            while (true)
            {
                rtChoice = GetIntFromUser();
//                if (rtChoice < 0 || rtChoice > options.Length) 
            }

            return rtChoice;
            
        }
    }
}