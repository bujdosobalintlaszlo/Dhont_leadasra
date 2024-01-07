using System;
namespace Dhondt
{
    /// <summary>
    /// Példányosítja a Szimulacio osztályt és meghívja a Lefuttat metódust.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Szimulacio sz = new Szimulacio();
            sz.Lefuttat();
            Console.ReadLine();
        }
    }
}