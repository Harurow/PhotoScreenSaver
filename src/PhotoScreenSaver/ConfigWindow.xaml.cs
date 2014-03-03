using System.IO;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;
using WinFormDialogResult = System.Windows.Forms.DialogResult;

namespace PhotoScreenSaver
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class ConfigWindow
	{
		public ConfigWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			var config = ScreenSaverConfigure.Instance;
			PhotoPathTextBox.Text = config.PhotoPath;
		}

		private void PhotoPathBrowseButton_Click(object sender, RoutedEventArgs e)
		{
			using (var dlg = new FolderBrowserDialog())
			{
				dlg.Description = @"スクリーンセーバーに表示する画像があるフォルダーを選んでから[OK]をクリックしてください。";
				dlg.ShowNewFolderButton = false;
				dlg.SelectedPath = PhotoPathTextBox.Text;

				if (dlg.ShowDialog() == WinFormDialogResult.OK)
				{
					var config = ScreenSaverConfigure.Instance;

					config.PhotoPath = dlg.SelectedPath;
					PhotoPathTextBox.Text = dlg.SelectedPath;
				}
			}
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			var config = ScreenSaverConfigure.Instance;
			config.Load();
			Close();
		}

		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			var path = PhotoPathTextBox.Text;
			if (Directory.Exists(path))
			{
				var config = ScreenSaverConfigure.Instance;
				config.PhotoPath = path;
				config.Save();
				Close();
			}
			else
			{
				MessageBox.Show(this, "指定したパスは無効です。", "エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}
	}
}
