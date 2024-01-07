using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Legacy;


namespace Dhondt
{
    /// <summary>
    /// Ez a class tartalmaz minden számítást ami a mandátumkiosztáshoz kapcsolódik. Ha a végeredményt szeretnénk akkor elég
    /// a Cserelget()-et használni mivel ez kiadja a pártok nevét és mellé a mandátumuk számát.
    /// </summary>

    class Szamol
    {

        //Partok példányosítása. Ezt használja az összes függvény ebben az osztályban.
        public Partok p { get; }

        /// <summary>
        /// Számol osztály konstruktora, példányosítja a Partok objektumot az adott fájlnévvel.
        /// </summary>
        public Szamol(string fajlNev)
        {
            p = new Partok(fajlNev);
        }

        public Szamol()
        {
        }

        /// <summary>
        /// Számítja és visszaadja azoknak a pártoknak a listáját, amelyek átlépnek a százalékos küszöb értékükön.(minden pártnak az utolsó adattagja a százalék értéke)
        /// </summary>
        /// <returns>A megadott küszöbértéket meghaladó pártok listája.</returns>
        public List<string> KuszobSzamit() => p.Parts.Where(part => Math.Round((double)part.SzavazatSzam / (double)SzavazatokOsszege(p) * 100) >= part.Szazalek).Select(part => part.PartNev).ToList();


        /// <summary>
        /// Osszegzi a Pártok szavazatszámát.
        /// </summary>
        /// <returns>Pártok szavazatszámának összege.</returns>
        private static int SzavazatokOsszege(Partok p) => p.Parts.Sum(part => part.SzavazatSzam);


        /// <summary>
        /// Visszaadja azon pártok neveit, amelyek kedvezményben részesülnek.(tehát a nemzetiség mezőn 1-es szám található)
        /// </summary>
        /// <returns>A kedvezményben részesülő pártok neveinek listája.</returns>
        public List<string> Kedvezmenyesek() => p.Parts.Where(part => part.Nemzete == 1).Select(part => part.PartNev).ToList();

        /// <summary>
        /// Kiosztja a mandátumokat a partok között az oszlopok alapján.(Kiválasztja a mandátumszámnyszi legnagyobb számot.)
        /// </summary>
        /// <returns>A mandátumot kapó szavazatszám értéke és pártneve lista.</returns>
        public List<(int, string)> MandatumKioszt() => p.Parts.SelectMany(part => part.oszlop.Select(item => (item.Item1, part.PartNev))).OrderByDescending(x => x.Item1).Take(Partok.Mandatum).ToList();

        /// <summary>
        /// Összefésüli azokat a pártokat melyek kedvezményezettek (nemzetiségiek) és a küszöböt is átlépték.
        /// </summary>
        /// <returns>String típusú lista mely a fenti tulajdonságú Pártnevekkel tér vissza.</returns>
        public List<string> UjKedvezemenyezett() => Kedvezmenyesek().Where(kedv => KuszobSzamit().Contains(kedv)).ToList();


        /// <summary>
        /// Kiválasztja azokat a pártokat, amelyek kedvezményezettek de nincsenek jelen a mandátumosok között.
        /// </summary>
        /// <returns>Azoknak a pártoknak a listája, amelyek kedvezményezettek de nincsenek jelen a mandátumosok között.</returns>
        public List<string> CsereNevek() => UjKedvezemenyezett().Where(kedv => !MandatumKioszt().Any(mand => mand.Item2 == kedv)).Distinct().ToList();

        /// <summary>
        /// Mindegyik párt max szavazatát keresi.
        /// </summary>
        /// <param name="csere">Kedvezményesek nevei.</param>
        /// <returns>Adott párt neve és max szavazata(1db azaz az eredeti)</returns>
        private IEnumerable<(int, string)> MaxErtekKiolvas(List<string> csere)
        {
            return csere.Select(kp =>
            {
                int max = p.Parts.Find(x => x.PartNev == kp).oszlop.Select(x => x.Item1).Max();
                return (max, kp);
            });
        }

        /// <summary>
        /// Megnézi,hogy van-e kedvezményes aki nem kapott mandátumot. H van kicseréli az utolsóval, vagy a legkisebb szavazatszámúval.
        /// </summary>
        /// <returns></returns>
        public List<(int, string)> Cserelget()
        {
            List<string> csere = CsereNevek();
            List<(int, string)> mandatumosok = MandatumKioszt().Take(Partok.Mandatum - csere.Count).ToList();
            var maxErtek = MaxErtekKiolvas(csere);
            mandatumosok.AddRange(maxErtek);
            return mandatumosok;
        }

        /// <summary>
        /// Megtalálja a pártot név szerint.
        /// </summary>
        private static Part Megkeres(Partok p, string partNev) => p.Parts.FirstOrDefault(part => part.PartNev == partNev);

        /// <summary>
        /// Átforgatja truera az adott párt oszlopjában az Item2 ha mandátumot ért el vele.
        /// </summary>
        public void Atfordit()
        {
            List<(int, string)> mandList = Cserelget();
            foreach (var mand in mandList)
            {
                string partNev = mand.Item2;
                int keresettSzam = mand.Item1;

                Part keresettPart = Megkeres(p, partNev);
                var hely = (keresettPart != null) ? keresettPart.oszlop.FirstOrDefault(item => item.Item1 == keresettSzam && !item.Item2) : default;

                if (keresettPart != null && hely != default)
                {
                    int index = keresettPart.oszlop.IndexOf(hely);
                    keresettPart.oszlop[index] = (keresettSzam, true);
                }
            }
        }

        //------------------------------------------------------------
        //Tesztek kezdete


        [Test]
        [TestCase(100000)]
        public void SzavazatokOsszegeTeszt(int vart)
        {
            int eredmeny = SzavazatokOsszege(new Partok("inp0.txt"));
            Assert.That(vart, Is.EqualTo(eredmeny));

        }
        
        [Test]
        [TestCase("PártA")]
        public void MegkeresTeszt(string vart)
        {
            //Partok p = new Partok("inp0.txt");
            Part keresettPart = Megkeres(new Partok("inp0.txt"), "PártA");
            Assert.That(vart, Is.EqualTo(keresettPart.PartNev));
        }

        //Tesztek vége
        //------------------------------------------------------------
    }

}