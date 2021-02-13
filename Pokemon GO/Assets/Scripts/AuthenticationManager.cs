using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AuthenticationManager : MonoBehaviour
{

    //variables de Firebase para autenticar los usuarios
    protected Firebase.Auth.FirebaseAuth auth;
    protected Firebase.Auth.FirebaseUser user;

    string displayName;
    public InputField inputFieldEmail, inputFieldPassword; //Entradas del usuario

    bool logged = false;
    string message = "";
    public Text UImessage; 
    private void Start()
    {
        InitializeFirebase();
    }

    private void Update()
    {
        if (logged) //si nos hemos loggueado exitosamente cargamos la escena del juego
        {
            InitializeFirebase();
            GetSessionProfile();
            StartCoroutine(LoadGameScene());
        }
        UImessage.text = message;
    }
    
    IEnumerator LoadGameScene()
    {
        float delayLoading = 2f;
        yield return new WaitForSeconds(delayLoading);
        SceneManager.LoadScene(1);
    }


    void InitializeFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance; //Iniciamos la autenticacion con el API de Firebase
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null); //empezamos desloggueados
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user) 
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null; 
            if (!signedIn && user != null)
            {
      
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                displayName = user.DisplayName ?? "";
            }
        }
    }
    public void RegisterNewUsers() //creamos nuevos usuarios pidiendo su correo y contraseña
    {
        string email = inputFieldEmail.text;
        string password = inputFieldPassword.text;

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                message = "Registro cancelado"; //mostramos un mensaje de error si el usuario canceló   la operación
                return;
            }
            if (task.IsFaulted)
            {
                message = "Asegúrate de llenar todos los campos y de que el usuario no exista";
                return;
            }

            // Si el usuario se creò correctamente le asignamos la variable del usuario al usuario que este ingresó por entrada
            Firebase.Auth.FirebaseUser newUser = task.Result;
            message = "Usuario registrado exitosamente";

        });
    }
   
    public void GetSessionProfile()
    {
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            string name = user.DisplayName;
            string email = user.Email;
            System.Uri photo_url = user.PhotoUrl;
            string uid = user.UserId;
        }
    }
    public void LoginWithEmail()
    {

        string email = inputFieldEmail.text;
        string password = inputFieldPassword.text;

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                message = "Inicio de sesión cancelado";
                return;
            }
            if (task.IsFaulted)
            {
                message = "Correo y/o contraseña incorrecta";
                return;
            }
            
            Firebase.Auth.FirebaseUser newUser = task.Result;
            message = "Inicio de sesión exitoso";
            logged = true;
        });
    }

    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }
}
