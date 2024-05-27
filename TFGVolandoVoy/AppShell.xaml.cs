﻿using Microsoft.Maui.Controls;
using Supabase.Interfaces;
using TFGVolandoVoy.Vista;

namespace TFGVolandoVoy
{
    public partial class AppShell : Shell
    {
        public static UsuarioModel CurrentUser { get; set; } = new UsuarioModel();

        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("InicioSesion", typeof(InicioSesion));
            Routing.RegisterRoute("ProvinciaVnt", typeof(ProvinciaVnt));
            Routing.RegisterRoute("LocalidadVnt", typeof(LocalidadVnt));
            Routing.RegisterRoute("CerrarSesion", typeof(InicioSesion));
            Routing.RegisterRoute("RetosVnt", typeof(RetosVnt));
            BindingContext = CurrentUser;

            GoToAsync("InicioSesion");
        }

        public async Task NavigateToPage(string route)
        {

            Page page = null;

            switch (route)
            {
                case "InicioSesion":
                    page = new InicioSesion();
                    break;
                case "ProvinciaVnt":
                    page = new ProvinciaVnt();
                    break;
                case "LocalidadVnt":
                    page = new LocalidadVnt();
                    break;
                case "RetosVnt":
                    page = new RetosVnt();
                    break;
            }

            if (page != null)
            {
                await Navigation.PushAsync(page);
            }
        }


    }
}
