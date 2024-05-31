using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFGVolandoVoy.Vista
{
    public class RetoViewModel : INotifyPropertyChanged
    {
        private Localidad _localidad;
        public Localidad Localidad
        {
            get => _localidad;
            set
            {
                _localidad = value;
                OnPropertyChanged(nameof(Localidad));
            }
        }

        private string _tipoDeReto;
        public string TipoDeReto
        {
            get => _tipoDeReto;
            set
            {
                _tipoDeReto = value;
                OnPropertyChanged(nameof(TipoDeReto));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
