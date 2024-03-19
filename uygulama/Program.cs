using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WingetInstaller
{
    public partial class Form1 : Form
    {
        private List<Program> programs;

        public Form1()
        {
            InitializeComponent();

            // CSV dosyasını oku
            programs = ReadCSV("programlar.csv");

            // Program listesini doldur
            foreach (var program in programs)
            {
                lstPrograms.Items.Add($"{program.Number}. {program.Name}");
            }
        }

        private void btnInstall_Click(object sender, EventArgs e)
        {
            // Seçilen programı al
            var selectedProgram = programs[lstPrograms.SelectedIndex];

            // Kurulum ilerleme çubuğunu göster
            prgInstall.Visible = true;

            // Winget komutunu çalıştır
            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/c winget install {selectedProgram.WingetCommand}";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            // Kurulum çıktısını oku
            string output = process.StandardOutput.ReadToEnd();

            // Kurulum tamamlandıktan sonra ilerleme çubuğunu gizle
            prgInstall.Visible = false;

            // Kurulum hatalıysa hata mesajı göster
            if (output.Contains("Error"))
            {
                MessageBox.Show(output, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Kurulum tamamlandı!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private List<Program> ReadCSV(string filePath)
        {
            List<Program> programs = new List<Program>();

            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var parts = line.Split(',');

                    if (parts.Length != 4)
                    {
                        continue;
                    }

                    programs.Add(new Program
                    {
                        Number = programs.Count + 1,
                        Name = parts[0],
                        WingetCommand = parts[1],
                        Version = parts[2],
                        Description = parts[3]
                    });
                }
            }

            return programs;
        }

        private class Program
        {
            public int Number { get; set; }
            public string Name { get; set; }
            public string WingetCommand { get; set; }
            public string Version { get; set; }
            public string Description { get; set; }
        }
    }
}
