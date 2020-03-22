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
            

                {
                    Console.WriteLine("LASKUT");
                    CSIHelsinkiContext list2 = new CSIHelsinkiContext();
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
                        Console.WriteLine($"{item.AsiakasNro}{item.name}{item.Selite}{item.LaskuNro} Summa{item.Summa}");
                    }

                    Console.ReadKey();
                }
                {
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
                        Console.WriteLine($"{item.asiakas} Summa{item.summa}");
                    }
                    Console.ReadKey();
                }

                {
                    //Serveri connectionstring
                    //Käytetty SQLBulkCopy viemään data XML:ltä SQL Serverille
                    string connectionString = "Server=DESKTOP-35CADGH\\SQLEMA; Database=CSIHelsinki; Trusted_Connection=True;";

                    DataSet ds = new DataSet();
                    DataTable sourceData = new DataTable();
                    DataTable sourceData2 = new DataTable();
                    //Tiedoston sijainti
                    ds.ReadXml(@"C:\Users\maria\source\CSIHki\Laskutus.xml");
                    sourceData = ds.Tables[0];
                    sourceData2 = ds.Tables[1];

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

                            //mapping lasku tauluun
                            bulkCopy.DestinationTableName = "Lasku";
                            //bulkCopy.ColumnMappings.Add("asiakas_nro", "Asiakas_nro");

                            //Tähän en saanut tehtyä oikean laista mappingiä. En löytänyt miten children element mapping tehdään.
                            //bulkCopy.ColumnMappings.Add("lasku_Nro", "lasku_Nro");
                            bulkCopy.ColumnMappings.Add("selite", "selite");
                            bulkCopy.ColumnMappings.Add("summa", "summa");

                        try
                        {
                            //Kirjoittaa serverille
                            bulkCopy.WriteToServer(sourceData2);
                        }
                        //kirjoittaa konsoliin viestin jos tulee error
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        finally
                        {
                            Console.WriteLine(ds.Tables[1]);
                            Console.ReadLine();
                        }

                        //mapping asiakastauluun
                        //bulkCopy.DestinationTableName = "Asiakas";
                        //tyhjentää edellisen mappingin
                        //bulkCopy.ColumnMappings.Clear();
                        //bulkCopy.ColumnMappings.Add("asiakas_nro", "Asiakas_nro");
                        //bulkCopy.ColumnMappings.Add("etunimi", "Etunimi");
                        //bulkCopy.ColumnMappings.Add("sukunimi", "Sukunimi");
                        //bulkCopy.ColumnMappings.Add("osoite", "Osoite");
                        //bulkCopy.ColumnMappings.Add("postinumero", "Postinumero");
                        //bulkCopy.ColumnMappings.Add("postitoimipaikka", "Postitoimipaikka");

                        //try
                        //{
                        //    bulkCopy.WriteToServer(sourceData);
                        //}
                        //catch (Exception ex)
                        //{
                        //    Console.WriteLine(ex.Message);
                        //}
                        //finally
                        //{
                        //    Console.WriteLine("Copied {0} so far...");
                        //    Console.ReadLine();
                        //}
                    }

                    }
                }

            

        }

        private static void OnSqlRowsTransfer(object sender, SqlRowsCopiedEventArgs e)
        {

            Console.WriteLine("Copied {0} so far...", e.RowsCopied);
            Console.ReadLine();
        }
    }
}
