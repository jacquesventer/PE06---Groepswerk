using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailingList.Lib.Entities;

using System.Data.OleDb;

namespace MailingList.Lib.Services
{
    public class DeelnemerServices
    {
        string bestandsPad = AppDomain.CurrentDomain.BaseDirectory + "MailingList.accdb";
        public List<Deelnemer> deelnemers;
        OleDbConnection dbConn;
        OleDbCommand sqlCommand;

        public DeelnemerServices()
        {
            dbConn = new OleDbConnection();
            dbConn.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + bestandsPad;
        }

        public bool ImportData()
        {
            bool gelukt = false;
            OleDbDataReader dbRead = null;
            string sqlOpdracht = "select * FROM tblMailingList";
            try
            {
                dbConn.Open();
                sqlCommand = new OleDbCommand(sqlOpdracht, dbConn);
                dbRead = sqlCommand.ExecuteReader();
                while (dbRead.Read())
                {
                    int id = dbRead.GetInt32(0);
                    string firstName = dbRead.GetString(1).ToString();
                    string lastName = dbRead.GetString(2).ToString();
                    string email = dbRead.GetString(3).ToString();
                    int phone = dbRead.GetInt32(4);
                    string street = dbRead.GetString(5).ToString();
                    int streetNumber = dbRead.GetInt32(6);
                    string city = dbRead.GetString(7).ToString();
                    int postalCode = dbRead.GetInt32(8);
                    string answer = dbRead.GetString(2).ToString();
                    Deelnemer deelnemer = new Deelnemer(id, firstName, lastName, email, phone, street, streetNumber, city, postalCode, answer);
                    deelnemers.Add(deelnemer);
                }
                gelukt = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (dbRead != null)
                {
                    dbRead.Close();
                }
                SluitConnectie();
            }
            return gelukt;
        }


        public bool NieuwDeelnemer(Deelnemer deelnemer)
        {
            bool gelukt = false;

            if (DbVoegToe(deelnemer))
            {
                int id = ZoekIDinDb(deelnemer);
                deelnemer.Id = id;
                deelnemers.Add(deelnemer);
                gelukt = true;
            }
            return gelukt;
        }

        public bool WijzigDeelnemer(Deelnemer gewijzigd)
        {
            bool gelukt = false;
            int id = gewijzigd.Id;
            if (DbWijzig(gewijzigd))
            {
                foreach (Deelnemer teWijzigen in deelnemers)
                {
                    if (teWijzigen.Id == id)
                    {
                        teWijzigen.FirstName = gewijzigd.FirstName;
                        teWijzigen.LastName = gewijzigd.LastName;
                        teWijzigen.Email = gewijzigd.Email;
                        teWijzigen.Phone = gewijzigd.Phone;
                        teWijzigen.Street = gewijzigd.Street;
                        teWijzigen.StreetNumber = gewijzigd.StreetNumber;
                        teWijzigen.City = gewijzigd.City;
                        teWijzigen.PostalCode = gewijzigd.PostalCode;
                        teWijzigen.Answer = gewijzigd.Answer;
                    }
                }
                gelukt = true;
            }
            return gelukt;
        }

        public bool VerwijderDeelnemer(Deelnemer teVerwijderen)
        {
            bool gelukt = false;
            if (DbVerwijder(teVerwijderen))
            {
                deelnemers.Remove(teVerwijderen);
                gelukt = true;
            }
            return gelukt;
        }

        public bool VeranderDeelnemer(Deelnemer gewijzigd, Deelnemer voorwijziging)
        {
            bool gelukt = false;
            bool toegevoegd = false;
            bool verwijderd = VerwijderDeelnemer(voorwijziging);
            if (verwijderd)
            {
                toegevoegd = NieuwDeelnemer(gewijzigd);
            }
            {
                gelukt = true;
            }
            return gelukt;
        }

        private bool DbVerwijder(Deelnemer leaver)
        {
            bool verwijderd = false;
            string deleteQuery = $"DELETE FROM tblMailingList WHERE id = {leaver.Id}";
            try
            {
                dbConn.Open();
                sqlCommand = new OleDbCommand(deleteQuery, dbConn);
                sqlCommand.ExecuteNonQuery();
                verwijderd = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                SluitConnectie();
            }
            return verwijderd;
        }

        private bool DbWijzig(Deelnemer deelnemer)
        {
            bool gewijzigd = true;
            try
            {
                string updateDeelnemer =
                    $"UPDATE tblPersoneel SET " +
                    $"FirstName = '{deelnemer.FirstName}', " +
                    $"LastName = '{deelnemer.LastName}',  " +
                    $"Email = {deelnemer.Email}, " +
                    $"Phone = '{deelnemer.Phone}' " +
                    $"Street = '{deelnemer.Street}' " +
                    $"StreetNumber = '{deelnemer.StreetNumber}' " +
                    $"City = '{deelnemer.City}' " +
                    $"PostalCode = '{deelnemer.PostalCode}' " +
                    $"WHERE Id = {deelnemer.Id}";
                dbConn.Open();
                sqlCommand = new OleDbCommand(updateDeelnemer, dbConn);
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                gewijzigd = false;
                Console.WriteLine(ex.Message);
            }
            finally
            {
                SluitConnectie();
            }
            return gewijzigd;
        }

        private bool DbVoegToe(Deelnemer deelnemer)
        {
            bool toegevoegd = true;
            string insertDeelnemer = "";
            if (deelnemer.Id == 0)
            {
                insertDeelnemer = $"INSERT INTO tblMailingList " +
                  $"(FirstName, LastName, Email, Phone, Street, StreetNumber, City, PostalCode) values" +
                  $"('{deelnemer.FirstName}', '{deelnemer.LastName}', '{deelnemer.Email}', '{deelnemer.Phone}', '{deelnemer.Street}', '{deelnemer.StreetNumber}', '{deelnemer.City}', '{deelnemer.PostalCode}')";
            }
            else
            {
                insertDeelnemer = $"INSERT INTO tblMailingList " +
                     $"(FirstName, LastName, Email, Phone, Street, StreetNumber, City, PostalCode) values" +
                     $"({deelnemer.Id}, '{deelnemer.FirstName}', '{deelnemer.LastName}', '{deelnemer.Email}', '{deelnemer.Phone}', '{deelnemer.Street}', '{deelnemer.StreetNumber}', '{deelnemer.City}', '{deelnemer.PostalCode}')";
            }
            try
            {
                dbConn.Open();
                sqlCommand = new OleDbCommand(insertDeelnemer, dbConn);
                int rowsAffected = sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                toegevoegd = false;
                Console.WriteLine(ex.Message);
            }
            finally
            {
                SluitConnectie();
            }
            return toegevoegd;
        }

        int ZoekIDinDb(Deelnemer deelnemer)
        {
            int id = -1;
            OleDbDataReader dbRead = null;
            string sqlOpdracht = "SELECT id FROM tblMailingList" +
                $"WHERE FirstName = '{deelnemer.FirstName}' AND LastName = '{deelnemer.LastName}'" +
                $"ORDER BY id DESC";
            try
            {
                dbConn.Open();
                sqlCommand = new OleDbCommand(sqlOpdracht, dbConn);
                dbRead = sqlCommand.ExecuteReader();
                dbRead.Read();
                id = dbRead.GetInt32(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (dbRead != null)
                {
                    dbRead.Close();
                }
                SluitConnectie();
            }
            return id;
        }

        void SluitConnectie()
        {
            if (dbConn != null) { dbConn.Close(); }
        }
    }
}
