using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClientProgettoAnagrafiche.ViewModels
{
    public partial class RicercaMergedViewModel : ObservableObject
    {
        [ObservableProperty]
        private int id;

        [ObservableProperty]
        private string tipo = string.Empty; // "Cliente" or "Contatto"

        [ObservableProperty]
        private string? nome;

        [ObservableProperty]
        private string? cognome;

        [ObservableProperty]
        private string? email;

        [ObservableProperty]
        private string? telefono;

        [ObservableProperty]
        private string? codiceFiscale;

        [ObservableProperty]
        private string? partitaIva;

        [ObservableProperty]
        private bool? isPersonaFisica;

        [ObservableProperty]
        private string? ragioneSociale;

        [ObservableProperty]
        private string displayName = string.Empty;

        [ObservableProperty]
        private string provenienza = string.Empty; // For Contatti

        [ObservableProperty]
        private string noteContatto = string.Empty; // For Contatti

        [ObservableProperty]
        private string noteCliente = string.Empty; // For Clienti

        [ObservableProperty]
        private DateTime? dataRegistrazione; // For Clienti - null for Contatti
    }
}
