using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;


namespace TFGVolandoVoy.ViewModels
{
    public class RetosVntViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<string> _ciudades;
        private ObservableCollection<string> _retos;
        private string _ciudadSeleccionada;

        public ObservableCollection<string> Ciudades
        {
            get => _ciudades;
            set
            {
                _ciudades = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Retos
        {
            get => _retos;
            set
            {
                _retos = value;
                OnPropertyChanged();
            }
        }

        public string CiudadSeleccionada
        {
            get => _ciudadSeleccionada;
            set
            {
                _ciudadSeleccionada = value;
                OnPropertyChanged();
                // Aquí puedes agregar la lógica para cargar los retos de la ciudad seleccionada
            }
        }

        public ICommand SeleccionarCiudadCommand { get; }

        public RetosVntViewModel()
        {
            // Inicializar las colecciones y comandos
            Ciudades = new ObservableCollection<string>
            {
                "Ciudad 1",
                "Ciudad 2",
                "Ciudad 3"
            };

            Retos = new ObservableCollection<string>();

            // Comando para seleccionar una ciudad
            SeleccionarCiudadCommand = new Command<string>(SeleccionarCiudad);
        }

        private void SeleccionarCiudad(string ciudad)
        {
            CiudadSeleccionada = ciudad;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
