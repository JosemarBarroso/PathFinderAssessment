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
        private ComboBox cmbMap = null!;
        private ComboBox cmbAlgorithm = null!;

        private Button btnLoadMap = null!;
        private Button btnRunSearch = null!;
        private Button btnStepSearch = null!;
        private Button btnReset = null!;

        private Panel pnlGrid = null!;

        private Label lblStatus = null!;
        private Label lblStart = null!;
        private Label lblGoal = null!;
        private Label lblPathLength = null!;
        private Label lblSortCount = null!;

        private TextBox txtResults = null!;


        // -------------------------------------------------------------
        // MAP DATA
        // -------------------------------------------------------------
        private int[,]? currentMap;

        private Coord currentStart;
        private Coord currentGoal;

        private string? currentMapFileName;

        private LinkedList<Coord>? currentPath;


        // -------------------------------------------------------------
        // STEP-BY-STEP SEARCH DATA
        // -------------------------------------------------------------
        private SteppablePathFinderInterface? currentStepPathFinder;

        private bool stepSearchStarted = false;

        private int stepNumber = 0;


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

            StartPosition =
                FormStartPosition.CenterScreen;

            FormBorderStyle =
                FormBorderStyle.FixedSingle;

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
            Label lblMap =
                new Label();

            lblMap.Text =
                "Terrain Map:";

            lblMap.Location =
                new Point(20, 20);

            lblMap.AutoSize = true;

            Controls.Add(lblMap);


            // =========================================================
            // MAP COMBO BOX
            // =========================================================
            cmbMap =
                new ComboBox();

            cmbMap.Location =
                new Point(120, 17);

            cmbMap.Width = 180;

            cmbMap.DropDownStyle =
                ComboBoxStyle.DropDownList;

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
            Label lblAlgorithm =
                new Label();

            lblAlgorithm.Text =
                "Algorithm:";

            lblAlgorithm.Location =
                new Point(330, 20);

            lblAlgorithm.AutoSize = true;

            Controls.Add(lblAlgorithm);


            // =========================================================
            // ALGORITHM COMBO BOX
            // =========================================================
            cmbAlgorithm =
                new ComboBox();

            cmbAlgorithm.Location =
                new Point(410, 17);

            cmbAlgorithm.Width = 190;

            cmbAlgorithm.DropDownStyle =
                ComboBoxStyle.DropDownList;

            cmbAlgorithm.Items.Add(
                "Breadth First Search");

            cmbAlgorithm.Items.Add(
                "Depth First Search");

            cmbAlgorithm.Items.Add(
                "Hill Climbing");

            cmbAlgorithm.Items.Add(
                "Best First Search");

            cmbAlgorithm.Items.Add(
                "Dijkstra's Search");

            cmbAlgorithm.Items.Add(
                "A* Search");


            // Default algorithm is BFS.
            cmbAlgorithm.SelectedIndex = 0;


            cmbAlgorithm.SelectedIndexChanged +=
                CmbAlgorithm_SelectedIndexChanged;


            Controls.Add(cmbAlgorithm);


            // =========================================================
            // LOAD MAP BUTTON
            // =========================================================
            btnLoadMap =
                new Button();

            btnLoadMap.Text =
                "Load Map";

            btnLoadMap.Location =
                new Point(630, 15);

            btnLoadMap.Size =
                new Size(100, 30);

            btnLoadMap.Click +=
                BtnLoadMap_Click;

            Controls.Add(btnLoadMap);


            // =========================================================
            // RUN SEARCH BUTTON
            // =========================================================
            btnRunSearch =
                new Button();

            btnRunSearch.Text =
                "Run Search";

            btnRunSearch.Location =
                new Point(740, 15);

            btnRunSearch.Size =
                new Size(100, 30);

            btnRunSearch.Enabled =
                false;

            btnRunSearch.Click +=
                BtnRunSearch_Click;

            Controls.Add(btnRunSearch);


            // =========================================================
            // STEP SEARCH BUTTON
            // =========================================================
            btnStepSearch =
                new Button();

            btnStepSearch.Text =
                "Step Search";

            btnStepSearch.Location =
                new Point(850, 15);

            btnStepSearch.Size =
                new Size(100, 30);

            btnStepSearch.Enabled =
                false;

            btnStepSearch.Click +=
                BtnStepSearch_Click;

            Controls.Add(btnStepSearch);


            // =========================================================
            // RESET BUTTON
            // =========================================================
            btnReset =
                new Button();

            btnReset.Text =
                "Reset";

            btnReset.Location =
                new Point(960, 15);

            btnReset.Size =
                new Size(100, 30);

            btnReset.Click +=
                BtnReset_Click;

            Controls.Add(btnReset);


            // =========================================================
            // GRID PANEL
            // =========================================================
            pnlGrid =
                new Panel();

            pnlGrid.Location =
                new Point(20, 70);

            pnlGrid.Size =
                new Size(700, 600);

            pnlGrid.BorderStyle =
                BorderStyle.FixedSingle;

            pnlGrid.BackColor =
                Color.White;

            Controls.Add(pnlGrid);


            // =========================================================
            // INFORMATION LABELS
            // =========================================================
            lblStatus =
                new Label();

            lblStatus.Text =
                "Status: No map loaded";

            lblStatus.Location =
                new Point(750, 80);

            lblStatus.AutoSize =
                true;

            Controls.Add(lblStatus);


            lblStart =
                new Label();

            lblStart.Text =
                "Start: -";

            lblStart.Location =
                new Point(750, 115);

            lblStart.AutoSize =
                true;

            Controls.Add(lblStart);


            lblGoal =
                new Label();

            lblGoal.Text =
                "Goal: -";

            lblGoal.Location =
                new Point(750, 145);

            lblGoal.AutoSize =
                true;

            Controls.Add(lblGoal);


            lblPathLength =
                new Label();

            lblPathLength.Text =
                "Path Length: -";

            lblPathLength.Location =
                new Point(750, 175);

            lblPathLength.AutoSize =
                true;

            Controls.Add(lblPathLength);


            lblSortCount =
                new Label();

            lblSortCount.Text =
                "A* Open List Sort Count: -";

            lblSortCount.Location =
                new Point(750, 205);

            lblSortCount.AutoSize =
                true;

            Controls.Add(lblSortCount);


            // =========================================================
            // RESULTS TEXTBOX
            // =========================================================
            txtResults =
                new TextBox();

            txtResults.Location =
                new Point(750, 250);

            txtResults.Size =
                new Size(350, 420);

            txtResults.Multiline =
                true;

            txtResults.ScrollBars =
                ScrollBars.Vertical;

            txtResults.ReadOnly =
                true;

            Controls.Add(txtResults);
        }


        // -------------------------------------------------------------
        // LOAD MAP
        // -------------------------------------------------------------
        private void BtnLoadMap_Click(
            object? sender,
            EventArgs e)
        {
            try
            {
                string? selectedMap =
                    cmbMap.SelectedItem?.ToString();


                if (string.IsNullOrWhiteSpace(
                    selectedMap))
                {
                    MessageBox.Show(
                        "Please select a terrain map.",
                        "Map Selection",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }


                currentMapFileName =
                    selectedMap;


                string mapFilePath =
                    Path.Combine(
                        AppContext.BaseDirectory,
                        "Maps",
                        selectedMap);


                currentMap =
                    MapLoader.LoadMap(
                        mapFilePath,
                        out currentStart,
                        out currentGoal);


                currentPath =
                    null;


                ResetStepSearch();


                DrawMap();


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
                    $"Map: {selectedMap}" +
                    Environment.NewLine);


                txtResults.AppendText(
                    $"Rows: {currentMap.GetLength(0)}" +
                    Environment.NewLine);


                txtResults.AppendText(
                    $"Columns: {currentMap.GetLength(1)}" +
                    Environment.NewLine);


                txtResults.AppendText(
                    $"Start: ({currentStart.Row}, {currentStart.Col})" +
                    Environment.NewLine);


                txtResults.AppendText(
                    $"Goal: ({currentGoal.Row}, {currentGoal.Col})" +
                    Environment.NewLine);


                btnRunSearch.Enabled =
                    true;


                // -----------------------------------------------------
                // STEP SEARCH SUPPORTED ALGORITHMS
                //
                // 0 = BFS
                // 1 = DFS
                // 2 = Hill Climbing
                // 3 = Best First Search
                // -----------------------------------------------------
                btnStepSearch.Enabled =
                    cmbAlgorithm.SelectedIndex == 0 ||
                    cmbAlgorithm.SelectedIndex == 1 ||
                    cmbAlgorithm.SelectedIndex == 2 ||
                    cmbAlgorithm.SelectedIndex == 3;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"The map could not be loaded." +
                    $"{Environment.NewLine}{Environment.NewLine}" +
                    ex.Message,
                    "Map Loading Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);


                lblStatus.Text =
                    "Status: Map loading failed";


                btnRunSearch.Enabled =
                    false;


                btnStepSearch.Enabled =
                    false;
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


            pnlGrid.Controls.Clear();


            int rows =
                currentMap.GetLength(0);


            int columns =
                currentMap.GetLength(1);


            int cellWidth =
                pnlGrid.ClientSize.Width /
                columns;


            int cellHeight =
                pnlGrid.ClientSize.Height /
                rows;


            int cellSize =
                Math.Min(
                    cellWidth,
                    cellHeight);


            int gridWidth =
                cellSize *
                columns;


            int gridHeight =
                cellSize *
                rows;


            int offsetX =
                (pnlGrid.ClientSize.Width -
                 gridWidth) / 2;


            int offsetY =
                (pnlGrid.ClientSize.Height -
                 gridHeight) / 2;


            for (int row = 0;
                 row < rows;
                 row++)
            {
                for (int column = 0;
                     column < columns;
                     column++)
                {
                    Label cell =
                        new Label();


                    cell.Size =
                        new Size(
                            cellSize,
                            cellSize);


                    cell.Location =
                        new Point(
                            offsetX +
                            (column * cellSize),

                            offsetY +
                            (row * cellSize));


                    cell.BorderStyle =
                        BorderStyle.FixedSingle;


                    cell.TextAlign =
                        ContentAlignment.MiddleCenter;


                    cell.Font =
                        new Font(
                            "Segoe UI",
                            Math.Max(
                                8,
                                cellSize / 4),
                            FontStyle.Bold);


                    // Store coordinate in Tag for later lookup.
                    cell.Tag =
                        new Coord(
                            row,
                            column);


                    int terrain =
                        currentMap[
                            row,
                            column];


                    // -------------------------------------------------
                    // TERRAIN DISPLAY
                    // -------------------------------------------------
                    switch (terrain)
                    {
                        case 0:

                            cell.BackColor =
                                Color.Black;

                            cell.ForeColor =
                                Color.White;

                            cell.Text =
                                "0";

                            break;


                        case 1:

                            cell.BackColor =
                                Color.White;

                            cell.ForeColor =
                                Color.Black;

                            cell.Text =
                                "1";

                            break;


                        case 2:

                            cell.BackColor =
                                Color.LightGreen;

                            cell.ForeColor =
                                Color.Black;

                            cell.Text =
                                "2";

                            break;


                        case 3:

                            cell.BackColor =
                                Color.LightBlue;

                            cell.ForeColor =
                                Color.Black;

                            cell.Text =
                                "3";

                            break;


                        default:

                            cell.BackColor =
                                Color.Gray;

                            cell.ForeColor =
                                Color.White;

                            cell.Text =
                                terrain.ToString();

                            break;
                    }


                    // -------------------------------------------------
                    // START
                    // -------------------------------------------------
                    if (row ==
                        currentStart.Row &&
                        column ==
                        currentStart.Col)
                    {
                        cell.BackColor =
                            Color.LimeGreen;

                        cell.ForeColor =
                            Color.Black;

                        cell.Text =
                            "S";
                    }


                    // -------------------------------------------------
                    // GOAL
                    // -------------------------------------------------
                    if (row ==
                        currentGoal.Row &&
                        column ==
                        currentGoal.Col)
                    {
                        cell.BackColor =
                            Color.OrangeRed;

                        cell.ForeColor =
                            Color.White;

                        cell.Text =
                            "G";
                    }


                    pnlGrid.Controls.Add(
                        cell);
                }
            }
        }


        // -------------------------------------------------------------
        // RUN COMPLETE SEARCH
        // -------------------------------------------------------------
        private void BtnRunSearch_Click(
            object? sender,
            EventArgs e)
        {
            if (currentMap == null ||
                string.IsNullOrWhiteSpace(
                    currentMapFileName))
            {
                MessageBox.Show(
                    "Please load a map before running a search.",
                    "No Map Loaded",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }


            try
            {
                // Stop previous step-by-step search.
                ResetStepSearch();


                DrawMap();


                Algorithm selectedAlgorithm =
                    GetSelectedAlgorithm();


                PathFinderInterface pathFinder =
                    PathFinderFactory.NewPathFinder(
                        selectedAlgorithm);


                LinkedList<Coord> path =
                    new LinkedList<Coord>();


                string algorithmName =
                    GetAlgorithmName(
                        selectedAlgorithm);


                lblStatus.Text =
                    $"Status: Running {algorithmName}...";


                Application.DoEvents();


                bool pathFound =
                    pathFinder.FindPath(
                        currentMap,
                        currentStart,
                        currentGoal,
                        ref path);


                if (pathFound)
                {
                    currentPath =
                        path;


                    HighlightPath(
                        path);


                    lblStatus.Text =
                        $"Status: Path found using {algorithmName}";


                    lblPathLength.Text =
                        $"Path Length: {path.Count()}";


                    txtResults.Clear();


                    txtResults.AppendText(
                        $"Algorithm: {algorithmName}" +
                        Environment.NewLine);


                    txtResults.AppendText(
                        $"Start: ({currentStart.Row}, {currentStart.Col})" +
                        Environment.NewLine);


                    txtResults.AppendText(
                        $"Goal: ({currentGoal.Row}, {currentGoal.Col})" +
                        Environment.NewLine);


                    txtResults.AppendText(
                        $"Path Length: {path.Count()}" +
                        Environment.NewLine);


                    txtResults.AppendText(
                        Environment.NewLine +
                        "Path:" +
                        Environment.NewLine);


                    path.ForEach(
                        coordinate =>
                        {
                            txtResults.AppendText(
                                $"({coordinate.Row}, {coordinate.Col})" +
                                Environment.NewLine);
                        });


                    // -------------------------------------------------
                    // A* OPEN LIST SORT COUNT
                    // -------------------------------------------------
                    int? sortCount =
                        null;


                    if (pathFinder is AStar aStar)
                    {
                        sortCount =
                            aStar.OpenListSortCount;


                        lblSortCount.Text =
                            $"A* Open List Sort Count: " +
                            $"{aStar.OpenListSortCount}";


                        txtResults.AppendText(
                            Environment.NewLine +
                            $"Open List sort count: " +
                            $"{aStar.OpenListSortCount}" +
                            Environment.NewLine);
                    }
                    else
                    {
                        lblSortCount.Text =
                            "A* Open List Sort Count: -";
                    }


                    // -------------------------------------------------
                    // WRITE RESULT FILE
                    // -------------------------------------------------
                    string outputFile =
                        PathWriter.WritePath(
                            currentMapFileName,
                            algorithmName,
                            path,
                            sortCount);


                    txtResults.AppendText(
                        Environment.NewLine +
                        "Output file:" +
                        Environment.NewLine +
                        outputFile);
                }
                else
                {
                    currentPath =
                        null;


                    lblStatus.Text =
                        $"Status: No path found using {algorithmName}";


                    lblPathLength.Text =
                        "Path Length: 0";


                    if (pathFinder is AStar aStar)
                    {
                        lblSortCount.Text =
                            $"A* Open List Sort Count: " +
                            $"{aStar.OpenListSortCount}";
                    }
                    else
                    {
                        lblSortCount.Text =
                            "A* Open List Sort Count: -";
                    }


                    txtResults.Clear();


                    txtResults.AppendText(
                        $"Algorithm: {algorithmName}" +
                        Environment.NewLine);


                    txtResults.AppendText(
                        "No path could be found.");
                }


                // -----------------------------------------------------
                // RE-ENABLE STEP SEARCH
                // BFS, DFS, Hill Climbing and Best First Search
                // -----------------------------------------------------
                btnStepSearch.Enabled =
                    cmbAlgorithm.SelectedIndex == 0 ||
                    cmbAlgorithm.SelectedIndex == 1 ||
                    cmbAlgorithm.SelectedIndex == 2 ||
                    cmbAlgorithm.SelectedIndex == 3;
            }
            catch (Exception ex)
            {
                lblStatus.Text =
                    "Status: Search failed";


                MessageBox.Show(
                    $"The search could not be completed." +
                    $"{Environment.NewLine}{Environment.NewLine}" +
                    ex.Message,
                    "Search Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        // -------------------------------------------------------------
        // ALGORITHM SELECTION CHANGED
        // -------------------------------------------------------------
        private void CmbAlgorithm_SelectedIndexChanged(
            object? sender,
            EventArgs e)
        {
            ResetStepSearch();


            if (currentMap != null)
            {
                DrawMap();


                currentPath =
                    null;


                lblPathLength.Text =
                    "Path Length: -";


                lblSortCount.Text =
                    "A* Open List Sort Count: -";


                lblStatus.Text =
                    $"Status: {currentMapFileName} loaded";
            }


            // ---------------------------------------------------------
            // Step-by-step visualisation currently supports:
            //
            // BFS
            // DFS
            // Hill Climbing
            // Best First Search
            // ---------------------------------------------------------
            btnStepSearch.Enabled =
                currentMap != null &&
                (cmbAlgorithm.SelectedIndex == 0 ||
                 cmbAlgorithm.SelectedIndex == 1 ||
                 cmbAlgorithm.SelectedIndex == 2 ||
                 cmbAlgorithm.SelectedIndex == 3);
        }


        // -------------------------------------------------------------
        // GET SELECTED ALGORITHM
        // -------------------------------------------------------------
        private Algorithm GetSelectedAlgorithm()
        {
            switch (
                cmbAlgorithm.SelectedIndex)
            {
                case 0:

                    return
                        Algorithm.BreadthFirst;


                case 1:

                    return
                        Algorithm.DepthFirst;


                case 2:

                    return
                        Algorithm.HillClimbing;


                case 3:

                    return
                        Algorithm.BestFirst;


                case 4:

                    return
                        Algorithm.Dijkstras;


                case 5:

                    return
                        Algorithm.AStar;


                default:

                    return
                        Algorithm.BreadthFirst;
            }
        }


        // -------------------------------------------------------------
        // GET ALGORITHM DISPLAY NAME
        // -------------------------------------------------------------
        private string GetAlgorithmName(
            Algorithm algorithm)
        {
            switch (algorithm)
            {
                case Algorithm.BreadthFirst:

                    return
                        "Breadth First Search";


                case Algorithm.DepthFirst:

                    return
                        "Depth First Search";


                case Algorithm.HillClimbing:

                    return
                        "Hill Climbing Search";


                case Algorithm.BestFirst:

                    return
                        "Best First Search";


                case Algorithm.Dijkstras:

                    return
                        "Dijkstra's Search";


                case Algorithm.AStar:

                    return
                        "A* Search";


                default:

                    return
                        "Unknown Search";
            }
        }


        // -------------------------------------------------------------
        // HIGHLIGHT FINAL PATH
        // -------------------------------------------------------------
        private void HighlightPath(
            LinkedList<Coord> path)
        {
            path.ForEach(
                coordinate =>
                {
                    // Do not replace start or goal formatting.
                    if (IsStartOrGoal(
                        coordinate))
                    {
                        return;
                    }


                    Label? cell =
                        FindGridCell(
                            coordinate);


                    if (cell != null)
                    {
                        cell.BackColor =
                            Color.Gold;


                        cell.ForeColor =
                            Color.Black;


                        cell.Text =
                            "P";
                    }
                });
        }


        // -------------------------------------------------------------
        // STEP SEARCH
        // -------------------------------------------------------------
        private void BtnStepSearch_Click(
            object? sender,
            EventArgs e)
        {
            if (currentMap == null)
            {
                MessageBox.Show(
                    "Please load a map before starting Step Search.",
                    "No Map Loaded",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }


            // ---------------------------------------------------------
            // Currently implemented step algorithms:
            //
            // 0 = BFS
            // 1 = DFS
            // 2 = Hill Climbing
            // 3 = Best First Search
            // ---------------------------------------------------------
            if (cmbAlgorithm.SelectedIndex != 0 &&
                cmbAlgorithm.SelectedIndex != 1 &&
                cmbAlgorithm.SelectedIndex != 2 &&
                cmbAlgorithm.SelectedIndex != 3)
            {
                MessageBox.Show(
                    "Step-by-step visualisation is currently available " +
                    "for Breadth First Search, Depth First Search, " +
                    "Hill Climbing and Best First Search.",
                    "Step Search",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }


            try
            {
                // -----------------------------------------------------
                // Determine algorithm dynamically.
                // -----------------------------------------------------
                Algorithm selectedAlgorithm =
                    GetSelectedAlgorithm();


                string algorithmName =
                    GetAlgorithmName(
                        selectedAlgorithm);


                // =====================================================
                // FIRST CLICK - INITIALISE SEARCH
                // =====================================================
                if (!stepSearchStarted)
                {
                    PathFinderInterface pathFinder =
                        PathFinderFactory.NewPathFinder(
                            selectedAlgorithm);


                    currentStepPathFinder =
                        pathFinder as
                        SteppablePathFinderInterface;


                    if (currentStepPathFinder == null)
                    {
                        throw new InvalidOperationException(
                            $"{algorithmName} does not support " +
                            "step-by-step execution.");
                    }


                    currentStepPathFinder.InitialiseStepSearch(
                        currentMap,
                        currentStart,
                        currentGoal);


                    stepSearchStarted =
                        true;


                    stepNumber =
                        0;


                    currentPath =
                        null;


                    DrawMap();


                    lblPathLength.Text =
                        "Path Length: -";


                    lblSortCount.Text =
                        "A* Open List Sort Count: -";


                    txtResults.Clear();


                    txtResults.AppendText(
                        $"{algorithmName} - Step Visualisation" +
                        Environment.NewLine);


                    txtResults.AppendText(
                        "====================================" +
                        Environment.NewLine);


                    txtResults.AppendText(
                        $"Start: ({currentStart.Row}, {currentStart.Col})" +
                        Environment.NewLine);


                    txtResults.AppendText(
                        $"Goal: ({currentGoal.Row}, {currentGoal.Col})" +
                        Environment.NewLine +
                        Environment.NewLine);
                }


                // =====================================================
                // EXECUTE EXACTLY ONE SEARCH EXPANSION
                // =====================================================
                SearchStepResult result =
                    currentStepPathFinder!.Step();


                stepNumber++;


                // Redraw original terrain.
                DrawMap();


                // Add Open, Closed and Current overlays.
                DrawSearchStep(
                    result);


                lblStatus.Text =
                    $"Status: {algorithmName} Step {stepNumber}";


                // =====================================================
                // DISPLAY STEP INFORMATION
                // =====================================================
                txtResults.AppendText(
                    $"Step {stepNumber}" +
                    Environment.NewLine);


                if (result.CurrentNode.HasValue)
                {
                    Coord current =
                        result.CurrentNode.Value;


                    txtResults.AppendText(
                        $"Expanded: ({current.Row}, {current.Col})" +
                        Environment.NewLine);
                }


                txtResults.AppendText(
                    $"Open List: {result.OpenList.Count()}" +
                    Environment.NewLine);


                txtResults.AppendText(
                    $"Closed List: {result.ClosedList.Count()}" +
                    Environment.NewLine);


                txtResults.AppendText(
                    "------------------------------------" +
                    Environment.NewLine);


                txtResults.SelectionStart =
                    txtResults.Text.Length;


                txtResults.ScrollToCaret();


                // =====================================================
                // SEARCH COMPLETE
                // =====================================================
                if (result.IsComplete)
                {
                    btnStepSearch.Enabled =
                        false;


                    if (result.PathFound &&
                        result.Path != null)
                    {
                        currentPath =
                            result.Path;


                        // Draw final path.
                        HighlightPath(
                            result.Path);


                        lblStatus.Text =
                            $"Status: {algorithmName} completed in " +
                            $"{stepNumber} steps";


                        lblPathLength.Text =
                            $"Path Length: {result.Path.Count()}";


                        txtResults.AppendText(
                            Environment.NewLine +
                            "GOAL REACHED" +
                            Environment.NewLine);


                        txtResults.AppendText(
                            $"Path Length: {result.Path.Count()}" +
                            Environment.NewLine);


                        txtResults.AppendText(
                            Environment.NewLine +
                            "Final Path:" +
                            Environment.NewLine);


                        result.Path.ForEach(
                            coordinate =>
                            {
                                txtResults.AppendText(
                                    $"({coordinate.Row}, {coordinate.Col})" +
                                    Environment.NewLine);
                            });


                        // ---------------------------------------------
                        // WRITE PATH OUTPUT FILE
                        // ---------------------------------------------
                        if (!string.IsNullOrWhiteSpace(
                            currentMapFileName))
                        {
                            string outputFile =
                                PathWriter.WritePath(
                                    currentMapFileName,
                                    algorithmName,
                                    result.Path);


                            txtResults.AppendText(
                                Environment.NewLine +
                                "Output file:" +
                                Environment.NewLine +
                                outputFile);
                        }
                    }
                    else
                    {
                        lblStatus.Text =
                            $"Status: {algorithmName} completed - " +
                            "no path found";


                        lblPathLength.Text =
                            "Path Length: 0";


                        txtResults.AppendText(
                            Environment.NewLine +
                            "SEARCH COMPLETE - NO PATH FOUND" +
                            Environment.NewLine);
                    }
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text =
                    "Status: Step search failed";


                MessageBox.Show(
                    $"Step search could not be completed." +
                    $"{Environment.NewLine}{Environment.NewLine}" +
                    ex.Message,
                    "Step Search Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        // -------------------------------------------------------------
        // DRAW STEP-BY-STEP SEARCH STATE
        // -------------------------------------------------------------
        private void DrawSearchStep(
            SearchStepResult result)
        {
            // =========================================================
            // CLOSED
            // =========================================================
            result.ClosedList.ForEach(
                coordinate =>
                {
                    Label? cell =
                        FindGridCell(
                            coordinate);


                    if (cell == null ||
                        IsStartOrGoal(
                            coordinate))
                    {
                        return;
                    }


                    cell.BackColor =
                        Color.LightGray;


                    cell.ForeColor =
                        Color.Black;


                    cell.Text =
                        "C";
                });


            // =========================================================
            // OPEN
            // =========================================================
            result.OpenList.ForEach(
                coordinate =>
                {
                    Label? cell =
                        FindGridCell(
                            coordinate);


                    if (cell == null ||
                        IsStartOrGoal(
                            coordinate))
                    {
                        return;
                    }


                    cell.BackColor =
                        Color.Khaki;


                    cell.ForeColor =
                        Color.Black;


                    cell.Text =
                        "O";
                });


            // =========================================================
            // CURRENT NODE
            // =========================================================
            if (result.CurrentNode.HasValue)
            {
                Coord current =
                    result.CurrentNode.Value;


                Label? currentCell =
                    FindGridCell(
                        current);


                if (currentCell != null &&
                    !IsStartOrGoal(
                        current))
                {
                    currentCell.BackColor =
                        Color.Orange;


                    currentCell.ForeColor =
                        Color.Black;


                    currentCell.Text =
                        "X";
                }
            }
        }


        // -------------------------------------------------------------
        // FIND GRID CELL BY COORDINATE
        // -------------------------------------------------------------
        private Label? FindGridCell(
            Coord coordinate)
        {
            foreach (
                Control control
                in pnlGrid.Controls)
            {
                if (control is Label cell &&
                    cell.Tag is Coord cellCoordinate &&
                    cellCoordinate.Row ==
                    coordinate.Row &&
                    cellCoordinate.Col ==
                    coordinate.Col)
                {
                    return cell;
                }
            }


            return null;
        }


        // -------------------------------------------------------------
        // CHECK WHETHER COORDINATE IS START OR GOAL
        // -------------------------------------------------------------
        private bool IsStartOrGoal(
            Coord coordinate)
        {
            bool isStart =
                coordinate.Row ==
                currentStart.Row &&
                coordinate.Col ==
                currentStart.Col;


            bool isGoal =
                coordinate.Row ==
                currentGoal.Row &&
                coordinate.Col ==
                currentGoal.Col;


            return
                isStart ||
                isGoal;
        }


        // -------------------------------------------------------------
        // RESET STEP SEARCH
        // -------------------------------------------------------------
        private void ResetStepSearch()
        {
            currentStepPathFinder =
                null;


            stepSearchStarted =
                false;


            stepNumber =
                0;
        }


        // -------------------------------------------------------------
        // RESET GUI
        // -------------------------------------------------------------
        private void BtnReset_Click(
            object? sender,
            EventArgs e)
        {
            ResetStepSearch();


            currentMap =
                null;


            currentPath =
                null;


            currentMapFileName =
                null;


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


            btnRunSearch.Enabled =
                false;


            btnStepSearch.Enabled =
                false;
        }
    }
}