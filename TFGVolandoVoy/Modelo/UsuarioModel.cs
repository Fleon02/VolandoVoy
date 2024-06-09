// Clase de modelo de usuario
using System.ComponentModel;

public class UsuarioModel : INotifyPropertyChanged
{
    private long _idUsuario;
    public long IdUsuario
    {
        get { return _idUsuario; }
        set
        {
            if (_idUsuario != value)
            {
                _idUsuario = value;
                OnPropertyChanged(nameof(IdUsuario));
            }
        }
    }

    private string _username;
    public string Username
    {
        get { return _username; }
        set
        {
            if (_username != value)
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }
    }

    private string _rol;
    public string Rol
    {
        get { return _rol; }
        set
        {
            if (_rol != value)
            {
                _rol = value;
                OnPropertyChanged(nameof(Rol));
            }
        }
    }

    private string _userImage;
    public string UserImage
    {
        get { return _userImage; }
        set
        {
            if (_userImage != value)
            {
                _userImage = value;
                OnPropertyChanged(nameof(UserImage));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
