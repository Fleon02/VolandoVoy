using TFGVolandoVoy.Converters;

namespace TFGVolandoVoy
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Resources.Add("StringNullOrEmptyToBoolConverter", new StringNullOrEmptyToBoolConverter());

            MainPage = new AppShell();
        }
    }
}
