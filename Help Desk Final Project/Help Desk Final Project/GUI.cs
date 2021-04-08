using System;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Help_Desk_Final_Project
{
    public partial class GUI : Form
    {
        public GUI()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            updateView();
        }

        // Connection String that connects the app to the SQL server
        string connectionString = "Data Source=DESKTOP-1NLHA95;Initial Catalog=TicketDB;Integrated Security=True";

        private void CreateButton_Click(object sender, EventArgs e)
        {
            // Opening a connection to the database
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();

            // Grabbing user input
            command.Parameters.AddWithValue("@Ticket_Name", CreateTicketNameTextBox.Text);
            command.Parameters.AddWithValue("@Date_of_Request", CreateDateOfRequestTextBox.Text);
            command.Parameters.AddWithValue("@Description_of_Request", CreateDescriptionOfRequestTextBox.Text);
            command.Parameters.AddWithValue("@Technician_Assigned", CreateTechnicianAssignedTextBox.Text);
            command.Parameters.AddWithValue("@Date_of_Completion", CreateDateOfCompletionTextBox.Text);
            command.Parameters.AddWithValue("@Notes", CreateNotesTextBox.Text);
            command.Parameters.AddWithValue("@Status", "Open");

            // Searching the table for a tablename
            command.CommandText = "SELECT * FROM TicketData WHERE Ticket_Name = @Ticket_Name";
            string dupeCheck = (command.ExecuteScalar() ?? "").ToString();           
         
                // Checking if dupecheck is an empty string and if so there are no duplicate ticketnames
                if(dupeCheck == "")
                {
                    command.CommandText = "SET IDENTITY_INSERT dbo.TicketData ON; INSERT INTO TicketData (Ticket_Name, Date_of_Request, Description_of_Request, Technician_Assigned, Date_of_Completion, Notes, Status) VALUES (@Ticket_Name, @Date_of_Request, @Description_of_Request, @Technician_Assigned, @Date_of_Completion, @Notes, @Status)";
                    command.ExecuteNonQuery();
                } else
                     MessageBox.Show("Ticket name already exists." + "\n" + "Please use a unique ticket name.");     
                
            connection.Close();
            updateView();
        }

        private void EditComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Opening a connection to the database
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();

            // Populating the combobox with all the ticket names
            command.Parameters.AddWithValue("@Ticket_Name", EditComboBox.SelectedItem);
            command.CommandText = "SELECT * FROM TicketData WHERE Ticket_Name = @Ticket_Name";

            // Using a data reader to iterate over the data in the table
            SqlDataReader reader = command.ExecuteReader();
            if(reader.Read())
            {
                EditDateOfRequestTextBox.Text = reader.GetString(1);
                EditDescriptionOfRequestTextBox.Text = reader.GetString(2);
                EditTechnicianAssignedTextBox.Text = reader.GetString(3);
                EditDateOfCompletionTextBox.Text = reader.GetString(4);
                EditNotesTextBox.Text = reader.GetString(5);
                EditTicketStatusTextBox.Text = reader.GetString(6);
            }

            connection.Close();
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            // Opening a connection to the database
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();

            // Grabbing user input
            command.Parameters.AddWithValue("@Ticket_Name", EditComboBox.Text);
            command.Parameters.AddWithValue("@Date_of_Request", EditDateOfRequestTextBox.Text);
            command.Parameters.AddWithValue("@Description_of_Request", EditDescriptionOfRequestTextBox.Text);
            command.Parameters.AddWithValue("@Technician_Assigned", EditTechnicianAssignedTextBox.Text);
            command.Parameters.AddWithValue("@Date_of_Completion", EditDateOfCompletionTextBox.Text);
            command.Parameters.AddWithValue("@Notes", EditNotesTextBox.Text);
            command.Parameters.AddWithValue("@Status", EditTicketStatusComboBox.Text);

            // Updating the table with the user input and prompting the user
            command.CommandText = "UPDATE TicketData SET Date_of_Request = @Date_of_Request, Description_of_Request = @Description_of_Request, Technician_Assigned = @Technician_Assigned, Date_of_Completion = @Date_of_Completion, Notes = @Notes, Status = @Status WHERE Ticket_Name = @Ticket_Name";

            if (command.ExecuteNonQuery() > 0)
                MessageBox.Show("Ticket was edited.");
            else
                MessageBox.Show("Failed to edit ticket.");

            // Clearing the form
            EditComboBox.ResetText();
            EditDateOfRequestTextBox.Clear();
            EditDescriptionOfRequestTextBox.Clear();
            EditTechnicianAssignedTextBox.Clear();
            EditDateOfCompletionTextBox.Clear();
            EditNotesTextBox.Clear();
            EditTicketStatusTextBox.Clear();
            EditTicketStatusComboBox.ResetText();

            connection.Close();
            updateView();
        }

        private void RemoveTicketButton_Click(object sender, EventArgs e)
        {
            // Opening a connection to the database
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();

            // Searching the table and deleting an entry
            command.Parameters.AddWithValue("@Ticket_Name", EditComboBox.SelectedItem);
            command.CommandText = "DELETE FROM TicketData WHERE Ticket_Name = @Ticket_Name";

            if (command.ExecuteNonQuery() > 0)            
                MessageBox.Show("Ticket was deleted.");                
            
            else
                MessageBox.Show("Failed to delete ticket.");

            // Clearing the form
            EditComboBox.ResetText();
            EditDateOfRequestTextBox.Clear();
            EditDescriptionOfRequestTextBox.Clear();
            EditTechnicianAssignedTextBox.Clear();
            EditDateOfCompletionTextBox.Clear();
            EditNotesTextBox.Clear();
            EditTicketStatusTextBox.Clear();
            EditTicketStatusComboBox.ResetText();

            connection.Close();
            updateView();
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {

            // Checking to see if the edit tab has been selected and adding items to the combobox
            if (TabControl.SelectedTab == TabControl.TabPages["EditTabPage"])
            {
                EditTicketStatusComboBox.Items.Clear();
                EditTicketStatusComboBox.Items.Add("Open");
                EditTicketStatusComboBox.Items.Add("Closed");
                EditTicketStatusComboBox.Items.Add("Escalated");
            }

            // checking if openticketspage tab has been selected
            if (TabControl.SelectedTab == TabControl.TabPages["OpenTicketsTabPage"])
            {
                // Opening a connection to the database
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();

                // Searching the database where tickets have the status of 'Open' & clearing the listbox
                command.CommandText = "SELECT * FROM TicketData WHERE Status = 'Open'";
                OpenTicketsListBox.Items.Clear();

                // Using a datareader to iterate over the data
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {                    
                    OpenTicketsListBox.Items.Add(reader["Ticket_Name"]);
                }

                connection.Close();
            }
        }

        private void OpenTicketsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Opening a connection to the database
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();

            // Searching the database
            command.Parameters.AddWithValue("@Ticket_Name", OpenTicketsListBox.SelectedItem);
            command.CommandText = "SELECT * FROM TicketData WHERE Ticket_Name = @Ticket_Name";

            // Using a datareader to populate textboxes
            SqlDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                OpenTicketNameTextBox.Text = reader.GetValue(0).ToString();
                OpenDateOfRequestTextBox.Text = reader.GetValue(1).ToString();
                OpenDescriptionOfRequestTextBox.Text = reader.GetValue(2).ToString();
                OpenTechnicianAssignedTextBox.Text = reader.GetValue(3).ToString();
                OpenDateOfCompletionTextBox.Text = reader.GetValue(4).ToString();
                OpenNotesTextBox.Text = reader.GetValue(5).ToString();
                OpenTicketStatusTextBox.Text = reader.GetValue(6).ToString();
            }

            connection.Close();
            updateView();
        }

        private void updateView()
        {
            // Opening a connection to the database
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();

            // Selects everything from the table and clears the datagridview
            command.CommandText = "SELECT * FROM TicketData";
            SqlDataReader reader = command.ExecuteReader();
            dataGridView.Rows.Clear();
            dataGridView.Columns.Clear();
            EditComboBox.Items.Clear();

            // Adds all the columns back to the datagridview
            dataGridView.Columns.Add("TicketNameCol", "Ticket_Name");
            dataGridView.Columns.Add("DateOfRequestCol", "Date_of_Request");
            dataGridView.Columns.Add("DescriptionOfRequestCol", "Description_of_Request");
            dataGridView.Columns.Add("TechnicianAssignedCol", "Technician_Assigned");
            dataGridView.Columns.Add("DateOfCompletionCol", "Date_of_Completion");
            dataGridView.Columns.Add("NotesCol", "Notes");
            dataGridView.Columns.Add("StatusCol", "Status");

            // Using a reader to iterate over the table and populate the datagridview & combobox
            while (reader.Read())
            {
                EditComboBox.Items.Add(reader.GetValue(0));
                dataGridView.Rows.Add(reader.GetValue(0), reader.GetValue(1), reader.GetValue(2), reader.GetValue(3), reader.GetValue(4), reader.GetValue(5), reader.GetValue(6));
            }
            connection.Close();
        }

        // Buttons that close the program
        private void CreateExitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void EditExitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Keypress event forces the user to only input numeric values 0-9
        private void CreateTicketNameTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
