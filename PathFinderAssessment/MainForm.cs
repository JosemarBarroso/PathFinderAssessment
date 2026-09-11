// COM 5113 Pathfinding Assessment

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PathFinderAssessment
{
    internal class MainForm : Form
    {
        // -------------------------------------------------------------
        // GUI CONTROLS
        // -------------------------------------------------------------
        private ComboBox cmbMap;
        private ComboBox cmbAlgorithm;

        private Button btnLoadMap;
        private Button btnRunSearch;
        private Button btnStepSearch;
        private Button btnReset;

        private Panel pnlGrid;

        private Label lblStatus;
        private Label lblStart;
        private Label lblGoal;
        private Label lblPathLength;
        private Label lblSortCount;

        private TextBox txtResults;


        // -------------------------------------------------------------
        // MAP DATA
        // -------------------------------------------------------------
        private int[,]? currentMap;
        private Coord currentStart;
        private Coord currentGoal;


        // -------------------------------------------------------------
        // CONSTRUCTOR
        // -------------------------------------------------------------
        public MainForm()
        {
            InitializeForm();
            InitializeControls();
        }


        // -------------------------------------------------------------
        // CONFIGURE MAIN WINDOW
        // -------------------------------------------------------------
        private void InitializeForm()
        {
            Text = "Pathfinding Visualiser";

            Width = 1150;
            Height = 750;

            StartPosition = FormStartPosition.CenterScreen;

            FormBorderStyle = FormBorderStyle.FixedSingle;

            MaximizeBox = false;
        }


        // -------------------------------------------------------------
        // CREATE ALL GUI CONTROLS
        // -------------------------------------------------------------
        private void InitializeControls()
        {
            // =========================================================
            // MAP LABEL
            // =========================================================
            Label lblMap = new Label();

            lblMap.Text = "Terrain Map:";
            lblMap.Location = new Point(20, 20);
            lblMap.AutoSize = true;

            Controls.Add(lblMap);


            // =========================================================
            // MAP COMBO BOX
            // =========================================================
            cmbMap = new ComboBox();

            cmbMap.Location = new Point(120, 17);
            cmbMap.Width = 180;

            cmbMap.DropDownStyle = ComboBoxStyle.DropDownList;

            cmbMap.Items.Add("test1Map.txt");
            cmbMap.Items.Add("test2Map.txt");
            cmbMap.Items.Add("test3Map.txt");
            cmbMap.Items.Add("test4Map.txt");
            cmbMap.Items.Add("test5Map.txt");
            cmbMap.Items.Add("test6Map.txt");

            cmbMap.SelectedIndex = 0;

            Controls.Add(cmbMap);


            // =========================================================
            // ALGORITHM LABEL
            // =========================================================
            Label lblAlgorithm = new Label();

            lblAlgorithm.Text = "Algorithm:";
            lblAlgorithm.Location = new Point(330, 20);
            lblAlgorithm.AutoSize = true;

            Controls.Add(lblAlgorithm);


            // =========================================================
            // ALGORITHM COMBO BOX
            // =========================================================
            cmbAlgorithm = new ComboBox();

            cmbAlgorithm.Location = new Point(410, 17);
            cmbAlgorithm.Width = 190;

            cmbAlgorithm.DropDownStyle = ComboBoxStyle.DropDownList;

            cmbAlgorithm.Items.Add("Breadth First Search");
            cmbAlgorithm.Items.Add("Depth First Search");
            cmbAlgorithm.Items.Add("Hill Climbing");
            cmbAlgorithm.Items.Add("Best First Search");
            cmbAlgorithm.Items.Add("Dijkstra's Search");
            cmbAlgorithm.Items.Add("A* Search");

            cmbAlgorithm.SelectedIndex = 0;

            Controls.Add(cmbAlgorithm);


            // =========================================================
            // LOAD MAP BUTTON
            // =========================================================
            btnLoadMap = new Button();

            btnLoadMap.Text = "Load Map";
            btnLoadMap.Location = new Point(630, 15);
            btnLoadMap.Size = new Size(100, 30);

            btnLoadMap.Click += BtnLoadMap_Click;

            Controls.Add(btnLoadMap);


            // =========================================================
            // RUN SEARCH BUTTON
            // =========================================================
            btnRunSearch = new Button();

            btnRunSearch.Text = "Run Search";
            btnRunSearch.Location = new Point(740, 15);
            btnRunSearch.Size = new Size(100, 30);

            btnRunSearch.Enabled = false;

            btnRunSearch.Click += BtnRunSearch_Click;

            Controls.Add(btnRunSearch);


            // =========================================================
            // STEP SEARCH BUTTON
            // =========================================================
            btnStepSearch = new Button();

            btnStepSearch.Text = "Step Search";
            btnStepSearch.Location = new Point(850, 15);
            btnStepSearch.Size = new Size(100, 30);

            // Step-by-step search will be implemented later.
            btnStepSearch.Enabled = false;

            btnStepSearch.Click += BtnStepSearch_Click;

            Controls.Add(btnStepSearch);


            // =========================================================
            // RESET BUTTON
            // =========================================================
            btnReset = new Button();

            btnReset.Text = "Reset";
            btnReset.Location = new Point(960, 15);
            btnReset.Size = new Size(100, 30);

            btnReset.Click += BtnReset_Click;

            Controls.Add(btnReset);


            // =========================================================
            // GRID PANEL
            // =========================================================
            pnlGrid = new Panel();

            pnlGrid.Location = new Point(20, 70);
            pnlGrid.Size = new Size(700, 600);

            pnlGrid.BorderStyle = BorderStyle.FixedSingle;
            pnlGrid.BackColor = Color.White;

            Controls.Add(pnlGrid);


            // =========================================================
            // INFORMATION LABELS
            // =========================================================
            lblStatus = new Label();

            lblStatus.Text = "Status: No map loaded";
            lblStatus.Location = new Point(750, 80);
            lblStatus.AutoSize = true;

            Controls.Add(lblStatus);


            lblStart = new Label();

            lblStart.Text = "Start: -";
            lblStart.Location = new Point(750, 115);
            lblStart.AutoSize = true;

            Controls.Add(lblStart);


            lblGoal = new Label();

            lblGoal.Text = "Goal: -";
            lblGoal.Location = new Point(750, 145);
            lblGoal.AutoSize = true;

            Controls.Add(lblGoal);


            lblPathLength = new Label();

            lblPathLength.Text = "Path Length: -";
            lblPathLength.Location = new Point(750, 175);
            lblPathLength.AutoSize = true;

            Controls.Add(lblPathLength);


            lblSortCount = new Label();

            lblSortCount.Text = "A* Open List Sort Count: -";
            lblSortCount.Location = new Point(750, 205);
            lblSortCount.AutoSize = true;

            Controls.Add(lblSortCount);


            // =========================================================
            // RESULTS TEXTBOX
            // =========================================================
            txtResults = new TextBox();

            txtResults.Location = new Point(750, 250);
            txtResults.Size = new Size(350, 420);

            txtResults.Multiline = true;
            txtResults.ScrollBars = ScrollBars.Vertical;
            txtResults.ReadOnly = true;

            Controls.Add(txtResults);
        }


        // -------------------------------------------------------------
        // LOAD MAP
        // -------------------------------------------------------------
        private void BtnLoadMap_Click(object? sender, EventArgs e)
        {
            try
            {
                // Get the map selected by the user.
                string? selectedMap =
                    cmbMap.SelectedItem?.ToString();

                if (string.IsNullOrWhiteSpace(selectedMap))
                {
                    MessageBox.Show(
                        "Please select a terrain map.",
                        "Map Selection",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }


                // Build the path to the Maps folder.
                string mapFilePath =
                    Path.Combine(
                        AppContext.BaseDirectory,
                        "Maps",
                        selectedMap);


                // Load the map using the existing MapLoader class.
                currentMap =
                    MapLoader.LoadMap(
                        mapFilePath,
                        out currentStart,
                        out currentGoal);


                // Draw the terrain.
                DrawMap();


                // Update information displayed on the right.
                lblStatus.Text =
                    $"Status: {selectedMap} loaded";

                lblStart.Text =
                    $"Start: ({currentStart.Row}, {currentStart.Col})";

                lblGoal.Text =
                    $"Goal: ({currentGoal.Row}, {currentGoal.Col})";

                lblPathLength.Text =
                    "Path Length: -";

                lblSortCount.Text =
                    "A* Open List Sort Count: -";


                txtResults.Clear();

                txtResults.AppendText(
                    $"Map: {selectedMap}{Environment.NewLine}");

                txtResults.AppendText(
                    $"Rows: {currentMap.GetLength(0)}{Environment.NewLine}");

                txtResults.AppendText(
                    $"Columns: {currentMap.GetLength(1)}{Environment.NewLine}");

                txtResults.AppendText(
                    $"Start: ({currentStart.Row}, {currentStart.Col}){Environment.NewLine}");

                txtResults.AppendText(
                    $"Goal: ({currentGoal.Row}, {currentGoal.Col}){Environment.NewLine}");


                // A map is now available, so a complete search can run.
                btnRunSearch.Enabled = true;

                // We will enable this when incremental search is implemented.
                btnStepSearch.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"The map could not be loaded.{Environment.NewLine}{Environment.NewLine}{ex.Message}",
                    "Map Loading Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                lblStatus.Text =
                    "Status: Map loading failed";

                btnRunSearch.Enabled = false;
                btnStepSearch.Enabled = false;
            }
        }


        // -------------------------------------------------------------
        // DRAW TERRAIN MAP
        // -------------------------------------------------------------
        private void DrawMap()
        {
            if (currentMap == null)
            {
                return;
            }


            // Remove any previously displayed map.
            pnlGrid.Controls.Clear();


            int rows =
                currentMap.GetLength(0);

            int columns =
                currentMap.GetLength(1);


            // Calculate a square cell size dynamically.
            // This allows maps of different dimensions to fit the panel.
            int cellWidth =
                pnlGrid.ClientSize.Width / columns;

            int cellHeight =
                pnlGrid.ClientSize.Height / rows;

            int cellSize =
                Math.Min(cellWidth, cellHeight);


            // Centre the grid inside the panel.
            int gridWidth =
                cellSize * columns;

            int gridHeight =
                cellSize * rows;

            int offsetX =
                (pnlGrid.ClientSize.Width - gridWidth) / 2;

            int offsetY =
                (pnlGrid.ClientSize.Height - gridHeight) / 2;


            // Create one Label for each map coordinate.
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    Label cell =
                        new Label();

                    cell.Size =
                        new Size(cellSize, cellSize);

                    cell.Location =
                        new Point(
                            offsetX + (column * cellSize),
                            offsetY + (row * cellSize));

                    cell.BorderStyle =
                        BorderStyle.FixedSingle;

                    cell.TextAlign =
                        ContentAlignment.MiddleCenter;

                    cell.Font =
                        new Font(
                            "Segoe UI",
                            Math.Max(8, cellSize / 4),
                            FontStyle.Bold);


                    int terrain =
                        currentMap[row, column];


                    // -------------------------------------------------
                    // TERRAIN APPEARANCE
                    // -------------------------------------------------
                    switch (terrain)
                    {
                        // Wall / non-traversable
                        case 0:
                            cell.BackColor = Color.Black;
                            cell.ForeColor = Color.White;
                            cell.Text = "0";
                            break;

                        // Open terrain - cost 1
                        case 1:
                            cell.BackColor = Color.White;
                            cell.ForeColor = Color.Black;
                            cell.Text = "1";
                            break;

                        // Woodland - cost 2
                        case 2:
                            cell.BackColor = Color.LightGreen;
                            cell.ForeColor = Color.Black;
                            cell.Text = "2";
                            break;

                        // Water - cost 3
                        case 3:
                            cell.BackColor = Color.LightBlue;
                            cell.ForeColor = Color.Black;
                            cell.Text = "3";
                            break;

                        default:
                            cell.BackColor = Color.Gray;
                            cell.ForeColor = Color.White;
                            cell.Text = terrain.ToString();
                            break;
                    }


                    // -------------------------------------------------
                    // START POSITION
                    // -------------------------------------------------
                    if (row == currentStart.Row &&
                        column == currentStart.Col)
                    {
                        cell.BackColor = Color.LimeGreen;
                        cell.ForeColor = Color.Black;
                        cell.Text = "S";
                    }


                    // -------------------------------------------------
                    // GOAL POSITION
                    // -------------------------------------------------
                    if (row == currentGoal.Row &&
                        column == currentGoal.Col)
                    {
                        cell.BackColor = Color.OrangeRed;
                        cell.ForeColor = Color.White;
                        cell.Text = "G";
                    }


                    pnlGrid.Controls.Add(cell);
                }
            }
        }


        // -------------------------------------------------------------
        // RUN COMPLETE SEARCH
        // -------------------------------------------------------------
        private void BtnRunSearch_Click(object? sender, EventArgs e)
        {
            // We will connect the existing search algorithms
            // in the next stage.

            lblStatus.Text =
                "Status: Run Search clicked";
        }


        // -------------------------------------------------------------
        // STEP SEARCH
        // -------------------------------------------------------------
        private void BtnStepSearch_Click(object? sender, EventArgs e)
        {
            // Incremental search will be implemented after
            // the normal Run Search function is working.

            lblStatus.Text =
                "Status: Step Search clicked";
        }


        // -------------------------------------------------------------
        // RESET GUI
        // -------------------------------------------------------------
        private void BtnReset_Click(object? sender, EventArgs e)
        {
            currentMap = null;

            pnlGrid.Controls.Clear();

            txtResults.Clear();

            lblStatus.Text =
                "Status: No map loaded";

            lblStart.Text =
                "Start: -";

            lblGoal.Text =
                "Goal: -";

            lblPathLength.Text =
                "Path Length: -";

            lblSortCount.Text =
                "A* Open List Sort Count: -";

            btnRunSearch.Enabled = false;

            btnStepSearch.Enabled = false;
        }
    }
}