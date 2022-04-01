using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EmployeeDirectory
{
    /// <summary>
    /// Interaction logic for EmployeeDirectory.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Data.CollectionViewSource employeeViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("employeeViewSource")));
            employeeViewSource.Source = ScanDirectory();
        }

        private List<Employee> ScanDirectory()
        {
            DobsonUtilities.LogHandler log = new DobsonUtilities.LogHandler();
            List<Employee> Employees = new List<Employee>();
            string[] props = new string[] { "samAccountName", "sn", "givenName", "telephoneNumber", "l", "st", "title", "department", "mobile", "mail" };

            using (DobsonUtilities.ProgressDialog progress = new DobsonUtilities.ProgressDialog())
            using (System.DirectoryServices.DirectoryEntry searchRoot = new System.DirectoryServices.DirectoryEntry("LDAP://" + System.DirectoryServices.ActiveDirectory.Domain.GetCurrentDomain().ToString()))
            using (System.DirectoryServices.DirectorySearcher search = new System.DirectoryServices.DirectorySearcher(searchRoot, props.ToArray().ToString(), null, System.DirectoryServices.SearchScope.Subtree))
            {
                log.AppendMessage("Accessing LDAP...");
                search.Filter = "(&(objectClass=user)(objectCategory=person)(sAMAccountType=805306368)(!userAccountControl:1.2.840.113556.1.4.803:=2))";
                System.DirectoryServices.SearchResultCollection searchResults = search.FindAll();
                progress.Maximum = searchResults.Count;
                progress.Step = 1;
                progress.Show();

                foreach (System.DirectoryServices.SearchResult searchResult in searchResults)
                {
                    progress.PerformStep();
                    System.Windows.Forms.Application.DoEvents();
                    if (searchResult != null && (!searchResult.Properties["samAccountName"][0].ToString().EndsWith("7") ||
                        !searchResult.Properties["samAccountName"][0].ToString().StartsWith("!") ||
                        !searchResult.Properties["samAccountName"][0].ToString().EndsWith("-p")))
                    {
                        List<string> attributes = new List<string>();

                        try
                        {
                            for (byte i = 1; i < props.Length; ++i) // start at 1 to ignore samAccountName attribute
                            {
                                if (searchResult.Properties[props[i]].Count > 0)
                                    attributes.Add(searchResult.Properties[props[i]][0].ToString());
                                else
                                    attributes.Add(string.Empty);
                            }
                        }
                        catch (IndexOutOfRangeException IOoRE)
                        {
                            log.AppendMessage("Exception occurred: " + IOoRE.Message.ToString());
                        }
                        catch (ArgumentOutOfRangeException AOoRE)
                        {
                            log.AppendMessage("Exception occurred: " + AOoRE.Message.ToString());
                        }
                        // ensure lastname, firstname, and phone are not empty
                        if (attributes.Count == 9 && !string.IsNullOrEmpty(attributes[0]) && !string.IsNullOrEmpty(attributes[1]) && !string.IsNullOrEmpty(attributes[2]))
                        {
                            if (!attributes[0].Any(char.IsDigit) && !attributes[1].Any(char.IsDigit)) // ensure lastname and firstname do not contain digits
                            {
                                if (attributes[2] != "Restricted") // do not publish phone numbers listed as restricted
                                {
                                    Employees.Add(new Employee(attributes[0], attributes[1], attributes[2], attributes[3], attributes[4],
                                        attributes[5], attributes[6], attributes[7], attributes[8]));
                                }
                            }
                        }
                        attributes.Clear();
                    }
                }
                log.AppendMessage("LDAP scanning completed.");
                progress.Close();
            }

            List<Employee> sortedEmployees = new List<Employee>(Employees.OrderBy(x => x.LastName).ToList());
            return sortedEmployees;
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                System.Windows.Data.CollectionViewSource employeeViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("employeeViewSource")));
                employeeViewSource.Source = ScanDirectory();
            }
            if (ModifierKeys.Control != 0 && e.Key == Key.P)
            {
                PrintDialog printDialog = new PrintDialog();
                printDialog.PageRangeSelection = PageRangeSelection.AllPages;
                printDialog.UserPageRangeEnabled = true;
                printDialog.ShowDialog();
            }
        }
    }
}
