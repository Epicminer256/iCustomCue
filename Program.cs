using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO.Compression;

namespace iCustomCue;

partial class MainMenuForm{
    private System.ComponentModel.IContainer components = null;
    protected override void Dispose(bool disposing){
        if (disposing && (components != null)){
            components.Dispose();
        }
        base.Dispose(disposing);
    }
}

public partial class MainMenuForm : Form
{
	
    public MainMenuForm()
    {
        string Config_Base_Path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "iCustomCue");
        string Soundpack_Path = System.IO.Path.Combine(Config_Base_Path, "Soundpacks");
        string Backup_User_Path = System.IO.Path.Combine(Config_Base_Path, "Backup");
        string iCue_Path = "";
        string iCue_Path_ConfigPath = System.IO.Path.Combine(Config_Base_Path,"iCuePath.txt");

        if(!Directory.Exists(Config_Base_Path)){
            Directory.CreateDirectory(Config_Base_Path);
            MessageBox.Show("Welcome to iCustomCue! \n\n All data for this program is located in a folder called \"iCustomCue\" in your documents");
        }

        if(File.Exists(iCue_Path_ConfigPath)){
            iCue_Path = File.ReadAllText(iCue_Path_ConfigPath);
            if(!File.Exists(System.IO.Path.Combine(iCue_Path,"iCUE.exe"))){
                MessageBox.Show("The file \"iCuePath\" in your config points to a invalid path. Delete that file and start this program again to fix it.");
                this.Close();
                Application.Exit();
                System.Environment.Exit(1); 
            }
        } else {
            if(File.Exists("C:\\Program Files\\Corsair\\Corsair iCUE5 Software\\iCUE.exe")){
                iCue_Path = "C:\\Program Files\\Corsair\\CORSAIR iCUE5 Software\\";
            }
            if(File.Exists("C:\\Program Files (x86)\\Corsair\\CORSAIR iCUE5 Software\\")){
                iCue_Path = "C:\\Program Files (x86)\\Corsair\\CORSAIR iCUE5 Software\\";
            }
            while(iCue_Path == ""){
                MessageBox.Show("iCue was not found in the default path, you will have to find it yourself (perhaps on a different drive?). Click Ok to pick the path.");
                using(var fbd = new FolderBrowserDialog()){
                    DialogResult result = fbd.ShowDialog();
                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath)){
                        if(File.Exists(System.IO.Path.Combine(fbd.SelectedPath,"iCUE.exe"))){
                            iCue_Path = fbd.SelectedPath;
                        } else {
                            MessageBox.Show("The directory picked is invalid, try again");
                        }
                        
                    } else {
                        MessageBox.Show("The directory was not picked, exiting");
                        this.Close();
                        Application.Exit();
                        System.Environment.Exit(1); 
                    }
                }
            }
            File.WriteAllText(iCue_Path_ConfigPath, iCue_Path);
        }

        string Backup_Sys_Path = System.IO.Path.Combine(iCue_Path, "sounds\\backup");
        string Current_Sound_Path = System.IO.Path.Combine(iCue_Path, "sounds\\default");
        
        // In short, if running in admin mode it will fix issues with permissions for regular users
        // In the future, I will want to make this 
        if(IsRunningAsAdmin()){
            const FileSystemRights rights = FileSystemRights.FullControl;
            var allUsers = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
            // Add Access Rule to the actual directory itself
            var accessRule = new FileSystemAccessRule(
                allUsers,
                rights,
                InheritanceFlags.None,
                PropagationFlags.NoPropagateInherit,
                AccessControlType.Allow);
            var info = new DirectoryInfo(System.IO.Path.Combine(iCue_Path, "sounds"));
            var security = info.GetAccessControl(AccessControlSections.Access);
            bool result;
            security.ModifyAccessRule(AccessControlModification.Set, accessRule, out result);
            // add inheritance
            var inheritedAccessRule = new FileSystemAccessRule(
                allUsers,
                rights,
                InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                PropagationFlags.InheritOnly,
                AccessControlType.Allow);
            bool inheritedResult;
            security.ModifyAccessRule(AccessControlModification.Add, inheritedAccessRule, out inheritedResult);
            info.SetAccessControl(security);
            MessageBox.Show("The program will now exit, you can run this program as a regular user");
            this.Close();
            Application.Exit();
        }

        if(!Directory.Exists(Backup_Sys_Path)){
            try{
                Directory.CreateDirectory(Backup_Sys_Path);
            } catch {
                MessageBox.Show("Can't write to iCue, we need permission to do so. This program will rerun as administrator to set the correct permissions.");
                RunAsAdmin();
                this.Close();
                Application.Exit();
            }
            Directory.CreateDirectory(Backup_Sys_Path);
            string[] allStockFiles = Directory.GetFiles(Current_Sound_Path);
            foreach(var file in allStockFiles){
                File.Copy(file, System.IO.Path.Combine( Backup_Sys_Path,Path.GetFileName(file) ));
            }
        }

        if(!Directory.Exists(Backup_User_Path))
        {
            Directory.CreateDirectory(Backup_User_Path);
            string[] allSysBackupFiles = Directory.GetFiles(Backup_Sys_Path);
            foreach(var file in allSysBackupFiles){
                File.Copy(file, System.IO.Path.Combine( Backup_User_Path,Path.GetFileName(file) ));
            }
        }

        if(!Directory.Exists(Soundpack_Path))
        {
            Directory.CreateDirectory(Soundpack_Path);
            Directory.CreateDirectory(System.IO.Path.Combine(Soundpack_Path, "Stock Sounds"));
            string[] allStockFiles = Directory.GetFiles(Backup_Sys_Path);
            foreach(var file in allStockFiles){
                File.Copy(file, System.IO.Path.Combine( System.IO.Path.Combine(Soundpack_Path, "Stock Sounds"),Path.GetFileName(file) ));
            }
        }

        this.components = new System.ComponentModel.Container();
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(400, 200);
		this.FormBorderStyle = FormBorderStyle.FixedSingle;
		this.MaximizeBox = false;
		this.MinimizeBox = false;
        this.Text = "iCustomCue";

		
		Button ChooseSoundpackButton;
        ChooseSoundpackButton = new Button();
        ChooseSoundpackButton.Name = "choosesoundpack";
        ChooseSoundpackButton.Text = "Choose soundpack";
        ChooseSoundpackButton.Location = new System.Drawing.Point(20, 140);
        ChooseSoundpackButton.Size = new System.Drawing.Size(360, 40);

		Button InstallSoundpackButton;
        InstallSoundpackButton = new Button();
        InstallSoundpackButton.Name = "installsoundpack";
        InstallSoundpackButton.Text = "Install new pack";
        InstallSoundpackButton.Location = new System.Drawing.Point(20, 20);
        InstallSoundpackButton.Size = new System.Drawing.Size(85, 40);

		Button UninstallSoundpackButton;
        UninstallSoundpackButton = new Button();
        UninstallSoundpackButton.Name = "uninstallsoundpack";
        UninstallSoundpackButton.Text = "Uninstall pack";
        UninstallSoundpackButton.Location = new System.Drawing.Point(105, 20);
        UninstallSoundpackButton.Size = new System.Drawing.Size(85, 40);

		Button RestoreFactory;
        RestoreFactory = new Button();
        RestoreFactory.Name = "restorefactory";
        RestoreFactory.Text = "Restore Factory Files";
        RestoreFactory.Location = new System.Drawing.Point(210, 20);
        RestoreFactory.Size = new System.Drawing.Size(170, 40);

		Button Close_iCue;
        Close_iCue = new Button();
        Close_iCue.Name = "closeicue";
        Close_iCue.Text = "Close iCue";
        Close_iCue.Location = new System.Drawing.Point(20, 80);
        Close_iCue.Size = new System.Drawing.Size(170, 40);

		Button Restart_iCue;
        Restart_iCue = new Button();
        Restart_iCue.Name = "restarticue";
        Restart_iCue.Text = "Restart/Open iCue";
        Restart_iCue.Location = new System.Drawing.Point(210, 80);
        Restart_iCue.Size = new System.Drawing.Size(170, 40);

        this.Controls.Add(InstallSoundpackButton);
        this.Controls.Add(UninstallSoundpackButton);
        this.Controls.Add(ChooseSoundpackButton);
        this.Controls.Add(Close_iCue);
        this.Controls.Add(Restart_iCue);
        this.Controls.Add(RestoreFactory);

        void Restart_iCue_Click(object sender, EventArgs e){
            System.Diagnostics.Process.Start("C:\\Windows\\System32\\taskkill.exe"," /F /IM iCUE.exe");
            Thread.Sleep(2000);
            System.Diagnostics.Process.Start(System.IO.Path.Combine(iCue_Path,"iCUE.exe"));
        }
        Restart_iCue.Click += Restart_iCue_Click;
		
        void ChooseSoundpackButton_Click(object sender, EventArgs e){
			PickSoundpack pk = new PickSoundpack(iCue_Path, Current_Sound_Path);
            pk.ShowDialog();
        }
        ChooseSoundpackButton.Click += ChooseSoundpackButton_Click;
		
        void InstallSoundpackButton_Click(object sender, EventArgs e){
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "zip files (*.zip)|*.zip|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK){
                string filePath = openFileDialog.FileName;
                string destinationDirectory = System.IO.Path.Combine(Soundpack_Path,Path.GetFileNameWithoutExtension(filePath));
                if(!Directory.Exists(destinationDirectory)){
                    Directory.CreateDirectory(destinationDirectory);
                }
                using (ZipArchive archive = ZipFile.OpenRead(filePath)){
                    foreach (ZipArchiveEntry entry in archive.Entries){
                        if (!entry.FullName.EndsWith("/")){
                            string destinationPath = Path.Combine(destinationDirectory, entry.Name);
                            entry.ExtractToFile(destinationPath, overwrite: true);
                        }
                    }
                }
                MessageBox.Show("Done!");
            }
        }
        InstallSoundpackButton.Click += InstallSoundpackButton_Click;
		
        void UninstallSoundpackButton_Click(object sender, EventArgs e){
            RemoveSoundpack pk = new RemoveSoundpack(iCue_Path, Current_Sound_Path);
            pk.ShowDialog();
        }
        UninstallSoundpackButton.Click += UninstallSoundpackButton_Click;
		
        void RestoreFactory_Click(object sender, EventArgs e) {
            System.Diagnostics.Process.Start("C:\\Windows\\System32\\taskkill.exe"," /F /IM iCUE.exe");
            Thread.Sleep(2000);
            string[] allCurrentFiles = Directory.GetFiles(Current_Sound_Path);
            foreach(var file in allCurrentFiles){
                File.Delete(file);
            }
			string[] allBackupFiles = Directory.GetFiles(Backup_Sys_Path);
            foreach(var file in allBackupFiles){
                File.Copy(file, System.IO.Path.Combine( Current_Sound_Path,Path.GetFileName(file)), true);
            }
            System.Diagnostics.Process.Start(System.IO.Path.Combine(iCue_Path,"iCUE.exe"));
            MessageBox.Show("Done!");
        }
        RestoreFactory.Click += RestoreFactory_Click;
		
        void Close_iCue_Click(object sender, EventArgs e){
            System.Diagnostics.Process.Start("C:\\Windows\\System32\\taskkill.exe"," /F /IM iCUE.exe");
        }
        Close_iCue.Click += Close_iCue_Click;
        
    }
    
    
    static void RunAsAdmin()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,System.AppDomain.CurrentDomain.FriendlyName+".exe");
        startInfo.UseShellExecute = true;
        startInfo.Verb = "runas"; // This will prompt for UAC elevation

        try{
            Process.Start(startInfo);
        }
        catch (System.ComponentModel.Win32Exception){
            // simply ignoring this catch.
        }

        Application.Exit(); // Exit the current process
    }
    static bool IsRunningAsAdmin(){
        WindowsIdentity identity = WindowsIdentity.GetCurrent();
        WindowsPrincipal principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }
}



