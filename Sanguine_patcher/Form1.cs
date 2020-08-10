using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Security.Cryptography;
namespace Sanguine_patcher
{
    public partial class Sanguine_patcher_window : Form
    {
        public Sanguine_patcher_window()
        {
            InitializeComponent();
        }
        static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
        private void Clear()
        {
            richTextBox1.Clear();
        }
        private void Button_check_update_Click(object sender, EventArgs e)
        {
            if (Button_check_update.Text == "Sprawdź aktualizacje")
            {
                Clear();
                richTextBox1.AppendText("Sprawdzanie nowej aktualizacji...");
                richTextBox1.AppendText(Environment.NewLine);
                Task.Run(async () => await Download("sanguine_dev","txt", ""));
                Task.Run(async () => await Download("sanguine_flora", "txt", ""));
                Task.Run(async () => await Download("sanguine_property", "txt", ""));
                Task.Run(async () => await Download("sanguine_character", "txt", ""));
                Task.Run(async () => await Download("sanguine_textures", "txt", "interface/"));
                Task.Run(async () => await Download("sanguine_addon", "txt", "addon/"));
                Task.Run(async () => await Download("sanguine_addon_02", "txt", "addon/"));
                Task.Run(async () => await Download("sanguine_addon_03", "txt", "addon/"));
                Task.Run(async () => await Download("sanguine_addon_homecoming_update", "txt", "addon/"));
                Task.Run(async () => await Download("sanguine_meshes", "txt", "creation/"));
                Task.Run(async () => await Download("sanguine_new_world", "txt", "disboard/"));
                Task.Run(async () => await Download("sanguine_npc", "txt", "disboard/"));
                Task.Run(async () => await Download("sanguine_character_assassin", "txt", "characters/"));
                Task.Run(async () => await Download("sanguine_character_shaman", "txt", "characters/"));
                Task.Run(async () => await Download("sanguine_character_sura", "txt", "characters/"));
                Task.Run(async () => await Download("sanguine_character_warrior", "txt", "characters/"));
                Task.Run(async () => await Download("sanguine_character_emotion", "txt", "characters/emotion/"));
                Task.Run(async () => await Download("sanguine_character_assassin_female", "txt", "characters/female/"));
                Task.Run(async () => await Download("sanguine_character_shaman_female", "txt", "characters/female/"));
                Task.Run(async () => await Download("sanguine_character_sura_female", "txt", "characters/female/"));
                Task.Run(async () => await Download("sanguine_character_warrior_female", "txt", "characters/female/"));
                return;
            }
            else
            {
                richTextBox1.AppendText(Environment.NewLine);
                richTextBox1.AppendText("??...");
            }
        }
        private async Task Download(string filename, string extension, string special_folder)
        {
            string locale = Path.GetDirectoryName(Application.ExecutablePath);
            string ftpf = "ftp://nirray.bplaced.net/www/sanguine/sanguine/";
            try
            {
                string url = "" + ftpf + special_folder + filename + "." + extension;
                NetworkCredential credentials = new NetworkCredential("", "");

                WebRequest sizeRequest = WebRequest.Create(url);
                sizeRequest.Credentials = credentials;
                sizeRequest.Method = WebRequestMethods.Ftp.GetFileSize;
                int size = (int)sizeRequest.GetResponse().ContentLength;
                WebRequest request = WebRequest.Create(url);
                request.Credentials = credentials;
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                using (Stream ftpStream = request.GetResponse().GetResponseStream())
                using (Stream fileStream = File.Create(locale + @"\patcher\"+filename+"." + extension))
                
                {
                    byte[] buffer = new byte[10240];
                    int read;
                    while ((read = ftpStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fileStream.Write(buffer, 0, read);
                        int position = (int)fileStream.Position;
                       
                        text_state.Invoke(
                            (MethodInvoker)(() => text_state.Text = url.Substring(url.LastIndexOf('/') + 1)));
                    }
                    fileStream.Close();
                }
            }
            catch (Exception e)
            {
                richTextBox1.Invoke(
                            (MethodInvoker)(() => richTextBox1.AppendText(Environment.NewLine+"[" + filename + "] " + e.Message+ Environment.NewLine)));
                // MessageBox.Show(e.Message);
            }
            System.Threading.Thread.Sleep(3000);
            string current_file = filename;
            string text = File.ReadAllText(locale + @"\patcher\" + current_file + ".txt");
            if (!File.Exists(locale + @"\sanguine\" + special_folder + current_file + ".epk"))
            {
                MessageBox.Show("[" + current_file + "] Brak pliku w folderze 'Sanguine'");
                await Task.Run(() => Download_pack(filename, "epk", special_folder));
                await Task.Run(() => Download_pack(filename, "eix", special_folder));
            }
            string current = CalculateMD5(locale + @"\sanguine\" + special_folder + current_file + ".epk");
            
            richTextBox1.Invoke(
                            (MethodInvoker)(() => richTextBox1.AppendText(Environment.NewLine+"[" + filename + "" + "" + "] " + "MD5: " + text+ Environment.NewLine)));
            if (text.Equals(current))
            {
                richTextBox1.Invoke(
                            (MethodInvoker)(() => richTextBox1.AppendText(Environment.NewLine + "[" + filename + "" + "" + "] " + "Posiadasz najnowszą wersje." + Environment.NewLine)));
            }
            else
            {
                richTextBox1.Invoke(
                            (MethodInvoker)(() => richTextBox1.AppendText(Environment.NewLine+"[" + filename + "" + "" + "] " + "Dostępna nowa aktualizacja!"+ Environment.NewLine)));
                System.Threading.Thread.Sleep(3000);
                await Task.Run(() => Download_pack(filename, "epk", special_folder));
                await Task.Run(() => Download_pack(filename, "eix", special_folder));
            }
        }

        private void Download_pack(string filename, string extension, string special_folder)
        {
            string locale = Path.GetDirectoryName(Application.ExecutablePath);
            string ftpf = "ftp://nirray.bplaced.net/www/sanguine/sanguine/";
            try
            {
                string url = "" + ftpf + special_folder + filename + "." + extension;
                NetworkCredential credentials = new NetworkCredential("nirray", "K2omop91");

                WebRequest sizeRequest = WebRequest.Create(url);
                sizeRequest.Credentials = credentials;
                sizeRequest.Method = WebRequestMethods.Ftp.GetFileSize;
                int size = (int)sizeRequest.GetResponse().ContentLength;
                WebRequest request = WebRequest.Create(url);
                request.Credentials = credentials;
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                using (Stream ftpStream = request.GetResponse().GetResponseStream())
                using (Stream fileStream = File.Create(locale + @"\sanguine\" + special_folder + filename + "." + extension))
                {
                    byte[] buffer = new byte[10240];
                    int read;
                    while ((read = ftpStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fileStream.Write(buffer, 0, read);
                        int position = (int)fileStream.Position;
                        text_state.Invoke(
                            (MethodInvoker)(() => text_state.Text = url.Substring(url.LastIndexOf('/') + 1) + " " + fileStream.Position + "/" + size + " bajtów"));
                    }
                    fileStream.Close();
                    richTextBox1.Invoke(
                           (MethodInvoker)(() => richTextBox1.AppendText(Environment.NewLine+"Pomyślnie pobrano plik: " + filename + "." + extension+ Environment.NewLine)));

                }
            }
            catch (Exception e)
            {

                richTextBox1.Invoke(
                            (MethodInvoker)(() => richTextBox1.AppendText("["+filename+"] " + e.Message)));
                // MessageBox.Show(e.Message);
            }
        }
        private void Button_check_version_Click(object sender, EventArgs e)
        {
            Clear();
            string locale = Path.GetDirectoryName(Application.ExecutablePath);
            string[] main_patch = Directory.GetFiles(locale +@"\sanguine\", "*.epk");
            string[] addon_patch = Directory.GetFiles(locale + @"\sanguine\addon\", "*.epk");
            string[] character_patch_01 = Directory.GetFiles(locale + @"\sanguine\characters\", "*.epk");
            string[] character_patch_02 = Directory.GetFiles(locale + @"\sanguine\characters\female", "*.epk");
            string[] character_patch_03 = Directory.GetFiles(locale + @"\sanguine\characters\emotion", "*.epk");
            string[] creation_patch = Directory.GetFiles(locale + @"\sanguine\creation\", "*.epk");
            string[] disboard_patch = Directory.GetFiles(locale + @"\sanguine\disboard\", "*.epk");
            string[] interface_patch = Directory.GetFiles(locale + @"\sanguine\interface\", "*.epk");
            richTextBox1.AppendText(Environment.NewLine);
            foreach (var item in main_patch)
            {

                richTextBox1.AppendText(Environment.NewLine+Path.GetFileName(item) + " " + CalculateMD5(item) + Environment.NewLine);
            }
            foreach (var item in addon_patch)
            {

                richTextBox1.AppendText(Environment.NewLine + Path.GetFileName(item) + " " + CalculateMD5(item)+ Environment.NewLine);

            }
            foreach (var item in character_patch_01)
            {

                richTextBox1.AppendText(Environment.NewLine+Path.GetFileName(item) + " " + CalculateMD5(item)+ Environment.NewLine);
            }
            foreach (var item in character_patch_02)
            {

                richTextBox1.AppendText(Environment.NewLine+Path.GetFileName(item) + " " + CalculateMD5(item)+ Environment.NewLine);
            }
            foreach (var item in character_patch_03)
            {

                richTextBox1.AppendText(Environment.NewLine+Path.GetFileName(item) + " " + CalculateMD5(item)+ Environment.NewLine);
            }
            foreach (var item in creation_patch)
            {

                richTextBox1.AppendText(Environment.NewLine + Path.GetFileName(item) + " " + CalculateMD5(item) + Environment.NewLine);
            }
            foreach (var item in disboard_patch)
            {

                richTextBox1.AppendText(Environment.NewLine + Path.GetFileName(item) + " " + CalculateMD5(item) + Environment.NewLine);
            }
            foreach (var item in interface_patch)
            {

                richTextBox1.AppendText(Environment.NewLine + Path.GetFileName(item) + " " + CalculateMD5(item) + Environment.NewLine);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string locale = Path.GetDirectoryName(Application.ExecutablePath);
            string[] main_patch = Directory.GetFiles(locale + @"\sanguine\", "*.epk");
            string[] addon_patch = Directory.GetFiles(locale + @"\sanguine\addon\", "*.epk");
            string[] character_patch_01 = Directory.GetFiles(locale + @"\sanguine\characters\", "*.epk");
            string[] character_patch_02 = Directory.GetFiles(locale + @"\sanguine\characters\female", "*.epk");
            string[] character_patch_03 = Directory.GetFiles(locale + @"\sanguine\characters\emotion", "*.epk");
            string[] creation_patch = Directory.GetFiles(locale + @"\sanguine\creation\", "*.epk");
            string[] disboard_patch = Directory.GetFiles(locale + @"\sanguine\disboard\", "*.epk");
            string[] interface_patch = Directory.GetFiles(locale + @"\sanguine\interface\", "*.epk");
            if (!Directory.Exists(locale + @"\serverside_patcher\"))
            {
                Directory.CreateDirectory(locale + @"\serverside_patcher\");
            }
            if (!Directory.Exists(locale + @"\serverside_patcher\addon\"))
            {
                Directory.CreateDirectory(locale + @"\serverside_patcher\addon\");
            }
            if (!Directory.Exists(locale + @"\serverside_patcher\characters\"))
            {
                Directory.CreateDirectory(locale + @"\serverside_patcher\characters\");
                if (!Directory.Exists(locale + @"\serverside_patcher\characters\female\"))
                {
                    Directory.CreateDirectory(locale + @"\serverside_patcher\characters\female\");
                }
                if (!Directory.Exists(locale + @"\serverside_patcher\characters\emotion\"))
                {
                    Directory.CreateDirectory(locale + @"\serverside_patcher\characters\emotion\");
                }
            }
            if (!Directory.Exists(locale + @"\serverside_patcher\creation\"))
            {
                Directory.CreateDirectory(locale + @"\serverside_patcher\creation\");
            }
            if (!Directory.Exists(locale + @"\serverside_patcher\disboard\"))
            {
                Directory.CreateDirectory(locale + @"\serverside_patcher\disboard\");
            }
            if (!Directory.Exists(locale + @"\serverside_patcher\interface\"))
            {
                Directory.CreateDirectory(locale + @"\serverside_patcher\interface\");
            }

            richTextBox1.Clear();
            richTextBox1.AppendText("Tworzenie aktualizacji");
            richTextBox1.AppendText(Environment.NewLine);

            foreach (var item in main_patch)
            {
                using (StreamWriter sw = File.CreateText(locale + @"\serverside_patcher\"+ Path.GetFileNameWithoutExtension(item) + ".txt"))
                {
                    sw.Write(CalculateMD5(item));
                }
            }
            foreach (var item in addon_patch)
            {
                using (StreamWriter sw = File.CreateText(locale + @"\serverside_patcher\addon\" + Path.GetFileNameWithoutExtension(item) + ".txt"))
                {
                    sw.Write(CalculateMD5(item));
                    richTextBox1.AppendText(Environment.NewLine+Path.GetFileName(item) + " " + CalculateMD5(item)+ Environment.NewLine);
                }
            }
            foreach (var item in character_patch_01)
            {
                using (StreamWriter sw = File.CreateText(locale + @"\serverside_patcher\characters\" + Path.GetFileNameWithoutExtension(item) + ".txt"))
                {
                    sw.Write(CalculateMD5(item));
                    richTextBox1.AppendText(Environment.NewLine + Path.GetFileName(item) + " " + CalculateMD5(item) + Environment.NewLine);
                }
            }
            foreach (var item in character_patch_02)
            {
                using (StreamWriter sw = File.CreateText(locale + @"\serverside_patcher\characters\female\" + Path.GetFileNameWithoutExtension(item) + ".txt"))
                {
                    sw.Write(CalculateMD5(item));
                    richTextBox1.AppendText(Environment.NewLine + Path.GetFileName(item) + " " + CalculateMD5(item) + Environment.NewLine);
                }
            }
            foreach (var item in character_patch_03)
            {
                using (StreamWriter sw = File.CreateText(locale + @"\serverside_patcher\characters\emotion\" + Path.GetFileNameWithoutExtension(item) + ".txt"))
                {
                    sw.Write(CalculateMD5(item));
                    richTextBox1.AppendText(Environment.NewLine + Path.GetFileName(item) + " " + CalculateMD5(item) + Environment.NewLine);
                }
            }
            foreach (var item in creation_patch)
            {
                using (StreamWriter sw = File.CreateText(locale + @"\serverside_patcher\creation\" + Path.GetFileNameWithoutExtension(item) + ".txt"))
                {
                    sw.Write(CalculateMD5(item));
                    richTextBox1.AppendText(Environment.NewLine + Path.GetFileName(item) + " " + CalculateMD5(item) + Environment.NewLine);
                }
            }
            foreach (var item in disboard_patch)
            {
                using (StreamWriter sw = File.CreateText(locale + @"\serverside_patcher\disboard\" + Path.GetFileNameWithoutExtension(item) + ".txt"))
                {
                    sw.Write(CalculateMD5(item));
                    richTextBox1.AppendText(Environment.NewLine + Path.GetFileName(item) + " " + CalculateMD5(item) + Environment.NewLine);
                }
            }
            foreach (var item in interface_patch)
            {
                using (StreamWriter sw = File.CreateText(locale + @"\serverside_patcher\interface\" + Path.GetFileNameWithoutExtension(item) + ".txt"))
                {
                    sw.Write(CalculateMD5(item));
                    richTextBox1.AppendText(Environment.NewLine + Path.GetFileName(item) + " " + CalculateMD5(item) + Environment.NewLine);
                }
            }
            System.Diagnostics.Process.Start("explorer.exe", locale + @"\serverside_patcher\");

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length;

            richTextBox1.ScrollToCaret();
        }

        private void Sanguine_patcher_window_Load(object sender, EventArgs e)
        {

        }
    }
}
