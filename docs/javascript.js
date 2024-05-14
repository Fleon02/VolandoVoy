<<<<<<< HEAD
// Obtener el nombre de usuario de la URL y mostrarlo en el encabezado
var username = obtenerParametroUrl('usuario');
document.getElementById("username").textContent = username;
document.getElementById("elusuario").value = username;

function obtenerParametroUrl(nombreParametro) {
    var urlParams = new URLSearchParams(window.location.hash.substring(1));
    return urlParams.get(nombreParametro);
}

function cambiarContrasena() {
    var nuevapass = document.getElementById("nuevapass").value;
    if (nuevapass!='') {
        alert('Cambiando contraseña para el usuario: ' + username);
    } else {
        alert('Introduce Contraseña');
    }
    // Aquí puedes agregar el código para enviar la solicitud a la aplicación MAUI
    // para cambiar la contraseña en la base de datoss
}
=======

        // Obtener el nombre de usuario de la URL y mostrarlo en el encabezado
        var username = obtenerParametroUrl('usuario');
        document.getElementById("username").textContent = username;
        document.getElementById("elusuario").value = username;

        function obtenerParametroUrl(nombreParametro) {
            var urlParams = new URLSearchParams(window.location.search);
            return urlParams.get(nombreParametro);
        }

        function cambiarContrasena() {
            var nuevapass = document.getElementById("nuevapass").value;
            if (nuevapass != '') {
                alert('Cambiando contraseña para el usuario: ' + username);
            } else {
                alert('Introduce Contraseña');
            }
            // Aquí puedes agregar el código para enviar la solicitud a la aplicación MAUI
            // para cambiar la contraseña en la base de datos
        }

        // Redirigir si no se proporciona un nombre de usuario en la URL
        if (!username) {
            window.location.replace("error.html"); // Cambia "error.html" a la página de error correspondiente
	       }
>>>>>>> main