public partial class PickSoundpack
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing){
        if (disposing && (components != null)){
            components.Dispose();
        }
        base.Dispose(disposing);
    }
	private void PickedPack(object sender, EventArgs e){
		Button clickedButton = (Button)sender;
		string buttonText = clickedButton.Text;

		MessageBox.Show("Button Text: " + buttonText);
	}
}

public partial class PickSoundpack : Form
{
	public PickSoundpack(string iCue_Path, string Current_Sound_Path){
		this.components = new System.ComponentModel.Container();
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(300, 600);
        this.Text = "Soundpack Picker - iCustomCue";
		
		string Config_Base_Path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "iCustomCue");
        string Soundpack_Path = System.IO.Path.Combine(Config_Base_Path, "Soundpacks");
		string[] allPacks = Directory.GetDirectories(Soundpack_Path);

		ListBox listBox1;
		listBox1 = new ListBox();
        listBox1.Location = new Point(0, 0);
		listBox1.Size = new Size(this.ClientSize.Width, this.ClientSize.Height - 40);
		
		void OnSelect(object sender, EventArgs e){
			string selectedItem = listBox1.SelectedItem.ToString();
			string[] AllPackFiles = Directory.GetFiles(System.IO.Path.Combine(Soundpack_Path, selectedItem));
			
			System.Diagnostics.Process.Start("C:\\Windows\\System32\\taskkill.exe"," /F /IM iCUE.exe");
			Thread.Sleep(2000);
			string[] allCurrentFiles = Directory.GetFiles(Current_Sound_Path);
            foreach(var file in allCurrentFiles){
                File.Delete(file);
            }
            foreach(var file in AllPackFiles){
				// For use in a future version
				// string inputFileName = Path.GetFileNameWithoutExtension(file);
				// string outputFilePath = Path.Combine(Current_Sound_Path, inputFileName+".wav");

				string outputFilePath = Path.Combine(Current_Sound_Path, Path.GetFileName(file));
				File.Copy(file, outputFilePath, true);
				
            }
			System.Diagnostics.Process.Start(System.IO.Path.Combine(iCue_Path,"iCUE.exe"));
			MessageBox.Show("Now Using: "+selectedItem);
		}
		
