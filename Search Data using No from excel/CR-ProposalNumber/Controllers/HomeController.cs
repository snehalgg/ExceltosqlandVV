using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CR_ProposalNumber.Models;
using OfficeOpenXml;

namespace CR_ProposalNumber.Controllers
{


    public class HomeController : Controller
    {
      //  private string connectionString = "Server=FGLAPNL207HFZT\\SQLEXPRESS;Database=uploadingfile;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
        string sqlConnectionString = "Server=FGLAPNL207HFZT\\SQLEXPRESS;Database=uploadingfile;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    string filePath = Path.Combine(Server.MapPath("~/App_Data"), Path.GetFileName(file.FileName));
                    file.SaveAs(filePath);

                    string connectionString = string.Empty;
                    if (Path.GetExtension(filePath).ToLower() == ".xls")
                    {
                        connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES;'";
                    }
                    else if (Path.GetExtension(filePath).ToLower() == ".xlsx")
                    {
                        connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties='Excel 12.0 Xml;HDR=YES;'";
                    }

                    using (OleDbConnection connection = new OleDbConnection(connectionString))
                    {
                        connection.Open();

                        DataTable dt = new DataTable();
                        OleDbCommand command = new OleDbCommand("SELECT * FROM [Sheet1$]", connection);
                        OleDbDataAdapter da = new OleDbDataAdapter(command);
                        da.Fill(dt);

                        string sqlConnectionString = "Server=FGLAPNL207HFZT\\SQLEXPRESS;Database=uploadingfile;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

                        using (var bulkCopy = new SqlBulkCopy(sqlConnectionString))
                        {
                            bulkCopy.DestinationTableName = "temptable";
                            bulkCopy.WriteToServer(dt);
                        }
                    }

                    try
                    {
                        string sqlConnectionString = "Server=FGLAPNL207HFZT\\SQLEXPRESS;Database=uploadingfile;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
                        using (SqlConnection connection = new SqlConnection(sqlConnectionString))
                        {
                            connection.Open();
                            SqlCommand command = new SqlCommand("InsertMatchingProposalDetails", connection);
                            command.CommandType = CommandType.StoredProcedure;
                            command.ExecuteNonQuery();

                            // Fetch updated temptable data
                            SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM temptable", connection);
                            DataTable updatedTable = new DataTable();
                            adapter.Fill(updatedTable);

                            // Convert DataTable to List<ProposalDetails>
                            List<ProposalDetails> proposalDetails = new List<ProposalDetails>();
                            foreach (DataRow row in updatedTable.Rows)
                            {
                                var detail = new ProposalDetails
                                {
                                    ProposalNumber = row["ProposalNumber"].ToString(),
                                    Status = row["Status"].ToString(), // Add other columns accordingly
                                    SubStatus = row["SubStatus"].ToString(),
                                    CollectionType = row["CollectionType"].ToString(),
                                    IMDCode = row["IMDCode"].ToString(),
                                    PolicyHolder = row["PolicyHolder"].ToString(),
                                    PremiumPayerApplicable = row["PremiumPayerApplicable"].ToString(),
                                    PayerID = row["PayerID"].ToString(),
                                    Premium = row["Premium"].ToString(),
                                    TotalTaxes = row["TotalTaxes"].ToString(),
                                    TotalPremiumDue = row["TotalPremiumDue"].ToString(),
                                    CollectionNumber = row["CollectionNumber"].ToString(),
                                    CollectionDate = row["CollectionDate"].ToString(),
                                    // Map all columns as per your model
                                };
                                proposalDetails.Add(detail);
                            }

                            // Pass proposalDetails (List<ProposalDetails>) to the Index view
                            return View("Index", proposalDetails);

                        }
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Error = "An error occurred while fetching updated data: " + ex.Message;
                        return View("Index");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "An error occurred: " + ex.Message;
                    return View("Index");
                }
            }

