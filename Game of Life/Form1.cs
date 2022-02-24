using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Game_of_Life
{
    public partial class Form1 : Form
    {
        int seed = 20;
        static int universeHeight = 20;
        static int universeWidth = 20;
        // The universe array
        bool[,] universe = new bool[universeWidth, universeHeight];
        bool[,] scratchPad = new bool[universeWidth, universeHeight];
        bool drawRec = true;
        bool drawNum = true;
        string HUDstring;
        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Gray;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;
        int allLiveCells = 0;
        int LivingCells = 0;

        public Form1()
        {
            InitializeComponent();

            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = true; // start timer running
        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {
            allLiveCells = 0;
            //Apply rules
            for (int x = 0; x < scratchPad.GetLength(0); x++)
            {
                for (int y = 0; y < scratchPad.GetLength(1); y++)
                {
                    LivingCells = CountNeighbors(x, y);

                    if (universe[x, y] == true)
                    {
                        if (LivingCells == 2 || LivingCells == 3)
                        {
                            scratchPad[x, y] = true;
                            ++allLiveCells;
                        }
                        else
                        {
                            scratchPad[x, y] = false;
                        }
                    }
                    else
                    {
                        if (LivingCells == 3)
                        {
                            scratchPad[x, y] = true;
                            ++allLiveCells;
                        }
                        else
                        {
                            scratchPad[x, y] = false;
                        }
                    }

                }
            }

            bool[,] temp = universe;
            universe = scratchPad;
            scratchPad = temp;
            //Next click funvtion interate nextgeneration() once

            // Increment generation count
            generations++;

            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            toolStripStatusLabel1.Text = "Living Cells: " + allLiveCells.ToString();
            graphicsPanel1.Invalidate();
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
            graphicsPanel1.Invalidate();
        }

        private void GraphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // Currently fits to window - I subtracted 1 from clientsize.width/heigth
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            float cellWidth = ((float)graphicsPanel1.ClientSize.Width - 1) / universe.GetLength(0); //returns length in deminsion 2D array so 0 1 for getlength

            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight = ((float)graphicsPanel1.ClientSize.Height - 1) / universe.GetLength(1);

            int penWidth = 1;

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, penWidth);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    RectangleF cellRect = Rectangle.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }

                    int neighbors = CountNeighbors(x, y);
                    string insideNum = "";
                    //show neighbors
                    Font font = new Font("Arial", 14f);

                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Center;
                    stringFormat.LineAlignment = StringAlignment.Center;

                    if (neighbors == 0)
                    {
                        insideNum = "";
                    }
                    if (neighbors > 0 || (neighbors == 0 && universe[x, y]))
                    {
                        insideNum = neighbors.ToString();
                    }
                    if (drawNum == true)
                    {
                        e.Graphics.DrawString(insideNum.ToString(), font, Brushes.Black, cellRect, stringFormat);

                    }

                    if (drawRec == true)
                    {
                        // Outline the cell with a pen
                        e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                    }
                }
            }
            if (hUDToolStripMenuItem.Checked)
            {
                HudPaint(sender, e);
            }
            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();

        }

        private void hUDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hUDToolStripMenuItem.Checked = !hUDToolStripMenuItem.Checked;
            graphicsPanel1.Invalidate();
        }

        private void GraphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = e.Y / cellHeight;



                // Toggle the cell's state
                // error if clicked in the gap from resizing ..  fix or handle error
                universe[x, y] = !universe[x, y];


                // Tell Windows you need to repaint ... tell windows you need refresh what you see in the window
                toolStripStatusLabel1.Text = "Living Cells: " + allLiveCells.ToString();

                graphicsPanel1.Invalidate();

            }
        }

        public int CountNeighbors(int x, int y)
        {
            int neighborCount = 0;
            int xLength = universe.GetLength(0);
            int yLength = universe.GetLength(1);
            for (int yOff = -1; yOff <= 1; yOff++)
            {
                for (int xOff = -1; xOff <= 1; xOff++)
                {
                    int xC = x + xOff;
                    int yC = y + yOff;
                    if (xOff == 0 && yOff == 0)
                    {
                        continue;
                    }
                    if (yC < 0)
                    {
                        continue;
                    }
                    if (xC < 0)
                    {
                        continue;
                    }
                    if (xC >= xLength)
                    {
                        continue;
                    }
                    if (yC >= yLength)
                    {
                        continue;
                    }
                    if (universe[xC, yC] == true) neighborCount++;





                }
            }
            return neighborCount;

        }

        private void RandomUniverse()
        {
            //takes a seed for seed
            int seedRandom = (int)DateTime.Now.Ticks;
            Random sRand = new Random(seedRandom);
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    //call Next
                    seedRandom = sRand.Next(0, 3);
                    //if random == 0 turn on
                    if (seedRandom == 0)
                    {
                        universe[x, y] = true;
                        scratchPad[x, y] = true;
                    }
                }
            }
            NextGeneration();
        }

        private void HudPaint(object sender, PaintEventArgs e)
        {
            Color hudColor = Color.FromArgb(200, Color.Red);
            Brush hudBrush = new SolidBrush(hudColor);
            RectangleF hudRect = Rectangle.Empty;


            Font font = new Font("Arial", 14f);
            StringFormat hudString = new StringFormat();
            hudString.Alignment = StringAlignment.Near;
            hudString.LineAlignment = StringAlignment.Near;

            HUDstring = "Generations: " + generations.ToString() + "\n" + "Cell Count: " + allLiveCells.ToString() + "\n" + "Universe Size: { Width = " + universeWidth.ToString()
                + "Height = " + universeHeight.ToString() + " } \n";

            e.Graphics.DrawString(HUDstring, font, hudBrush, graphicsPanel1.ClientRectangle, hudString);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }
            graphicsPanel1.Invalidate();
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void randomizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RandomUniverse();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2; dlg.DefaultExt = "cells";


            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamWriter writer = new StreamWriter(dlg.FileName);
                StringBuilder currentRow = new StringBuilder();
                writer.WriteLine("!Save Game of Life");

                // Iterate through the universe one row at a time.
                for (int y = 0; y < universeHeight; y++)
                {
                    // Create a string to represent the current row.
                    currentRow.Clear();
                    // Iterate through the current row one cell at a time.
                    for (int x = 0; x < universeWidth; x++)
                    {
                        // If the universe[x,y] is alive then append 'O' (capital O)
                        // to the row string.

                        // Else if the universe[x,y] is dead then append '.' (period)
                        // to the row string.

                        if (universe[x, y] == true)
                        {
                            currentRow.Append('O');
                        }
                        else if (universe[x, y] == false)
                        {
                            currentRow.Append('.');
                        }
                    }

                    // Once the current row has been read through and the 
                    // string constructed then write it to the file using WriteLine.
                    writer.WriteLine(currentRow);
                }

                // After all rows and columns have been written then close the file.
                writer.Close();
            }

        }

        private void saveToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            // I made the save for the File Save not the actual Icon on accident and I am not sure how to change it without redoing things.
            // I am going to leave it like this and maybe delete the original save icon.
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2; dlg.DefaultExt = "cells";


            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamWriter writer = new StreamWriter(dlg.FileName);
                StringBuilder currentRow = new StringBuilder();

                // Write any comments you want to include first.
                // Prefix all comment strings with an exclamation point.
                // Use WriteLine to write the strings to the file. 
                // It appends a CRLF for you.
                writer.WriteLine("!Save Game of Life");

                // Iterate through the universe one row at a time.
                for (int y = 0; y < 10; y++)
                {
                    // Create a string to represent the current row.
                    currentRow.Clear();
                    // Iterate through the current row one cell at a time.
                    for (int x = 0; x < 10; x++)
                    {
                        // If the universe[x,y] is alive then append 'O' (capital O)
                        // to the row string.

                        // Else if the universe[x,y] is dead then append '.' (period)
                        // to the row string.

                        if (universe[x, y] == true)
                        {
                            currentRow.Append('O');
                        }
                        else if (universe[x, y] == false)
                        {
                            currentRow.Append('.');
                        }
                    }

                    // Once the current row has been read through and the 
                    // string constructed then write it to the file using WriteLine.
                    writer.WriteLine(currentRow);
                }

                // After all rows and columns have been written then close the file.
                writer.Close();
            }

        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            generations = 0;


            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamReader reader = new StreamReader(dlg.FileName);

                // Create a couple variables to calculate the width and height
                // of the data in the file.
                int maxWidth = 0;
                int maxHeight = 0;

                // Iterate through the file once to get its size.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();

                    // If the row begins with '!' then it is a comment
                    // and should be ignored.
                    if (row.StartsWith("!"))
                    {
                        row = reader.ReadLine();
                    }
                    if (!row.StartsWith("!"))
                    {
                        // If the row is not a comment then it is a row of cells.
                        // Increment the maxHeight variable for each row read.
                        maxHeight++;

                        // Get the length of the current row string
                        // and adjust the maxWidth variable if necessary.
                        maxWidth = row.Length;

                    }


                }
                universeHeight = maxHeight;
                universeWidth = maxWidth;
                universe = new bool[universeWidth, universeHeight];
                scratchPad = new bool[universeWidth, universeHeight];

                // Resize the current universe and scratchPad
                // to the width and height of the file calculated above.

                // Reset the file pointer back to the beginning of the file.
                reader.BaseStream.Seek(0, SeekOrigin.Begin);

                // Iterate through the file again, this time reading in the cells.
                int _y = 0;
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();

                    // If the row begins with '!' then
                    // it is a comment and should be ignored.
                    if (row.StartsWith("!"))
                    {
                        row = reader.ReadLine();
                    }

                    // If the row is not a comment then 
                    // it is a row of cells and needs to be iterated through.
                    if (!row.StartsWith("!"))
                    {
                        for (int xPos = 0; xPos < row.Length; xPos++)
                        {
                            // If row[xPos] is a 'O' (capital O) then
                            // set the corresponding cell in the universe to alive.
                            if (row[xPos] == 'O')
                            {
                                universe[xPos, _y] = true;
                            }

                            // If row[xPos] is a '.' (period) then
                            // set the corresponding cell in the universe to dead.
                            if (row[xPos] == '.')
                            {
                                universe[xPos, _y] = false;
                            }
                        }
                        ++_y;
                        //row = reader.ReadLine();
                    }
                    allLiveCells = 0;
                    //Apply rules
                    for (int x = 0; x < scratchPad.GetLength(0); x++)
                    {
                        for (int y = 0; y < scratchPad.GetLength(1); y++)
                        {
                            LivingCells = CountNeighbors(x, y);

                            if (universe[x, y] == true)
                            {
                                if (LivingCells == 2 || LivingCells == 3)
                                {
                                    scratchPad[x, y] = true;
                                    ++allLiveCells;
                                }
                                else
                                {
                                    scratchPad[x, y] = false;
                                }
                            }
                            else
                            {
                                if (LivingCells == 3)
                                {
                                    scratchPad[x, y] = true;
                                    ++allLiveCells;
                                }
                                else
                                {
                                    scratchPad[x, y] = false;
                                }
                            }

                        }
                    }
                    toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
                    toolStripStatusLabel1.Text = "Living Cells: " + allLiveCells.ToString();
                }

                // Close the file.
                reader.Close();
            }
            graphicsPanel1.Invalidate();
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
             generations = 0;


                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "All Files|*.*|Cells|*.cells";
                dlg.FilterIndex = 2;

                if (DialogResult.OK == dlg.ShowDialog())
                {
                    StreamReader reader = new StreamReader(dlg.FileName);

                    // Create a couple variables to calculate the width and height
                    // of the data in the file.
                    int maxWidth = 0;
                    int maxHeight = 0;

                    // Iterate through the file once to get its size.
                    while (!reader.EndOfStream)
                    {
                        // Read one row at a time.
                        string row = reader.ReadLine();

                        // If the row begins with '!' then it is a comment
                        // and should be ignored.
                        if (row.StartsWith("!"))
                        {
                            row = reader.ReadLine();
                        }
                        if (!row.StartsWith("!"))
                        {
                            // If the row is not a comment then it is a row of cells.
                            // Increment the maxHeight variable for each row read.
                            maxHeight++;

                            // Get the length of the current row string
                            // and adjust the maxWidth variable if necessary.
                            maxWidth = row.Length;

                        }


                    }
                    universeHeight = maxHeight;
                    universeWidth = maxWidth;
                    universe = new bool[universeWidth, universeHeight];
                    scratchPad = new bool[universeWidth, universeHeight];

                    // Resize the current universe and scratchPad
                    // to the width and height of the file calculated above.

                    // Reset the file pointer back to the beginning of the file.
                    reader.BaseStream.Seek(0, SeekOrigin.Begin);

                    // Iterate through the file again, this time reading in the cells.
                    int _y = 0;
                    while (!reader.EndOfStream)
                    {
                        // Read one row at a time.
                        string row = reader.ReadLine();

                        // If the row begins with '!' then
                        // it is a comment and should be ignored.
                        if (row.StartsWith("!"))
                        {
                            row = reader.ReadLine();
                        }

                        // If the row is not a comment then 
                        // it is a row of cells and needs to be iterated through.
                        if (!row.StartsWith("!"))
                        {
                            for (int xPos = 0; xPos < row.Length; xPos++)
                            {
                                // If row[xPos] is a 'O' (capital O) then
                                // set the corresponding cell in the universe to alive.
                                if (row[xPos] == 'O')
                                {
                                    universe[xPos, _y] = true;
                                }

                                // If row[xPos] is a '.' (period) then
                                // set the corresponding cell in the universe to dead.
                                if (row[xPos] == '.')
                                {
                                    universe[xPos, _y] = false;
                                }
                            }
                            ++_y;
                            //row = reader.ReadLine();
                        }
                        allLiveCells = 0;
                        //Apply rules
                        for (int x = 0; x < scratchPad.GetLength(0); x++)
                        {
                            for (int y = 0; y < scratchPad.GetLength(1); y++)
                            {
                                LivingCells = CountNeighbors(x, y);

                                if (universe[x, y] == true)
                                {
                                    if (LivingCells == 2 || LivingCells == 3)
                                    {
                                        scratchPad[x, y] = true;
                                        ++allLiveCells;
                                    }
                                    else
                                    {
                                        scratchPad[x, y] = false;
                                    }
                                }
                                else
                                {
                                    if (LivingCells == 3)
                                    {
                                        scratchPad[x, y] = true;
                                        ++allLiveCells;
                                    }
                                    else
                                    {
                                        scratchPad[x, y] = false;
                                    }
                                }

                            }
                        }
                        toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
                        toolStripStatusLabel1.Text = "Living Cells: " + allLiveCells.ToString();
                    }

                    // Close the file.
                    reader.Close();
                }
                graphicsPanel1.Invalidate();
            }

        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {

                ColorDialog color = new ColorDialog();

                color.Color = graphicsPanel1.BackColor;

                if (DialogResult.OK == color.ShowDialog())
                {
                    graphicsPanel1.BackColor = color.Color;

                    graphicsPanel1.Invalidate();
                }
                
        }

        private void cellColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog CellColor = new ColorDialog();

            CellColor.Color = cellColor; //data in

            if (DialogResult.OK == CellColor.ShowDialog()) //if hit acccept button
            {
                cellColor = CellColor.Color; //data out, color choosen

                graphicsPanel1.Invalidate();
            }
        }

        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridToolStripMenuItem.Checked = !gridToolStripMenuItem.Checked;
            drawRec = !drawRec;
            graphicsPanel1.Invalidate();
        }

        private void neighborCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            neighborCountToolStripMenuItem.Checked = !neighborCountToolStripMenuItem.Checked;
            drawNum = !drawNum;
            graphicsPanel1.Invalidate();
        }

       

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            
        }

       
    }
}
