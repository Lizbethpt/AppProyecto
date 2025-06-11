using AppProyecto.Data;
using AppProyecto.Views; 
using PdfSharpCore.Fonts;
using System;
using System.IO;

namespace AppProyecto
{
    public partial class App : Application
    {
        private static AppDatabase _database;

        public static AppDatabase Database
        {
            get
            {
                if (_database == null)
                {
                    string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "embarques.db3");
                    _database = new AppDatabase(dbPath);
                }
                return _database;
            }
        }

        public App()
        {
            InitializeComponent();
            GlobalFontSettings.FontResolver = new EmbeddedFontResolver();
            MainPage = new NavigationPage(new MainPage());
        }
    }
}
