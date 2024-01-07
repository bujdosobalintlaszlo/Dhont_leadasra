using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Dhondt
{
    /// <summary>
    /// 
    /// </summary>
    class Szimulacio
    {
        //számontartja, hogy hanyadik pártot generálja
        private int counter = 1;

        private static Random r = new Random();

        //konstruktor
        public Szimulacio()
        {

        }

        /// <summary>
        /// Kigenerálja a pártoknak a számát 15 és 100 között
        /// </summary>
        /// <returns>Visszatér a pártszámmal.</returns>
        private int PartSzamGeneral() => r.Next(15, 100);

        /// <summary>
        /// Kigenerálja a mandátumok számát a pártszám figyelembe vételével.
        /// </summary>
        /// <returns>Visszatér a mandátumszámmal.</returns>
        private int MandatumGeneral() => PartSzamGeneral() > 50 ? PartSzamGeneral() * 3 : PartSzamGeneral() * 2;


        /// <summary>
        /// Lefuttatja azokat a függvényeket amikre szükségünk van a Console-on való megjelenítéshez.
        /// </summary>
        public void Lefuttat()
        {
            //A generálni kívánt file-ok száma.
            int db = 2;

            GeneralMasodikResz(db, PartSzamGeneral(), MandatumGeneral());
            for (int i = 0; i < db + 1; i++)
            {
                Szamol sz = new Szamol($"inp{i}.txt");
                sz.Atfordit();
                List<(int, string)> k = sz.Cserelget();
                ElsoTablaKiir(sz.p);
                Console.WriteLine();
                MasodikTabla(sz.p);
                Console.WriteLine();
                HarmadikTablaKiir(k);
                Console.WriteLine();
                NegyedikTabla(sz.p);
                Console.WriteLine();
                OtodikTablaKiir(k, sz.p);
                Console.WriteLine();
                HatodikTablaKiir(k);
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Azokból a szavazatszámokból amelyek sok szavazatot kapnak 1/5 eséllyel megtolja a szavazatjainak a számát.
        /// </summary>
        /// <returns>Visszatér egy igaz hamis értékkel.</returns>
        /// 
        private bool NagySzavazatuMegtol() => r.Next(1, 101) < 5 ? true : false;

        /// <summary>
        /// Eldönti, hogy nagyobb szavazat szám legyen vagy kisebb.
        /// </summary>
        /// <returns>Visszatér egy igaz hamis értékkel.</returns>
        private bool GeneralElsoResz() => r.Next(1, 16) < 15 ? false : true;

        /// <summary>
        /// A mandátum számot beleírja a fájlba + Egy sort ír a fájlba.
        /// </summary>
        /// <param name="n">indexelés</param>
        /// <param name="partSzam">Pártszám</param>
        /// <param name="mandatumSzam">MandátumSzám</param>
        private void SorbaIr(int n, int partSzam, int mandatumSzam)
        {
            using (StreamWriter w = new StreamWriter($"inp{n}.txt"))
            {
                List<int> szazalekok = new List<int>() { 5, 10, 15 };
                w.WriteLine(mandatumSzam);
                for (int i = 1; i <= partSzam; i++)
                {
                    bool elsoResz = GeneralElsoResz();
                    bool nagySzavazatu = NagySzavazatuMegtol();

                    int randomNumber;
                    if (nagySzavazatu)
                    {
                        randomNumber = r.Next(5000000, 10000000);
                    }
                    else
                    {
                        randomNumber = r.Next(0, 200000);
                    }

                    int index = elsoResz ? (nagySzavazatu ? r.Next(1, 3) : r.Next(1, 3)) : r.Next(0, 2);

                    w.WriteLine($"Párt{i} {r.Next(0, 2)} {randomNumber} {szazalekok[index]}");
                }
            }
        }


        /// <summary>
        /// Összetartja a Generálási folyamatot, itt fut le egybe.
        /// </summary>
        /// <param name="db">Mennyiszer fusson le</param>
        /// <param name="partSzam">PartSzám</param>
        /// <param name="mandatumSzam">Mandátumszám</param>
        private void GeneralMasodikResz(int db, int partSzam, int mandatumSzam)
        {
            int n = 1;
            while (n <= db)
            {
                SorbaIr(n, partSzam, mandatumSzam);
                n++;
            }
        }

        /// <summary>
        /// Megjelenítés PártNév - Szavazatszém szerint
        /// </summary>
        /// <param name="p">Partok típusú változó</param>
        private void ElsoTablaKiir(Partok p)
        {

            //Console-ra írás
            int maxPartNevLength = p.Parts.Max(x => x.PartNev.Length);
            Console.WriteLine($"{counter} választási eredmény");
            p.Parts.ForEach(x =>
            {
                string paddedPartNev = x.PartNev.PadRight(maxPartNevLength);
                string szavazatSzamText = x.SzavazatSzam.ToString().PadRight(maxPartNevLength);
                Console.WriteLine($"{paddedPartNev} - {szavazatSzamText}");
            });

            //File-ba írás
            using (StreamWriter w = new StreamWriter($"inp{counter}valasztasieredmeny.txt"))
            {
                w.WriteLine($"{counter} választási eredmény");
                p.Parts.ForEach(x =>
                {
                    w.WriteLine($"{x.PartNev} - {x.SzavazatSzam}");
                });
            }

            Console.WriteLine();
        }

        /// <summary>
        /// D'Hondt mátrix kiírása
        /// </summary>
        /// <param name="p">Partok típusú változó mely tartalmazza az összes pártot.</param>
        private void MasodikTabla(Partok p)
        {

            //Console-ra írás
            Console.WriteLine($"{counter} - D'Hondt mátrix számítása");
            foreach (var item in p.Parts)
            {
                Console.Write("Oszlop:");
                OszlopMegjelenit(item.oszlop);
            }

            //File-ba írás
            using (StreamWriter w = new StreamWriter($"inp{counter}dhondtmatrix.txt"))
            {
                Console.WriteLine($"{counter} - D'Hondt mátrix számítása");
                w.WriteLine($"{counter}választás");
                foreach (var item in p.Parts)
                {
                    w.Write("Osztói:");
                    for (int i = 0; i < item.oszlop.Count; i++)
                    {
                        w.Write(item.oszlop[i].Item1 + " ");
                    }
                    w.WriteLine();
                }
            }
        }

        private void OszlopMegjelenit(List<(int, bool)> oszlop)
        {
            foreach (var item in oszlop)
            {
                if (item.Item2)
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.Write(item.Item1 + " ");
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.Write(item.Item1 + " ");
                }
            }
            Console.WriteLine();
        }


        /// <summary>
        /// A végeredmény megjelenítése pártnév: kapottMandátumszám formátumban.
        /// </summary>
        /// <param name="l">Örökölt lista mely a Cserelget() függvény segítségével jön létre. Ezt a listát a Lefuttat metóduson belül hozzuk létre.Tartalma a szavazatszám és a hozzá tartozó párt.</param>
        private void HarmadikTablaKiir(List<(int, string)> l)
        {

            //ide gyűjtjük ki a pártokat és hozzá a mandátumokat
            Dictionary<string, int> partEsMandatum = HarmadikTablaDictionary(l);

            //A dictionary adatai rendezve
            List<string> eredmeny = RendezettAdatok(partEsMandatum);

            //Kiírás console-ra
            foreach (string entry in eredmeny)
            {
                Console.WriteLine(entry + "db");
            }

            //File-ba írás
            using (StreamWriter w = new StreamWriter($"inp{counter}kapottmandatumokszama.txt"))
            {
                w.WriteLine($"{counter}. választás végeredménye");
                foreach (string entry in eredmeny)
                {
                    w.WriteLine(entry + "db");
                }

            }

            Console.WriteLine();
        }

        /// <summary>
        /// Létrehoz egy olyan dictionary-t amely a pártok és a hozzájuk tartozó mandátumok számát tartalmazza. A HarmadikTablaKiir()-hoz tartozik.
        /// </summary>
        /// <param name="l">Örökölt lista mely a Cserelget() függvény segítségével jön létre. Ezt a listát a Lefuttat metóduson belül hozzuk létre.Tartalma a szavazatszám és a hozzá tartozó párt.</param>
        /// <returns>Dictionary. string = párt int = mandátumainak száma.</returns>
        private Dictionary<string, int> HarmadikTablaDictionary(List<(int, string)> l)
        {
            Dictionary<string, int> partEsMandatum = new Dictionary<string, int>();
            foreach (var elem in l)
            {
                //pillanatnyi párt
                string temp = elem.Item2;

                if (partEsMandatum.ContainsKey(temp))
                {
                    partEsMandatum[temp]++;
                }
                else
                {
                    partEsMandatum[temp] = 1;
                }
            }
            return partEsMandatum;
        }

        /// <summary>
        /// Rendezi a dictionary elemeit egy listába. A HarmadikTablaKiir()-hoz tartozik.
        /// </summary>
        /// <param name="dic">Dictionary. string = párt int = mandátumainak száma.</param>
        /// <returns>Lista. Egy eleme: párt mandátumok száma.</returns>
        private List<string> RendezettAdatok(Dictionary<string, int> dic)
        {
            List<string> result = dic.Select(pair => $"{pair.Key}: {pair.Value}").ToList();
            result.Sort();
            return result;
        }

        /// <summary>
        /// Kiírja a szavazatok arányát, kerekítve 1 tizedes jegyre.
        /// </summary>
        /// <param name="p">A pártok listáját tartalmazó osztály példánya.</param>
        private void NegyedikTabla(Partok p)
        {
            int ossz = SzavazatokSum(p);

            p.Parts.ForEach(x => {
                Console.WriteLine(x.PartNev + ": " + Math.Round((double)x.SzavazatSzam / ossz * 100, 1) + "%");
            });

            //File-ba írás.
            using (StreamWriter w = new StreamWriter($"inp{counter}szavazatokaranya.txt"))
            {
                w.WriteLine("Statisztika1");
                p.Parts.ForEach(x => {
                    w.WriteLine(x.PartNev + ": " + Math.Round((double)x.SzavazatSzam / ossz * 100, 1) + "%");
                });
            }
        }

        /// <summary>
        /// Összegzi a pártok szavazatszámát.
        /// </summary>
        /// <param name="p">A pártok listáját tartalmazó osztály példánya.</param>
        /// <returns>Szavazatok összege.</returns>
        private int SzavazatokSum(Partok p) => p.Parts.Sum(x => x.SzavazatSzam);


        /// <summary>
        /// Kiírja a szavazatok arányát a mandátumhoz jutó pártokon belül.
        /// </summary>
        /// <param name="k">Örökölt lista mely a Cserelget() függvény segítségével jön létre. Ezt a listát a Lefuttat metóduson belül hozzuk létre.Tartalma a szavazatszám és a hozzá tartozó párt.</param>
        /// <param name="p">A pártok listáját tartalmazó osztály példánya.</param>
        private void OtodikTablaKiir(List<(int, string)> k, Partok p)
        {
            List<string> partok = EgyediPartok(k);

            List<(string, int)> partEsSzavszam = PartEsSzavszamFeltolt(partok, p);

            int osszeg = BejutottakSzavazatSum(partEsSzavszam);

            //Console-ra írás
            partEsSzavszam.ForEach(x => { Console.WriteLine(x.Item1 + ": " + Math.Round((double)x.Item2 / osszeg * 100) + "%"); });

            //File-ba írás.
            using (StreamWriter w = new StreamWriter($"inp{counter}SzavazatAranyMandatummal"))
            {
                partEsSzavszam.ForEach(x => { w.WriteLine(x.Item1 + ": " + Math.Round((double)x.Item2 / osszeg * 100) + "%"); });
            }
        }

        /// <summary>
        /// Összegzi a szavazatok számát. (Mandátumot kapott pártokét).
        /// </summary>
        /// <param name="k">Örökölt lista mely a Cserelget() függvény segítségével jön létre. Ezt a listát a Lefuttat metóduson belül hozzuk létre.Tartalma a szavazatszám és a hozzá tartozó párt.</param>
        /// <returns>Szavazatok összege. (Mandátumot kapott pártokét).</returns>
        private int BejutottakSzavazatSum(List<(string, int)> k) => k.Sum(x => x.Item2);

        /// <summary>
        /// Minden pártot csak 1-szer rak a listába.
        /// </summary>
        /// <param name="k">Örökölt lista mely a Cserelget() függvény segítségével jön létre. Ezt a listát a Lefuttat metóduson belül hozzuk létre.Tartalma a szavazatszám és a hozzá tartozó párt.</param>
        /// <returns>Pártok listája.</returns>
        private List<string> EgyediPartok(List<(int, string)> k) => k.Select(x => x.Item2).Distinct().ToList();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="l">Egyedi mandátumot kapott elemek listája.</param>
        /// <param name="p">A pártok listáját tartalmazó osztály példánya.</param>
        /// <returns></returns>
        private List<(string, int)> PartEsSzavszamFeltolt(List<string> l, Partok p)
        {
            List<(string, int)> partEsSzavszam = new List<(string, int)>();
            l.ForEach(x =>
            {
                var part = p.Parts.FirstOrDefault(y => y.PartNev == x);
                if (part != null && part.oszlop.Count > 0 && part.oszlop[0].Item1 != null)
                {
                    int szam = Convert.ToInt32(part.oszlop[0].Item1);
                    partEsSzavszam.Add((x, szam));
                }
            });
            return partEsSzavszam;
        }

        /// <summary>
        /// Kiírja a mandátumok arányát.
        /// </summary>
        /// <param name="k">Örökölt lista mely a Cserelget() függvény segítségével jön létre. Ezt a listát a Lefuttat
        /// metóduson belül hozzuk létre.Tartalma a szavazatszám és a hozzá tartozó párt.</param>
        private void HatodikTablaKiir(List<(int, string)> k)
        {
            //Consolera írás.
            Dictionary<string, int> PartEsMandatumSzam = MandatumCount(k);
            foreach (var item in PartEsMandatumSzam)
            {
                Console.WriteLine(item.Key + ": " + Math.Round((double)item.Value / Partok.Mandatum * 100, 1) + "%");
            }

            //File-ba írás.
            using (StreamWriter w = new StreamWriter($"inp{counter}mandatumokaranya.txt"))
            {
                foreach (var item in PartEsMandatumSzam)
                {
                    w.WriteLine(item.Key + ": " + Math.Round((double)item.Value / Partok.Mandatum * 100, 1) + "%");
                }
            }

            //növeljük a változót ami számolja, hogy hanyadik filet generáljuk
            counter++;
        }

        /// <summary>
        /// Megcsinál egy dictionary-t ahol a kulcs a pártnév és a hozzá rendelt érték pedig a mandátumainak száma
        /// </summary>
        /// <param name="k"> Örökölt lista mely a Cserelget() függvény segítségével jön létre. Ezt a listát a Lefuttat metóduson belül hozzuk létre.Tartalma a szavazatszám és a hozzá tartozó párt.</param>
        /// <returns>Egy dictionary amiben megvannak a pártok és azok mandátumainak száma</returns>
        private Dictionary<string, int> MandatumCount(List<(int, string)> k) => k.GroupBy(item => item.Item2).ToDictionary(group => group.Key, group => group.Count());


        //----------------------------------------------------------------------------------------------------------------------------------------------
        //Tesztek kezdete
        //itt stringé kell csinálni az l-t meg az eredmenyt is
        private static readonly List<(int, string)> partok = new List<(int, string)>() { (47000, "PártA"), (16000, "PártB") };

        //KuszobSzamit() függvény tesztje. Ez a függvény a Szamol class-ban található.
        [Test]
        [TestCase("PártA PártB PártC PártD PártE")]
        public void KuszobTeszt(string vart)
        {
            List<string> l = vart.Split(' ').ToList();
            Szamol tSz = new Szamol("inp0.txt");
            List<string> tSzEredmeny = tSz.KuszobSzamit();
            CollectionAssert.AreEqual(l, tSzEredmeny);
        }

        //Kedvezmenyesek() függvény tesztje. Ez a függvény a Szamol class-ban található.
        [Test]
        [TestCase("PártA PártB PártC PártE")]
        public void KedvezmenyesekTeszt(string vart)
        {
            List<string> l = vart.Split(' ').ToList();
            Szamol tSz = new Szamol("inp0.txt");
            List<string> tSzEredmeny = tSz.Kedvezmenyesek();
            CollectionAssert.AreEqual(l, tSzEredmeny);
        }

        //MandatumKioszt() függvény tesztje. Ez a függvény a Szamol class-ban található.
        [Test]
        [TestCase("47000 PártA 23500 PártA 16000 PártB 15900 PártC 15667 PártA 12000 PártD 11750 PártA 9400 PártA 8000 PártB 7950 PártC")]
        public void MandatumKiosztTeszt(string sor)
        {
            Szamol sz = new Szamol("inp0.txt");
            List<(int, string)> eredmeny = sz.MandatumKioszt();
            string formattedEredmeny = string.Join(" ", eredmeny.Select(x => $"{x.Item1} {x.Item2}"));
            Assert.That(formattedEredmeny, Is.EqualTo(sor));
        }

        //UjKedvezemenyezett() függvény tesztje. Ez a függvény a Szamol class-ban található.
        [Test]
        [TestCase("PártA PártB PártC PártE")]
        public void UjKedvezemenyezett(string sor)
        {
            Szamol sz = new Szamol("inp0.txt");
            List<string> vart = sor.Split(' ').ToList();
            List<string> e = sz.UjKedvezemenyezett();
            CollectionAssert.AreEqual(vart, e);
        }

        //CsereNevek függvény tesztje. Ez a függvény a Szamol class-ban található.
        [Test]
        [TestCase("PártE")]
        public void CsereNevekTeszt(string sor)
        {
            Szamol sz = new Szamol("inp0.txt");
            List<string> vart = sor.Split(' ').ToList();
            List<string> e = sz.CsereNevek();
            CollectionAssert.AreEqual(vart, e);
        }

        //Cserelget() függvény tesztje. Ez a függvény a Szamol class-ban található.
        [Test]
        [TestCase("47000 PártA 23500 PártA 16000 PártB 15900 PártC 15667 PártA 12000 PártD 11750 PártA 9400 PártA 8000 PártB 6000 PártE")]
        public void Cserelget(string sor)
        {
            Szamol sz = new Szamol("inp0.txt");
            List<(int, string)> eredmeny = sz.Cserelget();
            CollectionAssert.AreEqual(sor, string.Join(" ", eredmeny.Select(x => $"{x.Item1} {x.Item2}")));
        }

        //MandatumCount() függvény tesztje. Ez a függvény a Szimulacio class-ban található.
        [Test]
        [TestCase("PártA 1 PártB 1")]
        public void MandatumCountTeszt(string vart)
        {

            Dictionary<string, int> eredmenyDic = MandatumCount(partok);
            string eredmeny = string.Join(" ", eredmenyDic.Select(x => $"{x.Key} {x.Value}"));
            Assert.That(eredmeny, Is.EqualTo(vart));
        }

        //PartEsSzavszamFeltolt() függvény tesztje. Ez a függvény a Szimulacio class-ban található.
        private static readonly List<string> partEsSzavList = new List<string>() { "PártA", "PártB", "PártC", "PártD", "PártE" };
        [Test]
        [TestCase("PártA 47000 PártB 16000 PártC 15900 PártD 12000 PártE 6000")]
        public void PartEsSzavszamFeltoltTeszt(string vart)
        {
            Partok pt = new Partok("inp0.txt");
            List<(string, int)> e = PartEsSzavszamFeltolt(partEsSzavList, pt);
            List<string> eEgybe = e.Select(t => $"{t.Item1} {t.Item2}").ToList();
            string eString = string.Join(" ", eEgybe);
            Assert.That(eString, Is.EqualTo(vart));
        }


        //HarmadikTablaDictionary() függvény tesztje. Ez a függvény a Szimulacio class-ban található.
        [Test]
        [MaxTime(1000)]
        [TestCase("PártA 5 PártB 2 PártC 1 PártD 1 PártE 1")]
        public void HarmadikTablaDictionaryTeszt(string vart)
        {
            Szamol sz = new Szamol("inp0.txt");
            List<(int, string)> k = sz.Cserelget();
            Dictionary<string, int> dic = HarmadikTablaDictionary(k);
            List<string> dicSplit = dic.Select(x => $"{x.Key} {x.Value}").ToList();
            string eredmeny = string.Join(" ", dicSplit);
            Assert.That(eredmeny, Is.EqualTo(vart));
        }


        //RendezettAdatokAdatokTeszt() függvény tesztje. Ez a függvény a Szimulacio class-ban található.
        private static readonly Dictionary<string, int> rednszerDic = new Dictionary<string, int>()
        {
            {"PártA", 5},
            {"PártB", 2},
            {"PártC", 1},
            {"PártD", 1},
            {"PártE", 1}
        };

        [Test]
        [Timeout(100)]
        [TestCase("PártA 5 PártB 2 PártC 1 PártD 1 PártE 1")]
        public void RendezettAdatokTeszt(string vart)
        {
            List<string> dicSplit = rednszerDic.Select(x => $"{x.Key} {x.Value}").ToList();
            string eredmeny = string.Join(" ", dicSplit);
            Assert.That(eredmeny, Is.EqualTo(vart));
        }

        //SzavazatSumTeszt() függvény tesztje. Ez a függvény a Szimulacio class-ban található.
        [Test]
        public void SzavazatokSumTeszt()
        {
            Partok p = new Partok("inp0.txt");
            int eredmeny = SzavazatokSum(p);
            Assert.That(100000, Is.EqualTo(eredmeny));
        }

        private static readonly List<(string, int)> bejutottL = new List<(string, int)>()
        {
            ("PártA", 47000),
            ("PártB", 16000),
            ("PártC", 15900),
            ("PártD", 12000),
            ("PártE", 6000)
        };

        //BejutottakSzavazatSumTeszt() függvény tesztje. Ez a függvény a Szimulacio class-ban található.
        [Test]
        public void BejutottakSzavazatSumTeszt([Values(96900)] int vart)
        {
            int eredmeny = BejutottakSzavazatSum(bejutottL);
            Assert.That(eredmeny, Is.EqualTo(vart));
        }

        //EgyedipartokTeszt() függvény tesztje. Ez a függvény a Szimulacio class-ban található.
        [Test]
        [TestCase("PártA PártB PártC PártD PártE")]
        public void EgyedipartokTeszt(string vart)
        {
            Szamol sz = new Szamol("inp0.txt");
            List<(int, string)> l = sz.Cserelget();
            List<string> eList = EgyediPartok(l);
            string eredmeny = string.Join(" ", eList);
            Assert.That(eredmeny, Is.EqualTo(vart));
        }
        //Tesztek vege
        //--------------------------------------------------------------------------------------------------------------------------------------
    }
}