		Button SelectPackButton;
		SelectPackButton = new Button();
		SelectPackButton.Name = "selectthispack";
        SelectPackButton.Text = "Select this pack";
		SelectPackButton.Location = new Point(0, this.ClientSize.Height-40);
		SelectPackButton.Size = new Size(this.ClientSize.Width, 40);
		SelectPackButton.Click += OnSelect;
		
		void Form1_Resize(object sender, EventArgs e){
			listBox1.Size = new Size(this.ClientSize.Width, this.ClientSize.Height - 40);
			SelectPackButton.Location = new Point(0, this.ClientSize.Height-40);
			SelectPackButton.Size = new Size(this.ClientSize.Width, 40);
		}
		this.Resize += Form1_Resize;
		
        listBox1.Size = new Size(this.ClientSize.Width, this.ClientSize.Height - 40);
		
		for (int i = 0; i < allPacks.Length; i++){
			listBox1.Items.Add(Path.GetFileName(allPacks[i]));
		}
		
		this.Controls.Add(listBox1);
		this.Controls.Add(SelectPackButton);
	}
}


public partial class RemoveSoundpack
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing){
        if (disposing && (components != null)){
            components.Dispose();
        }
        base.Dispose(disposing);
    }
	private void PickedPack(object sender, EventArgs e){
		Button clickedButton = (Button)sender;
		string buttonText = clickedButton.Text;

		MessageBox.Show("Button Text: " + buttonText);
	}
}
public partial class RemoveSoundpack : Form
{
	public RemoveSoundpack(string iCue_Path, string Current_Sound_Path){
		this.components = new System.ComponentModel.Container();
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(300, 600);
        this.Text = "Remove Soundpack - iCustomCue";
		
		string Config_Base_Path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "iCustomCue");
        string Soundpack_Path = System.IO.Path.Combine(Config_Base_Path, "Soundpacks");
		string[] allPacks = Directory.GetDirectories(Soundpack_Path);

