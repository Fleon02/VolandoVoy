// Clase de modelo de usuario
using System.ComponentModel;

public class UsuarioModel : INotifyPropertyChanged
{
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

    private ImageSource _userImage;
    public ImageSource UserImage
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
