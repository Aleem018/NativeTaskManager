using System; 
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace NativeTaskManager;

public class SystemTask
{
    public int ProcessId { get; set; }
    public string ProcessName { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"ID: {ProcessId} -> {ProcessName}";
    }
}


public partial class Form1 : Form
{
    private TextBox txtProcessId;
    private Button btnKill;
    private ListBox processList;
    private Button showProcessList;
    private TextBox launchBox;
    private Button launchBtn;
    private List<SystemTask> _masterProcessList = new List<SystemTask>();
    private TextBox txtSearch;

    public Form1()
    {
        // To setup the native window
        this.Text = "Native executioner";
        this.Size = new Size(700, 400);
        this.StartPosition = FormStartPosition.CenterScreen;

        // Setup the Input Box
        txtProcessId = new TextBox();
        txtProcessId.Location = new Point(60, 40);
        txtProcessId.Size = new Size(200, 30);
        txtProcessId.PlaceholderText = "Enter Process ID (e.g., 1423)";
        this.Controls.Add(txtProcessId);

        // To setup the terminate button
        btnKill = new Button();
        btnKill.Text = "TERMINATE TASK";
        btnKill.Location = new Point(60, 80);
        btnKill.Size = new Size(200, 40);
        btnKill.BackColor = Color.DarkRed;
        btnKill.ForeColor = Color.White;

        // To wire the button click to the C# method
        btnKill.Click += BtnKill_Click;
        this.Controls.Add(btnKill);


        // For the process list
        processList = new ListBox();
        processList.Location = new Point(300, 100);
        processList.Size = new Size(200, 200);
        this.Controls.Add(processList);

        // for the show process list button
        showProcessList = new Button();
        showProcessList.Text = "SHOW PROCESS LIST";
        showProcessList.Location = new Point(300, 10);
        showProcessList.Size = new Size(200, 40);
        showProcessList.BackColor = Color.Blue;
        showProcessList.ForeColor = Color.AntiqueWhite;

        showProcessList.Click += ProcessList_Click;
        this.Controls.Add(showProcessList);

        // To Setup the Search Box
        txtSearch = new TextBox();
        txtSearch.Location = new Point(300, 70);
        txtSearch.Size = new Size(200, 30);
        txtSearch.PlaceholderText = "Search processes...";

        // an event that fires every time the text changes
        txtSearch.TextChanged += TxtSearch_TextChanged;
        this.Controls. Add(txtSearch);


        // To launch a particular program...
        launchBox = new TextBox();
        launchBox.Location = new Point(60, 150);
        launchBox.Size = new Size(200, 30);
        launchBox.PlaceholderText = "enter a file name, e.g, notepad.exe";
        this.Controls.Add(launchBox);

        // now for the launch button
        launchBtn = new Button();
        launchBtn.Text = "RUN TASK";
        launchBtn.Location = new Point(60, 190);
        launchBtn.Size = new Size(200, 40);
        launchBtn.BackColor = Color.Green;
        launchBtn.ForeColor = Color.AliceBlue;

        launchBtn.Click += Launch_Click;
        this.Controls.Add(launchBtn);
        



    }

    // The engine (This replaces the [HttpDelete] API endpoint)
    private void BtnKill_Click(object? sender, EventArgs e)
    {
        try
        {
            // read the raw text from the input box
            string input = txtProcessId.Text;

            // convert the test into a number
            int targetId = int.Parse(input);

            Process process = Process.GetProcessById(targetId);

            process.Kill();

            MessageBox.Show("Target successfully terminated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtProcessId.Clear();
        }
        catch (ArgumentException)
        {
            MessageBox.Show("That Process ID does not exist.", "NotFound", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Execution failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ProcessList_Click(object? sender, EventArgs e)
    {
        try
        {
            Process[] processes = Process.GetProcesses();
            _masterProcessList.Clear();

            foreach (var process in processes)
            {
                var myTask = new SystemTask();

                myTask.ProcessId = process.Id;
                myTask.ProcessName = process.ProcessName;

                _masterProcessList.Add(myTask);
            }

            processList.Items.Clear();

            processList.Items.AddRange(_masterProcessList.ToArray());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Execution failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // The search engine
    private void TxtSearch_TextChanged(object? sender, EventArgs e)
    {
        string searchTerm = txtSearch.Text.ToLower();

        var filteredList = _masterProcessList.Where(task => task.ProcessName.ToLower().Contains(searchTerm)).ToArray();

        processList.Items.Clear();
        processList.Items.AddRange(filteredList);
    }

    private void Launch_Click(object? sender, EventArgs e)
    {
        try
        {
            string taskToLaunch = launchBox.Text;

            if (string.IsNullOrWhiteSpace(taskToLaunch))
            {
                MessageBox.Show("Please enter a program name to launch.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Process newProcess = Process.Start(taskToLaunch);

            if (newProcess != null)
            {
                MessageBox.Show($"Successfully launched {taskToLaunch}!");
            }

        }
        catch (System.ComponentModel.Win32Exception)
        {
            MessageBox.Show("Program does not exist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        catch (System.Exception ex)
        {
            MessageBox.Show($"An error was encountered while running the program: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
    }
}