            return View("Index");
        }

        //Download File starts
        [HttpPost]
        public ActionResult ExportToExcel()
        {
            try
            {
                string sqlConnectionString = "Server=FGLAPNL207HFZT\\SQLEXPRESS;Database=uploadingfile;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
                using (SqlConnection connection = new SqlConnection(sqlConnectionString))
                {
                    connection.Open();

                    // Fetch data from temptable
                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM temptable", connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Export data to Excel using OleDb
                    string filePath = Server.MapPath("~/App_Data/TemptableData.xls");
                    string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES;'";
                    using (OleDbConnection excelConnection = new OleDbConnection(connectionString))
                    {
                        excelConnection.Open();

                        string createTableQuery = "CREATE TABLE [Sheet1] (";
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            createTableQuery += $"[{dt.Columns[i].ColumnName}] NVARCHAR,";
                        }
                        createTableQuery = createTableQuery.TrimEnd(',') + ")";
                        using (OleDbCommand createTableCommand = new OleDbCommand(createTableQuery, excelConnection))
                        {
                            createTableCommand.ExecuteNonQuery();
                        }

                        using (OleDbCommand insertCommand = new OleDbCommand())
                        {
                            insertCommand.Connection = excelConnection;
                            foreach (DataRow row in dt.Rows)
                            {
                                string columnNames = string.Join(",", dt.Columns.Cast<DataColumn>().Select(c => "[" + c.ColumnName + "]"));
                                string values = string.Join(",", row.ItemArray.Select(r => "'" + r.ToString() + "'"));

                                insertCommand.CommandText = $"INSERT INTO [Sheet1] ({columnNames}) VALUES ({values})";
                                insertCommand.ExecuteNonQuery();
                            }
                        }

                        excelConnection.Close();
                    }

                    // Truncate the temptable
                    SqlCommand truncateCommand = new SqlCommand("TRUNCATE TABLE temptable", connection);
                    truncateCommand.ExecuteNonQuery();

                    // Download the Excel file
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                    System.IO.File.Delete(filePath); // Delete the file after reading its content
                    return File(fileBytes, "application/vnd.ms-excel", "TemptableData.xls");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred while exporting data: " + ex.Message;
                return View("Index");
            }
        }



        //Download File ends

        //Truncating File starts 
        // Truncate temptable after downloading Excel file
        public ActionResult TruncateTemptable()
        {
            try
            {
                string sqlConnectionString = "Server=FGLAPNL207HFZT\\SQLEXPRESS;Database=uploadingfile;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
                using (SqlConnection connection = new SqlConnection(sqlConnectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("TRUNCATE TABLE temptable", connection);
                    command.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred while truncating temptable: " + ex.Message;
                return View("Index");
            }
        }
        //Truncating file ends








        //public ActionResult Search()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Search(string proposalNumber)
        //{
        //    if (string.IsNullOrWhiteSpace(proposalNumber))
        //    {
        //        ViewBag.Error = "Please enter a Proposal Number.";
        //        return View("Index");
        //    }

        //    using (SqlConnection connection = new SqlConnection(sqlConnectionString))
        //    {
        //        connection.Open();

        //        string query = "SELECT * FROM MainTable WHERE ProposalNumber = @ProposalNumber";
        //        SqlCommand command = new SqlCommand(query, connection);
        //        command.Parameters.AddWithValue("@ProposalNumber", proposalNumber);

        //        SqlDataReader reader = command.ExecuteReader();

        //        if (reader.HasRows)
        //        {
        //            var proposalDetails = new List<ProposalDetails>();

        //            while (reader.Read())
        //            {
        //                var detail = new ProposalDetails
        //                {
        //                    ProposalNumber = reader["ProposalNumber"].ToString(),
        //                    Status = reader["Status"].ToString(),
        //                    SubStatus = reader["SubStatus"].ToString(),
        //                    CollectionType = reader["CollectionType"].ToString(),
        //                    IMDCode = reader["IMDCode"].ToString(),
        //                    PolicyHolder = reader["PolicyHolder"].ToString(),
        //                    PremiumPayerApplicable = reader["PremiumPayerApplicable"].ToString(),
        //                    PayerID = reader["PayerID"].ToString(),
        //                    Premium = reader["Premium"].ToString(),
        //                    TotalTaxes = reader["TotalTaxes"].ToString(),
        //                    TotalPremiumDue = reader["TotalPremiumDue"].ToString(),
        //                    CollectionNumber = reader["CollectionNumber"].ToString(),
        //                    CollectionDate = reader["CollectionDate"].ToString(),
        //                    // Map other columns accordingly
        //                };

        //                proposalDetails.Add(detail);
        //            }

        //            ViewBag.ProposalDetails = proposalDetails;
        //            return View("Index", proposalDetails);
        //        }
        //        else
        //        {
        //            ViewBag.SearchError = "Details not found for the provided Proposal Number.";
        //            ViewBag.ProposalDetails = new List<ProposalDetails>();
        //            return View("Index");
        //        }
        //    }
        //}



    }
}
