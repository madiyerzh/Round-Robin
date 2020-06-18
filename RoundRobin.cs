using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OS_Project
{
    public class RoundRobin
    {
        DataGridView dataGridView;


        public RoundRobin(ref DataGridView temp_dataGridView)
        {
            dataGridView = temp_dataGridView;
        }
        public void runRoundRobin(ref NewProcess[] multiNewProcesses, int quantum)
        {
            foreach (var NewProcess in multiNewProcesses)
            {
                NewProcess.remainingTime = NewProcess.time;
            }
            while (true)
            {
                bool executionFinished = true;
               
                foreach (var NewProcess in multiNewProcesses)
                {
                    if (NewProcess.remainingTime == 0)
                    {
                        NewProcess.status = "Completed";
                        updateDataGridView(dataGridView, multiNewProcesses);
                    }
                    else if (NewProcess.remainingTime > 0)
                    {
                        executionFinished = false;
                        if (NewProcess.remainingTime > quantum)
                        {
                            NewProcess.status = "Running";
                            updateDataGridView(dataGridView, multiNewProcesses);
                            executionTimer(quantum);
                            NewProcess.remainingTime = NewProcess.remainingTime - quantum;
                            NewProcess.status = "Ready";
                            updateDataGridView(dataGridView, multiNewProcesses);
                        }

                        else
                        {
                            while (NewProcess.IO > 0)
                            {
                                ioExecution(multiNewProcesses, NewProcess.ID, NewProcess.IO);
                                NewProcess.IO = NewProcess.IO - 1;
                            }

                            NewProcess.status = "Running";
                            updateDataGridView(dataGridView, multiNewProcesses);
                            executionTimer(NewProcess.remainingTime);
                            NewProcess.remainingTime = 0;
                            NewProcess.status = "Completed";
                            updateDataGridView(dataGridView, multiNewProcesses);
                        }  
                    }
                    if (NewProcess.IO > 0)
                    {
                        ioExecution(multiNewProcesses, NewProcess.ID, NewProcess.IO);
                        NewProcess.IO = NewProcess.IO - 1;
                    }
                }
                if (executionFinished == true)
                {
                    break;
                }
            }
        }

        public void updateDataGridView(DataGridView dataGridView, NewProcess[] multiNewProcesses)
        {
            dataGridView.Rows.Clear();
            dataGridView.Refresh();

            foreach (var NewProcess in multiNewProcesses)
            {
                string[] row = { NewProcess.ID.ToString(), NewProcess.name, NewProcess.remainingTime.ToString(), NewProcess.IO.ToString(), NewProcess.status };
                dataGridView.Rows.Add(row);
            }
        }

        public void ioExecution(NewProcess[] multiNewProcesses, int id, int interupt)
        {
            foreach (var NewProcess in multiNewProcesses)
            {
                if (NewProcess.ID == id && NewProcess.status != "Completed")
                {
                    NewProcess.status = "Waiting";
                }
            }
            updateDataGridView(dataGridView, multiNewProcesses);
            executionTimer(1);
            foreach (var NewProcess in multiNewProcesses)
            {
                if (NewProcess.ID == id && NewProcess.status!="Completed")
                {
                    NewProcess.status = "Ready";
                }
            }
            updateDataGridView(dataGridView, multiNewProcesses);
        }
        public void executionTimer(int tempTime)
        {
            int executionTime = tempTime * 1000;
            System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
            if (executionTime == 0 || executionTime < 0)
            {
                return;
            }
            timer1.Interval = executionTime;
            timer1.Enabled = true;
            timer1.Start();
            timer1.Tick += (s, e) =>
            {
                timer1.Enabled = false;
                timer1.Stop();
            };
            while (timer1.Enabled)
            {
                Application.DoEvents();
            }
        }
    }
}