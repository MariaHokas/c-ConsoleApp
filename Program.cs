using ConsoleAppCSI.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppCSI
{
    class Program
    {
            //NuGet Packages
            //Install-Package Microsoft.EntityFrameworkCore.Tools -Version 3.1.2
            //Install-Package Microsoft.EntityFrameworkCore.SqlServer
            //Install-Package Microsoft.EntityFrameworkCore.SqlServer.Design

        static void Main(string[] args)
        {
                        //EntityFramework ja LinQ-kielen avulla tulostetaan kaksi listaa Laskutaulun kaikki tiedot ja erillinen summataulu. 
                        //Asiakkaan nimi tuotu asiakastaulusta join:nin avulla laskuttauluun
                        //Summa laskettu Linq:n avulla Grouppaamalla asiakasnumerot
                        //EntityFrameWork ei tukenut näiden kahden listan yhdistämistä LinQ:n avulla, joten en valitettavasti saanut tehtyä tästä yhtä fiksua taulua.  
                        {
                        Console.WriteLine("LASKUT");
                        //Alla olevalla lauseella päästään käsiksi EF-tietokantaan. 
                            CSIHelsinkiContext list2 = new CSIHelsinkiContext();
                        //Syntaksi LinQ taulun tuovassa 
                            var laskuxx = (from a in list2.Lasku
                                            orderby a.AsiakasNro
                                            join t in list2.Asiakas on a.AsiakasNro equals t.AsiakasNro
                                            select new
                                            {
                                                a.AsiakasNro,
                                                name = t.Etunimi + ' ' + t.Sukunimi,
                                                a.LaskuNro,
                                                a.Selite,
                                                a.Summa
                                            }).ToList();

                            foreach (var item in laskuxx)
                            {
                            //Tulostaa taulukon
                                Console.WriteLine($"{item.AsiakasNro}{item.name}{item.Selite}{item.LaskuNro} Summa{item.Summa}");
                            }

                                Console.ReadKey();
                            }
                            {
                            //Tämä laskee laskujen yhteissumman per asiakasnro
                                Console.WriteLine("SUMMA");
                                CSIHelsinkiContext list1 = new CSIHelsinkiContext();

                                var count1 = from a in list1.Lasku
                                             group a by a.AsiakasNro into g
                                             select new
                                             {
                                                 asiakas = g.Key,
                                                 summa = g.Sum(y => y.Summa),
                                             };

                                foreach (var item in count1)
                                {
                                //Tulostaa taulukon
                                    Console.WriteLine($"{item.asiakas} Summa{item.summa}");
                                }
                                Console.ReadKey();
                            }

                            {
                                //Tietokanta päivittyy aina kun ohjelmaa käytetään
                                //Serveri connectionstring
                                //Käytetty SQLBulkCopy viemään data XML:ltä SQL Serverille
                                string connectionString = "Server=DESKTOP-35CADGH\\SQLEMA; Database=CSIHelsinki; Trusted_Connection=True;";

                                DataSet ds = new DataSet();
                                DataTable sourceData = new DataTable();
                                DataTable sourceData2 = new DataTable();
                                //Tiedoston sijainti
                                ds.ReadXml(@"C:\Users\maria\source\CSIHki\Laskutus.xml");
                                sourceData = ds.Tables[0];
                
                                // avaa kohdetiedot
                                using (SqlConnection destinationConnection =
                                            new SqlConnection(connectionString))
                                {
                                    // avaa yhteyden
                                    destinationConnection.Open();
                                    using (SqlBulkCopy bulkCopy =
                                            new SqlBulkCopy(destinationConnection.ConnectionString,
                                                SqlBulkCopyOptions.TableLock))
                                    {
                                    bulkCopy.SqlRowsCopied +=
                                        new SqlRowsCopiedEventHandler(OnSqlRowsTransfer);
                                    bulkCopy.NotifyAfter = 100;
                                    //kaikki tietueet lähetetään yhdessä erässä, koska en määritellyt erän määrää
                                    bulkCopy.BatchSize = 0;

                                    //mapping Asiakas tauluun
                                    bulkCopy.DestinationTableName = "Asiakas";
                                    bulkCopy.ColumnMappings.Add("asiakas_nro", "Asiakas_nro");
                                    bulkCopy.ColumnMappings.Add("etunimi", "Etunimi");
                                    bulkCopy.ColumnMappings.Add("sukunimi", "Sukunimi");
                                    bulkCopy.ColumnMappings.Add("osoite", "Osoite");
                                    bulkCopy.ColumnMappings.Add("postinumero", "Postinumero");
                                    bulkCopy.ColumnMappings.Add("postitoimipaikka", "Postitoimipaikka");              
                            
                                    try
                                        {
                                            //Kirjoittaa serverille
                                            bulkCopy.WriteToServer(sourceData);
                                        }
                                        //kirjoittaa konsoliin viestin jos tulee error
                                    catch (Exception ex)
                                        {
                                            Console.WriteLine(ex.Message);
                                        }
                                    finally
                                        {
                                            Console.WriteLine(ds.Tables[0]);
                                            Console.ReadLine();
                                        }
                                    //mapping laskutauluun
                                    //Tähän käytin eniten aikaa
                                    //Mutta en valitettavasti saanut tämän tauluntiedoista vietyä kun asiakas_nro:n tauluun
                                    //VS valitti että lähdetiedoston sarake eivät täsmää kirjoittamiini, vaikka kopioin arvot suoraan schemasta.
                                    //Error messagen googlaamisen tuloksena en löytänyt vastausta. Netissä kehotettiin tarkistamaan kirjoitus sekä isot ja pienet kirjaimet 
                                    //yritin mapata alla olevan lisäksi mm. laskut.lasku.laskunro, laskut[0], lasku[0]
                                    //Näillä ilmeisesti on erilainen mapping, koska ymmärtääkseni nämä ovat laskut elementin lapsia. 

                                    bulkCopy.DestinationTableName = "Lasku";
                                    //Clear alla tyhjentää edellisen mappingin
                                    bulkCopy.ColumnMappings.Clear();
                                    bulkCopy.ColumnMappings.Add("asiakas_nro", "Asiakas_nro");
                                    bulkCopy.ColumnMappings.Add("lasku_Nro", "lasku_Nro");
                                    bulkCopy.ColumnMappings.Add("selite", "selite");
                                    bulkCopy.ColumnMappings.Add("summa", "summa");

                                        try
                                            {
                                                bulkCopy.WriteToServer(sourceData);
                                            }
                                        catch (Exception ex)
                                            {
                                                Console.WriteLine(ex.Message);
                                            }
                                        finally
                                            {
                                                Console.WriteLine("Copied {0} so far...");
                                                Console.ReadLine();
                                            }
                                        }
                                    }
                                }          
                            }
                            //Aluksi haasteita tuotti se koska en ollut kahteen vuoteen tehnyt yhtään Console Appia, 
                            //mutta hetken muistelun jälkeen asioita palautui mieleen. Huomasin myös että samat EF:t ja LinQ:t yms. toimii mainiosti myös Console Appissa.   

                            private static void OnSqlRowsTransfer(object sender, SqlRowsCopiedEventArgs e)
                                    {
                                        Console.WriteLine("Copied {0} so far...", e.RowsCopied);
                                        Console.ReadLine();
                                    }
                           }
                       }