		ListBox listBox1;
		listBox1 = new ListBox();
        listBox1.Location = new Point(0, 0);
		listBox1.Size = new Size(this.ClientSize.Width, this.ClientSize.Height - 40);
		
		void OnSelect(object sender, EventArgs e){
			string selectedItem = listBox1.SelectedItem.ToString();
			string ToDelete = System.IO.Path.Combine(Soundpack_Path, selectedItem);
			
			Directory.Delete(ToDelete, true);
            
			MessageBox.Show("Deleted: "+selectedItem);
		}
		
		Button SelectPackButton;
		SelectPackButton = new Button();
		SelectPackButton.Name = "removethispack";
        SelectPackButton.Text = "Delete this pack";
		SelectPackButton.Location = new Point(0, this.ClientSize.Height-40);
		SelectPackButton.Size = new Size(this.ClientSize.Width, 40);
		SelectPackButton.Click += OnSelect;
		
		void Form1_Resize(object sender, EventArgs e){
			listBox1.Size = new Size(this.ClientSize.Width, this.ClientSize.Height - 40);
			SelectPackButton.Location = new Point(0, this.ClientSize.Height-40);
			SelectPackButton.Size = new Size(this.ClientSize.Width, 40);
		}
		this.Resize += Form1_Resize;
		
        listBox1.Size = new Size(this.ClientSize.Width, this.ClientSize.Height - 40);
		
		for (int i = 0; i < allPacks.Length; i++){
			listBox1.Items.Add(Path.GetFileName(allPacks[i]));
		}
		
		this.Controls.Add(listBox1);
		this.Controls.Add(SelectPackButton);
	}
}

static class Program
{
    [STAThread]
    static void Main(){
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.Run(new MainMenuForm());
    }    
}