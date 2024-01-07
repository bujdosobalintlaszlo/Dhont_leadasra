using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dhondt
{
    /// <summary>
    /// A pártot reprezentáló osztály, tárolja a párt nevét, nemzetét, szavazatszámát, bejutási küszöbének százalékát
	///és az oszlopon belül a hozzátartózó dhondt mátrix oszlopát..
    /// </summary>
    class Part
    {
        public string PartNev { get; }
        public int Nemzete { get; }
        public int SzavazatSzam { get; }
        public int Szazalek { get; }
        public List<(int, bool)> oszlop { get; }

        //A konstrultor megkap egy sort a fileból majd azt feldarabplja. Felhasználja az OszlopGen() függvényt is.
        public Part(string sor)
        {
            List<string> sorSplit = sor.Split(' ').ToList();
            PartNev = sorSplit[0];
            Nemzete = int.Parse(sorSplit[1]);
            SzavazatSzam = int.Parse(sorSplit[2]);
            Szazalek = int.Parse(sorSplit[3]);
            oszlop = OszlopGen();
        }
        public Part(string partNev, int nemzete, int szavazatSzam, int szazalek)
        {
            PartNev = partNev;
            Nemzete = nemzete;
            SzavazatSzam = szavazatSzam;
            Szazalek = szazalek;
        }

        //Feltölti az adott párt oszlopát a szavazatszámnak és a 1 mandátumig osztott számokkal. Mindegyik mellé
        //rendel egy false értéket ami a megjelenítésnél fontos. Ennek az értékét a Szamol class-ban az Atfordit()
        //metódus változtatja true-ra adott esetben.
        private List<(int, bool)> OszlopGen()
        {
            List<(int, bool)> oszlopa = new List<(int, bool)>();

            for (int j = 0; j < Partok.Mandatum; j++)
            {
                int szam = (int)Math.Round((double)SzavazatSzam / (j + 1), 0);
                oszlopa.Add((szam, false));
            }
            return oszlopa;
        }
    }
    /// <summary>
    /// Az OszlopGen() tesztje.
    /// </summary>
    [TestFixture]
    public class TesztClass
    {
        public static readonly int[] szazalekok = new int[3] { 5, 10, 15 };
        [Test]
        public void MyTest([Random(30000, 300000, 1)] int szavSzama, [Random(0, 1, 1)] int nemzetiE, [Random(0, 2, 5)] int index)
        {
            string sor = $"TesztPart {nemzetiE} {szavSzama} {szazalekok[index]}";
            Part p = new Part(sor);
            int vart = (int)Math.Round((double)p.SzavazatSzam / 3, 0);
            Assert.That(p.oszlop[2].Item1, Is.EqualTo(vart));
        }
    }
}