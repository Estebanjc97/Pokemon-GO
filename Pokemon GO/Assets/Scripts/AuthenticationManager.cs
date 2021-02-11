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
    private void Start()
    {
        InitializeFirebase();
    }

    private void Update()
    {
        if (logged)
        {
            ActivatedSession();
            GetSessionProfile();
            SceneManager.LoadScene(1);
        }

    }

    void InitializeFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance; //Iniciamos la autenticacion con el API de Firebase
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user) //verificamos si el usuario es el mismo que ingreso por ultima vez
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null; //verificamos si estamos logueados y posteriormente nos deslogueamos o nos logueamos de acuerdo al valor booleano
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
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
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled."); //mostramos un mensaje de error si el usuario canceló   la operación
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception); //mostramos un mensaje de error si ocurrio algo inesperado con la API de Firebase
                return;
            }

            // Si el usuario se creò correctamente le asignamos la variable del usuario al usuario que este ingresó por entrada
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });
    }
    public void ActivatedSession()
    {
        Firebase.Auth.FirebaseAuth auth;
        Firebase.Auth.FirebaseUser user;

        // Handle initialization of the necessary firebase modules:
        InitializeFirebase();
    }
   

    public void GetSessionProfile()
    {
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            string name = user.DisplayName;
            string email = user.Email;
            System.Uri photo_url = user.PhotoUrl;
            // The user's Id, unique to the Firebase project.
            // Do NOT use this value to authenticate with your backend server, if you
            // have one; use User.TokenAsync() instead.
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
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
            logged = true;
        });
    }
    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }
}
